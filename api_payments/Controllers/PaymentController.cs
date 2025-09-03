using DataLayer.DbConnection.Repository;
using Microsoft.AspNetCore.Mvc;
using ModelLayer;

namespace api_payments.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IPostgresRepository<Payment> _repository;

        public PaymentController(IPostgresRepository<Payment> repository)
        {
            _repository = repository;
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
            var param = new { p_pago_id = id };

            try
            {
                var result = await _repository.GetAsync("fn_pago_consultar", param);
                if (result == null || !result.Any())
                    return NotFound($"Pago con ID {id} no encontrado.");

                return Ok(result.First());
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
            var param = new { p_deuda_id = deudaId };

            try
            {
                var result = await _repository.GetAsync("fn_pago_listar_deuda", param);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al listar pagos de la deuda: {ex.Message}");
            }
        }
    }
}
