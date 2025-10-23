using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        // ✅ Obtener todos los horarios
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var schedules = await _db.FeedSchedules
                .OrderBy(s => s.Hour)
                .ThenBy(s => s.Minute)
                .ToListAsync();

            return Ok(schedules);
        }

        // ✅ Agregar nuevo horario
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Schedule schedule)
        {
            try
            {
                if (schedule == null)
                    return BadRequest("Datos inválidos");

                // PostgreSQL exige UTC para timestamptz
                schedule.CreatedAt = DateTime.UtcNow;

                _db.FeedSchedules.Add(schedule);
                await _db.SaveChangesAsync();

                return Ok(schedule);
            }
            catch (Exception ex)
            {
                return StatusCode(500,
                    $"Error al guardar: {ex.Message} | INNER: {ex.InnerException?.Message}");
            }
        }

        // ✅ Actualizar un horario existente
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Schedule updated)
        {
            var schedule = await _db.FeedSchedules.FindAsync(id);
            if (schedule == null)
                return NotFound("Horario no encontrado");

            schedule.Hour = updated.Hour;
            schedule.Minute = updated.Minute;
            schedule.DaysOfWeek = updated.DaysOfWeek;
            schedule.IsActive = updated.IsActive;

            await _db.SaveChangesAsync();
            return Ok(schedule);
        }

        // ✅ Eliminar horario
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var schedule = await _db.FeedSchedules.FindAsync(id);
            if (schedule == null)
                return NotFound("Horario no encontrado");

            _db.FeedSchedules.Remove(schedule);
            await _db.SaveChangesAsync();

            return Ok("Eliminado correctamente");
        }
    }
}

