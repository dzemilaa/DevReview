using System;
using MediatR;

namespace DevReview.Application.Mentors
{
    public class FollowMentorCommand : IRequest<Unit>
    {
        public Guid MentorId { get; set; }
        public Guid FollowerId { get; set; }
    }

    public class UnfollowMentorCommand : IRequest<Unit>
    {
        public Guid MentorId { get; set; }
        public Guid FollowerId { get; set; }
    }

    public class GetFollowedMentorIdsQuery : IRequest<System.Collections.Generic.IReadOnlyList<Guid>>
    {
        public Guid FollowerId { get; set; }
    }

    public class GetFollowedMentorsQuery : IRequest<System.Collections.Generic.IReadOnlyList<DevReview.Application.DTOs.MentorRankingDto>>
    {
        public Guid FollowerId { get; set; }
    }
}
