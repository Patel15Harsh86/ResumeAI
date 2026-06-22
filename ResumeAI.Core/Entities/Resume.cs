namespace ResumeAI.Core.Entities;

public class Resume
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty;
    public string? ExtractedText { get; set; }
    public string Status { get; set; } = "Pending";
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;
    public ICollection<AnalysisResult> AnalysisResults { get; set; } = new List<AnalysisResult>();
    public ICollection<InterviewSession> InterviewSessions { get; set; } = new List<InterviewSession>();
}