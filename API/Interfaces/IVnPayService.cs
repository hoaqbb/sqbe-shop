using API.DTOs.PaymentDtos;

namespace API.Interfaces
{
    public interface IVnPayService
    {
        string GetPaymentUrl(HttpContext httpContext, PaymentRequestDto vnPayRequestDto);
        VnPayResponseDto ExecutePayment(IQueryCollection collection);
    }
}
