using API.DTOs.UserDtos;

namespace API.Interfaces
{
    public interface IUserService
    {
        Task<UserDto?> GetUserInfoAsync();
    }
}
