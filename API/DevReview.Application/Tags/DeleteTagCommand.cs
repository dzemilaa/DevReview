using System;
using MediatR;

namespace DevReview.Application.Tags
{
    public class DeleteTagCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
    }
}
