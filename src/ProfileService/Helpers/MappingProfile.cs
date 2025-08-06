using AutoMapper;
using ProfileService.Data.Models;
using ProfileService.DTOs;

namespace ProfileService.Helpers
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<UserProfile, UserProfileDto>();
            CreateMap<UserProfileDto, UserProfile>();
            CreateMap<EditUserProfile, UserProfile>();
            CreateMap<UserProfile, EditUserProfile>();
        }
    }
}
