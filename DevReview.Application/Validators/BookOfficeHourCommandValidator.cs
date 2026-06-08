using DevReview.Application.OfficeHours;
using FluentValidation;

namespace DevReview.Application.Validators
{
    public class BookOfficeHourCommandValidator : AbstractValidator<BookOfficeHourCommand>
    {
        public BookOfficeHourCommandValidator()
        {
            RuleFor(x => x.BookingDescription).MaximumLength(1000);
        }
    }
}
