using DevReview.Application.ReviewRequests;
using FluentValidation;

namespace DevReview.Application.Validators
{
    public class AbandonReviewRequestCommandValidator : AbstractValidator<AbandonReviewRequestCommand>
    {
        public AbandonReviewRequestCommandValidator()
        {
            RuleFor(x => x.Reason).NotEmpty().MaximumLength(1000);
        }
    }
}
