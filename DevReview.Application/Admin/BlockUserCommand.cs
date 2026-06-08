using System;
using MediatR;

namespace DevReview.Application.Admin
{
    public class BlockUserCommand : IRequest<Unit>
    {
        public Guid UserId { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}
