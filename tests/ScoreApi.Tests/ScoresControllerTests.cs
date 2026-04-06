using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ScoreApi.Controllers;
using ScoreApi.Data;
using Xunit;

namespace ScoreApi.Tests
{
    public class ScoresControllerTests
    {
        private AppDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString()).Options;
            return new AppDbContext(options);
        }

        [Fact]
        public async Task Post_AddsScore()
        {
            using var ctx = CreateContext();
            var cfg = new ConfigurationBuilder().AddInMemoryCollection().Build();
            var ctl = new ScoresController(ctx, cfg);
            var res = await ctl.Post(new ScoreCreateDto("Alice", 42));
            Assert.NotNull(res);
            Assert.Equal(1, await ctx.Scores.CountAsync());
        }

        [Fact]
        public async Task Get_ReturnsList()
        {
            using var ctx = CreateContext();
            ctx.Scores.Add(new ScoreApi.Models.Score { Name = "A", Value = 10 });
            ctx.Scores.Add(new ScoreApi.Models.Score { Name = "B", Value = 20 });
            await ctx.SaveChangesAsync();
            var cfg = new ConfigurationBuilder().AddInMemoryCollection().Build();
            var ctl = new ScoresController(ctx, cfg);
            var ok = await ctl.Get(10, "desc");
            Assert.NotNull(ok);
        }

        [Fact]
        public async Task Delete_Unauthorized_WhenTokenMissing()
        {
            using var ctx = CreateContext();
            ctx.Scores.Add(new ScoreApi.Models.Score { Name = "X", Value = 1 });
            await ctx.SaveChangesAsync();
            var cfg = new ConfigurationBuilder().AddInMemoryCollection().Build();
            var ctl = new ScoresController(ctx, cfg);
            var result = await ctl.Delete(1);
            Assert.IsType<Microsoft.AspNetCore.Mvc.UnauthorizedResult>(result);
        }
    }
}
