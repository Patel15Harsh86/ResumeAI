namespace ResumeAI.Services.Parsing;

public interface IResumeParserService
{
    Task<string> ExtractTextAsync(string filePath, string fileType);
}