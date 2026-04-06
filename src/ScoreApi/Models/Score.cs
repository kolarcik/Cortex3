using System;
using System.ComponentModel.DataAnnotations;

namespace ScoreApi.Models
{
    public class Score
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        [Range(0, int.MaxValue)]
        public int Value { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
