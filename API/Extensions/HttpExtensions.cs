using API.Interfaces;
using System.Security.Claims;

namespace API.Extensions
{
    public static class HttpExtensions
    {
        private const string accessTokenCookieName = "accessToken";
        private const string cartIdCookieName = "cartId";
        public static Guid? GetUserIdFromTokenInsideCookie(this HttpContext httpContext, ITokenService tokenService)
        {
            if (httpContext.Request.Cookies.TryGetValue(accessTokenCookieName, out string? accessToken)
                && accessToken != null)
            {
                try
                {
                    var principal = tokenService.GetPrincipalFromAccessToken(accessToken);
                    var userIdString = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    if (Guid.TryParse(userIdString, out Guid parsedUserId))
                    {
                        return parsedUserId;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
            return null;
        }

        public static string? GetCartIdFromCookie(this HttpContext httpContext)
        {
            return httpContext.Request.Cookies.TryGetValue(cartIdCookieName, out string? cartId) 
                ? cartId 
                : string.Empty;
        }
    }
}
