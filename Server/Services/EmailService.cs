using System.Net;
using System.Net.Mail;

namespace Server.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<bool> SendPasswordResetEmailAsync(string toEmail, string newPassword)
    {
        try
        {
            var smtpHost = "smtp.gmail.com";
            var smtpPort = 587;
            var fromEmail = "anhnthe172469@fpt.edu.vn";
            var fromPassword = "mrju qfuy crvb geao";

            using var smtpClient = new SmtpClient(smtpHost, smtpPort)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(fromEmail, fromPassword)
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(fromEmail, "News Management System"),
                Subject = "Mật khẩu mới của bạn",
                Body = $@"
                    <html>
                    <body style='font-family: Arial, sans-serif;'>
                        <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                            <h2 style='color: #333;'>Đặt lại mật khẩu</h2>
                            <p>Xin chào,</p>
                            <p>Bạn đã yêu cầu đặt lại mật khẩu cho tài khoản của mình.</p>
                            <p>Mật khẩu mới của bạn là: <strong style='background-color: #f0f0f0; padding: 5px 10px; border-radius: 3px;'>{newPassword}</strong></p>
                            <p style='color: #666;'>Vui lòng đăng nhập và thay đổi mật khẩu sau khi đăng nhập thành công.</p>
                            <hr style='margin: 20px 0;'>
                            <p style='font-size: 12px; color: #888;'>
                                Email này được gửi tự động, vui lòng không trả lời email này.<br>
                                © 2025 News Management System
                            </p>
                        </div>
                    </body>
                    </html>",
                IsBodyHtml = true
            };

            mailMessage.To.Add(toEmail);

            await smtpClient.SendMailAsync(mailMessage);

            _logger.LogInformation($"Password reset email sent successfully to {toEmail}");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to send password reset email to {toEmail}");
            
            _logger.LogInformation($"[DEVELOPMENT] New password for {toEmail}: {newPassword}");
            return true;
        }
    }
}
