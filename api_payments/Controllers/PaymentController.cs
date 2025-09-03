using DataLayer.DbConnection.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using ModelLayer;
using System.Text.Json;

namespace api_payments.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IPostgresRepository<Payment> _repository;
        private readonly IDistributedCache _cache;

        public PaymentController(IPostgresRepository<Payment> repository, IDistributedCache cache)
        {
            _repository = repository;
            _cache = cache;
        }

        /// <summary>
        /// Crear un nuevo pago
        /// </summary>
        [HttpPost("create")]
        public async Task<IActionResult> CreatePayment([FromBody] Payment pago)
        {
            if (pago == null)
                return BadRequest("El pago es requerido.");

            var param = new
            {
                p_deuda_id = pago.deuda_id,
                p_monto = pago.monto
            };

            try
            {
                await _repository.PostAsync("sp_pago_crear", param);

                // 🔄 Invalidar cache relacionado con la deuda
                await _cache.RemoveAsync($"payments:debt:{pago.deuda_id}");

                return Ok(new { message = "Pago creado correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al crear pago: {ex.Message}");
            }
        }

        /// <summary>
        /// Editar un pago
        /// </summary>
        [HttpPut("edit/{id}")]
        public async Task<IActionResult> EditPayment(int id, [FromBody] Payment pago)
        {
            if (pago == null)
                return BadRequest("El pago es requerido.");

            var param = new
            {
                p_pago_id = id,
                p_monto = pago.monto
            };

            try
            {
                await _repository.UpdateAsync("sp_pago_editar", param);

                // 🔄 Invalidar cache del pago individual
                await _cache.RemoveAsync($"payment:{id}");

                return Ok(new { message = "Pago actualizado correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al editar pago: {ex.Message}");
            }
        }

        /// <summary>
        /// Eliminar un pago
        /// </summary>
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeletePayment(int id)
        {
            var param = new { p_pago_id = id };

            try
            {
                await _repository.DeleteAsync("sp_pago_eliminar", param);

                // 🔄 Invalidar cache del pago eliminado
                await _cache.RemoveAsync($"payment:{id}");

                return Ok(new { message = "Pago eliminado correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al eliminar pago: {ex.Message}");
            }
        }

        /// <summary>
        /// Consultar un pago por ID
        /// </summary>
        [HttpGet("get/{id}")]
        public async Task<IActionResult> GetPayment(int id)
        {
            string cacheKey = $"payment:{id}";
            var cachedPayment = await _cache.GetStringAsync(cacheKey);

            if (!string.IsNullOrEmpty(cachedPayment))
            {
                var cachedData = JsonSerializer.Deserialize<Payment>(cachedPayment);
                return Ok(cachedData);
            }

            var param = new { p_pago_id = id };

            try
            {
                var result = await _repository.GetAsync("fn_pago_consultar", param);
                if (result == null || !result.Any())
                    return NotFound($"Pago con ID {id} no encontrado.");

                var payment = result.First();

                // Guardar en cache
                await _cache.SetStringAsync(
                    cacheKey,
                    JsonSerializer.Serialize(payment),
                    new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                    });

                return Ok(payment);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al consultar pago: {ex.Message}");
            }
        }

        /// <summary>
        /// Listar todos los pagos de una deuda
        /// </summary>
        [HttpGet("list/debt/{deudaId}")]
        public async Task<IActionResult> ListPaymentsByDebt(int deudaId)
        {
            string cacheKey = $"payments:debt:{deudaId}";
            var cachedPayments = await _cache.GetStringAsync(cacheKey);

            if (!string.IsNullOrEmpty(cachedPayments))
            {
                var cachedData = JsonSerializer.Deserialize<IEnumerable<Payment>>(cachedPayments);
                return Ok(cachedData);
            }

            var param = new { p_deuda_id = deudaId };

            try
            {
                var result = await _repository.GetAsync("fn_pago_listar_deuda", param);

                // Guardar en cache
                await _cache.SetStringAsync(
                    cacheKey,
                    JsonSerializer.Serialize(result),
                    new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                    });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al listar pagos de la deuda: {ex.Message}");
            }
        }
    }
}
