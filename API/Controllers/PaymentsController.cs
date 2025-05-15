using API.Data.Entities;
using API.Data.Enums;
using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using PaypalServerSdk.Standard.Exceptions;

namespace API.Controllers
{
    public class PaymentsController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IVnPayService _vnPayService;
        private readonly EcommerceDbContext _context;
        private readonly IPayPalService _payPalService;

        public PaymentsController(IUnitOfWork unitOfWork, IVnPayService vnPayService, EcommerceDbContext context, IPayPalService payPalService)
        {
            _unitOfWork = unitOfWork;
            _vnPayService = vnPayService;
            _context = context;
            _payPalService = payPalService;
        }

        [HttpGet("vnpay-callback")]
        public async Task<IActionResult> VnPayPaymentCallback()
        {
            try
            {
                var queryCollection = HttpContext.Request.Query;
                var vnPayResponse = _vnPayService.ExecutePayment(queryCollection);

                if (vnPayResponse == null || !vnPayResponse.Success || vnPayResponse.VnPayResponseCode != "00")
                {
                    return BadRequest("Payment verification failed");
                }

                var payment = await _context.Payments
                    .Include(x => x.Order)
                    .SingleOrDefaultAsync(x => x.OrderId == vnPayResponse.OrderId);
                if (payment == null)
                {
                    return BadRequest("Payment record not found");
                }

                if (payment.Order == null)
                {
                    return NotFound("Order not found");
                }

                return await ProcessSuccessfulPayment(payment.Order, payment, vnPayResponse.TransactionId);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpGet("paypal-callback")]
        public async Task<IActionResult> PaypalExecute()
        {
            try
            {
                var queryCollection = HttpContext.Request.Query;
                queryCollection.TryGetValue("token", out StringValues token);

                if (string.IsNullOrEmpty(token))
                {
                    return BadRequest("Token is missing");
                }
            
                var paypalResponse = await _payPalService.ExecutePaymentAsync(token);
                if (paypalResponse == null || paypalResponse.Status != "Completed")
                {
                    return BadRequest("Payment verification failed");
                }

                var payment = await _context.Payments
                    .Include(x => x.Order)
                    .SingleOrDefaultAsync(x =>
                        x.OrderId == paypalResponse.OrderId);

                if (payment == null)
                {
                    return NotFound("Payment record not found");
                }
                if (payment.Order == null)
                {
                    return NotFound("Order not found");
                }

                return await ProcessSuccessfulPayment(payment.Order, payment, paypalResponse.TransactionId);
            }
            catch (ApiException e)
            {
                Console.WriteLine(e.Message);
                return BadRequest();
            }
        }

        private async Task<IActionResult> ProcessSuccessfulPayment(Order order, Payment payment, string transactionId)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // Update order status
                order.Status = (int)OrderStatusEnum.Confirmed;

                // Update payment information
                payment.Status = true;
                payment.TransactionId = transactionId;
                payment.UpdateAt = DateTime.UtcNow;

                _context.Orders.Update(order);
                _context.Payments.Update(payment);
                
                await _context.SaveChangesAsync();
                await _unitOfWork.CommitAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                var errorMessage = ex.InnerException?.Message ?? ex.Message;
                return BadRequest($"Transaction failed: {errorMessage}");
            }
        }
    }
}
