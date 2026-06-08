using DevReview.Application.ReviewRequests;
using FluentValidation;

namespace DevReview.Application.Validators
{
    public class AddCommentCommandValidator : AbstractValidator<AddCommentCommand>
    {
        public AddCommentCommandValidator()
        {
            RuleFor(x => x.Content).NotEmpty().MaximumLength(2000);
            RuleFor(x => x.StartLine).GreaterThanOrEqualTo(0).When(x => x.StartLine.HasValue);
            RuleFor(x => x.EndLine).GreaterThanOrEqualTo(0).When(x => x.EndLine.HasValue);
            RuleFor(x => x.SuggestedCode).MaximumLength(8000);
        }
    }
}
