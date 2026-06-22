using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ResumeAI.Core.DTOs;
using ResumeAI.Core.Entities;
using ResumeAI.Infrastructure.Data;
using ResumeAI.Services.AI;

namespace ResumeAI.API.Controllers;

[ApiController]
[Route("api/interview")]
[Authorize]
public class InterviewController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IGeminiService _gemini;

    public InterviewController(AppDbContext db, IGeminiService gemini)
    {
        _db = db;
        _gemini = gemini;
    }

    private Guid CurrentUserId =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub")!);

    [HttpPost("start")]
    public async Task<ActionResult<StartInterviewResponse>> Start(StartInterviewRequest request)
    {
        var resume = await _db.Resumes.FirstOrDefaultAsync(r => r.Id == request.ResumeId && r.UserId == CurrentUserId);
        if (resume is null) return NotFound("Resume not found.");
        if (string.IsNullOrWhiteSpace(resume.ExtractedText)) return BadRequest("Resume has no extracted text.");

        List<GeneratedQuestion> generatedQuestions;
        try
        {
            generatedQuestions = await _gemini.GenerateInterviewQuestionsAsync(
                resume.ExtractedText, request.JobTitle, request.Difficulty);
        }
        catch (InvalidOperationException ex) { return StatusCode(429, ex.Message); }
        catch (Exception ex) { return StatusCode(500, $"Failed to generate questions: {ex.Message}"); }

        var session = new InterviewSession
        {
            UserId = CurrentUserId,
            ResumeId = request.ResumeId,
            JobTitle = request.JobTitle,
            Difficulty = request.Difficulty,
            Status = "Active"
        };

        _db.InterviewSessions.Add(session);
        await _db.SaveChangesAsync();

        var questions = generatedQuestions.Select((q, index) => new InterviewQuestion
        {
            SessionId = session.Id,
            QuestionText = q.QuestionText,
            QuestionType = q.QuestionType,
            OrderIndex = index + 1
        }).ToList();

        _db.InterviewQuestions.AddRange(questions);
        await _db.SaveChangesAsync();

        return Ok(new StartInterviewResponse(
            session.Id,
            session.JobTitle,
            session.Difficulty,
            questions.Select(q => new InterviewQuestionDto(q.Id, q.QuestionText, q.QuestionType, q.OrderIndex)).ToList()
        ));
    }

    [HttpPost("answer")]
    public async Task<ActionResult<SubmitAnswerResponse>> SubmitAnswer(SubmitAnswerRequest request)
    {
        var session = await _db.InterviewSessions
            .Include(s => s.Questions)
            .FirstOrDefaultAsync(s => s.Id == request.SessionId && s.UserId == CurrentUserId);

        if (session is null) return NotFound("Session not found.");
        if (session.Status == "Completed") return BadRequest("Session already completed.");

        var question = session.Questions.FirstOrDefault(q => q.Id == request.QuestionId);
        if (question is null) return NotFound("Question not found.");
        if (!string.IsNullOrWhiteSpace(question.UserAnswer)) return BadRequest("Question already answered.");

        QuestionFeedback feedback;
        try
        {
            feedback = await _gemini.EvaluateAnswerAsync(question.QuestionText, request.Answer, session.JobTitle);
        }
        catch (InvalidOperationException ex) { return StatusCode(429, ex.Message); }
        catch (Exception ex) { return StatusCode(500, $"Failed to evaluate answer: {ex.Message}"); }

        question.UserAnswer = request.Answer;
        question.AIFeedback = feedback.Feedback;
        question.Score = feedback.Score;

        var allAnswered = session.Questions.All(q => !string.IsNullOrWhiteSpace(q.UserAnswer) || q.Id == question.Id);

        if (allAnswered)
        {
            var answeredQuestions = session.Questions
                .Where(q => !string.IsNullOrWhiteSpace(q.UserAnswer) || q.Id == question.Id)
                .ToList();
            session.TotalScore = answeredQuestions.Any()
                ? Math.Round(answeredQuestions.Average(q => q.Id == question.Id ? feedback.Score : q.Score), 2)
                : 0;
            session.Status = "Completed";
            session.CompletedAt = DateTime.UtcNow;
        }

        await _db.SaveChangesAsync();

        return Ok(new SubmitAnswerResponse(question.Id, feedback.Feedback, feedback.Score, allAnswered));
    }

    [HttpGet("{sessionId}/result")]
    public async Task<ActionResult<InterviewResultResponse>> GetResult(Guid sessionId)
    {
        var session = await _db.InterviewSessions
            .Include(s => s.Questions)
            .FirstOrDefaultAsync(s => s.Id == sessionId && s.UserId == CurrentUserId);

        if (session is null) return NotFound();

        return Ok(new InterviewResultResponse(
            session.Id, session.JobTitle, session.Difficulty, session.TotalScore, session.Status,
            session.Questions.OrderBy(q => q.OrderIndex).Select(q => new InterviewQuestionResultDto(
                q.Id, q.QuestionText, q.QuestionType, q.UserAnswer, q.AIFeedback, q.Score)).ToList(),
            session.StartedAt, session.CompletedAt));
    }

    [HttpGet("sessions")]
    public async Task<ActionResult<List<InterviewSessionListItem>>> GetSessions()
    {
        var sessions = await _db.InterviewSessions
            .Where(s => s.UserId == CurrentUserId)
            .OrderByDescending(s => s.StartedAt)
            .Select(s => new InterviewSessionListItem(
                s.Id, s.JobTitle, s.Difficulty, s.TotalScore, s.Status, s.StartedAt))
            .ToListAsync();

        return Ok(sessions);
    }
}