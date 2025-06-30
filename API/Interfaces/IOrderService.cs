using API.DTOs.OrderDtos;

namespace API.Interfaces
{
    public interface IOrderService
    {
        Task<CreateOrderResultDto> CreateOrderAsync(OrderRequestDto orderRequestDto);
        Task<OrderDetailDto?> GetOrderByIdAsync(Guid orderId, Guid? userId, string? role);
    }
}
