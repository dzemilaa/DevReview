using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DevReview.Application.DTOs;
using DevReview.Application.Interfaces;
using DevReview.Domain.Entities;
using MediatR;

namespace DevReview.Application.Mentors
{
    public class FollowMentorCommandHandler : IRequestHandler<FollowMentorCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;

        public FollowMentorCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(FollowMentorCommand request, CancellationToken cancellationToken)
        {
            if (request.FollowerId == request.MentorId)
                throw new InvalidOperationException("You cannot follow yourself.");

            var mentor = await _unitOfWork.Users.GetByIdAsync(request.MentorId, cancellationToken)
                ?? throw new KeyNotFoundException("Mentor not found.");

            var existing = await _unitOfWork.UserFollows.FindAsync(
                x => x.FollowerId == request.FollowerId && x.MentorId == request.MentorId,
                cancellationToken);

            if (existing.Any())
                return Unit.Value;

            await _unitOfWork.UserFollows.AddAsync(new UserFollow
            {
                FollowerId = request.FollowerId,
                MentorId = request.MentorId
            }, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }

    public class UnfollowMentorCommandHandler : IRequestHandler<UnfollowMentorCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UnfollowMentorCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(UnfollowMentorCommand request, CancellationToken cancellationToken)
        {
            var follows = await _unitOfWork.UserFollows.FindAsync(
                x => x.FollowerId == request.FollowerId && x.MentorId == request.MentorId,
                cancellationToken);

            foreach (var follow in follows)
                _unitOfWork.UserFollows.Remove(follow);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }

    public class GetFollowedMentorIdsQueryHandler : IRequestHandler<GetFollowedMentorIdsQuery, IReadOnlyList<Guid>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetFollowedMentorIdsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IReadOnlyList<Guid>> Handle(GetFollowedMentorIdsQuery request, CancellationToken cancellationToken)
        {
            var follows = await _unitOfWork.UserFollows.FindAsync(
                x => x.FollowerId == request.FollowerId,
                cancellationToken);

            return follows.Select(x => x.MentorId).ToList();
        }
    }

    public class GetFollowedMentorsQueryHandler : IRequestHandler<GetFollowedMentorsQuery, IReadOnlyList<MentorRankingDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetFollowedMentorsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IReadOnlyList<MentorRankingDto>> Handle(GetFollowedMentorsQuery request, CancellationToken cancellationToken)
        {
            var follows = await _unitOfWork.UserFollows.FindAsync(
                x => x.FollowerId == request.FollowerId, cancellationToken);

            var mentorIds = follows.Select(x => x.MentorId).ToList();
            if (mentorIds.Count == 0)
                return new List<MentorRankingDto>();

            var users = (await _unitOfWork.Users.FindAsync(x => mentorIds.Contains(x.Id), cancellationToken))
                .ToDictionary(x => x.Id);
            var reputationsByMentor = (await _unitOfWork.MentorReputations.FindAsync(x => mentorIds.Contains(x.MentorId), cancellationToken))
                .GroupBy(x => x.MentorId)
                .ToDictionary(g => g.Key, g => g.ToList());
            var languagesByUser = (await _unitOfWork.UserLanguages.FindAsync(l => mentorIds.Contains(l.UserId), cancellationToken))
                .GroupBy(l => l.UserId)
                .ToDictionary(g => g.Key, g => g.Select(l => l.Language).ToList());

            var result = new List<MentorRankingDto>();
            foreach (var mentorId in mentorIds)
            {
                if (!users.TryGetValue(mentorId, out var user)) continue;

                reputationsByMentor.TryGetValue(user.Id, out var reputations);
                languagesByUser.TryGetValue(user.Id, out var profileLanguages);

                result.Add(new MentorRankingDto
                {
                    MentorId = user.Id,
                    DisplayName = user.DisplayName,
                    UserName = user.UserName,
                    Bio = user.Bio,
                    HourlyRate = user.HourlyRate,
                    AvailableHoursPerWeek = user.AvailableHoursPerWeek,
                    OverallRating = user.AverageMentorRating,
                    TotalReviews = user.TotalReviews,
                    YearsOfExperience = user.YearsOfExperience,
                    ProfileLanguages = profileLanguages ?? new List<string>(),
                    LanguagesExpertise = (reputations ?? new List<MentorReputation>()).Select(r => new LanguageExpertiseDto
                    {
                        Language = r.Language,
                        Score = r.AverageScore,
                        TotalReviews = r.TotalReviews,
                        Score1Count = r.Score1Count,
                        Score2Count = r.Score2Count,
                        Score3Count = r.Score3Count,
                        Score4Count = r.Score4Count,
                        Score5Count = r.Score5Count,
                    }).ToList(),
                });
            }

            return result.OrderBy(x => x.DisplayName).ToList();
        }
    }
}
