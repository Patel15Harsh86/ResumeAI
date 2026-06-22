using Microsoft.AspNetCore.Mvc;
using ResumeAI.Web.Models;
using ResumeAI.Web.Services;

namespace ResumeAI.Web.Controllers;

public class ResumeController : Controller
{
    private readonly ApiClient _api;
    public ResumeController(ApiClient api) => _api = api;

    private bool IsAuthenticated => !string.IsNullOrEmpty(HttpContext.Session.GetString("JwtToken"));

    public async Task<IActionResult> Index()
    {
        if (!IsAuthenticated) return RedirectToAction("Login", "Account");
        var resumes = await _api.GetAsync<List<ResumeViewModel>>("/api/resumes") ?? new();
        return View(resumes);
    }

    [HttpPost]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        if (!IsAuthenticated) return RedirectToAction("Login", "Account");

        if (file == null || file.Length == 0)
        {
            TempData["Error"] = "Please select a file.";
            return RedirectToAction("Index");
        }

        using var content = new MultipartFormDataContent();
        using var stream = file.OpenReadStream();
        content.Add(new StreamContent(stream), "file", file.FileName);

        var result = await _api.PostFormAsync<ResumeViewModel>("/api/resumes/upload", content);

        if (result == null)
            TempData["Error"] = "Upload failed. Please try again.";
        else
            TempData["Success"] = $"Resume '{file.FileName}' uploaded and parsed successfully!";

        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Analyze(Guid id)
    {
        if (!IsAuthenticated) return RedirectToAction("Login", "Account");

        var result = await _api.PostAsync<AnalysisViewModel>("/api/analysis/analyze", new { ResumeId = id });

        if (result == null)
        {
            TempData["Error"] = "Analysis failed. Please try again later.";
            return RedirectToAction("Index");
        }

        return View(result);
    }

    public async Task<IActionResult> Delete(Guid id)
    {
        if (!IsAuthenticated) return RedirectToAction("Login", "Account");
        await _api.DeleteAsync($"/api/resumes/{id}");
        TempData["Success"] = "Resume deleted successfully.";
        return RedirectToAction("Index");
    }
}