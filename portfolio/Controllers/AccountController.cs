using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace portfolio.Controllers;

// Sign in and sign out for the editor side.
public class AccountController : Controller
{
    private readonly IConfiguration _config;

    public AccountController(IConfiguration config)
    {
        _config = config;
    }

    // GET /login : the sign-in form.
    [HttpGet("login")]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewBag.ReturnUrl = returnUrl;
        return View();
    }

    // POST /login : check the password and sign in.
    [HttpPost("login")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(string password, string? returnUrl = null)
    {
        // The real password comes from User Secrets, never from a file in the repository.
        var expected = _config["Editor:Password"];

        if (string.IsNullOrEmpty(expected) || !SamePassword(password, expected))
        {
            ModelState.AddModelError("", "Wrong password.");
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        var claims = new List<Claim> { new(ClaimTypes.Name, "editor") };
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

        // Only go back to an address on this site.
        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return LocalRedirect(returnUrl);
        }
        return Redirect("/editor/courses");
    }

    // POST /logout : sign out.
    [HttpPost("logout")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Courses");
    }

    // Compares two passwords in a way that takes the same time whether or not they match.
    private static bool SamePassword(string? typed, string expected)
    {
        var a = Encoding.UTF8.GetBytes(typed ?? "");
        var b = Encoding.UTF8.GetBytes(expected);
        return CryptographicOperations.FixedTimeEquals(SHA256.HashData(a), SHA256.HashData(b));
    }
}