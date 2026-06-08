using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DevReview.Application.DTOs;
using DevReview.Application.Interfaces;
using DevReview.Domain.Enums;
using MediatR;

namespace DevReview.Application.ReviewRequests
{
    public class GetPublicReviewRequestsHandler : IRequestHandler<GetPublicReviewRequestsQuery, IReadOnlyList<ReviewRequestDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetPublicReviewRequestsHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IReadOnlyList<ReviewRequestDto>> Handle(GetPublicReviewRequestsQuery request, CancellationToken cancellationToken)
        {
            var items = await _unitOfWork.ReviewRequests.FindAsync(
                x => x.IsPublic && x.Status == ReviewStatus.Completed,
                cancellationToken);

            return items
                .OrderByDescending(x => x.CompletedAt)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => _mapper.Map<ReviewRequestDto>(x))
                .ToList();
        }
    }
}
