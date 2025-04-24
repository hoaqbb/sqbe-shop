using API.Data.Entities;
using API.DTOs.UserDtos;
using System.Security.Claims;

namespace API.Interfaces
{
    public interface ITokenService
    {
        string GenerateAccessToken(IEnumerable<Claim> claims);
        string GenerateRefreshToken();
        List<Claim> GenerateClaims(User user);
        void SetTokenInsideCookies(TokenDto tokenDto, HttpContext httpContext);
    }
}
