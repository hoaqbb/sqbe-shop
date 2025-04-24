using API.Data.Entities;
using API.DTOs.UserDtos;
using API.Interfaces;
using Google.Apis.Auth;
using System.Security.Claims;
using System.Security.Cryptography;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace API.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;

        public TokenService(IConfiguration config)
        {
            _config = config;
        }

        public string GenerateAccessToken(IEnumerable<Claim> claims)
        {
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["SecretKey"]));
            var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
            var tokenOptions = new JwtSecurityToken(
                //issuer: "https://localhost:7100",
                //audience: "https://localhost:4200",
                claims: claims,
                expires: DateTime.Now.AddMinutes(5),
                signingCredentials: signinCredentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

            return tokenString;
        }

        public List<Claim> GenerateClaims(User user)
        {
            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role)
            };

            return claims;
        }

        public string GenerateRefreshToken()
        {
            //tao mang byte dai 32
            var randomNumber = new byte[32];
            //là một phương thức của System.Security.Cryptography, giúp tạo số ngẫu nhiên an toàn về mặt mật mã.
            using (var rng = RandomNumberGenerator.Create())
            {
                //Lấy 32 byte ngẫu nhiên và lưu vào mảng randomNumber.
                rng.GetBytes(randomNumber);
                //Chuyển đổi mảng byte thành chuỗi Base64 để có thể lưu trữ dưới dạng văn bản
                return Convert.ToBase64String(randomNumber);
            }
        }
        
        public void SetTokenInsideCookies(TokenDto tokenDto, HttpContext httpContext)
        {
            httpContext.Response.Cookies.Append("accessToken", tokenDto.AccessToken,
                new CookieOptions
                {
                    Expires = DateTime.UtcNow.AddDays(7),
                    Secure = true,
                    HttpOnly = true,
                    IsEssential = true,
                    SameSite = SameSiteMode.None
                });
            httpContext.Response.Cookies.Append("refreshToken", tokenDto.RefreshToken,
                new CookieOptions
                {
                    Expires = DateTime.UtcNow.AddDays(7),
                    Secure = true,
                    HttpOnly = true,
                    IsEssential = true,
                    SameSite = SameSiteMode.None
                });
        }
    }
}
