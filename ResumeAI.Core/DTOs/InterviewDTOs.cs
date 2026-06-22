namespace ResumeAI.Core.DTOs;

public record StartInterviewRequest(Guid ResumeId, string JobTitle, string Difficulty);
public record StartInterviewResponse(Guid SessionId, string JobTitle, string Difficulty, List<InterviewQuestionDto> Questions);
public record InterviewQuestionDto(Guid Id, string QuestionText, string QuestionType, int OrderIndex);
public record SubmitAnswerRequest(Guid SessionId, Guid QuestionId, string Answer);
public record SubmitAnswerResponse(Guid QuestionId, string AIFeedback, decimal Score, bool IsLastQuestion);
public record InterviewResultResponse(
    Guid SessionId,
    string JobTitle,
    string Difficulty,
    decimal TotalScore,
    string Status,
    List<InterviewQuestionResultDto> Questions,
    DateTime StartedAt,
    DateTime? CompletedAt
);
public record InterviewQuestionResultDto(
    Guid Id,
    string QuestionText,
    string QuestionType,
    string? UserAnswer,
    string? AIFeedback,
    decimal Score
);
public record InterviewSessionListItem(
    Guid Id,
    string JobTitle,
    string Difficulty,
    decimal TotalScore,
    string Status,
    DateTime StartedAt
);