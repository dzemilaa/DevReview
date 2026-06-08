using DevReview.Application.Admin;
using FluentValidation;

namespace DevReview.Application.Validators
{
    public class BlockUserCommandValidator : AbstractValidator<BlockUserCommand>
    {
        public BlockUserCommandValidator()
        {
            RuleFor(x => x.Reason).NotEmpty().MaximumLength(1000);
        }
    }
}
