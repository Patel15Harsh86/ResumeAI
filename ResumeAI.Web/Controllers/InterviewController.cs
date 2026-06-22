using Microsoft.AspNetCore.Mvc;
using ResumeAI.Web.Models;
using ResumeAI.Web.Services;
using System.Text.Json;

namespace ResumeAI.Web.Controllers;

public class InterviewController : Controller
{
    private readonly ApiClient _api;
    public InterviewController(ApiClient api) => _api = api;

    private bool IsAuthenticated => !string.IsNullOrEmpty(HttpContext.Session.GetString("JwtToken"));

    public async Task<IActionResult> Index()
    {
        if (!IsAuthenticated) return RedirectToAction("Login", "Account");
        var sessions = await _api.GetAsync<List<InterviewSessionViewModel>>("/api/interview/sessions") ?? new();
        return View(sessions);
    }

    [HttpGet]
    public async Task<IActionResult> Start()
    {
        if (!IsAuthenticated) return RedirectToAction("Login", "Account");
        var resumes = await _api.GetAsync<List<ResumeViewModel>>("/api/resumes") ?? new();
        return View(new StartInterviewViewModel { Resumes = resumes });
    }

    [HttpPost]
    public async Task<IActionResult> Start(StartInterviewViewModel model)
    {
        if (!IsAuthenticated) return RedirectToAction("Login", "Account");

        var response = await _api.PostRawAsync("/api/interview/start", new
        {
            ResumeId = model.SelectedResumeId,
            model.JobTitle,
            model.Difficulty
        });

        if (!response.IsSuccessStatusCode)
        {
            TempData["Error"] = "Failed to start interview. Please try again.";
            return RedirectToAction("Start");
        }

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var sessionId = doc.RootElement.GetProperty("sessionId").GetString();

        return RedirectToAction("Session", new { id = sessionId });
    }

    public async Task<IActionResult> Session(Guid id)
    {
        if (!IsAuthenticated) return RedirectToAction("Login", "Account");
        var result = await _api.GetAsync<InterviewResultViewModel>($"/api/interview/{id}/result");
        if (result == null) return NotFound();
        return View(result);
    }

    [HttpPost]
    public async Task<IActionResult> SubmitAnswer(Guid sessionId, Guid questionId, string answer)
    {
        if (!IsAuthenticated) return RedirectToAction("Login", "Account");

        await _api.PostAsync<object>("/api/interview/answer", new
        {
            SessionId = sessionId,
            QuestionId = questionId,
            Answer = answer
        });

        return RedirectToAction("Session", new { id = sessionId });
    }

    public async Task<IActionResult> Result(Guid id)
    {
        if (!IsAuthenticated) return RedirectToAction("Login", "Account");
        var result = await _api.GetAsync<InterviewResultViewModel>($"/api/interview/{id}/result");
        if (result == null) return NotFound();
        return View(result);
    }
}