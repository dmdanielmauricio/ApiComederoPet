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

        public FeedController(AppDbContext db)
        {
            _db = db;
        }

        [HttpPost("manual")]
        public IActionResult FeedNow()
        {
            var log = new FeedLog { Source = "manual" };
            _db.FeedLogs.Add(log);
            _db.SaveChanges();

            lastFeed = log.Timestamp;
            return Ok(new { message = "Comida enviada al comedero", lastFeed });
        }

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
