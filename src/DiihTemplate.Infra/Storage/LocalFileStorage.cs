#if STORAGE
using DiihTemplate.Core.Services;
using DiihTemplate.Core.Exceptions;
using Microsoft.Extensions.Options;

namespace DiihTemplate.Infra.Storage;

public class LocalFileStorage(IOptions<StorageSettings> options) : IFileStorage
{
    private readonly string _basePath = options.Value.BasePath;

    public async Task<string> UploadAsync(Stream stream, string fileName, string? folder = null,
        CancellationToken cancellationToken = default)
    {
        var relativePath = folder is not null ? Path.Combine(folder, fileName) : fileName;
        var fullPath = Path.Combine(_basePath, relativePath);

        var directory = Path.GetDirectoryName(fullPath)!;
        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        await using var fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write);
        await stream.CopyToAsync(fileStream, cancellationToken);

        return relativePath;
    }

    public Task<Stream> DownloadAsync(string path, CancellationToken cancellationToken = default)
    {
        var fullPath = Path.Combine(_basePath, path);

        if (!File.Exists(fullPath))
            throw new EntityNotFoundException($"File not found: {path}");

        Stream stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
        return Task.FromResult(stream);
    }

    public Task DeleteAsync(string path, CancellationToken cancellationToken = default)
    {
        var fullPath = Path.Combine(_basePath, path);

        if (File.Exists(fullPath))
            File.Delete(fullPath);

        return Task.CompletedTask;
    }

    public Task<bool> ExistsAsync(string path, CancellationToken cancellationToken = default)
    {
        var fullPath = Path.Combine(_basePath, path);
        return Task.FromResult(File.Exists(fullPath));
    }
}
#endif
