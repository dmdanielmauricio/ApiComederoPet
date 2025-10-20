using Microsoft.EntityFrameworkCore;
using PetFeederAPI.Models;

namespace PetFeederAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<FeedLog> FeedLogs { get; set; }

        public DbSet<FeedState> FeedStates { get; set; }

    }
}
