using API.Data.Entities;
using API.DTOs.CartDtos;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
    public class CartService : ICartService
    {
        private const int expiryDay = 7;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cacheService;
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly EcommerceDbContext _context;
        private string redisKey = string.Empty;

        public CartService(IUnitOfWork unitOfWork, ICacheService cacheService, IMapper mapper, ITokenService tokenService, IHttpContextAccessor httpContextAccessor, EcommerceDbContext context)
        {
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
            _mapper = mapper;
            _tokenService = tokenService;
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }

        public async Task<CartDto?> GetCartAsync(HttpContext httpContext)
        {
            CartDto? cart;
            var userId = httpContext.GetUserIdFromTokenInsideCookie(_tokenService);
            var cartId = httpContext.GetCartIdFromCookie();

            //user is logged in
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

                if (cartId != userId)
                    SetCartIdInsideCookie(httpContext, userId.ToString()!);

                return cart;
            }

            // Guest
            if (cartId == null)
            {
                return null;
            }

            redisKey = $"cart:guest:{cartId}";
            cart = await _cacheService.GetDataAsync<CartDto>(redisKey);

            if (cart == null)
            {
                cart = new CartDto { Id = cartId.ToString() };
                await _cacheService.SetDataAsync(redisKey, cart);
            }

            return cart;
        }

        public async Task<CartItemDto?> AddToCartAsync(CreateCartItemDto dto, Guid? userId, Guid? cartId)
        {
            var newItem = await _context.ProductVariants
                .ProjectTo<CartItemDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(x => x.Id == dto.ProductVariantId);

            if (newItem is null || newItem.ProductVariant.Quantity == 0)
                return null;

            redisKey = (userId is null) ? $"cart:guest:{cartId}" : $"cart:user:{userId}";

            var cart = await _cacheService.GetDataAsync<CartDto>(redisKey);

            if (userId is null)
            {
                return await HandleGuestCartAsync(dto, newItem, cart);
            }

            return await HandleUserCartAsync(dto, newItem, cart, userId);
        }

        public void RemoveCartIdInsideCookie(HttpContext httpContext)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None
            };

            httpContext.Response.Cookies.Delete("cartId", cookieOptions);
        }

        public void SetCartIdInsideCookie(HttpContext httpContext, string cartId)
        {
            httpContext.Response.Cookies.Append("cartId", cartId, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddDays(expiryDay)
            });
        }

        private async Task<CartItemDto> HandleGuestCartAsync(CreateCartItemDto dto, CartItemDto newItem, CartDto? cart)
        {
            if (cart == null)
            {
                newItem.Quantity = 1;

                var newCartId = Guid.NewGuid().ToString();
                var newCart = new CartDto
                {
                    Id = newCartId,
                    CartItems = new List<CartItemDto> { newItem }
                };

                redisKey = $"cart:guest:{newCartId}";
                await _cacheService.SetDataAsync(redisKey, newCart);

                // Set lại cartId vào cookie
                SetCartIdInsideCookie(_httpContextAccessor.HttpContext!, newCartId);

                return newItem;
            }

            var index = cart.CartItems.FindIndex(x => x.ProductVariant.Id == dto.ProductVariantId);
            // cart item is existed
            if (index >= 0)
            {
                cart.CartItems[index].Quantity = Math.Min(cart.CartItems[index].Quantity + 1, newItem.ProductVariant.Quantity);
                await _cacheService.SetDataAsync(redisKey, cart);
                return cart.CartItems[index];
            }

            newItem.Quantity = 1;
            cart.CartItems.Add(newItem);
            await _cacheService.SetDataAsync(redisKey, cart);
            return newItem;
        }

        private async Task<CartItemDto> HandleUserCartAsync(CreateCartItemDto dto, CartItemDto newItem, CartDto? cart, Guid? userId)
        {
            // chua co tren redis
            if (cart == null)
            {
                var userCart = await _unitOfWork.CartRepository
                    .GetSingleProjectedAsync<CartDto>(c => c.UserId == userId, _mapper.ConfigurationProvider);

                if (userCart == null) return null;

                var idx = userCart.CartItems
                    .FindIndex(x => x.ProductVariant.Id == dto.ProductVariantId);

                // cart item da ton tai
                if (idx >= 0)
                {
                    userCart.CartItems[idx].Quantity = Math.Min(userCart.CartItems[idx].Quantity + 1, newItem.ProductVariant.Quantity);
                    await _cacheService.SetDataAsync<CartDto>(redisKey, userCart);
                    _context.Update(userCart.CartItems[idx]);
                    await _context.SaveChangesAsync();
                    return userCart.CartItems[idx];
                }

                newItem.Quantity = 1;
                userCart.CartItems.Add(newItem);
                await _context.AddAsync(new CartItem
                {
                    CartId = userCart.Id,
                    ProductVariantId = newItem.ProductVariant.Id,
                    Quantity = 1
                });
                await _unitOfWork.SaveChangesAsync();

                await _cacheService.SetDataAsync(userCart.Id, userCart);
                return newItem;
            }

            var index = cart.CartItems.FindIndex(x => x.ProductVariant.Id == dto.ProductVariantId);
            if (index >= 0)
            {
                cart.CartItems[index].Quantity = Math.Min(cart.CartItems[index].Quantity + 1, newItem.ProductVariant.Quantity);
                await _cacheService.SetDataAsync(redisKey, cart);

                var itemInDb = await _context.FindAsync<CartItem>(cart.CartItems[index].Id);
                if (itemInDb != null)
                {
                    itemInDb.Quantity = cart.CartItems[index].Quantity;
                    _context.Update(itemInDb);
                    await _context.SaveChangesAsync();
                }

                return cart.CartItems[index];
            }

            newItem.Quantity = 1;
            var cartItem = new CartItem
            {
                CartId = cart.Id,
                ProductVariantId = newItem.ProductVariant.Id,
                Quantity = 1
            };

            await _context.AddAsync(cartItem);
            if (await _unitOfWork.SaveChangesAsync())
            {
                newItem.Id = cartItem.Id;
                cart.CartItems.Add(newItem);
                await _cacheService.SetDataAsync(redisKey, cart);
                return newItem;
            }

            throw new Exception("Failed to add item to cart.");
        }

    }
}
