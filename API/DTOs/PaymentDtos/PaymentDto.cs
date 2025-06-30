namespace API.DTOs.PaymentDtos
{
    public class PaymentDto
    {
        public string Method { get; set; } = null!;
        public string? Provider { get; set; }
        public decimal Amount { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public string? TransactionId { get; set; }
        public Guid? UserId { get; set; }
        public bool Status { get; set; }
        public string CurrencyCode { get; set; } = null!;
    }
}
