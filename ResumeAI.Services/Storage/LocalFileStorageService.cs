using Microsoft.Extensions.Configuration;

namespace ResumeAI.Services.Storage;

public class LocalFileStorageService : IFileStorageService
{
    private readonly string _basePath;

    public LocalFileStorageService(IConfiguration config)
    {
        _basePath = config["FileStorage:BasePath"] ?? Path.Combine(Directory.GetCurrentDirectory(), "UploadedResumes");
        if (!Directory.Exists(_basePath))
            Directory.CreateDirectory(_basePath);
    }

    public async Task<string> SaveFileAsync(Stream fileStream, string fileName, Guid userId)
    {
        var userFolder = Path.Combine(_basePath, userId.ToString());
        if (!Directory.Exists(userFolder))
            Directory.CreateDirectory(userFolder);

        var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";
        var fullPath = Path.Combine(userFolder, uniqueFileName);

        using var output = new FileStream(fullPath, FileMode.Create);
        await fileStream.CopyToAsync(output);

        return fullPath;
    }

    public Task<Stream> GetFileAsync(string filePath)
    {
        Stream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        return Task.FromResult(stream);
    }

    public void DeleteFile(string filePath)
    {
        if (File.Exists(filePath))
            File.Delete(filePath);
    }
}