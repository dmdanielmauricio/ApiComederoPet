using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetFeederAPI.Models
{
    [Table("FeedLogs")]
    public class FeedLog
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Column("Source")]
        public string Source { get; set; } = "";

        [Column("Timestamp")]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}