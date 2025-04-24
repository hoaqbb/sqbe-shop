using API.Data.Entities;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace API.Repositories
{
    public class AccountRepository : GenericRepository<User>, IAccountRepository
    {
        private readonly EcommerceDbContext _context;

        public AccountRepository(EcommerceDbContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == email);

            return user;
        }

        public async Task<User> RegisterAsync(User user, string password)
        {
            HashPassword(password, out byte[] passwordHash, out byte[] passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            user.Provider = "Local";

            await _context.Users.AddAsync(user);

            return user;
        }

        private void HashPassword(string password, out byte[] passwordHash, out byte[] key)
        {
            using var hmac = new HMACSHA512();

            passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            key = hmac.Key;
        }
    }
}
