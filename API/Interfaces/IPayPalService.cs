using API.DTOs.PaymentDtos;
using PaypalServerSdk.Standard.Models;

namespace API.Interfaces
{
    public interface IPayPalService
    {
        Task<string> CreatePaymentAsync(PaymentRequestDto requestDto);
        Task<PayPalResponseDto> ExecutePaymentAsync(string token);
    }
}
