using API.Data.Entities;
using API.DTOs.UserDtos;
using API.Interfaces;
using Google.Apis.Auth;
using System.Security.Claims;
using System.Security.Cryptography;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using API.Helpers;

namespace API.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;
        private readonly JwtEmailSettings _emailSettings;
        private readonly string _googleClientId;

        public TokenService(IConfiguration config, IOptions<JwtEmailSettings> emailSettings)
        {
            _config = config;
            _emailSettings = emailSettings.Value;
            _googleClientId = _config.GetSection("GoogleClientId").Value;
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

        public ClaimsPrincipal GetPrincipalFromAccessToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["SecretKey"])),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            //Kiểm tra xem securityToken có thực sự là một JwtSecurityToken không (kiểu ép kiểu an toàn)
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null //Token có phải là JWT không
                || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, //Token có sử dụng thuật toán HMAC SHA-256 để ký không
                    StringComparison.InvariantCultureIgnoreCase)) //so sánh không phân biệt hoa thường.
                throw new Exception("Invalid Token!");

            return principal;
        }

        public void RemoveTokenInsideCookies(HttpContext httpContext)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                IsEssential = true,
                Secure = true,
                SameSite = SameSiteMode.None
            };

            httpContext.Response.Cookies.Delete("accessToken", cookieOptions);
            httpContext.Response.Cookies.Delete("refreshToken", cookieOptions);
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

        public async Task<GoogleJsonWebSignature.Payload?> VerifyGoogleToken(string googleToken)
        {
            try
            {
                var settings = new GoogleJsonWebSignature.ValidationSettings()
                {
                    Audience = new List<string> { _googleClientId }
                };

                var payload = await GoogleJsonWebSignature.ValidateAsync(googleToken);
                return payload;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        public string GenerateEmailVerificationToken(User user)
        {
            var claims = GenerateClaims(user);

            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_emailSettings.Key));
            var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var tokenOptions = new JwtSecurityToken(
                //issuer: _options.Issuer,
                //audience: _options.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_emailSettings.ExpireMinutes),
                signingCredentials: signinCredentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

            return tokenString;
        }
    }
}
