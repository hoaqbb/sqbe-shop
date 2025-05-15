using API.DTOs.PaymentDtos;
using API.Helpers;
using API.Interfaces;
using Microsoft.Extensions.Options;
using PaypalServerSdk.Standard.Models;

namespace API.Services
{
    public class PayPalService : IPayPalService
    {
        private readonly IPayPalClientFactory _payPalClientFactory;
        private readonly IOptions<PayPalSettings> _config;

        public PayPalService(IPayPalClientFactory payPalClientFactory, IOptions<PayPalSettings> config)
        {
            _payPalClientFactory = payPalClientFactory;
            _config = config;
        }

        public async Task<string> CreatePaymentAsync(PaymentRequestDto requestDto)
        {
            try
            {
                var client = _payPalClientFactory.CreateClient();
                var amountToUSD = CurrencyHelper.VndToUsd(requestDto.Amount);

                CreateOrderInput orderRequest = new CreateOrderInput
                {
                    Body = new OrderRequest
                    {
                        Intent = CheckoutPaymentIntent.Capture,
                        PurchaseUnits = new List<PurchaseUnitRequest>
                        {
                            new PurchaseUnitRequest
                            {
                                ReferenceId = requestDto.OrderId.ToString(),
                                Amount = new AmountWithBreakdown
                                {
                                    CurrencyCode = _config.Value.CurrencyCode,
                                    MValue = amountToUSD.ToString("0.00")
                                },
                            },
                        },
                        ApplicationContext = new OrderApplicationContext
                        {
                            ReturnUrl = _config.Value.ReturnUrl,
                            CancelUrl = _config.Value.CancelUrl,
                            UserAction = OrderApplicationContextUserAction.PayNow,
                            BrandName = "SQ&BE Shop",
                            LandingPage = OrderApplicationContextLandingPage.Billing
                        }
                    },
                    Prefer = "return=minimal",
                };

                // Create order
                var order = await client.OrdersController.CreateOrderAsync(orderRequest);

                // Find approval URL
                string? approvalUrl = null;
                foreach (var link in order.Data.Links)
                {
                    if (link.Rel == "approve")
                    {
                        approvalUrl = link.Href;
                        break;
                    }
                }

                return approvalUrl;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<PayPalResponseDto> ExecutePaymentAsync(string token)
        {
            try
            {
                var client = _payPalClientFactory.CreateClient();

                // Capture the order (complete the payment)
                var captureResponse = await client.OrdersController.CaptureOrderAsync(new CaptureOrderInput(token, "application/json"));

                var transactionId = captureResponse.Data.Id;
                if (!Guid.TryParse(captureResponse.Data.PurchaseUnits[0].ReferenceId, out Guid orderId))
                    throw new Exception("Invalid order id");
                var status = captureResponse.Data.Status;

                return new PayPalResponseDto
                {
                    TransactionId = transactionId,
                    OrderId = orderId,
                    Status = status!.Value.ToString()
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
