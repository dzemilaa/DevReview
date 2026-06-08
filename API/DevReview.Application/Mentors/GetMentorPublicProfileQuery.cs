using System;
using DevReview.Application.DTOs;
using MediatR;

namespace DevReview.Application.Mentors
{
    public class GetMentorPublicProfileQuery : IRequest<MentorPublicProfileDto?>
    {
        public Guid MentorId { get; set; }
    }
}
