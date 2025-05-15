namespace API.DTOs.PaymentDtos
{
    public class PayPalResponseDto
    {
        public string TransactionId { get; set; }
        public string Status { get; set; }
        public Guid OrderId { get; set; }
    }
}
