using Microsoft.AspNetCore.Mvc;
using ResumeAI.Web.Models;
using ResumeAI.Web.Services;
using System.Text.Json;

namespace ResumeAI.Web.Controllers;

public class AccountController : Controller
{
    private readonly ApiClient _api;
    public AccountController(ApiClient api) => _api = api;

    [HttpGet]
    public IActionResult Login() => View(new LoginViewModel());

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        var response = await _api.PostRawAsync("/api/auth/login", new { model.Email, model.Password });

        if (!response.IsSuccessStatusCode)
        {
            model.ErrorMessage = "Invalid email or password.";
            return View(model);
        }

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var token = doc.RootElement.GetProperty("token").GetString();
        var fullName = doc.RootElement.GetProperty("fullName").GetString();
        var role = doc.RootElement.GetProperty("role").GetString();

        HttpContext.Session.SetString("JwtToken", token!);
        HttpContext.Session.SetString("FullName", fullName!);
        HttpContext.Session.SetString("Role", role!);
        HttpContext.Session.SetString("Email", model.Email);

        return RedirectToAction("Index", "Dashboard");
    }

    [HttpGet]
    public IActionResult Register() => View(new RegisterViewModel());

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        var response = await _api.PostRawAsync("/api/auth/register",
            new { model.Email, model.FullName, model.Password });

        if (!response.IsSuccessStatusCode)
        {
            model.ErrorMessage = "Registration failed. Email may already be in use.";
            return View(model);
        }

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var token = doc.RootElement.GetProperty("token").GetString();
        var fullName = doc.RootElement.GetProperty("fullName").GetString();
        var role = doc.RootElement.GetProperty("role").GetString();

        HttpContext.Session.SetString("JwtToken", token!);
        HttpContext.Session.SetString("FullName", fullName!);
        HttpContext.Session.SetString("Role", role!);
        HttpContext.Session.SetString("Email", model.Email);

        return RedirectToAction("Index", "Dashboard");
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }
}