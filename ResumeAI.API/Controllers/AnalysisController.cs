using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ResumeAI.Core.DTOs;
using ResumeAI.Core.Entities;
using ResumeAI.Infrastructure.Data;
using ResumeAI.Services.AI;

namespace ResumeAI.API.Controllers;

[ApiController]
[Route("api/analysis")]
[Authorize]
public class AnalysisController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IGeminiService _gemini;

    public AnalysisController(AppDbContext db, IGeminiService gemini)
    {
        _db = db;
        _gemini = gemini;
    }

    private Guid CurrentUserId =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub")!);

    [HttpPost("analyze")]
    public async Task<ActionResult<AnalysisResponse>> Analyze(AnalyzeRequest request)
    {
        var resume = await _db.Resumes.FirstOrDefaultAsync(r => r.Id == request.ResumeId && r.UserId == CurrentUserId);
        if (resume is null) return NotFound("Resume not found.");
        if (string.IsNullOrWhiteSpace(resume.ExtractedText)) return BadRequest("Resume has no extracted text to analyze.");

        string? jobDescriptionText = null;
        if (request.JobDescriptionId.HasValue)
        {
            var job = await _db.JobDescriptions.FirstOrDefaultAsync(j => j.Id == request.JobDescriptionId && j.UserId == CurrentUserId);
            jobDescriptionText = job?.Description;
        }

        GeminiAnalysisResult aiResult;
        try
        {
            aiResult = await _gemini.AnalyzeResumeAsync(resume.ExtractedText, jobDescriptionText);
        }
        catch (InvalidOperationException ex)
        {
            return StatusCode(429, ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"AI analysis failed: {ex.Message}");
        }

        var analysis = new AnalysisResult
        {
            ResumeId = resume.Id,
            JobDescriptionId = request.JobDescriptionId,
            OverallScore = aiResult.OverallScore,
            SkillScore = aiResult.SkillScore,
            ExperienceScore = aiResult.ExperienceScore,
            FormatScore = aiResult.FormatScore,
            Strengths = JsonSerializer.Serialize(aiResult.Strengths),
            Weaknesses = JsonSerializer.Serialize(aiResult.Weaknesses),
            Suggestions = JsonSerializer.Serialize(aiResult.Suggestions),
            AISummary = aiResult.Summary
        };

        _db.AnalysisResults.Add(analysis);
        resume.Status = "Analyzed";
        await _db.SaveChangesAsync();

        return Ok(new AnalysisResponse(
            analysis.Id, analysis.OverallScore, analysis.SkillScore, analysis.ExperienceScore, analysis.FormatScore,
            aiResult.Strengths, aiResult.Weaknesses, aiResult.Suggestions, analysis.AISummary, analysis.AnalyzedAt));
    }

    [HttpGet("{resumeId}")]
    public async Task<ActionResult<List<AnalysisResponse>>> GetByResumeId(Guid resumeId)
    {
        var results = await _db.AnalysisResults
            .Where(a => a.ResumeId == resumeId && a.Resume.UserId == CurrentUserId)
            .OrderByDescending(a => a.AnalyzedAt)
            .ToListAsync();

        var response = results.Select(a => new AnalysisResponse(
            a.Id, a.OverallScore, a.SkillScore, a.ExperienceScore, a.FormatScore,
            JsonSerializer.Deserialize<List<string>>(a.Strengths ?? "[]") ?? new(),
            JsonSerializer.Deserialize<List<string>>(a.Weaknesses ?? "[]") ?? new(),
            JsonSerializer.Deserialize<List<string>>(a.Suggestions ?? "[]") ?? new(),
            a.AISummary ?? "", a.AnalyzedAt)).ToList();

        return Ok(response);
    }

    [HttpPost("match-job")]
    public async Task<ActionResult<JobMatchResponse>> MatchJob(JobMatchRequest request)
    {
        var resume = await _db.Resumes.FirstOrDefaultAsync(r => r.Id == request.ResumeId && r.UserId == CurrentUserId);
        if (resume is null) return NotFound("Resume not found.");
        if (string.IsNullOrWhiteSpace(resume.ExtractedText)) return BadRequest("Resume has no extracted text.");

        var job = await _db.JobDescriptions.FirstOrDefaultAsync(j => j.Id == request.JobDescriptionId && j.UserId == CurrentUserId);
        if (job is null) return NotFound("Job description not found.");

        var requiredSkills = JsonSerializer.Deserialize<List<string>>(job.RequiredSkills ?? "[]") ?? new();

        GeminiAnalysisResult aiResult;
        try
        {
            aiResult = await _gemini.MatchResumeToJobAsync(resume.ExtractedText, job.Description, requiredSkills);
        }
        catch (InvalidOperationException ex) { return StatusCode(429, ex.Message); }
        catch (Exception ex) { return StatusCode(500, $"AI matching failed: {ex.Message}"); }

        // keyword-based skill matching
        var skillMatcher = new ResumeAI.Services.Matching.SkillMatcherService();
        var skillResults = skillMatcher.MatchSkills(resume.ExtractedText, requiredSkills);

        // save analysis
        var analysis = new AnalysisResult
        {
            ResumeId = resume.Id,
            JobDescriptionId = job.Id,
            OverallScore = aiResult.OverallScore,
            SkillScore = aiResult.SkillScore,
            ExperienceScore = aiResult.ExperienceScore,
            FormatScore = aiResult.FormatScore,
            Strengths = JsonSerializer.Serialize(aiResult.Strengths),
            Weaknesses = JsonSerializer.Serialize(aiResult.Weaknesses),
            Suggestions = JsonSerializer.Serialize(aiResult.Suggestions),
            AISummary = aiResult.Summary
        };

        _db.AnalysisResults.Add(analysis);
        await _db.SaveChangesAsync();

        // save skill matches
        var skillMatchEntities = skillResults.Select(s => new SkillMatch
        {
            AnalysisId = analysis.Id,
            SkillName = s.Skill,
            IsMatched = s.IsMatched,
            MatchScore = s.Score,
            Category = "Technical"
        }).ToList();

        _db.SkillMatches.AddRange(skillMatchEntities);
        resume.Status = "Analyzed";
        await _db.SaveChangesAsync();

        return Ok(new JobMatchResponse(
            analysis.Id, job.Title, job.Company,
            analysis.OverallScore, analysis.SkillScore, analysis.ExperienceScore, analysis.FormatScore,
            skillMatchEntities.Select(s => new SkillMatchResult(s.SkillName, s.IsMatched, s.MatchScore, s.Category)).ToList(),
            aiResult.Strengths, aiResult.Weaknesses, aiResult.Suggestions, aiResult.Summary));
    }
}