using Microsoft.AspNetCore.Mvc;
using AceJobAgencySecurity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.AspNetCore.Authentication;
using AceJobAgencySecurity.Services;
using System.Net; // For HtmlEncode

public class AccountController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly ILogger<AccountController> _logger;
    private readonly IDistributedCache _cache; // For managing sessions
    private readonly IAuditLogger _auditLogger; // Inject the audit logger service
    private readonly PasswordExpirationService _passwordExpirationService; // Service to check password expiration

    public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, ILogger<AccountController> logger, IDistributedCache cache, IAuditLogger auditLogger, PasswordExpirationService passwordExpirationService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
        _cache = cache;
        _auditLogger = auditLogger; // Initialize the audit logger
        _passwordExpirationService = passwordExpirationService;
    }

    // GET: Display the login form
    [HttpGet]
    public IActionResult Login()
    {
        return View(new LoginViewModel());
    }

    // POST: Handle login form submission
    [HttpPost]
    [ValidateAntiForgeryToken]  // Add CSRF protection
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    // Check if the user is locked out
                    if (await _userManager.IsLockedOutAsync(user))
                    {
                        var lockoutEnd = await _userManager.GetLockoutEndDateAsync(user);
                        if (lockoutEnd <= DateTimeOffset.UtcNow)
                        {
                            // Unlock user if the lockout period has expired
                            await _userManager.SetLockoutEndDateAsync(user, null);
                            await _userManager.ResetAccessFailedCountAsync(user);
                        }
                        else
                        {
                            ModelState.AddModelError("", "Your account is locked. Please try again later.");
                            return View(model);
                        }
                    }
                    // Check if the password has expired
                    var passwordExpired = await _passwordExpirationService.HasPasswordExpiredAsync(user);
                    if (passwordExpired)
                    {
                        ModelState.AddModelError("", "Your password has expired. Please change your password.");
                        return View(model);
                    }

                    if (await _userManager.CheckPasswordAsync(user, model.Password))
                    {
                        await _auditLogger.LogActivityAsync(user.Id, "User logged in successfully.");

                        // Check if a session exists in the cache
                        var existingSession = await _cache.GetStringAsync(user.Id);
                        if (existingSession != null)
                        {
                            await _cache.RemoveAsync(user.Id);
                        }

                        var sessionId = Guid.NewGuid().ToString();
                        await _cache.SetStringAsync(user.Id, sessionId, new DistributedCacheEntryOptions
                        {
                            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
                        });

                        Response.Cookies.Append("SessionId", sessionId, new CookieOptions
                        {
                            Expires = DateTime.Now.AddMinutes(30),
                            HttpOnly = true,
                            Secure = true,
                            SameSite = SameSiteMode.Strict
                        });

                        await _signInManager.SignInAsync(user, isPersistent: model.RememberMe);
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        await _userManager.AccessFailedAsync(user);
                        if (await _userManager.IsLockedOutAsync(user))
                        {
                            ModelState.AddModelError("", "Too many failed login attempts. Your account has been locked for 5 minutes.");
                        }
                        else
                        {
                            ModelState.AddModelError("", "Invalid login attempt.");
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Invalid login attempt.");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error during login: {ex.Message}");
            TempData["ErrorMessage"] = "An error occurred while processing your request. Please try again later.";
            return RedirectToAction("Error", "Home");
        }

        return View(model);
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View(new RegisterViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = new User { UserName = model.Email, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _auditLogger.LogActivityAsync(user.Id, "User registered successfully.");
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
        return View(model);
    }

    public async Task<IActionResult> Logout()
    {
        if (User.Identity?.Name != null)
        {
            await _auditLogger.LogActivityAsync(User.Identity.Name, "User logged out.");
        }

        Response.Cookies.Delete("SessionId");

        if (User.Identity?.Name != null)
        {
            await _cache.RemoveAsync(User.Identity.Name);
        }

        await _signInManager.SignOutAsync();
        await HttpContext.SignOutAsync();

        return RedirectToAction("Login");
    }

    public async Task<IActionResult> CheckSession()
    {
        var sessionId = Request.Cookies["SessionId"];
        if (sessionId == null)
        {
            return RedirectToAction("Login");
        }

        var user = await _userManager.GetUserAsync(User);
        var sessionExists = await _cache.GetStringAsync(user.Id);
        if (sessionExists == null)
        {
            return RedirectToAction("Login");
        }

        return View();
    }
}




















