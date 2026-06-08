using DevReview.Application.ReviewRequests;
using FluentValidation;

namespace DevReview.Application.Validators
{
    public class SubmitFinalReviewCommandValidator : AbstractValidator<SubmitFinalReviewCommand>
    {
        public SubmitFinalReviewCommandValidator()
        {
            RuleFor(x => x.Summary).NotEmpty().MaximumLength(2000);
            RuleFor(x => x.PriorityFixes).NotEmpty().MaximumLength(2000);
            RuleFor(x => x.QualityScore).InclusiveBetween(1, 5);
        }
    }
}
