namespace DevReview.Application.DTOs
{
    public class AdminStatisticsDto
    {
        public int TotalUsers { get; set; }
        public int TotalMentors { get; set; }
        public int TotalAuthors { get; set; }
        public int TotalReviewRequests { get; set; }
        public int TotalCompletedReviews { get; set; }
        public int TotalOfficeHourBookings { get; set; }
        public int TotalActiveUsers { get; set; }
    }
}
