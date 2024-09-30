using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;


namespace IAMService.Controllers;

public class AuthController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly IIdentityServerInteractionService _interaction;

    public AuthController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IIdentityServerInteractionService interaction)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _interaction = interaction;
    }

    [HttpPost]
    [Route("api/auth/register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var user = new IdentityUser { UserName = request.UserName, Email = request.Email };
        var result = await _userManager.CreateAsync(user, request.Password);

        if (result.Succeeded)
        {
            // Automatically sign the user in after registration
            await _signInManager.SignInAsync(user, isPersistent: false);
            return RedirectToAction("Index", "Home");
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return View();
    }

    public class RegisterRequest
    {
        public string UserName { get; set;}
        public string Email { get; set;}
        public string Password { get; set;}

    }

    [HttpGet]
    [Route("/auth/login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        return View();
    }

    // Action method for user login
    [HttpPost]
    [Route("api/auth/login")]
    public async Task<IActionResult> LoginAPI([FromBody]LoginRequest request)
    {
        var result = await _signInManager.PasswordSignInAsync(request.UserName!, request.Password!, false, lockoutOnFailure: true);
        if (result.Succeeded)
        {
             return Ok(new { ReturnUrl = request.ReturnUrl });
        }

        return Unauthorized();
    }

    public class LoginRequest 
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string RedirectUri { get; set; }
        public string State { get; set; }
        public string ReturnUrl { get; set; }

    }

    // Action method for user logout
    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

}
