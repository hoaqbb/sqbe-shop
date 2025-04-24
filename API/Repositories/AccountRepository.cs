using API.Data.Entities;
using API.DTOs.UserDtos;
using API.Interfaces;
using Google.Apis.Auth;
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

        public async Task<User?> AuthenticateAsync(LoginDto loginDto)
        {
            var user = await _context.Users
                .SingleOrDefaultAsync(x => x.Email == loginDto.Email && x.Provider == "Local");
            if (user is null) return null;

            using var hmac = new HMACSHA512(user.PasswordSalt);

            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i]) return null;
            }

            return user;
        }

        public async Task<User> ExternalRegisterAsync(GoogleJsonWebSignature.Payload payload)
        {
            HashPassword(payload.JwtId, out byte[] passwordHash, out byte[] key);

            var user = new User
            {
                Lastname = payload.FamilyName,
                Firstname = payload.GivenName,
                Email = payload.Email,
                Provider = payload.Issuer,
                Gender = 0,
                IsAuthenticated = payload.EmailVerified,
                PasswordHash = passwordHash,
                PasswordSalt = key
            };

            await AddAsync(user);

            return user;
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
