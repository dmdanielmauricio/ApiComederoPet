// Models/FeedState.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiComederoPet.Models
{
    public class FeedState
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Column("ShouldFeed")]  // ← Cambiar de "ComandoManual" a "ShouldFeed"
        public bool ShouldFeed { get; set; }

        [Column("LastFed")]  // ← Cambiar de "ÚltimaFeed" a "LastFed"
        public DateTime LastFed { get; set; }
    }
}
