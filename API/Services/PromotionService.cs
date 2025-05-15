using API.Data.Entities;
using API.Data.Enums;
using API.DTOs.PromotionDtos;
using API.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
    public class PromotionService : IPromotionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PromotionService(
            IUnitOfWork unitOfWork,
            IMapper mapper
            )
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        
        public async Task<PromotionValidationResultDto<Promotion>> ValidatePromotionAsync(string promotionCode, decimal subtotal, Guid? userId)
        {
            if (string.IsNullOrEmpty(promotionCode))
            {
                return new PromotionValidationResultDto<Promotion>
                {
                    IsValid = false,
                    Message = "No promotion code provided"
                };
            }

            var promotion = await _unitOfWork.Repository<Promotion>()
                .GetSingleOrDefaultAsync(p => p.Code == promotionCode
                    && p.IsActive
                    && p.ValidateFrom < DateTime.UtcNow
                    && p.ValidateTo > DateTime.UtcNow);

            if (promotion == null)
            {
                return new PromotionValidationResultDto<Promotion>
                {
                    IsValid = false,
                    Message = "Invalid promotion code"
                };
            }

            // Check if promotion is restricted to registered users
            if (promotion.IsUserRestricted && userId == null)
            {
                return new PromotionValidationResultDto<Promotion>
                {
                    IsValid = false,
                    Message = "This promotion is for registered users only"
                };
            }

            // Check if promotion has remaining usage count
            if (promotion.UsageLimit - promotion.UsageCount <= 0)
            {
                return new PromotionValidationResultDto<Promotion>
                {
                    IsValid = false,
                    Message = "Promotion code has been fully redeemed"
                };
            }

            // Check if user has already used this promotion
            if (userId != null && promotion.IsUserRestricted)
            {
                var isUsed = await _unitOfWork.Repository<UserPromotion>()
                    .ExistsAsync(x => x.UserId == userId && x.PromotionId == promotion.Id);

                if (isUsed)
                {
                    return new PromotionValidationResultDto<Promotion>
                    {
                        IsValid = false,
                        Message = "You have already used this promotion"
                    };
                }
            }

            // Calculate discount amount
            decimal discountAmount = CalculateDiscount(subtotal, promotion);

            return new PromotionValidationResultDto<Promotion>
            {
                IsValid = true,
                Promotion = promotion,
                DiscountAmount = discountAmount,
                Message = "Promotion is valid"
            };
        }

        public decimal CalculateDiscount(decimal subtotal, Promotion promotion)
        {
            decimal discountAmount = 0;
            if (promotion.MinOrderAmount > subtotal) return discountAmount;

            if (promotion.DiscountType == (int)DiscountTypeEnum.FixedAmount)
            {
                discountAmount = promotion.DiscountValue;

                // Cannot discount more than the subtotal
                if (discountAmount > subtotal)
                {
                    discountAmount = subtotal;
                }
            }
            else if (promotion.DiscountType == (int)DiscountTypeEnum.Percentage)
            {
                discountAmount = subtotal * (promotion.DiscountValue / 100);

                // Apply maximum discount if applicable
                if (promotion.MaxDiscountValue.HasValue
                    && discountAmount > promotion.MaxDiscountValue.Value)
                {
                    discountAmount = promotion.MaxDiscountValue.Value;
                }
            }

            return discountAmount;
        }

        public async Task<PromotionValidationResultDto<PromotionDto>> ApplyPromotionAsync(string promotionCode, decimal subtotal, Guid? userId)
        {
            var validationResult = await ValidatePromotionAsync(promotionCode, subtotal, userId);

            if (!validationResult.IsValid)
            {
                return new PromotionValidationResultDto<PromotionDto>
                {
                    IsValid = false,
                    Message = validationResult.Message
                };
            }

            return new PromotionValidationResultDto<PromotionDto>
            {
                IsValid = true,
                Message = "Promotion applied successfully",
                Promotion = _mapper.Map<PromotionDto>(validationResult.Promotion),
                DiscountAmount = validationResult.DiscountAmount
            };
        }
    }
}
