using System.Net.Sockets;
using MailKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace CaferEmailLib;

public class CaferEmailSender: ICaferEmailSender
{
    private readonly string _emailAddress;
    private readonly string _emailPassword;
    private readonly string _issuer;
    private readonly string _smtpHost;
    private readonly int _port;
    
    public CaferEmailSender(string emailAddress, string emailPassword,
        string issuer, string smtpHost, int port)
    {
        _emailAddress = emailAddress;
        _emailPassword = emailPassword;
        _issuer = issuer;
        _smtpHost = smtpHost;
        _port = port;
    }
    
    public MimeMessage BuildEmail(string recipientEmail, string? recipientName, string subject, string body)
    {
        if (string.IsNullOrWhiteSpace(recipientName))
        {
            recipientName = "";
        }

        var emailMessage = new MimeMessage();
        emailMessage.From.Add(new MailboxAddress(_issuer, _emailAddress));
        emailMessage.To.Add(new MailboxAddress(recipientName, recipientEmail));
        emailMessage.Subject = subject;
        emailMessage.Body = new TextPart("html") { Text = body };

        return emailMessage;
    }
    
    public async Task<Exception?> SendEmailAsync(MimeMessage emailMessage,
        SecureSocketOptions secureSocketOptions)
    {
        using var client = new SmtpClient();
        try
        {
            await client.ConnectAsync(_smtpHost, _port, secureSocketOptions);
            await client.AuthenticateAsync(_emailAddress, _emailPassword);
            await client.SendAsync(emailMessage);
            await client.DisconnectAsync(true);
        }
        catch (ArgumentNullException argEx)
        {
            return argEx;
        }
        catch (ArgumentOutOfRangeException outOfRangeEx)
        {
            return outOfRangeEx;
        }
        catch (ArgumentException argEx)
        {
            return argEx;
        }
        catch (ObjectDisposedException objDisposedEx)
        {
            return objDisposedEx;
        }
        catch (ServiceNotConnectedException notConnectedEx)
        {
            return notConnectedEx;
        }
        catch (ServiceNotAuthenticatedException notAuthEx)
        {
            return notAuthEx;
        }
        catch (InvalidOperationException invalidOpEx)
        {
            return invalidOpEx;
        }
        catch (NotSupportedException notSupportedEx)
        {
            return notSupportedEx;
        }
        catch (OperationCanceledException canceledEx)
        {
            return canceledEx;
        }
        catch (SocketException socketEx)
        {
            return socketEx;
        }
        catch (SslHandshakeException sslEx)
        {
            return sslEx;
        }
        catch (SaslException saslEx)
        {
            return saslEx;
        }
        catch (AuthenticationException authEx)
        {
            return authEx;
        }
        catch (SmtpCommandException smtpCmdEx)
        {
            return smtpCmdEx;
        }
        catch (SmtpProtocolException smtpProtoEx)
        {
            return smtpProtoEx;
        }
        catch (IOException ioEx)
        {
            return ioEx;
        }
        catch (CommandException cmdEx)
        {
            return cmdEx;
        }
        catch (ProtocolException protocolEx)
        {
            return protocolEx;
        }
        catch (Exception ex)
        {
            return ex;
        }

        return null;
    }

}