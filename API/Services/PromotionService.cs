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

        public async Task<PromotionValidationResultDto<PromotionDetailDto>> CreateAsync(CreatePromotionDto dto)
        {
            try
            {
                var validationResult = ValidatePromotionDto(dto);
                if (!string.IsNullOrEmpty(validationResult))
                {
                    return new PromotionValidationResultDto<PromotionDetailDto>
                    {
                        IsValid = false,
                        Message = validationResult
                    };
                }

                // Check if promotion code already exists
                var existingPromotion = await _unitOfWork.Repository<Promotion>()
                    .ExistsAsync(p => p.Code == dto.Code);
                if (existingPromotion)
                {
                    return new PromotionValidationResultDto<PromotionDetailDto>
                    {
                        IsValid = false,
                        Message = "Promotion code already exists"
                    };
                }

                // Create the promotion entity
                var promotion = new Promotion
                {
                    Code = dto.Code.Trim().ToUpper(),
                    Description = dto.Description?.Trim(),
                    DiscountType = dto.DiscountType,
                    DiscountValue = dto.DiscountValue,
                    MinOrderAmount = dto.MinOrderAmount,
                    UsageLimit = dto.UsageLimit,
                    UsageCount = 0,
                    IsActive = dto.IsActive,
                    IsUserRestricted = dto.IsUserRestricted,
                    ValidateFrom = dto.ValidateFrom,
                    ValidateTo = dto.ValidateTo,
                    MaxDiscountValue = dto.DiscountType == 0 ? dto.MaxDiscountValue : null
                };

                await _unitOfWork.Repository<Promotion>().AddAsync(promotion);
                if (await _unitOfWork.SaveChangesAsync())
                    return new PromotionValidationResultDto<PromotionDetailDto>
                    {
                        IsValid = true,
                        Message = "Promotion created successfully!",
                        Promotion = _mapper.Map<PromotionDetailDto>(promotion)
                    };

                return new PromotionValidationResultDto<PromotionDetailDto>
                {
                    IsValid = false,
                    Message = "Falied to creates the promotion"
                };
            }
            catch (Exception ex)
            {
                return new PromotionValidationResultDto<PromotionDetailDto>
                {
                    IsValid = false,
                    Message = "An error occurred while creating the promotion"
                };
            }
        }

        public async Task<PromotionValidationResultDto<PromotionDetailDto>> UpdateAsync(int id,UpdatePromotionDto dto)
        {
            try
            {
                // Check if promotion exists
                var existingPromotion = await _unitOfWork.Repository<Promotion>()
                    .FindByIdAsync(id);
                if (existingPromotion == null)
                {
                    return new PromotionValidationResultDto<PromotionDetailDto>
                    {
                        IsValid = false,
                        Message = "Promotion not found"
                    };
                }

                var validationResult = ValidateUpdatePromotionDto(dto);
                if (!string.IsNullOrEmpty(validationResult))
                {
                    return new PromotionValidationResultDto<PromotionDetailDto>
                    {
                        IsValid = false,
                        Message = validationResult
                    };
                }

                // Check if promotion code already exists
                if (!string.IsNullOrEmpty(dto.Code) && dto.Code.Trim().ToUpper() != existingPromotion.Code)
                {
                    var isPromotionCodeExisted = await _unitOfWork.Repository<Promotion>()
                        .ExistsAsync(p => p.Code == dto.Code);
                    if (isPromotionCodeExisted)
                    {
                        return new PromotionValidationResultDto<PromotionDetailDto>
                        {
                            IsValid = false,
                            Message = "Promotion code already exists"
                        };
                    }
                }

                // Update the promotion entity
                existingPromotion.Code = dto.Code;
                existingPromotion.Description = dto.Description;
                existingPromotion.IsActive = dto.IsActive;
                existingPromotion.DiscountValue = dto.DiscountValue;
                existingPromotion.MaxDiscountValue = dto.MaxDiscountValue;
                existingPromotion.DiscountType = dto.DiscountType;
                existingPromotion.UsageLimit = dto.UsageLimit;
                existingPromotion.IsUserRestricted = dto.IsUserRestricted;
                existingPromotion.ValidateFrom = dto.ValidateFrom;
                existingPromotion.ValidateTo = dto.ValidateTo;
                existingPromotion.MinOrderAmount = dto.MinOrderAmount;
                existingPromotion.UpdateAt = DateTime.UtcNow;

                _unitOfWork.Repository<Promotion>().Update(existingPromotion);
                if (await _unitOfWork.SaveChangesAsync())
                    return new PromotionValidationResultDto<PromotionDetailDto>
                    {
                        IsValid = true,
                        Message = "Promotion updated successfully.",
                        Promotion = _mapper.Map<PromotionDetailDto>(existingPromotion)
                    };

                return new PromotionValidationResultDto<PromotionDetailDto> 
                { 
                    IsValid = false, 
                    Message = "Falied to updates the promotion"
                };
            }
            catch (Exception ex)
            {
                return new PromotionValidationResultDto<PromotionDetailDto>
                {
                    IsValid = false,
                    Message = "An error occurred while updating the promotion"
                };
            }
        }

        public async Task<bool> DeteleAsync(int id)
        {
            var promotion = await _unitOfWork.Repository<Promotion>().FindByIdAsync(id);
            if (promotion == null)
                return false;

            _unitOfWork.Repository<Promotion>().Delete(promotion);
            if (await _unitOfWork.SaveChangesAsync())
                return true;

            return false;
        }

        private string? ValidatePromotionDto(CreatePromotionDto dto)
        {
            // Validate discount type (assuming 0 = percentage, 1 = fixed amount)
            if (dto.DiscountType < 0 || dto.DiscountType > 1)
            {
                return "DiscountType must be 0 (percentage) or 1 (fixed amount)";
            }

            // For percentage discount, validate range and max discount value
            if (dto.DiscountType == 0) // Percentage
            {
                if (dto.DiscountValue > 100)
                {
                    return "Percentage discount cannot exceed 100%";
                }

                if (dto.MaxDiscountValue.HasValue && dto.MaxDiscountValue <= 0)
                {
                    return "MaxDiscountValue must be greater than 0 when specified";
                }
            }
            else // Fixed amount
            {
                if (dto.MaxDiscountValue.HasValue)
                {
                    return "MaxDiscountValue should not be specified for fixed amount discounts";
                }
            }

            // Validate date range
            if (dto.ValidateFrom >= dto.ValidateTo)
            {
                return "ValidateFrom must be earlier than ValidateTo";
            }

            // Validate usage limit
            if (dto.UsageLimit < 0)
            {
                return "UsageLimit cannot be negative";
            }

            // Validate minimum order amount
            if (dto.MinOrderAmount.HasValue && dto.MinOrderAmount <= 0)
            {
                return "MinOrderAmount must be greater than 0 when specified";
            }

            return null;
        }

        private string? ValidateUpdatePromotionDto(UpdatePromotionDto dto)
        {
            // Validate discount type (assuming 0 = percentage, 1 = fixed amount)
            if (dto.DiscountType < 0 || dto.DiscountType > 1)
            {
                return "DiscountType must be 0 (percentage) or 1 (fixed amount)";
            }

            // Validate discount value
            if (dto.DiscountValue <= 0)
            {
                return "DiscountValue must be greater than 0";
            }

            // For percentage discount, validate range and max discount value
            if (dto.DiscountType == 0) // Percentage
            {
                if (dto.DiscountValue > 100)
                {
                    return "Percentage discount cannot exceed 100%";
                }

                if (dto.MaxDiscountValue.HasValue && dto.MaxDiscountValue <= 0)
                {
                    return "MaxDiscountValue must be greater than 0 when specified";
                }
            }
            else // Fixed amount
            {
                if (dto.MaxDiscountValue.HasValue)
                {
                    return "MaxDiscountValue should not be specified for fixed amount discounts";
                }
            }

            // Validate date range
            if (dto.ValidateFrom >= dto.ValidateTo)
            {
                return "ValidateFrom must be earlier than ValidateTo";
            }

            // Allow updating past promotions but warn about active status
            if (dto.ValidateFrom < DateTime.Now.Date && dto.IsActive == true)
            {
                return "Cannot activate a promotion with a past start date";
            }

            // Validate usage limit
            if (dto.UsageLimit < 0)
            {
                return "UsageLimit cannot be negative";
            }

            // Validate minimum order amount
            if (dto.MinOrderAmount.HasValue && dto.MinOrderAmount <= 0)
            {
                return "MinOrderAmount must be greater than 0 when specified";
            }

            return null;
        }
    }
}
