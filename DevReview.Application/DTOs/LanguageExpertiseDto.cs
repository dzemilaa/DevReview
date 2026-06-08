namespace DevReview.Application.DTOs
{
    public class LanguageExpertiseDto
    {
        public string Language { get; set; } = string.Empty;
        public decimal Score { get; set; }
        public int TotalReviews { get; set; }
        public int Score1Count { get; set; }
        public int Score2Count { get; set; }
        public int Score3Count { get; set; }
        public int Score4Count { get; set; }
        public int Score5Count { get; set; }
    }
}
