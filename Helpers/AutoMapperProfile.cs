using DoAn.Data;
using DoAn.DTO;
using AutoMapper;
namespace DoAn.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile() {
            CreateMap<RoomUpdateDTO, Room>()
           .ForMember(dest => dest.Images, opt => opt.Ignore()) 
           .ForMember(dest => dest.CreatedAt, opt => opt.Ignore()); 
        }
    }
}   
