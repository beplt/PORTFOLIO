using Microsoft.AspNetCore.Mvc;

namespace portfolio.Controllers;

// Friendly pages for errors.
public class ErrorsController : Controller
{
    // /error : anything unexpected. The exception handler sends visitors here.
    [Route("error")]
    public IActionResult Index()
    {
        return View();
    }

    // /error/404 : a page or id that does not exist.
    [Route("error/{code:int}")]
    public IActionResult Status(int code)
    {
        return code == 404 ? View("NotFound") : View("Index");
    }
}