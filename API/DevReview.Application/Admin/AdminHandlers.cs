using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DevReview.Application.DTOs;
using DevReview.Application.Interfaces;
using DevReview.Domain.Enums;
using DevReview.Domain.Entities;
using MediatR;

namespace DevReview.Application.Admin
{
    public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, IReadOnlyList<AdminUserDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetUsersQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IReadOnlyList<AdminUserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
        {
            var users = await _unitOfWork.Users.GetAllAsync(cancellationToken);
            return users.Select(_mapper.Map<AdminUserDto>).ToList();
        }
    }

    public class BlockUserCommandHandler : IRequestHandler<BlockUserCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;

        public BlockUserCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(BlockUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(request.UserId, cancellationToken);
            if (user == null)
            {
                throw new ApplicationException("User not found.");
            }

            user.IsBlocked = true;
            user.BlockedAt = DateTime.UtcNow;
            user.BlockReason = request.Reason;
            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }

    public class UnblockUserCommandHandler : IRequestHandler<UnblockUserCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UnblockUserCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(UnblockUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(request.UserId, cancellationToken);
            if (user == null)
            {
                throw new ApplicationException("User not found.");
            }

            user.IsBlocked = false;
            user.BlockedAt = null;
            user.BlockReason = null;
            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }

    public class DeleteReviewRequestCommandHandler : IRequestHandler<DeleteReviewRequestCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteReviewRequestCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(DeleteReviewRequestCommand request, CancellationToken cancellationToken)
        {
            var entity = await _unitOfWork.ReviewRequests.GetByIdAsync(request.ReviewRequestId, cancellationToken)
                ?? throw new KeyNotFoundException("Review request not found.");

            _unitOfWork.ReviewRequests.Remove(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }

    public class DeleteCommentCommandHandler : IRequestHandler<DeleteCommentCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteCommentCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
        {
            var comment = await _unitOfWork.Comments.GetByIdAsync(request.CommentId, cancellationToken)
                ?? throw new KeyNotFoundException("Comment not found.");

            _unitOfWork.Comments.Remove(comment);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }

    public class GetSystemStatisticsQueryHandler : IRequestHandler<GetSystemStatisticsQuery, AdminStatisticsDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetSystemStatisticsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<AdminStatisticsDto> Handle(GetSystemStatisticsQuery request, CancellationToken cancellationToken)
        {
            var users = await _unitOfWork.Users.GetAllAsync(cancellationToken);
            var reviewRequests = await _unitOfWork.ReviewRequests.GetAllAsync(cancellationToken);
            var reviews = await _unitOfWork.Reviews.GetAllAsync(cancellationToken);
            var officeHours = await _unitOfWork.OfficeHours.GetAllAsync(cancellationToken);

            return new AdminStatisticsDto
            {
                TotalUsers = users.Count,
                TotalMentors = users.Count(x => x.Role == UserRoles.Mentor),
                TotalAuthors = users.Count(x => x.Role == UserRoles.Author),
                TotalReviewRequests = reviewRequests.Count,
                TotalCompletedReviews = reviews.Count,
                TotalOfficeHourBookings = officeHours.Count(x => x.Status == OfficeHourStatus.Booked || x.Status == OfficeHourStatus.Completed),
                TotalActiveUsers = users.Count(x => x.IsActive && !x.IsBlocked)
            };
        }
    }
}
