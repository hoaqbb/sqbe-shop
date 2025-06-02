using System.ComponentModel.DataAnnotations;

namespace API.DTOs.PromotionDtos
{
    public class UpdatePromotionDto
    {
        [Required]
        public string Code { get; set; } = null!;
        public string? Description { get; set; }
        [Required]
        [Range(0, 1)]
        public int DiscountType { get; set; }
        [Required]
        [Range(0, int.MaxValue)]
        public decimal DiscountValue { get; set; }
        public decimal? MinOrderAmount { get; set; }
        [Range(0, int.MaxValue)]
        public int UsageLimit { get; set; }
        [Range(0, int.MaxValue)]
        public bool IsActive { get; set; }
        public bool IsUserRestricted { get; set; }
        [Required]
        public DateTime ValidateFrom { get; set; }
        [Required]
        public DateTime ValidateTo { get; set; }
        public int? MaxDiscountValue { get; set; }
    }
}
