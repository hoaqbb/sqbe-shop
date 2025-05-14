namespace API.DTOs.PaymentDtos
{
    public class PaymentRequestDto
    {
        public decimal Amount { get; set; }
        public Guid OrderId { get; set; }
    }
}
