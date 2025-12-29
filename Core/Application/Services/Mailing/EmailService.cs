using System.Net;
using System.Net.Mail;
using System.Text;
using Application.Common.Models;
using Application.Common.Settings;
using Application.Interfaces.Logging;
using Application.Interfaces.Mailing;
using Microsoft.Extensions.Options;

namespace Application.Services.Mailing;

public class EmailService : IEmailService
{
    private readonly EmailSettings _emailSettings;
    private readonly ILoggerManager _logger;

    public EmailService(
        IOptions<EmailSettings> emailSettings,
        ILoggerManager logger)
    {
        _emailSettings = emailSettings.Value;
        _logger = logger;
    }
    public async Task<Result> SendEmailAsync(
        string toEmail,
        string subject,
        string body,
        bool isHtml = true,
        CancellationToken cancellationToken = default)
    {
        return await SendEmailAsync(
            new List<string> { toEmail },
            subject,
            body,
            isHtml,
            cancellationToken: cancellationToken
        );
    }
    public async Task<Result> SendEmailAsync(
        List<string> toEmails,
        string subject,
        string body,
        bool isHtml = true,
        List<string>? ccEmails = null,
        List<string>? bccEmails = null,
        Dictionary<string, byte[]>? attachments = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            using var mailMessage = new MailMessage();

            mailMessage.From = new MailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName);
            
            foreach (var toEmail in toEmails)
            {
                mailMessage.To.Add(new MailAddress(toEmail));
            }

            if (ccEmails != null)
            {
                foreach (var ccEmail in ccEmails)
                {
                    mailMessage.CC.Add(new MailAddress(ccEmail));
                }
            }

            if (bccEmails != null)
            {
                foreach (var bccEmail in bccEmails)
                {
                    mailMessage.Bcc.Add(new MailAddress(bccEmail));
                }
            }

            mailMessage.Subject = subject;
            mailMessage.Body = body;
            mailMessage.IsBodyHtml = isHtml;
            mailMessage.BodyEncoding = Encoding.UTF8;
            mailMessage.SubjectEncoding = Encoding.UTF8;

            if (attachments != null)
            {
                foreach (var attachment in attachments)
                {
                    var stream = new MemoryStream(attachment.Value);
                    mailMessage.Attachments.Add(new Attachment(stream, attachment.Key));
                }
            }

            using var smtpClient = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.SmtpPort)
            {
                EnableSsl = _emailSettings.EnableSsl,
                UseDefaultCredentials = _emailSettings.UseDefaultCredentials,
                Credentials = new NetworkCredential(_emailSettings.Username, _emailSettings.Password),
                Timeout = 30000
            };
            await smtpClient.SendMailAsync(mailMessage, cancellationToken);

            _logger.LogInfo($"Email sent to: {string.Join(", ", toEmails)} with subject: {subject}");
            return Result.Success("Email sent successfully.");

        }
        catch (SmtpException smtpEx)
        {
            _logger.LogError($"SMTP Error in SendEmailAsync: {smtpEx.Message}");
            return Result.Failure("Failed to send email due to SMTP error.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in SendEmailAsync: {ex.Message}");
            return Result.Failure("Failed to send email.");
        }
    }
    
    public async Task<Result> SendPasswordResetEmailAsync(
        string toEmail,
        string userName,
        string resetToken,
        string resetLink,
        CancellationToken cancellationToken = default)
    {
        var subject = "Password Reset Request";
        var body = $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background: linear-gradient(135deg, #fa709a 0%, #fee140 100%); color: white; padding: 30px; text-align: center; border-radius: 10px 10px 0 0; }}
        .content {{ background: #f9f9f9; padding: 30px; border-radius: 0 0 10px 10px; }}
        .button {{ display: inline-block; background: #fa709a; color: white; padding: 12px 30px; text-decoration: none; border-radius: 5px; margin: 20px 0; }}
        .warning {{ background: #fff3cd; border: 1px solid #ffc107; padding: 15px; border-radius: 5px; margin: 15px 0; }}
        .footer {{ text-align: center; color: #666; font-size: 12px; margin-top: 20px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>🔐 Password Reset Request</h1>
        </div>
        <div class='content'>
            <h2>Hi {userName},</h2>
            <p>We received a request to reset your password. Click the button below to create a new password:</p>
            
            <a href='{resetLink}' class='button'>Reset Password</a>
            
            <div class='warning'>
                <p><strong>⚠️ Security Notice:</strong></p>
                <ul style='margin: 10px 0;'>
                    <li>This link will expire in 24 hours</li>
                    <li>If you didn't request this, please ignore this email</li>
                    <li>Never share this link with anyone</li>
                </ul>
            </div>
            
            <p style='color: #666; font-size: 14px;'>
                If the button doesn't work, copy and paste this link into your browser:<br>
                <code style='background: #eee; padding: 5px; display: block; margin-top: 10px; word-break: break-all;'>{resetLink}</code>
            </p>
        </div>
        <div class='footer'>
            <p>© 2024 Project Management System. All rights reserved.</p>
        </div>
    </div>
</body>
</html>";

        return await SendEmailAsync(
            toEmail,
            subject,
            body,
            isHtml: true,
            cancellationToken: cancellationToken);
    }

}