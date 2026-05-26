using AutoMapper;
using SmartLib.Models.Dto.Create;
using SmartLib.Models.Dto.Response;
using SmartLib.Models.Entities;

namespace SmartLib.Models.Dto.Profiles
{
    public class FineProfile : Profile
    {
        public FineProfile()
        {
            CreateMap<CreateFineDto, Fine>();
            CreateMap<Fine, FineResponseDto>();
        }
    }
}
