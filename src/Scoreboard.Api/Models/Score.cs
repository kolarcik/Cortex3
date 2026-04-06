using System;
using System.ComponentModel.DataAnnotations;

namespace Scoreboard.Api.Models
{
    public class Score
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;
        [Range(0, int.MaxValue)]
        public int Value { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
