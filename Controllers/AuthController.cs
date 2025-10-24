using ApiComederoPet.Data;
using ApiComederoPet.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace ApiComederoPet.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _db;

        public AuthController(AppDbContext db)
        {
            _db = db;
        }

        // ✅ Registro de usuario (con manejo de errores y logging)
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User user)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(user.Username) || string.IsNullOrWhiteSpace(user.Password))
                    return BadRequest("Todos los campos son obligatorios.");

                // Verificar si ya existe el usuario
                var exists = await _db.Users.AnyAsync(u => u.Username == user.Username);
                if (exists)
                    return BadRequest("El usuario ya existe.");

                // 🔐 Encriptar la contraseña con SHA256 antes de guardarla
                using (var sha = SHA256.Create())
                {
                    var bytes = Encoding.UTF8.GetBytes(user.Password);
                    var hash = sha.ComputeHash(bytes);
                    user.Password = BitConverter.ToString(hash).Replace("-", "").ToLower();
                }

                // Guardar el usuario en la base de datos
                _db.Users.Add(user);
                await _db.SaveChangesAsync();

                return Ok(new { message = "✅ Usuario registrado correctamente." });
            }
            catch (Exception ex)
            {
                // 🧾 Log detallado (devuelve el mensaje interno de error)
                return StatusCode(500, new
                {
                    error = "Error al registrar el usuario.",
                    inner = ex.InnerException?.Message ?? ex.Message
                });
            }
        }

        // ✅ Inicio de sesión con contraseña encriptada
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] User user)
        {
            if (string.IsNullOrWhiteSpace(user.Username) || string.IsNullOrWhiteSpace(user.Password))
                return BadRequest("Todos los campos son obligatorios.");

            // Encriptar la contraseña ingresada antes de compararla
            using (var sha = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(user.Password);
                var hash = sha.ComputeHash(bytes);
                user.Password = BitConverter.ToString(hash).Replace("-", "").ToLower();
            }

            var dbUser = await _db.Users
                .FirstOrDefaultAsync(u => u.Username == user.Username && u.Password == user.Password);

            if (dbUser == null)
                return Unauthorized("Credenciales inválidas.");

            return Ok(new { message = "✅ Login exitoso." });
        }
    }
}

