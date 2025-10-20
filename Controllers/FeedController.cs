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

        public FeedController(AppDbContext db)
        {
            _db = db;

            // Asegurarnos de que exista siempre un registro de estado
            if (!_db.FeedStates.Any())
            {
                _db.FeedStates.Add(new FeedState());
                _db.SaveChanges();
            }
        }

        // Activar alimentación manual desde web o curl
        [HttpPost("manual")]
        public IActionResult FeedNow()
        {
            var state = _db.FeedStates.First();
            var log = new FeedLog { Source = "manual" };
            _db.FeedLogs.Add(log);

            state.ShouldFeed = true;
            state.LastFed = log.Timestamp;

            _db.SaveChanges();

            return Ok(new { message = "Comida enviada al comedero", lastFeed = log.Timestamp });
        }

        // Consultar si hay comando pendiente (ESP32)
        [HttpGet("check")]
        public IActionResult CheckCommand()
        {
            var state = _db.FeedStates.First();
            return Ok(new { comandoManual = state.ShouldFeed });
        }

        // Resetear comando pendiente después de ejecutar
        [HttpPost("reset")]
        public IActionResult ResetCommand()
        {
            var state = _db.FeedStates.First();
            state.ShouldFeed = false;
            _db.SaveChanges();

            return Ok(new { status = "ok" });
        }

        // Estado general
        [HttpGet("status")]
        public IActionResult GetStatus()
        {
            var state = _db.FeedStates.First();
            return Ok(new
            {
                status = "ok",
                ultimaAlimentacion = state.LastFed.ToString("yyyy-MM-dd HH:mm:ss") ?? "Nunca"
            });
        }
    }
}
