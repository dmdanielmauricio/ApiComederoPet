using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiComederoPet.Data;
using ApiComederoPet.Models;

namespace ApiComederoPet.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FeedController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly ILogger<FeedController> _logger;

        public FeedController(AppDbContext db, ILogger<FeedController> logger)
        {
            _db = db;
            _logger = logger;

            // Asegurar registro de estado único
            if (!_db.FeedStates.Any())
            {
                _db.FeedStates.Add(new FeedState
                {
                    ShouldFeed = false,
                    LastFed = DateTime.UtcNow,
                    LastHeartbeatUtc = null
                });
                _db.SaveChanges();
            }
        }

        // ✅ Alimentación manual
        [HttpPost("manual")]
        public IActionResult FeedNow()
        {
            try
            {
                var state = _db.FeedStates.First();

                _db.FeedLogs.Add(new FeedLog
                {
                    Source = "manual",
                    Timestamp = DateTime.UtcNow
                });

                state.ShouldFeed = true;
                state.LastFed = DateTime.UtcNow;

                _db.SaveChanges();

                _logger.LogInformation("Comando manual enviado correctamente");
                return Ok(new { message = "Comida enviada al comedero", shouldFeed = true, lastFed = state.LastFed });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en FeedNow");
                return StatusCode(500, new { error = ex.Message, details = ex.InnerException?.Message });
            }
        }

        // ✅ Consulta para la ESP32: ¿hay comando pendiente?
        [HttpGet("check")]
        public IActionResult CheckCommand()
        {
            var state = _db.FeedStates.FirstOrDefault();
            return Ok(new { comandoManual = state?.ShouldFeed ?? false });
        }

        // ✅ Reset tras ejecutar en la ESP32
        [HttpPost("reset")]
        public IActionResult ResetCommand()
        {
            var state = _db.FeedStates.FirstOrDefault();
            if (state != null)
            {
                state.ShouldFeed = false;
                state.LastFed = DateTime.UtcNow;
                _db.SaveChanges();
            }
            return Ok(new { status = "ok" });
        }

        // ✅ Latido de la ESP32 (llamar cada 10–20 s)
        [HttpPost("heartbeat")]
        public IActionResult Heartbeat()
        {
            var state = _db.FeedStates.FirstOrDefault();
            if (state == null)
            {
                state = new FeedState
                {
                    ShouldFeed = false,
                    LastFed = DateTime.UtcNow,
                    LastHeartbeatUtc = DateTime.UtcNow
                };
                _db.FeedStates.Add(state);
            }
            else
            {
                state.LastHeartbeatUtc = DateTime.UtcNow;
            }

            _db.SaveChanges();
            return Ok(new { status = "ok" });
        }

        // ✅ Estado general (incluye conectado + hora local Colombia)
        [HttpGet("status")]
        public IActionResult GetStatus()
        {
            var state = _db.FeedStates.FirstOrDefault();

            if (state == null)
            {
                return Ok(new { status = "ok", conectado = false, ultimaAlimentacion = "Nunca", pendingCommand = false });
            }

            // Hora Colombia (con fallback)
            DateTime localTime;
            try
            {
                var tz = TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time");
                localTime = TimeZoneInfo.ConvertTimeFromUtc(state.LastFed.ToUniversalTime(), tz);
            }
            catch
            {
                localTime = state.LastFed.ToUniversalTime().AddHours(-5);
            }

            // Conectado si hubo heartbeat hace < 25 s
            bool conectado = state.LastHeartbeatUtc.HasValue &&
                             (DateTime.UtcNow - state.LastHeartbeatUtc.Value) < TimeSpan.FromSeconds(25);

            return Ok(new
            {
                status = "ok",
                conectado,
                ultimaAlimentacion = localTime.ToString("yyyy-MM-dd HH:mm:ss"),
                pendingCommand = state.ShouldFeed
            });
        }
    }
}
