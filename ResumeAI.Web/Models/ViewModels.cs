namespace ResumeAI.Web.Models;

public class LoginViewModel
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? ErrorMessage { get; set; }
}

public class RegisterViewModel
{
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? ErrorMessage { get; set; }
}

public class DashboardViewModel
{
    public string FullName { get; set; } = string.Empty;
    public int TotalResumes { get; set; }
    public int TotalAnalyses { get; set; }
    public int TotalInterviews { get; set; }
    public decimal AverageScore { get; set; }
    public List<ResumeViewModel> RecentResumes { get; set; } = new();
    public List<InterviewSessionViewModel> RecentInterviews { get; set; } = new();
}

public class ResumeViewModel
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; }
}

public class AnalysisViewModel
{
    public Guid Id { get; set; }
    public decimal OverallScore { get; set; }
    public decimal SkillScore { get; set; }
    public decimal ExperienceScore { get; set; }
    public decimal FormatScore { get; set; }
    public List<string> Strengths { get; set; } = new();
    public List<string> Weaknesses { get; set; } = new();
    public List<string> Suggestions { get; set; } = new();
    public string AISummary { get; set; } = string.Empty;
    public DateTime AnalyzedAt { get; set; }
}

public class JobViewModel
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<string> RequiredSkills { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}

public class CreateJobViewModel
{
    public string Title { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string RequiredSkillsText { get; set; } = string.Empty;
}

public class JobMatchViewModel
{
    public Guid AnalysisId { get; set; }
    public string JobTitle { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;
    public decimal OverallScore { get; set; }
    public decimal SkillScore { get; set; }
    public decimal ExperienceScore { get; set; }
    public decimal FormatScore { get; set; }
    public List<SkillMatchViewModel> SkillMatches { get; set; } = new();
    public List<string> Strengths { get; set; } = new();
    public List<string> Weaknesses { get; set; } = new();
    public List<string> Suggestions { get; set; } = new();
    public string AISummary { get; set; } = string.Empty;
}

public class SkillMatchViewModel
{
    public string SkillName { get; set; } = string.Empty;
    public bool IsMatched { get; set; }
    public decimal MatchScore { get; set; }
    public string Category { get; set; } = string.Empty;
}

public class InterviewSessionViewModel
{
    public Guid Id { get; set; }
    public string JobTitle { get; set; } = string.Empty;
    public string Difficulty { get; set; } = string.Empty;
    public decimal TotalScore { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime StartedAt { get; set; }
}

public class InterviewResultViewModel
{
    public Guid SessionId { get; set; }
    public string JobTitle { get; set; } = string.Empty;
    public string Difficulty { get; set; } = string.Empty;
    public decimal TotalScore { get; set; }
    public string Status { get; set; } = string.Empty;
    public List<InterviewQuestionViewModel> Questions { get; set; } = new();
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}

public class InterviewQuestionViewModel
{
    public Guid Id { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public string QuestionType { get; set; } = string.Empty;
    public string? UserAnswer { get; set; }
    public string? AIFeedback { get; set; }
    public decimal Score { get; set; }
    public int OrderIndex { get; set; }
}

public class StartInterviewViewModel
{
    public List<ResumeViewModel> Resumes { get; set; } = new();
    public string JobTitle { get; set; } = string.Empty;
    public Guid SelectedResumeId { get; set; }
    public string Difficulty { get; set; } = "Medium";
}