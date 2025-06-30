using System.Security.Claims;

namespace API.Extensions
{
    public static class ClaimsPrincipleExtentions
    {
        public static Guid? GetUserId(this ClaimsPrincipal user)
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (Guid.TryParse(userIdClaim, out Guid userId))
            {
                return userId;
            }
            return null;
        }

        public static string? GetUserRole(this ClaimsPrincipal user)
        {
            return user.FindFirstValue(ClaimTypes.Role);
        }
    }
}
