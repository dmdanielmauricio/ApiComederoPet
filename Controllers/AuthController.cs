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

        // 🔐 Método auxiliar para cifrar contraseñas
        private string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha.ComputeHash(bytes);
            return BitConverter.ToString(hash).Replace("-", "").ToLower(); // formato hexadecimal
        }

        // ✅ Registro de usuario (guarda contraseña cifrada)
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User user)
        {
            if (string.IsNullOrWhiteSpace(user.Username) || string.IsNullOrWhiteSpace(user.Password))
                return BadRequest("Todos los campos son obligatorios.");

            // Verificar si el usuario ya existe
            var exists = await _db.Users.AnyAsync(u => u.Username == user.Username);
            if (exists)
                return BadRequest("El usuario ya existe.");

            // Encriptar contraseña antes de guardar
            user.Password = HashPassword(user.Password);

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            return Ok(new { message = "✅ Usuario registrado correctamente." });
        }

        // ✅ Inicio de sesión (valida usando el hash)
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] User user)
        {
            if (string.IsNullOrWhiteSpace(user.Username) || string.IsNullOrWhiteSpace(user.Password))
                return BadRequest("Todos los campos son obligatorios.");

            // Hashear la contraseña ingresada
            var hashedPassword = HashPassword(user.Password);

            // Buscar usuario con contraseña cifrada
            var dbUser = await _db.Users
                .FirstOrDefaultAsync(u => u.Username == user.Username && u.Password == hashedPassword);

            if (dbUser == null)
                return Unauthorized("Credenciales inválidas.");

            return Ok(new { message = "✅ Login exitoso." });
        }
    }
}
