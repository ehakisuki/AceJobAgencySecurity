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

    public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, ILogger<AccountController> logger, IDistributedCache cache, IAuditLogger auditLogger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
        _cache = cache;
        _auditLogger = auditLogger; // Initialize the audit logger
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
                if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
                {
                    // Log successful login activity
                    await _auditLogger.LogActivityAsync(user.Id, "User logged in successfully.");

                    // Check if a session exists in the cache
                    var existingSession = await _cache.GetStringAsync(user.Id);
                    if (existingSession != null)
                    {
                        // If a session exists, invalidate the previous session (from another tab/device)
                        await _cache.RemoveAsync(user.Id);
                    }

                    // Generate a unique session ID
                    var sessionId = Guid.NewGuid().ToString();

                    // Store the session ID in the cache (Redis or In-Memory)
                    await _cache.SetStringAsync(user.Id, sessionId, new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30) // Set session timeout (30 minutes)
                    });

                    // Set the session ID in the cookie
                    var cookieOptions = new CookieOptions
                    {
                        Expires = DateTime.Now.AddMinutes(30),
                        HttpOnly = true, // Prevent client-side access to the cookie
                        Secure = true, // Ensure cookie is transmitted over HTTPS
                        SameSite = SameSiteMode.Strict // Restrict cookie sending to same-site requests
                    };
                    Response.Cookies.Append("SessionId", sessionId, cookieOptions);

                    // Sign in the user
                    await _signInManager.SignInAsync(user, isPersistent: model.RememberMe);
                    return RedirectToAction("Index", "Home");  // Redirect to homepage after successful login
                }
                ModelState.AddModelError("", "Invalid login attempt.");
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

    // GET: Display the registration form
    [HttpGet]
    public IActionResult Register()
    {
        var model = new RegisterViewModel();
        return View(model);
    }

    // POST: Handle form submission for registration
    [HttpPost]
    [ValidateAntiForgeryToken]  // Add CSRF protection
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = new User
            {
                UserName = model.Email,
                Email = model.Email
            };

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

    // Implement the logout action
    public async Task<IActionResult> Logout()
    {
        // Log the logout activity
        if (User.Identity?.Name != null)
        {
            await _auditLogger.LogActivityAsync(User.Identity.Name, "User logged out.");
        }

        // Clear the session cookie (this will invalidate the session)
        Response.Cookies.Delete("SessionId");

        // Remove the session data from cache
        if (User.Identity?.Name != null)
        {
            await _cache.RemoveAsync(User.Identity.Name);  // Safely remove session from cache
        }

        // Sign out the user from the Identity system
        await _signInManager.SignOutAsync();

        // Clear the local session
        await HttpContext.SignOutAsync();

        // Redirect to the login page
        return RedirectToAction("Login");
    }

    // Action to check if session is expired and redirect to login page
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

        return View(); // You can return a session valid view or dashboard page here
    }
}



















