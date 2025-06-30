using API.Data.Enums;
using API.DTOs.PaymentDtos;
using AutoMapper.Configuration.Annotations;
using System.Text.Json.Serialization;

namespace API.DTOs.OrderDtos
{
    public class OrderDetailDto
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public decimal Subtotal { get; set; }
        public decimal DiscountAmount { get; set; }
        public int ShippingFee { get; set; }
        public string Fullname { get; set; } = null!;
        public string? Email { get; set; }
        public string Address { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string? Note { get; set; }
        public short Status { get; set; }
        public short DeliveryMethod { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public Guid UserId { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public PaymentDto? PaymentInfo { get; set; }
        public virtual ICollection<OrderItemDto> OrderItems { get; set; }
    }
}
