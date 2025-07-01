using API.Data.Entities;
using API.DTOs.OrderDtos;
using API.Helpers;
using API.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using System.Text;

namespace API.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailSettings _emailSettings;
        private readonly ITokenService _tokenService;

        public EmailSender(IOptions<EmailSettings> emailSettings, ITokenService tokenService)
        {
            _emailSettings = emailSettings.Value;
            _tokenService = tokenService;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string htmlMessage)
        {
            try
            {
                var email = new MimeMessage();
                email.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.SenderEmail));
                email.To.Add(MailboxAddress.Parse(toEmail));
                email.Subject = subject;

                var builder = new BodyBuilder { HtmlBody = htmlMessage };
                email.Body = builder.ToMessageBody();

                using var smtp = new SmtpClient();
                await smtp.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.Port, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(_emailSettings.Username, _emailSettings.Password);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                throw;
            }
            
        }

        public async Task SendVerificationEmailAsync(User user)
        {
            var emailToken = _tokenService.GenerateEmailVerificationToken(user);

            var verifyUrl = $"{_emailSettings.CallbackUrl}account/verify-email?token={emailToken}";

            var subject = "Xác thực tài khoản của bạn tại SQ&BE Shop";
            var htmlContent = $@"
            <h3>Xin chào {user.Lastname} {user.Firstname},</h3>
            <p>Bạn vừa đăng ký tài khoản trên SQ&BE Shop. Nhấn vào liên kết bên dưới để xác thực email:</p>
            <p><a href='{verifyUrl}'>Xác thực ngay</a></p>
            <p>Liên kết này sẽ hết hạn sau 15 phút.</p>
            ";

            await SendEmailAsync(user.Email, subject, htmlContent);
        }

        public async Task SendResetPasswordEmailAsync(User user)
        {
            var token = _tokenService.GenerateEmailVerificationToken(user);

            var resetUrl = $"{_emailSettings.CallbackUrl}account/reset-password?token={token}";

            var subject = "Đặt lại mật khẩu";
            var htmlContent = $@"
                <h3>Xin chào {user.Lastname} {user.Firstname},</h3>
                <p>Bạn đã yêu cầu đặt lại mật khẩu. Nhấn vào liên kết bên dưới để tiếp tục:</p>
                <p><a href='{resetUrl}'>Đặt lại mật khẩu</a></p>
                <p>Nếu bạn không yêu cầu, vui lòng bỏ qua email này.</p>";

            await SendEmailAsync(user.Email, subject, htmlContent);
        }

        public async Task SendOrderConfirmationEmailAsync(OrderDetailDto order)
        {
            var subject = $"[Đặt hàng thành công] Đơn hàng #{order.Id}";
            var body = BuildOrderConfirmationEmailBody(order);

            await SendEmailAsync(order.Email, subject, body);
        }

        private string BuildOrderConfirmationEmailBody(OrderDetailDto order)
        {
            var sb = new StringBuilder();

            sb.AppendLine("<h3>Cảm ơn bạn đã đặt hàng tại SQ&BE Shop!</h3>");
            sb.AppendLine($"<p><strong>Họ tên:</strong> {order.Fullname}</p>");
            sb.AppendLine($"<p><strong>Địa chỉ:</strong> {order.Address}</p>");
            sb.AppendLine($"<p><strong>Điện thoại:</strong> {order.PhoneNumber}</p>");
            if (!string.IsNullOrEmpty(order.Note))
            {
                sb.AppendLine($"<p><strong>Ghi chú:</strong> {order.Note}</p>");
            }

            // payment info
            if (order.PaymentInfo is not null)
            {
                sb.AppendLine("<h4>💳 Thông tin thanh toán:</h4>");
                sb.AppendLine("<ul>");
                sb.AppendLine($"<li><strong>Phương thức:</strong> {order.PaymentInfo.Method}</li>");
                if (!string.IsNullOrEmpty(order.PaymentInfo.Provider))
                    sb.AppendLine($"<li><strong>Nhà cung cấp:</strong> {order.PaymentInfo.Provider}</li>");
                if (!string.IsNullOrEmpty(order.PaymentInfo.TransactionId))
                    sb.AppendLine($"<li><strong>Mã giao dịch:</strong> {order.PaymentInfo.TransactionId}</li>");
                sb.AppendLine($"<li><strong>Trạng thái:</strong> {(order.PaymentInfo.Status ? "Đã thanh toán" : "Chưa thanh toán")}</li>");
                sb.AppendLine($"<li><strong>Tiền thanh toán:</strong> {order.PaymentInfo.Amount:N0} {order.PaymentInfo.CurrencyCode}</li>");
                sb.AppendLine("</ul>");
            }

            sb.AppendLine("<h4>📦 Chi tiết đơn hàng:</h4>");
            sb.AppendLine("<table border='1' cellspacing='0' cellpadding='5'>");
            sb.AppendLine("<thead><tr>" +
                "<th>Ảnh</th>" +
                "<th>Tên sản phẩm</th>" +
                "<th>Số lượng</th>" +
                "<th>Đơn giá</th>" +
                "<th>Giảm giá(%)</th>" +
                "<th>Thành tiền</th>" +
                "</tr></thead>");
            sb.AppendLine("<tbody>");

            foreach (var item in order.OrderItems)
            {
                var total = item.Quantity * item.Price;
                sb.AppendLine("<tr>");
                sb.AppendLine($"<td><img style='width: 70px' src='{item.ProductImageUrl}'/></td>");
                sb.AppendLine($"<td>" +
                    $"<div><strong>{item.ProductName}</strong></div>" +
                    $"<div>{item.ProductColor} / {item.ProductSize}</div>" +
                    $"</td>");
                sb.AppendLine($"<td>{item.Quantity}</td>");
                sb.AppendLine($"<td>{item.Price:N0}đ</td>");
                sb.AppendLine($"<td>{item.Discount}</td>");
                sb.AppendLine($"<td>{total:N0}đ</td>");
                sb.AppendLine("</tr>");
            }

            sb.AppendLine("</tbody></table>");

            sb.AppendLine($"<p><strong>Tạm tính:</strong> {order.Subtotal:N0}đ</p>");
            sb.AppendLine($"<p><strong>Giảm giá:</strong> -{order.DiscountAmount:N0}đ</p>");
            sb.AppendLine($"<p><strong>Phí giao hàng:</strong> {order.ShippingFee:N0}đ</p>");
            sb.AppendLine($"<p><strong>Tổng thanh toán:</strong> <span style='color:green'><b>{order.Amount:N0}đ</b></span></p>");
            sb.AppendLine("<p style='margin-top: 20px;'>Chúng tôi sẽ liên hệ với bạn để xác nhận đơn hàng và tiến hành giao hàng sớm nhất!</p>");

            return sb.ToString();
        }
    }
}
