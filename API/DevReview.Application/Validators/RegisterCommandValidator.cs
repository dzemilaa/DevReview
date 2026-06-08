using DevReview.Application.Auth;
using FluentValidation;

namespace DevReview.Application.Validators
{
    public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
    {
        public RegisterCommandValidator()
        {
            RuleFor(x => x.UserName).NotEmpty().MinimumLength(4);
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.DisplayName).NotEmpty().MaximumLength(50);
            RuleFor(x => x.Password).NotEmpty().MinimumLength(8);
            RuleFor(x => x.Role)
                .NotEmpty()
                .Must(r => r == DevReview.Domain.Enums.UserRoles.Author
                        || r == DevReview.Domain.Enums.UserRoles.Mentor)
                .WithMessage("Role must be 'Author' or 'Mentor'.");
        }
    }
}
