using Microsoft.EntityFrameworkCore;
using ScoreApi.Models;

namespace ScoreApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Score> Scores { get; set; }
    }
}
