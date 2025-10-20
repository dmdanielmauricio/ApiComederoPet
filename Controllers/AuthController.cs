using Microsoft.AspNetCore.Mvc;
using PetFeederAPI.Data;
using PetFeederAPI.Models;

namespace PetFeederAPI.Controllers
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

        [HttpPost("register")]
        public IActionResult Register(User user)
        {
            if (_db.Users.Any(u => u.Username == user.Username))
                return BadRequest("Usuario ya existe");

            _db.Users.Add(user);
            _db.SaveChanges();
            return Ok("Usuario registrado");
        }

        [HttpPost("login")]
        public IActionResult Login(User user)
        {
            var dbUser = _db.Users.FirstOrDefault(u => u.Username == user.Username && u.Password == user.Password);
            if (dbUser == null)
                return Unauthorized("Credenciales inválidas");

            return Ok("Login exitoso");
        }
    }
}
