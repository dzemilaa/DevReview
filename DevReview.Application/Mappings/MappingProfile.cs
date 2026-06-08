using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using DevReview.Application.DTOs;
using DevReview.Application.DTOs.Auth;
using DevReview.Domain.Entities;

namespace DevReview.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDto>();
            CreateMap<User, AdminUserDto>();
            CreateMap<CodeFile, CodeFileDto>();
            CreateMap<Comment, CommentDto>();
            CreateMap<OfficeHour, OfficeHourDto>();
            CreateMap<OfficeHour, OfficeHourDetailDto>()
                .ForMember(dest => dest.MentorName, opt => opt.MapFrom(src => src.Mentor != null ? src.Mentor.DisplayName : string.Empty))
                .ForMember(dest => dest.MentorEmail, opt => opt.MapFrom(src => src.Mentor != null ? src.Mentor.Email : string.Empty))
                .ForMember(dest => dest.BookedByName, opt => opt.MapFrom(src => src.BookedBy != null ? src.BookedBy.DisplayName : null))
                .ForMember(dest => dest.BookedByEmail, opt => opt.MapFrom(src => src.BookedBy != null ? src.BookedBy.Email : null));

            CreateMap<ReviewRequest, ReviewRequestDto>()
                .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => SplitTags(src.Tags)));

            CreateMap<ReviewRequest, ReviewRequestDetailDto>()
                .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => SplitTags(src.Tags)));

            CreateMap<Tag, TagDto>();
            CreateMap<Notification, NotificationDto>();
            CreateMap<UserLanguage, UserLanguageDto>();
        }

        private static List<string> SplitTags(string tags)
        {
            return string.IsNullOrWhiteSpace(tags)
                ? new List<string>()
                : tags.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToList();
        }
    }
}
