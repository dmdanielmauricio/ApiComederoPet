using System.ComponentModel.DataAnnotations;

namespace ApiComederoPet.Models
{
    public class Schedule
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [DataType(DataType.Time)]
        public DateTime Time { get; set; }

        public string Days { get; set; } = string.Empty;

        public bool Active { get; set; } = true;
    }
}
