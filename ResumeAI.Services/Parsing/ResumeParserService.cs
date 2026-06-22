using System.Text;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using UglyToad.PdfPig;

namespace ResumeAI.Services.Parsing;

public class ResumeParserService : IResumeParserService
{
    public async Task<string> ExtractTextAsync(string filePath, string fileType)
    {
        return fileType.ToUpperInvariant() switch
        {
            "PDF" => await Task.Run(() => ExtractFromPdf(filePath)),
            "DOCX" => await Task.Run(() => ExtractFromDocx(filePath)),
            _ => throw new NotSupportedException($"File type '{fileType}' is not supported.")
        };
    }

    private static string ExtractFromPdf(string filePath)
    {
        var sb = new StringBuilder();
        using var document = PdfDocument.Open(filePath);
        foreach (var page in document.GetPages())
        {
            sb.AppendLine(page.Text);
        }
        return sb.ToString();
    }

    private static string ExtractFromDocx(string filePath)
    {
        var sb = new StringBuilder();
        using var wordDoc = WordprocessingDocument.Open(filePath, false);
        var body = wordDoc.MainDocumentPart?.Document.Body;
        if (body is null) return string.Empty;

        foreach (var paragraph in body.Elements<Paragraph>())
        {
            sb.AppendLine(paragraph.InnerText);
        }
        return sb.ToString();
    }
}