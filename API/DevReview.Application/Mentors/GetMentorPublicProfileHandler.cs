using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DevReview.Application.DTOs;
using DevReview.Application.Interfaces;
using DevReview.Domain.Enums;
using MediatR;

namespace DevReview.Application.Mentors
{
    public class GetMentorPublicProfileHandler : IRequestHandler<GetMentorPublicProfileQuery, MentorPublicProfileDto?>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetMentorPublicProfileHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<MentorPublicProfileDto?> Handle(GetMentorPublicProfileQuery request, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(request.MentorId, cancellationToken);
            if (user == null || user.Role != UserRoles.Mentor)
                return null;

            var reputations = await _unitOfWork.MentorReputations.FindAsync(
                x => x.MentorId == request.MentorId, cancellationToken);

            var userLanguages = await _unitOfWork.UserLanguages.FindAsync(
                l => l.UserId == request.MentorId, cancellationToken);

            var completedReviews = await _unitOfWork.ReviewRequests.FindAsync(
                x => x.MentorId == request.MentorId
                     && x.IsPublic
                     && x.Status == ReviewStatus.Completed,
                cancellationToken);

            return new MentorPublicProfileDto
            {
                Id = user.Id,
                DisplayName = user.DisplayName,
                UserName = user.UserName,
                Bio = user.Bio,
                GitHubUrl = user.GitHubUrl,
                YearsOfExperience = user.YearsOfExperience,
                HourlyRate = user.HourlyRate,
                AvailableHoursPerWeek = user.AvailableHoursPerWeek,
                AverageMentorRating = user.AverageMentorRating,
                TotalReviews = user.TotalReviews,
                ProfileLanguages = userLanguages.Select(l => l.Language).ToList(),
                LanguagesExpertise = reputations.Select(r => new LanguageExpertiseDto
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
                PublicReviews = completedReviews
                    .OrderByDescending(x => x.CreatedAt)
                    .Select(x => _mapper.Map<ReviewRequestDto>(x))
                    .ToList(),
            };
        }
    }
}
