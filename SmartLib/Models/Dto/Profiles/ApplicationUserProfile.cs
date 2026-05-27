using AutoMapper;
using SmartLib.Models.Dto.Create;
using SmartLib.Models.Dto.Response;
using SmartLib.Models.Entities;

namespace SmartLib.Models.Dto.Profiles
{
    public class ApplicationUserProfile : Profile
    {
        public ApplicationUserProfile()
        {
            CreateMap<CreateApplicationUserDto, ApplicationUser>();
            CreateMap<ApplicationUser, ApplicationUserResponse>();
        }
    }
}
