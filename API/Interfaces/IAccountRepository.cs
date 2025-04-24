using API.Data.Entities;
using API.DTOs.UserDtos;
using Google.Apis.Auth;

namespace API.Interfaces
{
    public interface IAccountRepository : IGenericRepository<User>
    {
        Task<User> ExternalRegisterAsync(GoogleJsonWebSignature.Payload payload);
        Task<User?> GetUserByEmailAsync(string email);
        Task<User?> AuthenticateAsync(LoginDto loginDto);
        Task<User> RegisterAsync(User user, string password);
        Task RemoveUserTokenAsync(User user);
    }
}
