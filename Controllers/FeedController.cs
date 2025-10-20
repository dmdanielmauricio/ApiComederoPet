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

        // Endpoint que se llama desde la web para alimentar manualmente
        [HttpPost("manual")]
        public IActionResult FeedNow()
        {
            var log = new FeedLog { Source = "manual" };
            _db.FeedLogs.Add(log);
            _db.SaveChanges();

            // Marcar que hay comando pendiente para la ESP32
            comandoManual = true;
            lastFeed = log.Timestamp;

            return Ok(new { message = "Comida enviada al comedero", lastFeed });
        }

        // Endpoint para que la ESP32 consulte si hay comando manual pendiente
        [HttpGet("check")]
        public IActionResult CheckCommand()
        {
            return Ok(new { comandoManual });
        }

        // Endpoint para que la ESP32 confirme que ejecutó el comando
        [HttpPost("reset")]
        public IActionResult ResetCommand()
        {
            comandoManual = false;
            return Ok(new { message = "Comando reseteado" });
        }

        // Endpoint de estado para mostrar última alimentación
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
