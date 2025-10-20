using Microsoft.AspNetCore.Mvc;
using PetFeederAPI.Data;
using PetFeederAPI.Models;

namespace PetFeederAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FeedController : ControllerBase
    {
        private readonly AppDbContext _db;
        private static DateTime? lastFeed = null;
        private static bool comandoManual = false;

        public FeedController(AppDbContext db)
        {
            _db = db;
        }

        // Activar alimentación manual desde web o curl
        [HttpPost("manual")]
        public IActionResult FeedNow()
        {
            var log = new FeedLog { Source = "manual" };
            _db.FeedLogs.Add(log);
            _db.SaveChanges();

            lastFeed = log.Timestamp;
            comandoManual = true; // marca comando pendiente para ESP32

            return Ok(new { message = "Comida enviada al comedero", lastFeed });
        }

        // Consultar si hay comando pendiente (ESP32)
        [HttpGet("check")]
        public IActionResult CheckCommand()
        {
            return Ok(new { comandoManual });
        }

        // Resetear comando pendiente después de ejecutar
        [HttpPost("reset")]
        public IActionResult ResetCommand()
        {
            comandoManual = false;
            return Ok(new { status = "ok" });
        }

        // Estado general
        [HttpGet("status")]
        public IActionResult GetStatus()
        {
            return Ok(new
            {
                status = "ok",
                ultimaAlimentacion = lastFeed?.ToString("yyyy-MM-dd HH:mm:ss") ?? "Nunca"
            });
        }
    }
}
