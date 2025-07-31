using API.DTOs.OrderDtos;

namespace API.Interfaces
{
    public interface IOrderService
    {
        Task<CreateOrderResultDto> CreateOrderAsync(OrderRequestDto orderRequestDto);
        Task<object> GetUserOrdersAsync(Guid userId);
        Task<OrderDetailDto?> GetOrderByIdAsync(Guid orderId, Guid? userId, string? role);
        Task<bool> UpdateOrderStatusAsync(Guid orderId, short orderStatus);
    }
}
