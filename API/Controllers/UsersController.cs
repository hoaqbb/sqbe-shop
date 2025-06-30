using API.Data.Entities;
using API.DTOs.OrderDtos;
using API.DTOs.UserDtos;
using API.Extensions;
using API.Helpers.Params;
using API.Interfaces;
using API.Specifications.OrderSpecifications;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class UsersController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;
        private readonly IProductService _productService;
        private readonly IMapper _mapper;

        public UsersController(IUnitOfWork unitOfWork, IUserService userService, IProductService productService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _userService = userService;
            _productService = productService;
            _mapper = mapper;
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<UserDto>> GetUserInfo()
        {
            var user = await _userService.GetUserInfoAsync();
            if(user == null) return NotFound();

            return Ok(user);
        }

        [Authorize]
        [HttpGet("orders")]
        public async Task<ActionResult> GetUserOrders([FromQuery] UserOrderParams param)
        {
            var userId = User.GetUserId();
            if(userId == null)  return Unauthorized();
            var spec = new UserOrderSpecification(param, (Guid)userId!);

            var result = await CreatePaginatedResult<Order, OrderDto>(_unitOfWork.Repository<Order>(), spec, param.PageIndex, param.PageSize, _mapper.ConfigurationProvider);

            return Ok(result);
        }

        [Authorize]
        [HttpGet("liked-product")]
        public async Task<ActionResult> GetUserLikedProduct()
        {
            var userId = User.GetUserId();
            if (userId == null) return Unauthorized();

            var userLikedProducts = await _productService.GetUserLikedProductsAsync((Guid)userId);

            return Ok(userLikedProducts);
        }
    }
}
