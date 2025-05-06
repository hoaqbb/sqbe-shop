using API.DTOs.CartDtos;

namespace API.Interfaces
{
    public interface ICartService
    {
        Task<CartDto?> GetCartAsync(HttpContext context);
        Task<CartItemDto?> AddToCartAsync(CreateCartItemDto dto, Guid? userId, Guid? cartId);
        Task<bool> UpdateCartItemAsync(HttpContext httpContext, UpdateCartItemDto updateCartItemDto, int cartItemId);
        void SetCartIdInsideCookie(HttpContext httpContext, string cartId);
        void RemoveCartIdInsideCookie(HttpContext httpContext);
    }
}
