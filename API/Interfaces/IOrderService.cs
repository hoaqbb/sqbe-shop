using API.DTOs.OrderDtos;

namespace API.Interfaces
{
    public interface IOrderService
    {
        Task<CreateOrderResultDto> CreateOrderAsync(OrderRequestDto orderRequestDto);
    }
}
