using AutoMapper;
using SmartLib.Models.Entities;

namespace SmartLib.Models.Dto.Profiles
{
    public class BorrowRecordProfile : Profile
    {
        public BorrowRecordProfile()
        {
            CreateMap<BorrowRecord, BorrowRecordDto>().ReverseMap();
        }
    }
}
