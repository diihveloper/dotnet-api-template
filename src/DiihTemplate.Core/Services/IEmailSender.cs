#if SMTP
namespace DiihTemplate.Core.Services;

public interface IEmailSender
{
    Task SendAsync(EmailMessage message, CancellationToken cancellationToken = default);
}

public class EmailMessage
{
    public required string To { get; set; }
    public required string Subject { get; set; }
    public required string Body { get; set; }
    public bool IsHtml { get; set; } = true;
    public List<string>? Cc { get; set; }
    public List<string>? Bcc { get; set; }
    public List<EmailAttachment>? Attachments { get; set; }
}

public class EmailAttachment
{
    public required string FileName { get; set; }
    public required byte[] Content { get; set; }
    public string ContentType { get; set; } = "application/octet-stream";
}
#endif
