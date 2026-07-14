using Microsoft.EntityFrameworkCore;
using TaskManagementApi.Entities;

namespace TaskManagementApi.Data 
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Entities.Task> Tasks { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Entities.Task>(entity =>
            {
                // TODO
                // entity.HasIndex(t => t.UserId); // For future auth
            });
        }
    }
}
