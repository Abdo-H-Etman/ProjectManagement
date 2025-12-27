using System;
using Application.Common.Models;

namespace Application.Interfaces.Mailing;

public interface IEmailService
{
    Task<Result> SendEmailAsync(
        string toEmail,
        string subject,
        string body,
        bool isHtml = true,
        CancellationToken cancellationToken = default);

    Task<Result> SendEmailAsync(
        List<string> toEmails,
        string subject,
        string body,
        bool isHtml = true,
        List<string>? ccEmails = null,
        List<string>? bccEmails = null,
        Dictionary<string, byte[]>? attachments = null,
        CancellationToken cancellationToken = default);

    Task<Result> SendPasswordResetEmailAsync(
        string toEmail,
        string userName,
        string resetToken,
        string resetLink,
        CancellationToken cancellationToken = default);    
}
