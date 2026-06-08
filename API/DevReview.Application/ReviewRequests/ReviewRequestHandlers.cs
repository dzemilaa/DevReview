using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DevReview.Application.DTOs;
using DevReview.Application.Interfaces;
using DevReview.Domain.Entities;
using DevReview.Domain.Enums;
using MediatR;

namespace DevReview.Application.ReviewRequests
{
    public class GetReviewRequestsQueryHandler : IRequestHandler<GetReviewRequestsQuery, IReadOnlyList<ReviewRequestDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetReviewRequestsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IReadOnlyList<ReviewRequestDto>> Handle(GetReviewRequestsQuery request, CancellationToken cancellationToken)
        {
            IReadOnlyList<ReviewRequest> items;

            if (request.OwnerId.HasValue || request.MentorId.HasValue || request.Status.HasValue)
            {
                items = await _unitOfWork.ReviewRequests.FindAsync(x =>
                    (!request.OwnerId.HasValue || x.OwnerId == request.OwnerId)
                    && (!request.MentorId.HasValue || x.MentorId == request.MentorId)
                    && (!request.Status.HasValue || x.Status == request.Status),
                    cancellationToken);
            }
            else
            {
                items = await _unitOfWork.ReviewRequests.GetAllAsync(cancellationToken);
            }

            return items.Select(x => _mapper.Map<ReviewRequestDto>(x)).ToList();
        }
    }

    public class GetReviewRequestByIdQueryHandler : IRequestHandler<GetReviewRequestByIdQuery, ReviewRequestDetailDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetReviewRequestByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ReviewRequestDetailDto> Handle(GetReviewRequestByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await _unitOfWork.GetReviewRequestWithDetailsAsync(request.ReviewRequestId, cancellationToken)
                ?? throw new KeyNotFoundException($"Review request '{request.ReviewRequestId}' was not found.");

            if (request.RequirePublic && !entity.IsPublic)
            {
                throw new UnauthorizedAccessException("This review request is private.");
            }

            var dto = _mapper.Map<ReviewRequestDetailDto>(entity);
            var fileNames = entity.CodeFiles.ToDictionary(f => f.Id, f => f.FileName);
            var flatComments = entity.Comments.Select(c =>
            {
                var mapped = _mapper.Map<CommentDto>(c);
                mapped.AuthorName = c.User?.DisplayName ?? c.User?.UserName ?? "User";
                if (c.CodeFileId.HasValue && fileNames.TryGetValue(c.CodeFileId.Value, out var name))
                {
                    mapped.CodeFilePath = name;
                }
                return mapped;
            }).ToList();
            dto.Comments = BuildCommentTree(flatComments);

            dto.CodeFiles = entity.CodeFiles
                .OrderBy(f => f.OrderIndex)
                .Select(f => _mapper.Map<CodeFileDto>(f))
                .ToList();

            var finalReview = entity.Reviews.OrderByDescending(r => r.CreatedAt).FirstOrDefault();
            if (finalReview is not null)
            {
                dto.FinalReview = new FinalReviewSummaryDto
                {
                    Summary = finalReview.Summary,
                    PriorityFixes = finalReview.PriorityFixes,
                    QualityScore = finalReview.QualityScore,
                    CreatedAt = finalReview.CreatedAt,
                    AuthorRatingOfMentor = finalReview.AuthorRatingOfMentor,
                    MentorRatingOfAuthor = finalReview.MentorRatingOfAuthor,
                };
            }

            return dto;
        }

        private static IList<CommentDto> BuildCommentTree(IList<CommentDto> flat)
        {
            var byId = flat.ToDictionary(c => c.Id);
            foreach (var comment in flat)
            {
                comment.Replies = new List<CommentDto>();
            }

            var roots = new List<CommentDto>();
            foreach (var comment in flat)
            {
                if (comment.ParentCommentId.HasValue && byId.TryGetValue(comment.ParentCommentId.Value, out var parent))
                {
                    parent.Replies.Add(comment);
                }
                else
                {
                    roots.Add(comment);
                }
            }

            return roots.OrderBy(c => c.CreatedAt).ToList();
        }
    }

    public class CreateReviewRequestCommandHandler : IRequestHandler<CreateReviewRequestCommand, ReviewRequestDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateReviewRequestCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ReviewRequestDto> Handle(CreateReviewRequestCommand request, CancellationToken cancellationToken)
        {
            var entity = new ReviewRequest
            {
                Title = request.Title,
                Description = request.Description,
                ProgrammingLanguage = request.ProgrammingLanguage,
                Framework = request.Framework,
                Difficulty = request.Difficulty,
                IsPublic = request.IsPublic,
                IsPaid = request.IsPaid,
                Price = request.Price,
                Tags = request.Tags != null ? string.Join(',', request.Tags) : string.Empty,
                OwnerId = request.OwnerId,
                CodeFiles = request.CodeFiles?.Select(x => new CodeFile
                {
                    FileName = x.FileName,
                    Content = x.Content,
                    OrderIndex = x.OrderIndex
                }).ToList() ?? new List<CodeFile>()
            };

            await _unitOfWork.ReviewRequests.AddAsync(entity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return _mapper.Map<ReviewRequestDto>(entity);
        }
    }

    public class ClaimReviewRequestCommandHandler : IRequestHandler<ClaimReviewRequestCommand, ReviewRequestDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ClaimReviewRequestCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ReviewRequestDto> Handle(ClaimReviewRequestCommand request, CancellationToken cancellationToken)
        {
            var entity = await _unitOfWork.ReviewRequests.GetByIdAsync(request.ReviewRequestId, cancellationToken)
                ?? throw new KeyNotFoundException($"Review request '{request.ReviewRequestId}' was not found.");

            if (entity.OwnerId == request.MentorId)
            {
                throw new InvalidOperationException("You cannot claim your own review request.");
            }

            if (entity.Status != ReviewStatus.Open)
            {
                throw new InvalidOperationException("Only open review requests can be claimed.");
            }

            entity.MentorId = request.MentorId;
            entity.ClaimedAt = DateTime.UtcNow;
            entity.ClaimTimeout = DateTime.UtcNow.AddHours(24);
            entity.Status = ReviewStatus.Claimed;

            _unitOfWork.ReviewRequests.Update(entity);
            await _unitOfWork.Notifications.AddAsync(new Notification
            {
                UserId = entity.OwnerId,
                Message = $"Your review request '{entity.Title}' has been claimed.",
                Type = NotificationType.RequestClaimed
            }, cancellationToken);
            await _unitOfWork.Notifications.AddAsync(new Notification
            {
                UserId = entity.MentorId!.Value,
                Message = $"You claimed review request '{entity.Title}'.",
                Type = NotificationType.RequestClaimed
            }, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return _mapper.Map<ReviewRequestDto>(entity);
        }
    }

    public class AbandonReviewRequestCommandHandler : IRequestHandler<AbandonReviewRequestCommand, ReviewRequestDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AbandonReviewRequestCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ReviewRequestDto> Handle(AbandonReviewRequestCommand request, CancellationToken cancellationToken)
        {
            var entity = await _unitOfWork.ReviewRequests.GetByIdAsync(request.ReviewRequestId, cancellationToken)
                ?? throw new KeyNotFoundException($"Review request '{request.ReviewRequestId}' was not found.");

            if (entity.MentorId != request.MentorId)
            {
                throw new UnauthorizedAccessException("Only the assigned mentor can abandon this review request.");
            }

            if (entity.Status is not ReviewStatus.Claimed and not ReviewStatus.InReview)
            {
                throw new InvalidOperationException("Only claimed or in-review requests can be abandoned.");
            }

            entity.Status = ReviewStatus.Abandoned;
            entity.CompletedAt = DateTime.UtcNow;

            _unitOfWork.ReviewRequests.Update(entity);
            await _unitOfWork.Notifications.AddAsync(new Notification
            {
                UserId = entity.OwnerId,
                Message = $"Your review request '{entity.Title}' was abandoned by the mentor.",
                Type = NotificationType.RequestAbandoned
            }, cancellationToken);
            await _unitOfWork.Notifications.AddAsync(new Notification
            {
                UserId = request.MentorId,
                Message = $"You abandoned review request '{entity.Title}'. Reason: {request.Reason}",
                Type = NotificationType.RequestAbandoned
            }, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return _mapper.Map<ReviewRequestDto>(entity);
        }
    }

    public class AddCommentCommandHandler : IRequestHandler<AddCommentCommand, CommentDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AddCommentCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<CommentDto> Handle(AddCommentCommand request, CancellationToken cancellationToken)
        {
            var reviewRequest = await _unitOfWork.ReviewRequests.GetByIdAsync(request.ReviewRequestId, cancellationToken)
                ?? throw new KeyNotFoundException($"Review request '{request.ReviewRequestId}' was not found.");

            var isAssignedMentor = reviewRequest.MentorId == request.AuthorId;
            var isOwner = reviewRequest.OwnerId == request.AuthorId;
            var isInlineComment = !string.IsNullOrWhiteSpace(request.CodeFilePath) || request.StartLine.HasValue;
            var isGeneralComment = !isInlineComment && request.ParentCommentId == null;

            if (!isGeneralComment && reviewRequest.Status is not ReviewStatus.Claimed and not ReviewStatus.InReview)
            {
                throw new InvalidOperationException("Inline comments and replies can only be added to claimed or in-review requests.");
            }

            if (isInlineComment && !isAssignedMentor)
            {
                throw new UnauthorizedAccessException("Only the assigned mentor can add inline code comments.");
            }

            if (!isGeneralComment && request.ParentCommentId == null && !isAssignedMentor)
            {
                throw new UnauthorizedAccessException("Only the assigned mentor can start a new comment thread.");
            }

            if (!isGeneralComment && reviewRequest.Status == ReviewStatus.Claimed)
            {
                reviewRequest.Status = ReviewStatus.InReview;
                _unitOfWork.ReviewRequests.Update(reviewRequest);
            }

            CodeFile? codeFile = null;
            if (!string.IsNullOrWhiteSpace(request.CodeFilePath))
            {
                codeFile = (await _unitOfWork.CodeFiles.FindAsync(x => x.ReviewRequestId == request.ReviewRequestId && x.FileName == request.CodeFilePath, cancellationToken))
                    .FirstOrDefault();

                if (codeFile is null)
                {
                    throw new KeyNotFoundException($"Code file '{request.CodeFilePath}' was not found for this review request.");
                }
            }

            if (request.ParentCommentId.HasValue)
            {
                var parent = await _unitOfWork.Comments.GetByIdAsync(request.ParentCommentId.Value, cancellationToken)
                    ?? throw new KeyNotFoundException("Parent comment not found.");
                if (parent.ReviewRequestId != request.ReviewRequestId)
                {
                    throw new InvalidOperationException("Parent comment does not belong to this review request.");
                }
            }

            var comment = new Comment
            {
                Content = request.Content,
                Type = request.CommentType,
                StartLine = request.StartLine,
                EndLine = request.EndLine,
                SuggestedCode = request.SuggestedCode,
                UserId = request.AuthorId,
                ReviewRequestId = request.ReviewRequestId,
                CodeFileId = codeFile?.Id,
                ParentCommentId = request.ParentCommentId
            };

            await _unitOfWork.Comments.AddAsync(comment, cancellationToken);
            await _unitOfWork.Notifications.AddAsync(new Notification
            {
                UserId = reviewRequest.OwnerId,
                Message = $"A new comment was posted on review request '{reviewRequest.Title}'.",
                Type = NotificationType.NewComment
            }, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return _mapper.Map<CommentDto>(comment);
        }
    }

    public class ResolveCommentCommandHandler : IRequestHandler<ResolveCommentCommand, CommentDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ResolveCommentCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<CommentDto> Handle(ResolveCommentCommand request, CancellationToken cancellationToken)
        {
            var reviewRequest = await _unitOfWork.ReviewRequests.GetByIdAsync(request.ReviewRequestId, cancellationToken)
                ?? throw new KeyNotFoundException($"Review request '{request.ReviewRequestId}' was not found.");

            if (reviewRequest.MentorId != request.MentorId)
            {
                throw new UnauthorizedAccessException("Only the assigned mentor can resolve comments.");
            }

            var comment = await _unitOfWork.Comments.GetByIdAsync(request.CommentId, cancellationToken)
                ?? throw new KeyNotFoundException("Comment not found.");

            if (comment.ReviewRequestId != request.ReviewRequestId)
            {
                throw new InvalidOperationException("Comment does not belong to this review request.");
            }

            comment.IsResolved = true;
            _unitOfWork.Comments.Update(comment);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return _mapper.Map<CommentDto>(comment);
        }
    }

    public class SubmitFinalReviewCommandHandler : IRequestHandler<SubmitFinalReviewCommand, ReviewRequestDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SubmitFinalReviewCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ReviewRequestDto> Handle(SubmitFinalReviewCommand request, CancellationToken cancellationToken)
        {
            var entity = await _unitOfWork.ReviewRequests.GetByIdAsync(request.ReviewRequestId, cancellationToken)
                ?? throw new KeyNotFoundException($"Review request '{request.ReviewRequestId}' was not found.");

            if (entity.MentorId != request.MentorId)
            {
                throw new UnauthorizedAccessException("Only the assigned mentor can submit the final review.");
            }

            if (entity.Status is not ReviewStatus.Claimed and not ReviewStatus.InReview)
            {
                throw new InvalidOperationException("Only claimed or in-review requests can be finalized.");
            }

            var mentorRootComments = (await _unitOfWork.Comments.FindAsync(
                c => c.ReviewRequestId == request.ReviewRequestId && c.UserId == request.MentorId && c.ParentCommentId == null,
                cancellationToken)).ToList();

            if (mentorRootComments.Count > 0)
            {
                var resolvedCount = mentorRootComments.Count(c => c.IsResolved);
                if ((double)resolvedCount / mentorRootComments.Count < 0.8)
                {
                    throw new InvalidOperationException(
                        "At least 80% of mentor comments must be marked resolved before finalizing the review.");
                }
            }

            entity.Status = ReviewStatus.PendingApproval;
            _unitOfWork.ReviewRequests.Update(entity);

            var review = new Review
            {
                ReviewRequestId = request.ReviewRequestId,
                ReviewerId = request.MentorId,
                Summary = request.Summary,
                PriorityFixes = request.PriorityFixes,
                QualityScore = request.QualityScore,
            };

            await _unitOfWork.Reviews.AddAsync(review, cancellationToken);

            await _unitOfWork.Notifications.AddAsync(new Notification
            {
                UserId = entity.OwnerId,
                Message = $"Your review request '{entity.Title}' has been reviewed. Please approve or reject publishing it to the Knowledge Base.",
                Type = NotificationType.ReviewCompleted
            }, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return _mapper.Map<ReviewRequestDto>(entity);
        }
    }

    public class ApplySuggestionCommandHandler : IRequestHandler<ApplySuggestionCommand, CodeFileDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ApplySuggestionCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<CodeFileDto> Handle(ApplySuggestionCommand request, CancellationToken cancellationToken)
        {
            var reviewRequest = await _unitOfWork.ReviewRequests.GetByIdAsync(request.ReviewRequestId, cancellationToken)
                ?? throw new KeyNotFoundException($"Review request '{request.ReviewRequestId}' was not found.");

            if (reviewRequest.OwnerId != request.AuthorId)
            {
                throw new UnauthorizedAccessException("Only the request owner can apply suggested changes.");
            }

            var comment = await _unitOfWork.Comments.GetByIdAsync(request.CommentId, cancellationToken)
                ?? throw new KeyNotFoundException("Comment not found.");

            if (comment.ReviewRequestId != request.ReviewRequestId)
            {
                throw new InvalidOperationException("Comment does not belong to this review request.");
            }

            if (string.IsNullOrWhiteSpace(comment.SuggestedCode))
            {
                throw new InvalidOperationException("This comment does not include a suggested code change.");
            }

            if (!comment.StartLine.HasValue)
            {
                throw new InvalidOperationException("This comment is not anchored to a line range.");
            }

            if (!comment.CodeFileId.HasValue)
            {
                throw new InvalidOperationException("This comment is not linked to a code file.");
            }

            var codeFile = await _unitOfWork.CodeFiles.GetByIdAsync(comment.CodeFileId.Value, cancellationToken)
                ?? throw new KeyNotFoundException("Code file not found.");

            var startLine = comment.StartLine.Value;
            var endLine = comment.EndLine ?? comment.StartLine.Value;
            if (startLine < 1 || endLine < startLine)
            {
                throw new InvalidOperationException("Invalid line range on comment.");
            }

            var lines = codeFile.Content.Split('\n').ToList();
            if (startLine > lines.Count)
            {
                throw new InvalidOperationException("Comment line range is outside the file.");
            }

            var replacementLines = comment.SuggestedCode.Replace("\r\n", "\n").Split('\n').ToList();
            lines.RemoveRange(startLine - 1, endLine - startLine + 1);
            lines.InsertRange(startLine - 1, replacementLines);
            codeFile.Content = string.Join('\n', lines);

            _unitOfWork.CodeFiles.Update(codeFile);
            comment.IsResolved = true;
            _unitOfWork.Comments.Update(comment);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return _mapper.Map<CodeFileDto>(codeFile);
        }
    }

    public class SubmitPeerRatingCommandHandler : IRequestHandler<SubmitPeerRatingCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public SubmitPeerRatingCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<Unit> Handle(SubmitPeerRatingCommand request, CancellationToken cancellationToken)
        {
            if (request.Rating is < 1 or > 5)
            {
                throw new ArgumentOutOfRangeException(nameof(request.Rating), "Rating must be between 1 and 5.");
            }

            var reviewRequest = await _unitOfWork.ReviewRequests.GetByIdAsync(request.ReviewRequestId, cancellationToken)
                ?? throw new KeyNotFoundException($"Review request '{request.ReviewRequestId}' was not found.");

            if (reviewRequest.Status != ReviewStatus.Completed)
            {
                throw new InvalidOperationException("Ratings can only be submitted for completed reviews.");
            }

            var review = (await _unitOfWork.Reviews.FindAsync(
                r => r.ReviewRequestId == request.ReviewRequestId,
                cancellationToken)).FirstOrDefault()
                ?? throw new KeyNotFoundException("Review record not found.");

            var userId = _currentUserService.UserId;

            if (reviewRequest.OwnerId == userId)
            {
                if (review.AuthorRatingOfMentor.HasValue)
                {
                    throw new InvalidOperationException("You have already rated this mentor.");
                }

                review.AuthorRatingOfMentor = request.Rating;

                var mentor = await _unitOfWork.Users.GetByIdAsync(reviewRequest.MentorId!.Value, cancellationToken)
                    ?? throw new KeyNotFoundException("Mentor not found.");

                var languages = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                if (!string.IsNullOrWhiteSpace(reviewRequest.ProgrammingLanguage))
                    languages.Add(reviewRequest.ProgrammingLanguage.Trim());
                if (!string.IsNullOrWhiteSpace(reviewRequest.Framework))
                    languages.Add(reviewRequest.Framework.Trim());
                if (!string.IsNullOrWhiteSpace(reviewRequest.Tags))
                {
                    foreach (var tag in reviewRequest.Tags.Split(',', StringSplitOptions.RemoveEmptyEntries))
                    {
                        var t = tag.Trim();
                        if (!string.IsNullOrEmpty(t))
                            languages.Add(t);
                    }
                }
                if (languages.Count == 0)
                    languages.Add("General");

                var existingReputations = (await _unitOfWork.MentorReputations.FindAsync(
                    x => x.MentorId == reviewRequest.MentorId, cancellationToken)).ToList();

                foreach (var language in languages)
                {
                    var reputation = existingReputations.FirstOrDefault(x => string.Equals(x.Language, language, StringComparison.OrdinalIgnoreCase));
                    if (reputation == null)
                    {
                        reputation = new MentorReputation
                        {
                            MentorId = reviewRequest.MentorId.Value,
                            Language = language,
                            AverageScore = request.Rating,
                            TotalReviews = 1
                        };
                        IncrementScoreCount(reputation, request.Rating);
                        await _unitOfWork.MentorReputations.AddAsync(reputation, cancellationToken);
                    }
                    else
                    {
                        var totalReviews = reputation.TotalReviews + 1;
                        reputation.AverageScore = Math.Round((reputation.AverageScore * reputation.TotalReviews + request.Rating) / totalReviews, 2);
                        reputation.TotalReviews = totalReviews;
                        IncrementScoreCount(reputation, request.Rating);
                        _unitOfWork.MentorReputations.Update(reputation);
                    }
                }

                mentor.AverageMentorRating = Math.Round((mentor.AverageMentorRating * mentor.TotalReviews + request.Rating) / Math.Max(mentor.TotalReviews + 1, 1), 2);
                mentor.TotalReviews += 1;
                _unitOfWork.Users.Update(mentor);

                await _unitOfWork.Notifications.AddAsync(new Notification
                {
                    UserId = reviewRequest.MentorId.Value,
                    Message = $"You received a new rating for review request '{reviewRequest.Title}'.",
                    Type = NotificationType.NewRatingReceived
                }, cancellationToken);
            }
            else if (reviewRequest.MentorId == userId)
            {
                if (review.MentorRatingOfAuthor.HasValue)
                {
                    throw new InvalidOperationException("You have already rated this author.");
                }

                review.MentorRatingOfAuthor = request.Rating;

                var author = await _unitOfWork.Users.GetByIdAsync(reviewRequest.OwnerId, cancellationToken)
                    ?? throw new KeyNotFoundException("Author not found.");

                author.AverageAuthorRating = Math.Round(
                    (author.AverageAuthorRating * author.TotalAuthorRatings + request.Rating) / Math.Max(author.TotalAuthorRatings + 1, 1), 2);
                author.TotalAuthorRatings += 1;
                _unitOfWork.Users.Update(author);
            }
            else
            {
                throw new UnauthorizedAccessException("Only the author or assigned mentor can submit a rating.");
            }

            _unitOfWork.Reviews.Update(review);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }

        private static void IncrementScoreCount(MentorReputation reputation, int score)
        {
            switch (score)
            {
                case 1: reputation.Score1Count++; break;
                case 2: reputation.Score2Count++; break;
                case 3: reputation.Score3Count++; break;
                case 4: reputation.Score4Count++; break;
                case 5: reputation.Score5Count++; break;
            }
        }
    }

    public class ApproveReviewCommandHandler : IRequestHandler<ApproveReviewCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ApproveReviewCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(ApproveReviewCommand request, CancellationToken cancellationToken)
        {
            var entity = await _unitOfWork.ReviewRequests.GetByIdAsync(request.ReviewRequestId, cancellationToken)
                ?? throw new KeyNotFoundException($"Review request '{request.ReviewRequestId}' was not found.");

            if (entity.OwnerId != request.AuthorId)
                throw new UnauthorizedAccessException("Only the author can approve or reject the review.");

            if (entity.Status != ReviewStatus.PendingApproval)
                throw new InvalidOperationException("This review is not pending approval.");

            if (request.Approved)
            {
                entity.Status = ReviewStatus.Completed;
                entity.CompletedAt = DateTime.UtcNow;

                if (entity.MentorId.HasValue)
                {
                    await _unitOfWork.Notifications.AddAsync(new Notification
                    {
                        UserId = entity.MentorId.Value,
                        Message = $"The author approved publishing '{entity.Title}' to the Knowledge Base.",
                        Type = NotificationType.ReviewCompleted
                    }, cancellationToken);
                }
            }
            else
            {
                entity.Status = ReviewStatus.Completed;
                entity.CompletedAt = DateTime.UtcNow;
                entity.IsPublic = false;

                if (entity.MentorId.HasValue)
                {
                    await _unitOfWork.Notifications.AddAsync(new Notification
                    {
                        UserId = entity.MentorId.Value,
                        Message = $"The author declined publishing '{entity.Title}' to the Knowledge Base.",
                        Type = NotificationType.ReviewCompleted
                    }, cancellationToken);
                }
            }

            _unitOfWork.ReviewRequests.Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
