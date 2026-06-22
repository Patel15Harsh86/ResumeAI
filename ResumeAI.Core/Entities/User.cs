namespace ResumeAI.Core.Entities;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = "Candidate";
    public string? ProfilePicture { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Resume> Resumes { get; set; } = new List<Resume>();
    public ICollection<JobDescription> JobDescriptions { get; set; } = new List<JobDescription>();
    public ICollection<InterviewSession> InterviewSessions { get; set; } = new List<InterviewSession>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}