using System.ComponentModel.DataAnnotations;

namespace API.DTOs.OrderDtos
{
    public class OrderRequestDto
    {
        [Required]
        public string Fullname { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public byte DeliveryMethod { get; set; }
        public string? Note { get; set; }
        [Required]
        public decimal Amount { get; set; }
        [Required]
        public decimal Subtotal { get; set; }
        public decimal? DiscountAmount { get; set; }
        [Required]
        public string Address { get; set; } = null!;
        [Required]
        public string PaymentMethod { get; set; }
        [Required]
        public int ShippingFee { get; set; }
        public string? PromotionCode { get; set; }
    }
}
