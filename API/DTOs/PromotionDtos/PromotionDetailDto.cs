namespace API.DTOs.PromotionDtos
{
    public class PromotionDetailDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = null!;
        public string? Description { get; set; }
        public int DiscountType { get; set; }
        public decimal DiscountValue { get; set; }
        public decimal? MinOrderAmount { get; set; }
        public int UsageLimit { get; set; }
        public int UsageCount { get; set; }
        public bool IsActive { get; set; }
        public bool IsUserRestricted { get; set; }
        public DateTime ValidateFrom { get; set; }
        public DateTime ValidateTo { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public int? MaxDiscountValue { get; set; }
    }
}
