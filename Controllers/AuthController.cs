using Microsoft.AspNetCore.Mvc;
using ApiComederoPet.Data;
using ApiComederoPet.Models;
using Microsoft.EntityFrameworkCore;

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

        // ✅ Registro de usuario
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User user)
        {
            if (string.IsNullOrWhiteSpace(user.Username) || string.IsNullOrWhiteSpace(user.Password))
                return BadRequest("Todos los campos son obligatorios.");

            // Verificar si ya existe el usuario
            var exists = await _db.Users.AnyAsync(u => u.Username == user.Username);
            if (exists)
                return BadRequest("El usuario ya existe.");

            // Agregar nuevo usuario
            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            return Ok(new { message = "✅ Usuario registrado correctamente." });
        }

        // ✅ Inicio de sesión
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] User user)
        {
            if (string.IsNullOrWhiteSpace(user.Username) || string.IsNullOrWhiteSpace(user.Password))
                return BadRequest("Todos los campos son obligatorios.");

            var dbUser = await _db.Users
                .FirstOrDefaultAsync(u => u.Username == user.Username && u.Password == user.Password);

            if (dbUser == null)
                return Unauthorized("Credenciales inválidas.");

            return Ok(new { message = "✅ Login exitoso." });
        }
    }
}

