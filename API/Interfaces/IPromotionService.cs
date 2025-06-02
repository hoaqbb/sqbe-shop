using API.Data.Entities;
using API.DTOs.PromotionDtos;

namespace API.Interfaces
{
    public interface IPromotionService
    {
        Task<PromotionValidationResultDto<Promotion>> ValidatePromotionAsync(string promotionCode, decimal subtotal, Guid? userId);
        Task<PromotionValidationResultDto<PromotionDto>> ApplyPromotionAsync(string promotionCode, decimal subtotal, Guid? userId);
        Task<PromotionValidationResultDto<PromotionDetailDto>> CreateAsync(CreatePromotionDto dto);
        Task<PromotionValidationResultDto<PromotionDetailDto>> UpdateAsync(int id, UpdatePromotionDto dto);
        Task<bool> DeteleAsync(int id);
        decimal CalculateDiscount(decimal subtotal, Promotion promotion);
    }
}
