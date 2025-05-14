namespace API.DTOs.PaymentDtos
{
    public class VnPayResponseDto
    {
        public bool Success { get; set; }
        public string PaymentMethod { get; set; }
        public string OrderDescription { get; set; }
        public Guid OrderId { get; set; }
        public string TransactionId { get; set; }
        public string VnPayResponseCode { get; set; }
        public string TransactionStatus { get; set; }
    }
}
