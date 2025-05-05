using API.DTOs.CartDtos;

namespace API.Interfaces
{
    public interface ICartService
    {
        Task<CartDto?> GetOrCreateCartAsync(HttpContext context);
        void SetCartIdInsideCookie(HttpContext httpContext, string cartId);
        void RemoveCartIdInsideCookie(HttpContext httpContext);
    }
}
