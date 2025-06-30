using API.Data.Entities;
using API.Data.Enums;
using API.DTOs.OrderDtos;
using AutoMapper;

namespace API.Helpers.MappingProfiles
{
    public class OrderProfiles : Profile
    {
        public OrderProfiles()
        {
            CreateMap<Order, OrderDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src =>
                    (OrderStatusEnum)src.Status))
                .ForMember(dest => dest.PaymentMethod, opt => opt.MapFrom(src =>
                    src.Payments.FirstOrDefault(p => p.OrderId == src.Id)!.Provider))
                ;

            CreateMap<Order, OrderDetailDto>()
                .ForMember(dest => dest.PaymentInfo, opt => opt.MapFrom(src =>
                    src.Payment));

            CreateMap<OrderItem, OrderItemDto>();
        }
    }
}
