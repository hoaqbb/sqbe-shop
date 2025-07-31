using API.Data.Entities;
using API.DTOs.UserDtos;
using AutoMapper;

namespace API.Helpers.MappingProfiles
{
    public class UserProfiles : Profile
    {
        public UserProfiles()
        {
            CreateMap<RegisterDto, User>();
            CreateMap<User, UserDto>();
            CreateMap<User, UserDetailDto>()
                .ForMember(dest => dest.UserOrders, opt => opt.MapFrom(src =>
                    src.Orders));
        }
    }
}
