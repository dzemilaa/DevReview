using DevReview.Application.Users;
using FluentValidation;

namespace DevReview.Application.Validators
{
    public class UpdateUserProfileCommandValidator : AbstractValidator<UpdateUserProfileCommand>
    {
        public UpdateUserProfileCommandValidator()
        {
            RuleFor(x => x.DisplayName).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Bio).MaximumLength(2000);
            RuleFor(x => x.GitHubUrl).MaximumLength(500);
            RuleFor(x => x.YearsOfExperience).GreaterThanOrEqualTo(0).LessThanOrEqualTo(60);
            RuleFor(x => x.HourlyRate).GreaterThanOrEqualTo(0);
            RuleFor(x => x.AvailableHoursPerWeek).GreaterThanOrEqualTo(0).LessThanOrEqualTo(168);
        }
    }
}
