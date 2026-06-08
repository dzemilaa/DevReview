using DevReview.Application.ReviewRequests;
using FluentValidation;

namespace DevReview.Application.Validators
{
    public class ClaimReviewRequestCommandValidator : AbstractValidator<ClaimReviewRequestCommand>
    {
        public ClaimReviewRequestCommandValidator()
        {
            RuleFor(x => x.ReviewRequestId).NotEmpty();
            RuleFor(x => x.MentorId).NotEmpty();
        }
    }
}
