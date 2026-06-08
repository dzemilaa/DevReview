using DevReview.Application.OfficeHours;
using FluentValidation;

namespace DevReview.Application.Validators
{
    public class SubmitOfficeHourFeedbackCommandValidator : AbstractValidator<SubmitOfficeHourFeedbackCommand>
    {
        public SubmitOfficeHourFeedbackCommandValidator()
        {
            RuleFor(x => x.Impression).NotEmpty().MaximumLength(2000);
        }
    }
}
