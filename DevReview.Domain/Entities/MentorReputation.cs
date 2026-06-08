using System;

namespace DevReview.Domain.Entities
{
    public class MentorReputation
    {
        public Guid MentorId { get; set; }
        public User? Mentor { get; set; }

        public string Language { get; set; } = string.Empty;
        public decimal AverageScore { get; set; }
        public int TotalReviews { get; set; }
        public int Score1Count { get; set; }
        public int Score2Count { get; set; }
        public int Score3Count { get; set; }
        public int Score4Count { get; set; }
        public int Score5Count { get; set; }
    }
}
