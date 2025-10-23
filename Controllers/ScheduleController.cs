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
        public IActionResult GetAll()
        {
            var schedules = _db.FeedSchedules.ToList();
            return Ok(schedules);
        }

        [HttpPost]
        public IActionResult Add([FromBody] Schedule schedule)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _db.FeedSchedules.Add(schedule);
            _db.SaveChanges();
            return CreatedAtAction(nameof(GetAll), new { id = schedule.Id }, schedule);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var s = _db.FeedSchedules.Find(id);
            if (s == null) return NotFound();

            _db.FeedSchedules.Remove(s);
            _db.SaveChanges();
            return NoContent();
        }
    }
}
