using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Scoreboard.Api.Data;
using Scoreboard.Api.Models;

namespace Scoreboard.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ScoresController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _config;
        public ScoresController(AppDbContext db, IConfiguration config)
        {
            _db = db; _config = config;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] int limit = 10, [FromQuery] string order = "desc")
        {
            var q = _db.Scores.AsQueryable();
            q = order.ToLower() == "asc" ? q.OrderBy(s => s.Value) : q.OrderByDescending(s => s.Value);
            var items = await q.Take(limit).ToListAsync();
            return Ok(items);
        }

        public class ScoreDto { public string? Name { get; set; } public int? Score { get; set; } }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ScoreDto dto)
        {
            if (dto == null) return BadRequest("Missing body");
            if (string.IsNullOrWhiteSpace(dto.Name) || dto.Name.Length > 50) return BadRequest("Invalid name");
            if (dto.Score == null || dto.Score < 0) return BadRequest("Invalid score");

            var s = new Score { Name = dto.Name.Trim(), Value = dto.Score.Value };
            _db.Scores.Add(s);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = s.Id }, s);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var token = Request.Headers["X-Admin-Token"].FirstOrDefault();
            var admin = _config.GetValue<string>("ADMIN_TOKEN") ?? Environment.GetEnvironmentVariable("ADMIN_TOKEN");
            if (string.IsNullOrEmpty(admin) || token != admin) return Forbid();

            var s = await _db.Scores.FindAsync(id);
            if (s == null) return NotFound();
            _db.Scores.Remove(s);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
