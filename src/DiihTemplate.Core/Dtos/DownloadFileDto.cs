namespace DiihTemplate.Core.Dtos;

public class DownloadFileDto
{
    public string? Base64Data { get; set; }
    public string? Extension { get; set; }
    public byte[]? FileBytes { get; set; }

    public string? ContentType { get; set; }
    public string? ContentLength { get; set; }
    public string? Url { get; set; }
}