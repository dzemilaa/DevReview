using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DevReview.Application.DTOs;
using DevReview.Application.Interfaces;
using DevReview.Domain.Entities;
using MediatR;

namespace DevReview.Application.Search
{
    public class SearchReviewRequestsQueryHandler : IRequestHandler<SearchReviewRequestsQuery, PagedResultDto<ReviewRequestDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SearchReviewRequestsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public Task<PagedResultDto<ReviewRequestDto>> Handle(SearchReviewRequestsQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.ReviewRequests.Query()
                .Where(x => x.IsPublic || x.OwnerId == request.CurrentUserId || x.MentorId == request.CurrentUserId);

            if (!string.IsNullOrWhiteSpace(request.SearchLanguage))
            {
                var language = request.SearchLanguage.Trim().ToLower();
                query = query.Where(x => x.ProgrammingLanguage != null && x.ProgrammingLanguage.ToLower().Contains(language));
            }

            if (!string.IsNullOrWhiteSpace(request.Framework))
            {
                var framework = request.Framework.Trim().ToLower();
                query = query.Where(x => x.Framework != null && x.Framework.ToLower().Contains(framework));
            }

            if (!string.IsNullOrWhiteSpace(request.SearchTag))
            {
                var tag = request.SearchTag.Trim().ToLower();
                query = query.Where(x => x.Tags != null && x.Tags.ToLower().Contains(tag));
            }

            if (request.SearchDifficulty.HasValue)
            {
                query = query.Where(x => x.Difficulty == request.SearchDifficulty.Value);
            }

            if (request.Status.HasValue)
            {
                query = query.Where(x => x.Status == request.Status.Value);
            }

            if (request.MinPrice.HasValue)
            {
                query = query.Where(x => x.Price >= request.MinPrice.Value);
            }

            if (request.MaxPrice.HasValue)
            {
                query = query.Where(x => x.Price <= request.MaxPrice.Value);
            }

            var totalItems = query.Count();
            var pageSize = Math.Max(1, request.PageSize);
            var currentPage = Math.Max(1, request.Page);
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            var items = query
                .OrderByDescending(x => x.CreatedAt)
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize)
                .ToList()
                .Select(_mapper.Map<ReviewRequestDto>)
                .ToList();

            var pageResult = new PagedResultDto<ReviewRequestDto>
            {
                TotalItems = totalItems,
                TotalPages = totalPages,
                CurrentPage = currentPage,
                PageSize = pageSize,
                Items = items
            };

            return Task.FromResult(pageResult);
        }
    }

    public class SearchMentorsQueryHandler : IRequestHandler<SearchMentorsQuery, PagedResultDto<MentorRankingDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SearchMentorsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedResultDto<MentorRankingDto>> Handle(SearchMentorsQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.Users.Query()
                .Where(x => x.Role == DevReview.Domain.Enums.UserRoles.Mentor);

            if (!string.IsNullOrWhiteSpace(request.Query))
            {
                var searchText = request.Query.Trim().ToLower();
                query = query.Where(x => (x.DisplayName != null && x.DisplayName.ToLower().Contains(searchText))
                                         || (x.UserName != null && x.UserName.ToLower().Contains(searchText)));
            }

            if (request.MinimumRating.HasValue)
            {
                query = query.Where(x => x.AverageMentorRating >= request.MinimumRating.Value);
            }

            if (request.MaxHourlyRate.HasValue)
            {
                query = query.Where(x => x.HourlyRate <= request.MaxHourlyRate.Value);
            }

            if (request.MinimumYearsOfExperience.HasValue)
            {
                query = query.Where(x => x.YearsOfExperience >= request.MinimumYearsOfExperience.Value);
            }

            var allMentors = query.ToList();

            if (!string.IsNullOrWhiteSpace(request.Language))
            {
                var language = request.Language.Trim().ToLower();
                var mentorIds = allMentors.Select(x => x.Id).ToList();

                var reputationMatches = (await _unitOfWork.MentorReputations.FindAsync(
                    r => mentorIds.Contains(r.MentorId) && r.Language.ToLower().Contains(language),
                    cancellationToken)).Select(r => r.MentorId).ToHashSet();

                var languageMatches = (await _unitOfWork.UserLanguages.FindAsync(
                    l => mentorIds.Contains(l.UserId) && l.Language.ToLower().Contains(language),
                    cancellationToken)).Select(l => l.UserId).ToHashSet();

                allMentors = allMentors.Where(x => reputationMatches.Contains(x.Id) || languageMatches.Contains(x.Id)).ToList();
            }

            var totalItems = allMentors.Count;
            var pageSize = Math.Max(1, request.PageSize);
            var currentPage = Math.Max(1, request.Page);
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            var mentors = allMentors
                .OrderByDescending(x => x.AverageMentorRating)
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var mentorIdList = mentors.Select(x => x.Id).ToList();
            var allReputations = (await _unitOfWork.MentorReputations.FindAsync(
                r => mentorIdList.Contains(r.MentorId), cancellationToken)).ToList();
            var allProfileLanguages = (await _unitOfWork.UserLanguages.FindAsync(
                l => mentorIdList.Contains(l.UserId), cancellationToken)).ToList();

            var items = mentors.Select(mentor => new MentorRankingDto
            {
                MentorId = mentor.Id,
                DisplayName = mentor.DisplayName,
                UserName = mentor.UserName,
                Bio = mentor.Bio,
                HourlyRate = mentor.HourlyRate,
                AvailableHoursPerWeek = mentor.AvailableHoursPerWeek,
                OverallRating = mentor.AverageMentorRating,
                TotalReviews = mentor.TotalReviews,
                YearsOfExperience = mentor.YearsOfExperience,
                ProfileLanguages = allProfileLanguages.Where(l => l.UserId == mentor.Id).Select(l => l.Language).ToList(),
                LanguagesExpertise = allReputations.Where(r => r.MentorId == mentor.Id).Select(x => new LanguageExpertiseDto
                {
                    Language = x.Language,
                    Score = x.AverageScore,
                    TotalReviews = x.TotalReviews,
                    Score1Count = x.Score1Count,
                    Score2Count = x.Score2Count,
                    Score3Count = x.Score3Count,
                    Score4Count = x.Score4Count,
                    Score5Count = x.Score5Count,
                }).ToList()
            }).ToList();

            var pageResult = new PagedResultDto<MentorRankingDto>
            {
                TotalItems = totalItems,
                TotalPages = totalPages,
                CurrentPage = currentPage,
                PageSize = pageSize,
                Items = items
            };

            return pageResult;
        }
    }
}
