using AutoMapper;
using SmartLib.Models.Entities;

namespace SmartLib.Models.Dto.Profiles
{
    public class BookProfile : Profile
    {
        public BookProfile()
        {
            CreateMap<Book, BookDto>().ReverseMap();
        }
    }
}
