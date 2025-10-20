// Models/FeedState.cs
namespace PetFeederAPI.Models
{
    public class FeedState
    {
        public int Id { get; set; } = 1; // siempre habrá solo un registro
        public bool ComandoManual { get; set; } = false;
        public DateTime? LastFeed { get; set; }
    }
}
