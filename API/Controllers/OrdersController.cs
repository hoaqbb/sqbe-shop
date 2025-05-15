using API.DTOs.OrderDtos;
using API.Interfaces;
using API.Services;
using Microsoft.AspNetCore.Mvc;

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

        [HttpGet]
        public async Task<ActionResult> GetUserOrders()
        {
            
            return Ok();
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
