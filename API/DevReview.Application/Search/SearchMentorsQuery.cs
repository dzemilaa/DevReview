using DevReview.Application.DTOs;
using MediatR;

namespace DevReview.Application.Search
{
    public class SearchMentorsQuery : IRequest<PagedResultDto<MentorRankingDto>>
    {
        public string? Query { get; set; }
        public string? Language { get; set; }
        public decimal? MinimumRating { get; set; }
        public int? MinimumYearsOfExperience { get; set; }
        public decimal? MaxHourlyRate { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
