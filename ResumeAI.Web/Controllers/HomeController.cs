using Microsoft.AspNetCore.Mvc;

namespace ResumeAI.Web.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        if (!string.IsNullOrEmpty(HttpContext.Session.GetString("JwtToken")))
            return RedirectToAction("Index", "Dashboard");
        return RedirectToAction("Login", "Account");
    }
}