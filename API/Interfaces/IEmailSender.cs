using API.Data.Entities;
using API.DTOs.OrderDtos;

namespace API.Interfaces
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string toEmail, string subject, string htmlMessage);
        Task SendVerificationEmailAsync(User user);
        Task SendResetPasswordEmailAsync(User user);
        Task SendOrderConfirmationEmailAsync(OrderDetailDto order);
    }
}
