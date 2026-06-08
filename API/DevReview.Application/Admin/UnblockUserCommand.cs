using System;
using MediatR;

namespace DevReview.Application.Admin
{
    public class UnblockUserCommand : IRequest<Unit>
    {
        public Guid UserId { get; set; }
    }
}
