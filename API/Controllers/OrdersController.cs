using API.DTOs.OrderDtos;
using API.Extensions;
using API.Interfaces;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaypalServerSdk.Standard.Models;

namespace API.Controllers
{
    public class OrdersController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOrderService _orderService;

        public OrdersController(IUnitOfWork unitOfWork, IOrderService orderService)
        {
            _unitOfWork = unitOfWork;
            _orderService = orderService;
        }

        [Authorize]
        [HttpGet("{orderId:Guid}")]
        public async Task<ActionResult<OrderDetailDto>> GetOrderById(Guid orderId)
        {
            var userRole = User.GetUserRole();
            if(string.IsNullOrEmpty(userRole))
                return Unauthorized();
            var userId = User.GetUserId();

            var order = await _orderService.GetOrderByIdAsync(orderId, userId, userRole);
            if (order == null)
            {
                return NotFound();
            }

            return Ok(order);
        }

        [HttpPost]
        public async Task<ActionResult> CreateOrder([FromBody]OrderRequestDto requestDto)
        {
            try
            {
                var result = await _orderService.CreateOrderAsync(requestDto);

                if (!result.Success)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

    }
}
