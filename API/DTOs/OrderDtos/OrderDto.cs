using API.Data.Enums;

namespace API.DTOs.OrderDtos
{
    public class OrderDto
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public OrderStatusEnum Status { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public string Address { get; set; } = null!;
        public string Fullname { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string PaymentMethod { get; set; }
    }
}
