using DevReview.Application.ReviewRequests;
using FluentValidation;

namespace DevReview.Application.Validators
{
    public class CreateReviewRequestCommandValidator : AbstractValidator<CreateReviewRequestCommand>
    {
        public CreateReviewRequestCommandValidator()
        {
            RuleFor(x => x.Title).NotEmpty().MaximumLength(120);
            RuleFor(x => x.Description).NotEmpty().MaximumLength(2000);
            RuleFor(x => x.ProgrammingLanguage).NotEmpty().MaximumLength(50);
            RuleFor(x => x.Framework).NotEmpty().MaximumLength(50);
            RuleFor(x => x.Price).GreaterThan(0).When(x => x.IsPaid);
            RuleFor(x => x.Tags).NotNull();
            RuleFor(x => x.CodeFiles).NotNull();
        }
    }
}
