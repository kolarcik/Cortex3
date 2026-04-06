using Microsoft.EntityFrameworkCore;
using Scoreboard.Api.Models;

namespace Scoreboard.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Score> Scores { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Score>(b => {
                b.HasKey(s => s.Id);
                b.Property(s => s.Name).HasMaxLength(50).IsRequired();
                b.Property(s => s.Value).IsRequired();
                b.Property(s => s.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });
        }
    }
}
