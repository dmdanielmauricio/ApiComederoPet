using System.ComponentModel.DataAnnotations;

namespace ApiComederoPet.Models
{
    public class Schedule
    {
        [Key]
        public int Id { get; set; }

        [Range(0, 23)]
        public int Hour { get; set; }  // 🕐 hora del día

        [Range(0, 59)]
        public int Minute { get; set; } // 🕐 minutos

        [Required]
        public string DaysOfWeek { get; set; } = "Lunes,Martes,Miercoles"; // 📅 días activos

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
