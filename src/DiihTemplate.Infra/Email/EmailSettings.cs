#if SMTP
namespace DiihTemplate.Infra.Email;

public class EmailSettings
{
    public const string SectionName = "Email";

    public required string Host { get; set; }
    public int Port { get; set; } = 587;
    public required string Username { get; set; }
    public required string Password { get; set; }
    public required string FromEmail { get; set; }
    public string? FromName { get; set; }
    public bool UseSsl { get; set; } = true;
}
#endif
