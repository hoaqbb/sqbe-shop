using API.DTOs.PaymentDtos;
using API.Helpers;
using API.Interfaces;
using Microsoft.Extensions.Options;

namespace API.Services
{
    public class VnPayService : IVnPayService
    {
        private readonly IOptions<VnPaySettings> _config;

        public VnPayService(IOptions<VnPaySettings> config)
        {
            _config = config;
        }
        public string GetPaymentUrl(HttpContext httpContext, PaymentRequestDto vnPayRequestDto)
        {
            VnPayLibrary vnpay = new VnPayLibrary();

            vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", _config.Value.TmnCode);
            vnpay.AddRequestData("vnp_Amount", (vnPayRequestDto.Amount * 100).ToString());
            vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress(httpContext));
            vnpay.AddRequestData("vnp_Locale", "vn");
            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang: " + vnPayRequestDto.OrderId.ToString());
            vnpay.AddRequestData("vnp_OrderType", "other");
            vnpay.AddRequestData("vnp_ReturnUrl", _config.Value.ReturnUrl);
            vnpay.AddRequestData("vnp_TxnRef", vnPayRequestDto.OrderId.ToString());
            vnpay.AddRequestData("vnp_ExpireDate", DateTime.Now.AddMinutes(15).ToString("yyyyMMddHHmmss"));

            string paymentUrl = vnpay.CreateRequestUrl(_config.Value.Url, _config.Value.HashSecret);

            return paymentUrl;
        }

        public VnPayResponseDto ExecutePayment(IQueryCollection collection)
        {
            VnPayLibrary vnpay = new VnPayLibrary();
            foreach (var (key, value) in collection)
            {
                if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                {
                    vnpay.AddResponseData(key, value.ToString());
                }
            }

            long vnp_Amount = Convert.ToInt64(vnpay.GetResponseData("vnp_Amount")) / 100;
            Guid.TryParse(vnpay.GetResponseData("vnp_TxnRef"), out Guid orderId);
            string vnpayTransactionId = vnpay.GetResponseData("vnp_TransactionNo");
            string vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
            string vnp_TransactionStatus = vnpay.GetResponseData("vnp_TransactionStatus");
            string vnp_OrderInfo = vnpay.GetResponseData("vnp_OrderInfo");
            string vnp_SecureHash = collection.FirstOrDefault(p => p.Key == "vnp_SecureHash").Value;
            string vnp_TxnRef = vnpay.GetResponseData("vnp_TxnRef");

            bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, _config.Value.HashSecret);
            if (!checkSignature)
            {
                return new VnPayResponseDto
                {
                    Success = false
                };
            }

            return new VnPayResponseDto
            {
                Success = true,
                OrderId = orderId,
                PaymentMethod = "VnPay",
                OrderDescription = vnp_OrderInfo,
                TransactionId = vnpayTransactionId,
                VnPayResponseCode = vnp_ResponseCode,
                TransactionStatus = vnp_TransactionStatus
            };
        }
    }
}
