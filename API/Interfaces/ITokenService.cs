using API.Data.Entities;
using API.DTOs.UserDtos;
using Google.Apis.Auth;
using System.Security.Claims;

namespace API.Interfaces
{
    public interface ITokenService
    {
        string GenerateAccessToken(IEnumerable<Claim> claims);
        string GenerateRefreshToken();
        List<Claim> GenerateClaims(User user);
        ClaimsPrincipal GetPrincipalFromAccessToken(string token);
        void SetTokenInsideCookies(TokenDto tokenDto, HttpContext httpContext);
        void RemoveTokenInsideCookies(HttpContext httpContext);
        Task<GoogleJsonWebSignature.Payload?> VerifyGoogleToken(string googleToken);
    }
}
