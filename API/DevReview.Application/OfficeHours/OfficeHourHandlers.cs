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

namespace DevReview.Application.OfficeHours
{
    public class GetAvailableOfficeHoursQueryHandler : IRequestHandler<GetAvailableOfficeHoursQuery, IReadOnlyList<OfficeHourDetailDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAvailableOfficeHoursQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IReadOnlyList<OfficeHourDetailDto>> Handle(GetAvailableOfficeHoursQuery request, CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;
            var officeHours = await _unitOfWork.OfficeHours.FindAsync(
                x => x.Status == OfficeHourStatus.Available
                     && x.StartTime > now
                     && (!request.CurrentUserId.HasValue || x.MentorId != request.CurrentUserId.Value),
                cancellationToken);
            var mentorIds = officeHours.Select(x => x.MentorId).Distinct().ToList();
            var mentors = (await _unitOfWork.Users.FindAsync(x => mentorIds.Contains(x.Id), cancellationToken))
                .ToDictionary(x => x.Id);

            return officeHours
                .OrderBy(x => x.StartTime)
                .Select(oh =>
                {
                    var dto = _mapper.Map<OfficeHourDetailDto>(oh);
                    mentors.TryGetValue(oh.MentorId, out var mentor);
                    dto.MentorName = mentor?.DisplayName ?? string.Empty;
                    dto.MentorEmail = string.Empty;
                    return dto;
                })
                .ToList();
        }
    }

    public class GetMentorScheduleQueryHandler : IRequestHandler<GetMentorScheduleQuery, IReadOnlyList<OfficeHourDetailDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetMentorScheduleQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IReadOnlyList<OfficeHourDetailDto>> Handle(GetMentorScheduleQuery request, CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;
            var schedule = await _unitOfWork.OfficeHours.FindAsync(
                x => x.MentorId == request.MentorId
                     && (x.Status != OfficeHourStatus.Available || x.StartTime > now)
                     && x.Status != OfficeHourStatus.Cancelled,
                cancellationToken);
            var userIds = schedule
                .Select(x => x.MentorId)
                .Concat(schedule.Where(x => x.BookedById.HasValue).Select(x => x.BookedById!.Value))
                .Distinct()
                .ToList();
            var users = (await _unitOfWork.Users.FindAsync(x => userIds.Contains(x.Id), cancellationToken))
                .ToDictionary(x => x.Id);

            return schedule
                .OrderBy(x => x.StartTime)
                .Select(oh =>
                {
                    var dto = _mapper.Map<OfficeHourDetailDto>(oh);
                    users.TryGetValue(oh.MentorId, out var mentor);
                    dto.MentorName = mentor?.DisplayName ?? string.Empty;
                    dto.MentorEmail = mentor?.Email ?? string.Empty;
                    if (oh.BookedById.HasValue)
                    {
                        users.TryGetValue(oh.BookedById.Value, out var booker);
                        dto.BookedByName = booker?.DisplayName;
                        dto.BookedByEmail = booker?.Email;
                    }
                    return dto;
                })
                .ToList();
        }
    }

    public class GetOfficeHourByIdQueryHandler : IRequestHandler<GetOfficeHourByIdQuery, OfficeHourDetailDto?>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetOfficeHourByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<OfficeHourDetailDto?> Handle(GetOfficeHourByIdQuery request, CancellationToken cancellationToken)
        {
            var oh = await _unitOfWork.OfficeHours.GetByIdAsync(request.OfficeHourId, cancellationToken);
            if (oh == null || oh.MentorId != request.MentorId)
                return null;

            var dto = _mapper.Map<OfficeHourDetailDto>(oh);
            var mentor = await _unitOfWork.Users.GetByIdAsync(oh.MentorId, cancellationToken);
            dto.MentorName = mentor?.DisplayName ?? string.Empty;
            dto.MentorEmail = mentor?.Email ?? string.Empty;

            if (oh.BookedById.HasValue)
            {
                var booker = await _unitOfWork.Users.GetByIdAsync(oh.BookedById.Value, cancellationToken);
                dto.BookedByName = booker?.DisplayName;
                dto.BookedByEmail = booker?.Email;
            }
            return dto;
        }
    }

    public class CreateOfficeHourCommandHandler : IRequestHandler<CreateOfficeHourCommand, OfficeHourDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public CreateOfficeHourCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<OfficeHourDto> Handle(CreateOfficeHourCommand request, CancellationToken cancellationToken)
        {
            var mentorId = _currentUserService.UserId;

            var officeHour = new OfficeHour
            {
                MentorId = mentorId,
                StartTime = request.StartTime,
                DurationMinutes = request.DurationMinutes,
                EndTime = request.StartTime.AddMinutes(request.DurationMinutes),
                Topic = request.Topic,
                Price = request.Price,
                Status = OfficeHourStatus.Available
            };

            await _unitOfWork.OfficeHours.AddAsync(officeHour, cancellationToken);

            var mentor = await _unitOfWork.Users.GetByIdAsync(mentorId, cancellationToken);
            var mentorName = mentor?.DisplayName ?? "Mentor";
            var followers = await _unitOfWork.UserFollows.FindAsync(x => x.MentorId == mentorId, cancellationToken);
            foreach (var follow in followers)
            {
                await _unitOfWork.Notifications.AddAsync(new Notification
                {
                    UserId = follow.FollowerId,
                    Message = $"{mentorName} added a new office hour: '{request.Topic}'.",
                    Type = NotificationType.OfficeHourBooked
                }, cancellationToken);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return _mapper.Map<OfficeHourDto>(officeHour);
        }
    }

    public class BookOfficeHourCommandHandler : IRequestHandler<BookOfficeHourCommand, OfficeHourDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public BookOfficeHourCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<OfficeHourDto> Handle(BookOfficeHourCommand request, CancellationToken cancellationToken)
        {
            var officeHour = await _unitOfWork.OfficeHours.GetByIdAsync(request.OfficeHourId, cancellationToken)
                ?? throw new KeyNotFoundException("Office hour not found.");

            if (officeHour.Status != OfficeHourStatus.Available)
            {
                throw new InvalidOperationException("Only available office hours can be booked.");
            }

            if (officeHour.MentorId == _currentUserService.UserId)
            {
                throw new InvalidOperationException("Mentors cannot book their own office hours.");
            }

            officeHour.Status = OfficeHourStatus.Booked;
            officeHour.BookedById = _currentUserService.UserId;
            officeHour.BookingDescription = request.BookingDescription;

            _unitOfWork.OfficeHours.Update(officeHour);
            await _unitOfWork.Notifications.AddAsync(new Notification
            {
                UserId = officeHour.MentorId,
                Message = $"Your office hour '{officeHour.Topic}' has been booked.",
                Type = NotificationType.OfficeHourBooked
            }, cancellationToken);
            await _unitOfWork.Notifications.AddAsync(new Notification
            {
                UserId = _currentUserService.UserId,
                Message = $"You successfully booked office hour '{officeHour.Topic}'.",
                Type = NotificationType.OfficeHourBooked
            }, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return _mapper.Map<OfficeHourDto>(officeHour);
        }
    }

    public class CancelOfficeHourCommandHandler : IRequestHandler<CancelOfficeHourCommand, OfficeHourDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public CancelOfficeHourCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<OfficeHourDto> Handle(CancelOfficeHourCommand request, CancellationToken cancellationToken)
        {
            var officeHour = await _unitOfWork.OfficeHours.GetByIdAsync(request.OfficeHourId, cancellationToken)
                ?? throw new KeyNotFoundException("Office hour not found.");

            if (officeHour.Status is OfficeHourStatus.Cancelled or OfficeHourStatus.Completed)
            {
                throw new InvalidOperationException("This office hour cannot be cancelled.");
            }

            if (DateTime.UtcNow >= officeHour.StartTime)
            {
                throw new InvalidOperationException("Office hours cannot be cancelled after they have started.");
            }

            var userId = _currentUserService.UserId;
            if (userId != officeHour.MentorId && userId != officeHour.BookedById)
            {
                throw new UnauthorizedAccessException("Only the mentor or the booked user can cancel this office hour.");
            }

            officeHour.Status = OfficeHourStatus.Cancelled;
            _unitOfWork.OfficeHours.Update(officeHour);

            var receiverId = userId == officeHour.MentorId ? officeHour.BookedById : officeHour.MentorId;
            if (receiverId.HasValue)
            {
                await _unitOfWork.Notifications.AddAsync(new Notification
                {
                    UserId = receiverId.Value,
                    Message = $"Office hour '{officeHour.Topic}' was cancelled.",
                    Type = NotificationType.OfficeHourCancelled
                }, cancellationToken);
            }

            await _unitOfWork.Notifications.AddAsync(new Notification
            {
                UserId = userId,
                Message = $"You cancelled office hour '{officeHour.Topic}'.",
                Type = NotificationType.OfficeHourCancelled
            }, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return _mapper.Map<OfficeHourDto>(officeHour);
        }
    }

    public class SubmitOfficeHourFeedbackCommandHandler : IRequestHandler<SubmitOfficeHourFeedbackCommand, OfficeHourDetailDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public SubmitOfficeHourFeedbackCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<OfficeHourDetailDto> Handle(SubmitOfficeHourFeedbackCommand request, CancellationToken cancellationToken)
        {
            var officeHour = await _unitOfWork.OfficeHours.GetByIdAsync(request.OfficeHourId, cancellationToken)
                ?? throw new KeyNotFoundException("Office hour not found.");

            if (DateTime.UtcNow < officeHour.EndTime)
            {
                throw new InvalidOperationException("Feedback can only be submitted after the office hour has completed.");
            }

            var currentUserId = _currentUserService.UserId;
            if (currentUserId == officeHour.MentorId)
            {
                officeHour.MentorImpression = request.Impression;
            }
            else if (officeHour.BookedById == currentUserId)
            {
                officeHour.AuthorImpression = request.Impression;
            }
            else
            {
                throw new UnauthorizedAccessException("Only the mentor or booked author can submit feedback.");
            }

            if (!string.IsNullOrWhiteSpace(officeHour.MentorImpression) && !string.IsNullOrWhiteSpace(officeHour.AuthorImpression))
            {
                officeHour.Status = OfficeHourStatus.Completed;
                await _unitOfWork.Notifications.AddAsync(new Notification
                {
                    UserId = officeHour.MentorId,
                    Message = $"Office hour '{officeHour.Topic}' has been marked completed.",
                    Type = NotificationType.OfficeHourCompleted
                }, cancellationToken);
                if (officeHour.BookedById.HasValue)
                {
                    await _unitOfWork.Notifications.AddAsync(new Notification
                    {
                        UserId = officeHour.BookedById.Value,
                        Message = $"Office hour '{officeHour.Topic}' has been marked completed.",
                        Type = NotificationType.OfficeHourCompleted
                    }, cancellationToken);
                }
            }

            _unitOfWork.OfficeHours.Update(officeHour);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return _mapper.Map<OfficeHourDetailDto>(officeHour);
        }
    }

    public class GetMyBookedOfficeHoursQueryHandler : IRequestHandler<GetMyBookedOfficeHoursQuery, IReadOnlyList<OfficeHourDetailDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public GetMyBookedOfficeHoursQueryHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<IReadOnlyList<OfficeHourDetailDto>> Handle(GetMyBookedOfficeHoursQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            var bookings = await _unitOfWork.OfficeHours.FindAsync(
                x => x.BookedById == userId && x.Status != OfficeHourStatus.Available,
                cancellationToken);

            var mentorIds = bookings.Select(x => x.MentorId).Distinct().ToList();
            var mentors = (await _unitOfWork.Users.FindAsync(x => mentorIds.Contains(x.Id), cancellationToken))
                .ToDictionary(x => x.Id);

            return bookings
                .OrderByDescending(x => x.StartTime)
                .Select(oh =>
                {
                    var dto = _mapper.Map<OfficeHourDetailDto>(oh);
                    mentors.TryGetValue(oh.MentorId, out var mentor);
                    dto.MentorName = mentor?.DisplayName ?? string.Empty;
                    dto.MentorEmail = mentor?.Email ?? string.Empty;
                    return dto;
                })
                .ToList();
        }
    }
}
