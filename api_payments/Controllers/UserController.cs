using DataLayer.DbConnection.Repository;
using Microsoft.AspNetCore.Mvc;
using ModelLayer;

namespace api_payments.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IPostgresRepository<User> _repository;

        public UserController(IPostgresRepository<User> repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Crear un nuevo usuario
        /// </summary>
        [HttpPost("create")]
        public async Task<IActionResult> CreateUser([FromBody] User usuario)
        {
            if (usuario == null)
                return BadRequest("El usuario es requerido.");

            var param = new
            {
                p_usuario = usuario.usuario,
                p_correo = usuario.correo,
                p_contrasenia = usuario.contrasenia
            };

            try
            {
                await _repository.PostAsync("sp_usuario_crear", param);
                return Ok(new { message = "Usuario creado correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al crear usuario: {ex.Message}");
            }
        }

        /// <summary>
        /// Editar un usuario
        /// </summary>
        [HttpPut("edit/{id}")]
        public async Task<IActionResult> EditUser(int id, [FromBody] User usuario)
        {
            if (usuario == null)
                return BadRequest("El usuario es requerido.");

            var param = new
            {
                p_usuario_id = id,
                p_usuario = usuario.usuario,
                p_correo = usuario.correo,
                p_contrasenia = usuario.contrasenia
            };

            try
            {
                await _repository.UpdateAsync("sp_usuario_editar", param);
                return Ok(new { message = "Usuario actualizado correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al editar usuario: {ex.Message}");
            }
        }

        /// <summary>
        /// Eliminar usuario
        /// </summary>
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var param = new { p_usuario_id = id };

            try
            {
                await _repository.DeleteAsync("sp_usuario_eliminar", param);
                return Ok(new { message = "Usuario eliminado correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al eliminar usuario: {ex.Message}");
            }
        }

        /// <summary>
        /// Consultar un usuario por ID
        /// </summary>
        [HttpGet("get/{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var param = new { p_usuario_id = id };

            try
            {
                var result = await _repository.GetAsync("fn_usuario_consultar", param);
                if (result == null || !result.Any())
                    return NotFound($"Usuario con ID {id} no encontrado.");

                return Ok(result.First());
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al consultar usuario: {ex.Message}");
            }
        }

        /// <summary>
        /// Listar todos los usuarios
        /// </summary>
        [HttpGet("list")]
        public async Task<IActionResult> ListUsers()
        {
            try
            {
                var result = await _repository.GetAsync("fn_usuario_listar");
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al listar usuarios: {ex.Message}");
            }
        }
    }
}
