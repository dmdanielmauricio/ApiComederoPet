using System.ComponentModel.DataAnnotations;

namespace PetFeederAPI.Models
{
    public class Schedule
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime Time { get; set; }
        public bool Active { get; set; } = true;
    }
}

