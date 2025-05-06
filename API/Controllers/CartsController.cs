using API.DTOs.CartDtos;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartsController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly ICartService _cartService;
 
        public CartsController(ITokenService tokenService, ICartService cartService)
        {
            _tokenService = tokenService;
            _cartService = cartService;
        }

        [HttpGet]
        public async Task<ActionResult<CartDto>> GetCart()
        {
            var cart = await _cartService.GetCartAsync(HttpContext);

            if (cart == null)
                cart = new CartDto();

            return Ok(cart);
        }

        [HttpPost("add-item")]
        public async Task<ActionResult<CartItemDto>> AddToCart([FromBody]CreateCartItemDto createCartItemDto)
        {
            Guid? userId = HttpContext.GetUserIdFromTokenInsideCookie(_tokenService);
            Guid? cartId = HttpContext.GetCartIdFromCookie();

            var result = await _cartService.AddToCartAsync(createCartItemDto, userId, cartId);

            if (result == null)
                return BadRequest("Unable to add item to cart.");

            return Ok(result);
        }

        [HttpPut("items/{itemId}")]
        public async Task<ActionResult<CartDto>> UpdateCartItem([FromBody]UpdateCartItemDto updateCartItemDto, int itemId)
        {
            var result = await _cartService.UpdateCartItemAsync(HttpContext, updateCartItemDto, itemId);
            if (!result) return NotFound("Cart item not found.");

            return NoContent();
        }
    }
}
