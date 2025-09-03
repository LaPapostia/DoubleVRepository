using DataLayer.DbConnection.Repository;
using Microsoft.AspNetCore.Mvc;
using ModelLayer;

namespace api_payments.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        /// <summary>
        /// Declared repository for the creation of the instance request
        /// </summary>
        private readonly IPostgresRepository<dynamic> _repository;

        /// <summary>
        /// constructor for the assign of the repository value
        /// </summary>
        /// <param name="repository"></param>
        public UserController(IPostgresRepository<dynamic> repository)
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
                /// Executing the sentence
                await _repository.PostAsync("sp_usuario_crear", param);

                return Ok(new { UsuarioId = usuario });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al crear usuario: {ex.Message}");
            }
        }
    }
}
