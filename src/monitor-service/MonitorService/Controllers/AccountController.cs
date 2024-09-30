using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace MonitorService.Controllers;

[Route("[controller]")]
public class AccountController : Controller
{
    public AccountController()
    {
        
    }

    [HttpGet]
    [Route("Login")]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    [Route("Login")]
    public async Task<IActionResult> Login(string username, string password)
    {
        // Validate the user's credentials (simplified for this example)
        if (username == "admin" && password == "password")
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, username)
            };

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));

            return Redirect("/hc-ui");
        }

        // If login fails, return to the login view
        return View();
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login");
    }
}
