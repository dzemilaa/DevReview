using DevReview.Application.OfficeHours;
using FluentValidation;

namespace DevReview.Application.Validators
{
    public class CancelOfficeHourCommandValidator : AbstractValidator<CancelOfficeHourCommand>
    {
        public CancelOfficeHourCommandValidator()
        {
            RuleFor(x => x.CancellationReason).MaximumLength(1000);
        }
    }
}
