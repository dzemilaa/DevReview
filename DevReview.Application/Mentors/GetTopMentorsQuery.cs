using DevReview.Application.DTOs;
using MediatR;

namespace DevReview.Application.Mentors
{
    public class GetTopMentorsQuery : IRequest<IReadOnlyList<MentorRankingDto>>
    {
        public string? Language { get; set; }
        public decimal? MinimumRating { get; set; }
        public int? MinimumYearsOfExperience { get; set; }
        /// <summary>When "week", only reviews completed in the last 7 days count.</summary>
        public string? Period { get; set; }
    }
}
