namespace ResumeAI.Core.DTOs;

public record ResumeUploadResponse(Guid Id, string FileName, string FileType, string Status, DateTime UploadedAt);
public record ResumeListItem(Guid Id, string FileName, string FileType, string Status, DateTime UploadedAt);
public record ResumeDetail(Guid Id, string FileName, string FileType, string Status, string? ExtractedText, DateTime UploadedAt);