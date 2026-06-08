using System;
using MediatR;
using DevReview.Application.DTOs;

namespace DevReview.Application.Tags
{
    public class UpdateTagCommand : IRequest<TagDto>
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
