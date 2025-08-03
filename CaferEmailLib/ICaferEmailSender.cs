using MailKit.Security;
using MimeKit;

namespace CaferEmailLib;

public interface ICaferEmailSender
{
    MimeMessage BuildEmail(string recipientEmail, string? recipientName, string subject, string body); 
    Task<Exception?> SendEmailAsync(MimeMessage emailMessage, SecureSocketOptions secureSocketOptions);
}