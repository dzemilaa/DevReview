using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DevReview.Application.DTOs;
using DevReview.Application.Interfaces;
using DevReview.Domain.Entities;
using MediatR;

namespace DevReview.Application.Mentors
{
    public class GetTopMentorsQueryHandler : IRequestHandler<GetTopMentorsQuery, IReadOnlyList<MentorRankingDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetTopMentorsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IReadOnlyList<MentorRankingDto>> Handle(GetTopMentorsQuery request, CancellationToken cancellationToken)
        {
            var useWeekly = string.Equals(request.Period, "week", StringComparison.OrdinalIgnoreCase);

            if (useWeekly)
            {
                return await BuildWeeklyRankingAsync(request, cancellationToken);
            }

            var reputations = await _unitOfWork.MentorReputations.FindAsync(x =>
                string.IsNullOrEmpty(request.Language) || x.Language == request.Language,
                cancellationToken);

            var mentors = reputations
                .GroupBy(x => x.MentorId)
                .Select(group => new
                {
                    MentorId = group.Key,
                    OverallScore = group.Sum(x => x.AverageScore * x.TotalReviews) / Math.Max(group.Sum(x => x.TotalReviews), 1),
                    TotalReviews = group.Sum(x => x.TotalReviews),
                    Languages = group.Select(x => x).ToList()
                })
                .Where(x => !request.MinimumRating.HasValue || x.OverallScore >= request.MinimumRating.Value)
                .ToList();

            var mentorGroupIds = mentors.Select(x => x.MentorId).ToList();
            var users = (await _unitOfWork.Users.FindAsync(x => mentorGroupIds.Contains(x.Id), cancellationToken))
                .ToDictionary(x => x.Id);
            var allUserLanguages = (await _unitOfWork.UserLanguages.FindAsync(l => mentorGroupIds.Contains(l.UserId), cancellationToken))
                .GroupBy(l => l.UserId)
                .ToDictionary(g => g.Key, g => g.Select(l => l.Language).ToList());

            var ranking = new List<MentorRankingDto>();
            foreach (var mentorGroup in mentors)
            {
                if (!users.TryGetValue(mentorGroup.MentorId, out var user)) continue;

                if (request.MinimumYearsOfExperience.HasValue && user.YearsOfExperience < request.MinimumYearsOfExperience.Value)
                    continue;

                allUserLanguages.TryGetValue(user.Id, out var profileLanguages);
                ranking.Add(new MentorRankingDto
                {
                    MentorId = user.Id,
                    DisplayName = user.DisplayName,
                    UserName = user.UserName,
                    Bio = user.Bio,
                    HourlyRate = user.HourlyRate,
                    AvailableHoursPerWeek = user.AvailableHoursPerWeek,
                    OverallRating = Math.Round(mentorGroup.OverallScore, 2),
                    TotalReviews = user.TotalReviews,
                    YearsOfExperience = user.YearsOfExperience,
                    ProfileLanguages = profileLanguages ?? new List<string>(),
                    LanguagesExpertise = mentorGroup.Languages.Select(x => new LanguageExpertiseDto
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
                });
            }

            return ranking.OrderByDescending(x => x.OverallRating).ThenByDescending(x => x.TotalReviews).ToList();
        }

        private async Task<IReadOnlyList<MentorRankingDto>> BuildWeeklyRankingAsync(
            GetTopMentorsQuery request,
            CancellationToken cancellationToken)
        {
            var since = DateTime.UtcNow.AddDays(-7);
            var weekReviews = (await _unitOfWork.Reviews.FindAsync(r => r.CreatedAt >= since, cancellationToken)).ToList();

            var byMentor = weekReviews
                .Where(r => r.AuthorRatingOfMentor.HasValue)
                .GroupBy(r => r.ReviewerId)
                .Select(g => new
                {
                    MentorId = g.Key,
                    OverallScore = g.Average(r => (double)r.AuthorRatingOfMentor!.Value),
                    TotalReviews = g.Count()
                })
                .Where(x => !request.MinimumRating.HasValue || (decimal)x.OverallScore >= request.MinimumRating.Value)
                .ToList();

            var weekMentorIds = byMentor.Select(x => x.MentorId).ToList();
            var weekUsers = (await _unitOfWork.Users.FindAsync(x => weekMentorIds.Contains(x.Id), cancellationToken))
                .ToDictionary(x => x.Id);
            var weekReputations = (await _unitOfWork.MentorReputations.FindAsync(
                x => weekMentorIds.Contains(x.MentorId)
                     && (string.IsNullOrEmpty(request.Language) || x.Language == request.Language),
                cancellationToken))
                .GroupBy(x => x.MentorId)
                .ToDictionary(g => g.Key, g => g.ToList());
            var weekUserLanguages = (await _unitOfWork.UserLanguages.FindAsync(l => weekMentorIds.Contains(l.UserId), cancellationToken))
                .GroupBy(l => l.UserId)
                .ToDictionary(g => g.Key, g => g.Select(l => l.Language).ToList());

            var ranking = new List<MentorRankingDto>();
            foreach (var mentorGroup in byMentor)
            {
                if (!weekUsers.TryGetValue(mentorGroup.MentorId, out var user)) continue;

                if (request.MinimumYearsOfExperience.HasValue
                    && user.YearsOfExperience < request.MinimumYearsOfExperience.Value)
                    continue;

                weekReputations.TryGetValue(user.Id, out var reputations);
                weekUserLanguages.TryGetValue(user.Id, out var profileLanguages);

                ranking.Add(new MentorRankingDto
                {
                    MentorId = user.Id,
                    DisplayName = user.DisplayName,
                    UserName = user.UserName,
                    Bio = user.Bio,
                    HourlyRate = user.HourlyRate,
                    AvailableHoursPerWeek = user.AvailableHoursPerWeek,
                    OverallRating = Math.Round((decimal)mentorGroup.OverallScore, 2),
                    TotalReviews = mentorGroup.TotalReviews,
                    YearsOfExperience = user.YearsOfExperience,
                    ProfileLanguages = profileLanguages ?? new List<string>(),
                    LanguagesExpertise = (reputations ?? new List<MentorReputation>()).Select(x => new LanguageExpertiseDto
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
                });
            }

            return ranking.OrderByDescending(x => x.OverallRating).ThenByDescending(x => x.TotalReviews).ToList();
        }
    }
}
