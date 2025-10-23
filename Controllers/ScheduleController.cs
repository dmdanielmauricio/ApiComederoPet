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
            var schedules = _db.FeedSchedules.OrderBy(s => s.Hour).ToList();
            return Ok(schedules);
        }

        [HttpPost]
        public IActionResult Add(Schedule schedule)
        {
            try
            {
                if (schedule == null)
                    return BadRequest("Datos inválidos");

                schedule.CreatedAt = DateTime.Now;

                _db.FeedSchedules.Add(schedule);
                _db.SaveChanges();

                return Ok(schedule);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al guardar: {ex.Message} | INNER: {ex.InnerException?.Message}");
            }
        }


        [HttpPut("{id}")]
        public IActionResult Update(int id, Schedule updated)
        {
            var schedule = _db.FeedSchedules.Find(id);
            if (schedule == null)
                return NotFound();

            schedule.Hour = updated.Hour;
            schedule.Minute = updated.Minute;
            schedule.DaysOfWeek = updated.DaysOfWeek;
            schedule.IsActive = updated.IsActive;
            _db.SaveChanges();

            return Ok(schedule);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var schedule = _db.FeedSchedules.Find(id);
            if (schedule == null)
                return NotFound();

            _db.FeedSchedules.Remove(schedule);
            _db.SaveChanges();

            return Ok("Eliminado correctamente");
        }
    }
}
