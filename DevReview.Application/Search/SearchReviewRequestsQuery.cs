using System;
using DevReview.Application.DTOs;
using DevReview.Domain.Enums;
using MediatR;

namespace DevReview.Application.Search
{
    public class SearchReviewRequestsQuery : IRequest<PagedResultDto<ReviewRequestDto>>
    {
        public string? SearchLanguage { get; set; }
        public string? Framework { get; set; }
        public string? SearchTag { get; set; }
        public DifficultyLevel? SearchDifficulty { get; set; }
        public ReviewStatus? Status { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public Guid? CurrentUserId { get; set; }
    }
}
