
using AdminApi.Application.Contracts.Services;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace AdminApi.Infrastructure.Services;

public sealed class SmtpEmailSender(IConfiguration _configuration) : IEmailSender
{
    public async Task SendAsync(
        string to,
        string subject,
        string body,
        CancellationToken cancellationToken = default)
    {
        var host = _configuration["Email:SmtpHost"]
            ?? throw new InvalidOperationException("SMTP host is not configured.");

        var port = int.Parse(_configuration["Email:SmtpPort"] ?? "587");

        var username = _configuration["Email:Username"]
            ?? throw new InvalidOperationException("SMTP username is not configured.");

        var password = _configuration["Email:Password"]
            ?? throw new InvalidOperationException("SMTP password is not configured.");

        var message = new MimeMessage();
        message.From.Add(MailboxAddress.Parse(username));
        message.To.Add(MailboxAddress.Parse(to));
        message.Subject = subject;
        message.Body = new TextPart("plain") { Text = body };

        using var client = new MailKit.Net.Smtp.SmtpClient();

        await client.ConnectAsync(host, port, SecureSocketOptions.StartTls, cancellationToken);

        await client.AuthenticateAsync(username, password, cancellationToken);
        await client.SendAsync(message, cancellationToken);
        await client.DisconnectAsync(true, cancellationToken);
    }
}
