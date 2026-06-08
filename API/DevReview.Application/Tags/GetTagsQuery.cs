using System.Collections.Generic;
using DevReview.Application.DTOs;
using MediatR;

namespace DevReview.Application.Tags
{
    public class GetTagsQuery : IRequest<IReadOnlyList<TagDto>>
    {
    }
}
