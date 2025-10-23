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

            // Asegurarnos de que exista siempre un registro de estado
            if (!_db.FeedStates.Any())
            {
                _db.FeedStates.Add(new FeedState
                {
                    ShouldFeed = false,
                    LastFed = DateTime.UtcNow
                });
                _db.SaveChanges();
            }
        }

        // ✅ CORREGIDO: Activar alimentación manual
        [HttpPost("manual")]
        public IActionResult FeedNow()
        {
            try
            {
                var state = _db.FeedStates.FirstOrDefault();

                if (state == null)
                {
                    return StatusCode(500, new { error = "No se encontró el estado inicial" });
                }

                // Crear el log primero
                var log = new FeedLog
                {
                    Source = "manual",
                    Timestamp = DateTime.UtcNow
                };
                _db.FeedLogs.Add(log);

                // Actualizar el estado
                state.ShouldFeed = true;
                state.LastFed = DateTime.UtcNow; // Usar DateTime.UtcNow directamente

                // Guardar todo junto
                _db.SaveChanges();

                _logger.LogInformation("Comando manual enviado correctamente");

                return Ok(new
                {
                    message = "Comida enviada al comedero",
                    shouldFeed = true,
                    lastFed = state.LastFed
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en FeedNow");
                return StatusCode(500, new
                {
                    error = ex.Message,
                    details = ex.InnerException?.Message
                });
            }
        }

        // ✅ Consultar si hay comando pendiente (ESP32)
        [HttpGet("check")]
        public IActionResult CheckCommand()
        {
            var state = _db.FeedStates.FirstOrDefault();

            if (state == null)
            {
                return Ok(new { comandoManual = false });
            }

            return Ok(new { comandoManual = state.ShouldFeed });
        }

        // ✅ Resetear comando pendiente después de ejecutar
        [HttpPost("reset")]
        public IActionResult ResetCommand()
        {
            var state = _db.FeedStates.FirstOrDefault();

            if (state != null)
            {
                state.ShouldFeed = false;
                state.LastFed = DateTime.UtcNow; // Actualizar cuando se ejecuta
                _db.SaveChanges();
            }

            return Ok(new { status = "ok" });
        }

        // ✅ Estado general
        [HttpGet("status")]
        public IActionResult GetStatus()
        {
            var state = _db.FeedStates.FirstOrDefault();

            if (state == null)
            {
                return Ok(new
                {
                    status = "ok",
                    ultimaAlimentacion = "Nunca",
                    pendingCommand = false
                });
            }
            // Convertir hora UTC → hora Colombia
            DateTime localTime;
            try
            {
                localTime = TimeZoneInfo.ConvertTimeFromUtc(
                    state.LastFed.ToUniversalTime(),
                    TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time") // UTC-5 (Bogotá)
                );
            }
            catch
            {
                // fallback si Render no reconoce el nombre del huso horario
                localTime = state.LastFed.AddHours(-5);
            }
            return Ok(new
            {
                status = "ok",
                ultimaAlimentacion = state.LastFed.ToString("yyyy-MM-dd HH:mm:ss"),
                pendingCommand = state.ShouldFeed
            });
        }
    }
}