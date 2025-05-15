using API.Data.Enums;

namespace API.DTOs.PromotionDtos
{
    public class PromotionDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = null!;
        public string? Description { get; set; }
        public DiscountTypeEnum DiscountType { get; set; }
        public decimal DiscountValue { get; set; }
        public decimal? MaxDiscountValue { get; set; }
        public decimal? MinOrderAmount { get; set; }
    }
}
