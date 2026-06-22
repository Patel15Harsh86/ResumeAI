using ResumeAI.Core.DTOs;

namespace ResumeAI.Services.AI;

public interface IGeminiService
{
    Task<GeminiAnalysisResult> AnalyzeResumeAsync(string resumeText, string? jobDescription = null);
    Task<GeminiAnalysisResult> MatchResumeToJobAsync(string resumeText, string jobDescription, List<string> requiredSkills);
    Task<List<GeneratedQuestion>> GenerateInterviewQuestionsAsync(string resumeText, string jobTitle, string difficulty);
    Task<QuestionFeedback> EvaluateAnswerAsync(string question, string answer, string jobTitle);
}

public record GeneratedQuestion(string QuestionText, string QuestionType);
public record QuestionFeedback(string Feedback, decimal Score);