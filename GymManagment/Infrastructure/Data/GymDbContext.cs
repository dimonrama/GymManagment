using Microsoft.EntityFrameworkCore;
using GymManagment.Domain.Models;
using Microsoft.Data.Sqlite;

namespace GymManagment.Infrastructure.Data
{
    public class GymDbContext : DbContext 
    { 
        public DbSet<Trainer> Trainers { get; set; }
        public DbSet<Member> Members { get; set; }       

        public GymDbContext() { } // для миграций конструктор

        public GymDbContext(DbContextOptions<GymDbContext> options) : base(options) { } // информация о подлкючении, передается в настрйоки базового класса

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("Data Source=gym.db");
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Member>().HasOne(m => m.Trainer).WithMany(t => t.Members).HasForeignKey(m => m.TrainerId).OnDelete(DeleteBehavior.SetNull);
        }
    }
}
