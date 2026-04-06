using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScoreApi.Data;
using ScoreApi.Models;

namespace ScoreApi.Controllers
{
    [ApiController]
    [Route("scores")]
    public class ScoresController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _cfg;
        public ScoresController(AppDbContext db, IConfiguration cfg)
        {
            _db = db;
            _cfg = cfg;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ScoreCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name) || dto.Name.Length > 100) return BadRequest("Invalid name");
            if (dto.Score < 0) return BadRequest("Score must be non-negative");
            var s = new Score { Name = dto.Name.Trim(), Value = dto.Score };
            _db.Scores.Add(s);
            await _db.SaveChangesAsync();
            return Created(string.Empty, new { id = s.Id, name = s.Name, score = s.Value, createdAt = s.CreatedAt });
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] int limit = 10, [FromQuery] string order = "desc")
        {
            limit = Math.Clamp(limit, 1, 100);
            bool desc = order?.ToLower() != "asc";
            var q = _db.Scores.AsNoTracking();
            q = desc ? q.OrderByDescending(s => s.Value).ThenByDescending(s => s.CreatedAt) : q.OrderBy(s => s.Value).ThenByDescending(s => s.CreatedAt);
            var list = await q.Take(limit).Select(s => new { id = s.Id, name = s.Name, score = s.Value, createdAt = s.CreatedAt }).ToListAsync();
            return Ok(list);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var token = Request.Headers["X-Admin-Token"].FirstOrDefault();
            var expected = _cfg["AdminToken"] ?? string.Empty;
            if (string.IsNullOrEmpty(expected) || token != expected) return Unauthorized();
            var score = await _db.Scores.FindAsync(id);
            if (score == null) return NotFound();
            _db.Scores.Remove(score);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }

    public record ScoreCreateDto(string Name, int Score);
}
