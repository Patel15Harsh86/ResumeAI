namespace ResumeAI.Services.Storage;

public interface IFileStorageService
{
    Task<string> SaveFileAsync(Stream fileStream, string fileName, Guid userId);
    Task<Stream> GetFileAsync(string filePath);
    void DeleteFile(string filePath);
}