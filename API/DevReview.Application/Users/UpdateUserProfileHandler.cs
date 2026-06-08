using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DevReview.Application.DTOs;
using DevReview.Application.DTOs.Auth;
using DevReview.Application.Interfaces;
using MediatR;

namespace DevReview.Application.Users
{
    public class UpdateUserProfileHandler : IRequestHandler<UpdateUserProfileCommand, UserDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public UpdateUserProfileHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<UserDto> Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(_currentUserService.UserId, cancellationToken)
                ?? throw new KeyNotFoundException("User not found.");

            user.DisplayName = request.DisplayName.Trim();
            user.Bio = request.Bio?.Trim();
            user.GitHubUrl = request.GitHubUrl?.Trim();
            user.YearsOfExperience = request.YearsOfExperience;
            user.HourlyRate = request.HourlyRate;
            user.AvailableHoursPerWeek = request.AvailableHoursPerWeek;

            _unitOfWork.Users.Update(user);

            var existingLanguages = await _unitOfWork.UserLanguages.FindAsync(x => x.UserId == user.Id, cancellationToken);
            foreach (var lang in existingLanguages)
            {
                _unitOfWork.UserLanguages.Remove(lang);
            }

            foreach (var langDto in request.UserLanguages)
            {
                await _unitOfWork.UserLanguages.AddAsync(new Domain.Entities.UserLanguage
                {
                    UserId = user.Id,
                    Language = langDto.Language,
                    Proficiency = langDto.Proficiency
                }, cancellationToken);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var reputations = await _unitOfWork.MentorReputations.FindAsync(x => x.MentorId == user.Id, cancellationToken);
            var userDto = _mapper.Map<UserDto>(user);
            userDto.LanguagesExpertise = reputations.Select(x => new LanguageExpertiseDto
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
