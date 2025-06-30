using API.Data.Entities;
using API.Data.Enums;
using API.DTOs.OrderDtos;
using API.DTOs.PromotionDtos;
using API.DTOs.PaymentDtos;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITokenService _tokenService;
        private readonly ICartService _cartService;
        private readonly EcommerceDbContext _context;
        private readonly IPromotionService _promotionService;
        private readonly IVnPayService _vnPayService;
        private readonly IPayPalService _payPalService;
        private readonly ILogger<OrderService> _logger;

        public OrderService(
            IUnitOfWork unitOfWork,
            IHttpContextAccessor httpContextAccessor,
            ITokenService tokenService,
            ICartService cartService,
            EcommerceDbContext context,
            IPromotionService promotionService,
            IVnPayService vnPayService,
            IPayPalService payPalService,
            ILogger<OrderService> logger
            )
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _tokenService = tokenService;
            _cartService = cartService;
            _context = context;
            _promotionService = promotionService;
            _vnPayService = vnPayService;
            _payPalService = payPalService;
            _logger = logger;
        }

        public async Task<CreateOrderResultDto> CreateOrderAsync(OrderRequestDto requestDto)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
                return new CreateOrderResultDto { Success = false, Message = "Invalid HTTP context" };

            var userId = httpContext.GetUserIdFromTokenInsideCookie(_tokenService);

            var cart = await _cartService.GetCartAsync(httpContext);

            if (cart == null || !cart.CartItems.Any())
                return new CreateOrderResultDto { Success = false, Message = "Cart is empty" };

            decimal calculatedSubtotal = cart.CartItems
                .Sum(item => (item.Discount > 0)
                    ? item.Price * (item.Discount / 100) * item.Quantity
                    : item.Price * item.Quantity);

            if(calculatedSubtotal != requestDto.Subtotal)
                return new CreateOrderResultDto { Success = false, Message = "Subtotal mismatch" };

            decimal? calculatedTotal = calculatedSubtotal;

            var promotionResult = await ValidatePromotionAsync(requestDto, calculatedSubtotal, userId);

            if (!string.IsNullOrEmpty(requestDto.PromotionCode) && !promotionResult.IsValid)
                return new CreateOrderResultDto { Success = false, Message = promotionResult.Message };

            calculatedTotal = calculatedTotal - promotionResult.DiscountAmount + requestDto.ShippingFee;

            // check amount is match
            if (calculatedTotal != requestDto.Amount)
                return new CreateOrderResultDto { Success = false, Message = "Amount mismatch" };
            Order? order = null;
            try
            {
                await _unitOfWork.BeginTransactionAsync();
                foreach (var item in cart.CartItems)
                {
                    if (item.ProductVariant?.Id > 0)
                    {
                        var productVariant = await _context.ProductVariants
                            .FirstOrDefaultAsync(pv => pv.Id == item.ProductVariant.Id);

                        if (productVariant == null)
                        {
                            throw new Exception($"Product variant with ID: {item.ProductVariant.Id} not found");
                        }

                        if (productVariant.Quantity < item.Quantity)
                        {
                            throw new Exception($"Product '{item.Name}' has insufficient quantity");
                        }

                        // minus the quantity of products in stock
                        productVariant.Quantity -= item.Quantity;
                        _context.ProductVariants.Update(productVariant);
                    }
                }

                order = new Order
                {
                    Id = Guid.NewGuid(),
                    Fullname = requestDto.Fullname,
                    Email = requestDto.Email,
                    PhoneNumber = requestDto.PhoneNumber,
                    DeliveryMethod = requestDto.DeliveryMethod,
                    Note = requestDto.Note,
                    Amount = requestDto.Amount,
                    Subtotal = calculatedSubtotal,
                    DiscountAmount = (decimal)promotionResult.DiscountAmount,
                    Address = requestDto.Address,
                    ShippingFee = requestDto.ShippingFee,
                    PromotionId = promotionResult.Promotion?.Id,
                    UserId = userId,
                    Status = (int)OrderStatusEnum.Pending
                };

                await _context.Orders.AddAsync(order);
                await _context.SaveChangesAsync();

                // Tạo chi tiết đơn hàng
                var orderItems = new List<OrderItem>();
                foreach (var item in cart.CartItems)
                {
                    var orderItem = new OrderItem
                    {
                        OrderId = order.Id,
                        ProductName = item.Name,
                        Price = item.Price,
                        Quantity = item.Quantity,
                        Discount = (short)item.Discount,
                        ProductImageUrl = item.Photo,
                        ProductVariantId = item.ProductVariant?.Id,
                        ProductColor = item.ProductVariant?.Color,
                        ProductSize = item.ProductVariant?.Size
                    };

                    orderItems.Add(orderItem);
                    await _context.OrderItems.AddAsync(orderItem);
                }

                // If promotion was used, record it
                if (promotionResult.Promotion != null && userId != null)
                {
                    promotionResult.Promotion.UsageCount++;
                    _context.Promotions.Update(promotionResult.Promotion);

                    var userPromotion = new UserPromotion
                    {
                        UserId = (Guid)userId,
                        PromotionId = promotionResult.Promotion.Id,
                        UsedAt = DateTime.UtcNow
                    };
                    await _context.UserPromotions.AddAsync(userPromotion);
                }

                await _context.SaveChangesAsync();
                await _unitOfWork.CommitAsync();

                // delete cart
                try
                {
                    var cartCleared = await _cartService.ClearCartAsync(httpContext);
                    if (!cartCleared)
                    {
                        // We don't want to fail the order just because cart clearing failed
                        _logger.LogWarning("Failed to clear cart after successful order creation. Order ID: {OrderId}", order.Id);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error clearing cart after order creation. Order ID: {OrderId}", order.Id);
                }
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                return new CreateOrderResultDto
                {
                    Success = false,
                    Message = $"Order creation failed: {ex.Message}"
                };
            }

            return await ProcessPaymentAsync(requestDto, order, userId, httpContext);
        }
        private async Task<PromotionValidationResultDto<Promotion>> ValidatePromotionAsync(
        OrderRequestDto requestDto,
        decimal calculatedSubtotal,
        Guid? userId)
        {
            if (string.IsNullOrEmpty(requestDto.PromotionCode))
            {
                return new PromotionValidationResultDto<Promotion>
                {
                    IsValid = true,
                    DiscountAmount = 0
                };
            }

            return await _promotionService.ValidatePromotionAsync(
                requestDto.PromotionCode,
                calculatedSubtotal,
            userId);
        }

        private async Task<CreateOrderResultDto> ProcessPaymentAsync(
            OrderRequestDto requestDto,
            Order order,
            Guid? userId,
            HttpContext httpContext)
        {
            switch (requestDto.PaymentMethod)
            {
                case "cod":
                    return new CreateOrderResultDto
                    {
                        Success = true,
                        Message = "Order created successfully",
                        OrderId = order.Id,
                        PaymentMethod = "cod",
                        RedirectUrl = null
                    };

                case "vnpay":
                    try
                    {
                        // For VNPay, generate payment URL and return for redirection
                        var paymentUrl = _vnPayService.GetPaymentUrl(httpContext, new PaymentRequestDto
                        {
                            OrderId = order.Id,
                            Amount = order.Amount,
                        });

                        var payment = new Payment
                        {
                            Method = "Internet Banking",
                            Amount = order.Amount,
                            CurrencyCode = "VND",
                            OrderId = order.Id,
                            Provider = "VnPay",
                            Status = false,
                            UserId = userId
                        };

                        await _context.AddAsync(payment);
                        await _context.SaveChangesAsync();
                        order.PaymentId = payment.Id;
                        await _context.SaveChangesAsync();

                        return new CreateOrderResultDto
                        {
                            Success = true,
                            Message = "Please redirect to payment gateway",
                            OrderId = order.Id,
                            PaymentMethod = "vnpay",
                            RedirectUrl = paymentUrl
                        };
                    }
                    catch (Exception ex)
                    {
                        // If payment URL generation fails, mark order as failed
                        order.Status = (int)OrderStatusEnum.Cancelled;
                        await _context.SaveChangesAsync();

                        return new CreateOrderResultDto
                        {
                            Success = false,
                            Message = $"Failed to create payment URL: {ex.Message}",
                            OrderId = order.Id,
                            PaymentMethod = "vnpay"
                        };
                    }

                case "paypal":
                    try 
                    {
                        // For PayPal, generate payment URL and return for redirection
                        var paymentUrl = await _payPalService.CreatePaymentAsync(new PaymentRequestDto
                        {
                            OrderId = order.Id,
                            Amount = order.Amount,
                        });

                        var amountToUSD = CurrencyHelper.VndToUsd(order.Amount);

                        var payment = new Payment
                        {
                            Method = "Internet Banking",
                            Amount = amountToUSD,
                            CurrencyCode = "USD",
                            OrderId = order.Id,
                            Provider = "PayPal",
                            Status = false,
                            UserId = userId
                        };
                        
                        await _context.AddAsync(payment);
                        await _context.SaveChangesAsync();
                        order.PaymentId = payment.Id;
                        await _context.SaveChangesAsync();

                        return new CreateOrderResultDto
                        {
                            Success = true,
                            Message = "Please redirect to payment gateway",
                            OrderId = order.Id,
                            PaymentMethod = "paypal",
                            RedirectUrl = paymentUrl
                        };
                    }
                    catch (Exception ex)
                    {
                        // If payment URL generation fails, mark order as failed
                        order.Status = (int)OrderStatusEnum.Cancelled;
                        await _context.SaveChangesAsync();

                        return new CreateOrderResultDto
                        {
                            Success = false,
                            Message = $"Failed to create payment URL: {ex.Message}",
                            OrderId = order.Id,
                            PaymentMethod = "paypal"
                        };
                    }

                default:
                    return new CreateOrderResultDto
                    {
                        Success = false,
                        Message = $"Unsupported payment method: {requestDto.PaymentMethod}",
                        OrderId = order.Id
                    };
            }
        }

        public async Task<OrderDetailDto?> GetOrderByIdAsync(Guid orderId, Guid? userId, string? role)
        {
            try
            {
                var order = await _unitOfWork.Repository<Order>()
                    .GetSingleProjectedAsync<OrderDetailDto>(o => o.Id == orderId, _mapper.ConfigurationProvider);

                if (role == "Admin")
                {
                    return order;
                }
                if (role == "Customer")
                {
                    if(order!.UserId == userId)
                    {
                        return order;
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving order with ID: {OrderId}", orderId);
                throw;
            }
            
        }
    }
}
