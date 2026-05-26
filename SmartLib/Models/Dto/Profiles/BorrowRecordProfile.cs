using AutoMapper;
using SmartLib.Models.Dto.Create;
using SmartLib.Models.Dto.Response;
using SmartLib.Models.Entities;

namespace SmartLib.Models.Dto.Profiles
{
    public class BorrowRecordProfile : Profile
    {
        public BorrowRecordProfile()
        {
            CreateMap<CreateBorrowRecordDto, BorrowRecord>();
            CreateMap<BorrowRecord, BorrowRecordResponseDto>();
        }
    }
}
