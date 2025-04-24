using API.Data.Entities;
using API.DTOs.UserDtos;

namespace API.Interfaces
{
    public interface IAccountRepository : IGenericRepository<User>
    {
        Task<User?> GetUserByEmailAsync(string email);
        Task<User> RegisterAsync(User user, string password);
    }
}
