using Microsoft.AspNetCore.Mvc;
using ApiComederoPet.Data;
using ApiComederoPet.Models;

namespace ApiComederoPet.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ScheduleController : ControllerBase
    {
        private readonly AppDbContext _db;

        public ScheduleController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public IActionResult GetAll() => Ok(_db.FeedSchedules.ToList());

        [HttpPost]
        public IActionResult Add(Schedule schedule)
        {
            _db.FeedSchedules.Add(schedule);
            _db.SaveChanges();
            return Ok(schedule);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var s = _db.FeedSchedules.Find(id);
            if (s == null) return NotFound();
            _db.FeedSchedules.Remove(s);
            _db.SaveChanges();
            return Ok("Eliminado");
        }
    }
}
