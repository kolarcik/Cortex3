using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Scoreboard.Api.Controllers;
using Scoreboard.Api.Data;
using Xunit;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Scoreboard.Tests
{
    public class ScoresControllerTests
    {
        private AppDbContext CreateContext()
        {
            var opts = new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase("testdb").Options;
            return new AppDbContext(opts);
        }

        [Fact]
        public async Task Post_and_Get_Workflow()
        {
            var db = CreateContext();
            var inMemorySettings = new Dictionary<string,string>();
            IConfiguration config = new ConfigurationBuilder().AddInMemoryCollection(inMemorySettings).Build();
            var controller = new ScoresController(db, config);
            var dto = new ScoresController.ScoreDto { Name = "Alice", Score = 123 };
            var postRes = await controller.Post(dto) as CreatedAtActionResult;
            Assert.NotNull(postRes);
            var getRes = await controller.Get(10, "desc") as OkObjectResult;
            Assert.NotNull(getRes);
        }

        [Fact]
        public async Task Delete_Forbid_Without_Token()
        {
            var db = CreateContext();
            var config = new ConfigurationBuilder().AddInMemoryCollection().Build();
            var controller = new ScoresController(db, config);
            var del = await controller.Delete(1);
            Assert.IsType<ForbidResult>(del);
        }
    }
}
