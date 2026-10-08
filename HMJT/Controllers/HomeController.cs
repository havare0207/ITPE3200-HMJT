using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using HMJT.Models;

namespace HMJT.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    // Shows a friendly page for status codes such as 404.
    [Route("Home/Status")]
    public IActionResult Status(int code)
    {
        _logger.LogWarning(
            "Status code {StatusCode} returned for {Path}",
            code,
            HttpContext.Request.Path);

        Response.StatusCode = code;
        ViewData["Code"] = code;

        return View();
    }

    // Handles unexpected application errors.
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        var exceptionFeature =
            HttpContext.Features.Get<IExceptionHandlerPathFeature>();

        if (exceptionFeature != null)
        {
            _logger.LogError(
                exceptionFeature.Error,
                "Unhandled exception while processing {Path}",
                exceptionFeature.Path);
        }

        return View(new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        });
    }
}