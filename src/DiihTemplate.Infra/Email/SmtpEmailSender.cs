#if SMTP
using DiihTemplate.Core.Services;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace DiihTemplate.Infra.Email;

public class SmtpEmailSender(IOptions<EmailSettings> options, ILogger<SmtpEmailSender> logger) : IEmailSender
{
    private readonly EmailSettings _settings = options.Value;

    public async Task SendAsync(EmailMessage message, CancellationToken cancellationToken = default)
    {
        var email = new MimeMessage();
        email.From.Add(new MailboxAddress(_settings.FromName ?? _settings.FromEmail, _settings.FromEmail));
        email.To.Add(MailboxAddress.Parse(message.To));
        email.Subject = message.Subject;

        if (message.Cc is { Count: > 0 })
            foreach (var cc in message.Cc)
                email.Cc.Add(MailboxAddress.Parse(cc));

        if (message.Bcc is { Count: > 0 })
            foreach (var bcc in message.Bcc)
                email.Bcc.Add(MailboxAddress.Parse(bcc));

        var builder = new BodyBuilder();

        if (message.IsHtml)
            builder.HtmlBody = message.Body;
        else
            builder.TextBody = message.Body;

        if (message.Attachments is { Count: > 0 })
            foreach (var attachment in message.Attachments)
                builder.Attachments.Add(attachment.FileName, attachment.Content,
                    ContentType.Parse(attachment.ContentType));

        email.Body = builder.ToMessageBody();

        using var client = new SmtpClient();
        await client.ConnectAsync(_settings.Host, _settings.Port, _settings.UseSsl,
            cancellationToken);
        await client.AuthenticateAsync(_settings.Username, _settings.Password, cancellationToken);
        await client.SendAsync(email, cancellationToken);
        await client.DisconnectAsync(true, cancellationToken);

        logger.LogInformation("Email sent to {To} with subject {Subject}", message.To, message.Subject);
    }
}
#endif
