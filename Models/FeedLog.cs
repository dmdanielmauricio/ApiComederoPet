namespace PetFeederAPI.Models
{
    public class FeedLog
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public string Source { get; set; } = "manual"; // manual o automático
    }
}
