using DevReview.Application.OfficeHours;
using FluentValidation;

namespace DevReview.Application.Validators
{
    public class CreateOfficeHourCommandValidator : AbstractValidator<CreateOfficeHourCommand>
    {
        public CreateOfficeHourCommandValidator()
        {
            RuleFor(x => x.StartTime).Must(startTime => startTime > DateTime.UtcNow)
                .WithMessage("Start time must be in the future.");
            RuleFor(x => x.DurationMinutes).GreaterThan(0);
            RuleFor(x => x.Topic).NotEmpty().MaximumLength(150);
            RuleFor(x => x.Price).GreaterThan(0).When(x => x.Price.HasValue);
        }
    }
}
