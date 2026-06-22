using Microsoft.AspNetCore.Mvc;

namespace ResumeAI.Web.Controllers;

public class ErrorController : Controller
{
    [Route("Error/{statusCode}")]
    public IActionResult HttpStatusCodeHandler(int statusCode)
    {
        ViewData["StatusCode"] = statusCode;
        ViewData["Message"] = statusCode switch
        {
            404 => "The page you're looking for doesn't exist.",
            401 => "You need to login to access this page.",
            403 => "You don't have permission to access this page.",
            500 => "Something went wrong on our end.",
            _ => "An unexpected error occurred."
        };
        return View("Error");
    }
}