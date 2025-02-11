using AceJobAgencySecurity.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;

public class ErrorController : Controller
{
    private readonly IWebHostEnvironment _env;

    public ErrorController(IWebHostEnvironment env)
    {
        _env = env;
    }

    // This action will handle errors like 404, 403, etc.
    public IActionResult Index(int code)
    {
        var errorModel = new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
            ShowRequestId = _env.IsDevelopment(),
            ErrorTitle = "Unknown Error",
            ErrorMessage = "An unexpected error occurred. Please try again later."
        };

        switch (code)
        {
            case 404:
                errorModel.ErrorTitle = "Page Not Found";
                errorModel.ErrorMessage = "Sorry, the page you're looking for doesn't exist. Please check the URL or go back to the homepage.";
                break;
            case 403:
                errorModel.ErrorTitle = "Forbidden";
                errorModel.ErrorMessage = "You don't have permission to access this page. Please contact support if you believe this is an error.";
                break;
            default:
                errorModel.ErrorTitle = "Error";
                errorModel.ErrorMessage = "An unexpected error occurred while processing your request.";
                break;
        }

        return View("Error", errorModel);  // Return to the error page with the custom model
    }

    // You can also have a fallback error page
    public IActionResult Error()
    {
        var errorMessage = TempData["ErrorMessage"]?.ToString() ?? "An unexpected error occurred.";
        var errorModel = new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
            ShowRequestId = _env.IsDevelopment(),
            ErrorTitle = "Oops! Something went wrong.",
            ErrorMessage = errorMessage
        };

        return View("Error", errorModel);
    }
}



