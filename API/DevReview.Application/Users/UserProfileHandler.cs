using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DevReview.Application.DTOs;
using DevReview.Application.DTOs.Auth;
using DevReview.Application.Interfaces;
using DevReview.Application.Users;
using MediatR;

namespace DevReview.Application.Users
{
    public class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQuery, UserDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public GetUserProfileQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<UserDto> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(_currentUserService.UserId, cancellationToken);
            if (user == null)
            {
                throw new System.Exception("User not found.");
            }

            var reputations = await _unitOfWork.MentorReputations.FindAsync(x => x.MentorId == user.Id, cancellationToken);
            var languageExpertise = reputations.Select(x => new LanguageExpertiseDto
            {
                Language = x.Language,
                Score = x.AverageScore,
                TotalReviews = x.TotalReviews,
                Score1Count = x.Score1Count,
                Score2Count = x.Score2Count,
                Score3Count = x.Score3Count,
                Score4Count = x.Score4Count,
                Score5Count = x.Score5Count,
            }).ToList();

            var userDto = _mapper.Map<UserDto>(user);
            userDto.LanguagesExpertise = languageExpertise;

            var userLanguages = await _unitOfWork.UserLanguages.FindAsync(x => x.UserId == user.Id, cancellationToken);
            userDto.UserLanguages = userLanguages.Select(x => new UserLanguageDto
            {
                Language = x.Language,
                Proficiency = x.Proficiency
            }).ToList();

            return userDto;
        }
    }
}
