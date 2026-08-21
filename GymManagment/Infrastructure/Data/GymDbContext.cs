using Microsoft.EntityFrameworkCore;
using GymManagment.Domain.Models;


namespace GymManagment.Infrastructure.Data
{
    public class GymDbContext : DbContext 
    { 
        public DbSet<Trainer> Trainers { get; set; }
        public DbSet<Member> Members { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public GymDbContext() { } // для миграций конструктор

        public GymDbContext(DbContextOptions<GymDbContext> options) : base(options) { } // информация о подлкючении, передается в настрйоки базового класса

        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Member>().HasOne(m => m.Trainer).WithMany(t => t.Members).HasForeignKey(m => m.TrainerId).OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<User>().Property(u => u.Role).HasConversion<string>();
        }
    }
}
