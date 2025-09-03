using DataLayer.DbConnection.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using ModelLayer;
using System.Text.Json;

namespace api_payments.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IPostgresRepository<User> _repository;
        private readonly IDistributedCache _cache;

        public UserController(IPostgresRepository<User> repository, IDistributedCache cache)
        {
            _repository = repository;
            _cache = cache;
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

                // Invalidate cache
                await _cache.RemoveAsync("users_list");

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

                // Invalidate cache
                await _cache.RemoveAsync("users_list");
                await _cache.RemoveAsync($"user_{id}");

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

                // Invalidate cache
                await _cache.RemoveAsync("users_list");
                await _cache.RemoveAsync($"user_{id}");

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
            var cacheKey = $"user_{id}";

            try
            {
                // Try get from cache
                var cachedUser = await _cache.GetStringAsync(cacheKey);
                if (!string.IsNullOrEmpty(cachedUser))
                {
                    var user = JsonSerializer.Deserialize<User>(cachedUser);
                    return Ok(user);
                }

                // Get from DB
                var param = new { p_usuario_id = id };
                var result = await _repository.GetAsync("fn_usuario_consultar", param);

                if (result == null || !result.Any())
                    return NotFound($"Usuario con ID {id} no encontrado.");

                var userFromDb = result.First();

                // Save to cache
                await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(userFromDb),
                    new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                    });

                return Ok(userFromDb);
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
            var cacheKey = "users_list";

            try
            {
                // Try get from cache
                var cachedUsers = await _cache.GetStringAsync(cacheKey);
                if (!string.IsNullOrEmpty(cachedUsers))
                {
                    var users = JsonSerializer.Deserialize<IEnumerable<User>>(cachedUsers);
                    return Ok(users);
                }

                // Get from DB
                var result = await _repository.GetAsync("fn_usuario_listar");

                // Save to cache
                await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(result),
                    new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                    });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al listar usuarios: {ex.Message}");
            }
        }
    }
}
