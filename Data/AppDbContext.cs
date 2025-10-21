using Microsoft.EntityFrameworkCore;
using PetFeederAPI.Models;

namespace PetFeederAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<FeedState> FeedStates { get; set; }
        public DbSet<FeedLog> FeedLogs { get; set; }
        public DbSet<Schedule> FeedSchedules { get; set; }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FeedState>(entity =>
            {
                entity.ToTable("FeedStates");
                entity.HasKey(e => e.Id);
            });

            modelBuilder.Entity<FeedLog>(entity =>
            {
                entity.ToTable("FeedLogs");
                entity.HasKey(e => e.Id);
            });

            modelBuilder.Entity<Schedule>(entity =>
            {
                entity.ToTable("FeedSchedules");
                entity.HasKey(e => e.Id);
            });
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Username).IsRequired();
                entity.Property(e => e.Password).IsRequired();
            });
        }
    }
}