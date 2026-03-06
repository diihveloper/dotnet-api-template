#if STORAGE
namespace DiihTemplate.Core.Services;

public interface IFileStorage
{
    Task<string> UploadAsync(Stream stream, string fileName, string? folder = null,
        CancellationToken cancellationToken = default);

    Task<Stream> DownloadAsync(string path, CancellationToken cancellationToken = default);

    Task DeleteAsync(string path, CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(string path, CancellationToken cancellationToken = default);
}
#endif
