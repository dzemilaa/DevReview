using MediatR;
using DevReview.Application.DTOs;

namespace DevReview.Application.Tags
{
    public class CreateTagCommand : IRequest<TagDto>
    {
        public string Name { get; set; } = string.Empty;
    }
}
