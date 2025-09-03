using DataLayer.DbConnection.Repository;
using Microsoft.AspNetCore.Mvc;
using ModelLayer;

namespace api_payments.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DebtController : ControllerBase
    {
        private readonly IPostgresRepository<Debt> _repository;

        public DebtController(IPostgresRepository<Debt> repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Crear una nueva deuda
        /// </summary>
        [HttpPost("create")]
        public async Task<IActionResult> CreateDebt([FromBody] Debt deuda)
        {
            if (deuda == null)
                return BadRequest("La deuda es requerida.");

            var param = new
            {
                p_deudor_id = deuda.deudor_id,
                p_acreedor_id = deuda.acreedor_id,
                p_monto = deuda.monto
            };

            try
            {
                await _repository.PostAsync("sp_deuda_crear", param);
                return Ok(new { message = "Deuda creada correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al crear deuda: {ex.Message}");
            }
        }

        /// <summary>
        /// Editar una deuda
        /// </summary>
        [HttpPut("edit/{id}")]
        public async Task<IActionResult> EditDebt(int id, [FromBody] Debt deuda)
        {
            if (deuda == null)
                return BadRequest("La deuda es requerida.");

            var param = new
            {
                p_deuda_id = id,
                p_monto = deuda.monto
            };

            try
            {
                await _repository.UpdateAsync("sp_deuda_editar", param);
                return Ok(new { message = "Deuda actualizada correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al editar deuda: {ex.Message}");
            }
        }

        /// <summary>
        /// Eliminar una deuda
        /// </summary>
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteDebt(int id)
        {
            var param = new { p_deuda_id = id };

            try
            {
                await _repository.DeleteAsync("sp_deuda_eliminar", param);
                return Ok(new { message = "Deuda eliminada correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al eliminar deuda: {ex.Message}");
            }
        }

        /// <summary>
        /// Consultar una deuda por ID
        /// </summary>
        [HttpGet("get/{id}")]
        public async Task<IActionResult> GetDebt(int id)
        {
            var param = new { p_deuda_id = id };

            try
            {
                var result = await _repository.GetAsync("fn_deuda_consultar", param);
                if (result == null || !result.Any())
                    return NotFound($"Deuda con ID {id} no encontrada.");

                return Ok(result.First());
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al consultar deuda: {ex.Message}");
            }
        }

        /// <summary>
        /// Listar todas las deudas de un usuario
        /// </summary>
        [HttpGet("list/user/{usuarioId}")]
        public async Task<IActionResult> ListDebtsByUser(int usuarioId)
        {
            var param = new { p_usuario_id = usuarioId };

            try
            {
                var result = await _repository.GetAsync("fn_deuda_listar_usuario", param);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al listar deudas del usuario: {ex.Message}");
            }
        }
    }
}
