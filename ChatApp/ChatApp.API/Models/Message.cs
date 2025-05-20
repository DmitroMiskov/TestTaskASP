using System;
using System.ComponentModel.DataAnnotations;

namespace ChatApp.Models
{
    public class Message
    {
        public int Id { get; set; }
        
        [Required]
        public string Username { get; set; } = string.Empty; // Змінено з UserName на Username
        
        [Required]
        public string Content { get; set; } = string.Empty;
        
        public DateTime Timestamp { get; set; }
        
        public float? SentimentScore { get; set; } // Додали властивість SentimentScore
        
        public string SentimentType => SentimentScore.HasValue
            ? (SentimentScore > 0.75 ? "positive" : SentimentScore < 0.25 ? "negative" : "neutral")
            : "unknown";
    }
}