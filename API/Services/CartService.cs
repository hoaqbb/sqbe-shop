using API.Data.Entities;
using API.DTOs.CartDtos;
using API.Extensions;
using API.Interfaces;
using AutoMapper;

namespace API.Services
{
    public class CartService : ICartService
    {
        private const int expiryDay = 7;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cacheService;
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;

        public CartService(IUnitOfWork unitOfWork, ICacheService cacheService, IMapper mapper, ITokenService tokenService)
        {
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
            _mapper = mapper;
            _tokenService = tokenService;
        }

        public async Task<CartDto?> GetOrCreateCartAsync(HttpContext httpContext)
        {
            string redisKey;
            CartDto? cart;
            var userId = httpContext.GetUserIdFromTokenInsideCookie(_tokenService);

            if (userId != null)
            {
                redisKey = $"cart:user:{userId}";

                cart = await _cacheService.GetDataAsync<CartDto>(redisKey);

                if (cart == null)
                {
                    cart = await _unitOfWork.CartRepository
                        .GetSingleProjectedAsync<CartDto>(x => x.UserId == userId, _mapper.ConfigurationProvider);

                    if (cart != null)
                        await _cacheService.SetDataAsync(redisKey, cart);
                }

                if (!httpContext.Request.Cookies.ContainsKey("cartId"))
                    SetCartIdInsideCookie(httpContext, userId.ToString()!);

                return cart;
            }

            // Guest
            var cartId = httpContext.GetCartIdFromCookie();
            if (string.IsNullOrEmpty(cartId))
            {
                cartId = Guid.NewGuid().ToString();
                SetCartIdInsideCookie(httpContext, cartId);
            }

            redisKey = $"cart:guest:{cartId}";
            cart = await _cacheService.GetDataAsync<CartDto>(redisKey);

            if (cart == null)
            {
                cart = new CartDto { Id = cartId };
                await _cacheService.SetDataAsync(redisKey, cart);
            }

            return cart;
        }

        public void RemoveCartIdInsideCookie(HttpContext httpContext)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            };

            httpContext.Response.Cookies.Delete("cartId", cookieOptions);
        }

        public void SetCartIdInsideCookie(HttpContext httpContext, string cartId)
        {
            httpContext.Response.Cookies.Append("cartId", cartId, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddDays(expiryDay)
            });
        }
    }
}
