using reCAPTCHA.AspNetCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AceJobAgencySecurity.Data;
using AceJobAgencySecurity.Models;
using AceJobAgencySecurity.Services;

var builder = WebApplication.CreateBuilder(args);

//testing
// Register IHttpClientFactory
builder.Services.AddHttpClient();

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure Identity to use ApplicationDbContext
builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Register the AuditLogger service
builder.Services.AddScoped<IAuditLogger, AuditLogger>();

// Register RateLimitingService
builder.Services.AddScoped<IRateLimitingService, RateLimitingService>();

// Configure password policy
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 12;
    options.Password.RequireNonAlphanumeric = true;
});

// Enable session management
builder.Services.AddDistributedMemoryCache(); // For caching
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set the session timeout duration
    options.Cookie.HttpOnly = true; // Ensures the cookie is only accessible by the server
    options.Cookie.IsEssential = true; // Makes session cookie essential for the app to function
});

// Add reCAPTCHA
builder.Services.AddRecaptcha(options =>
{
    options.SiteKey = builder.Configuration["ReCaptcha:SiteKey"];
    options.SecretKey = builder.Configuration["ReCaptcha:SecretKey"];
});

// Add controllers with views and enable data annotations localization
builder.Services.AddControllersWithViews()
    .AddDataAnnotationsLocalization();  // Enable localization for validation messages

builder.Services.AddLogging(options =>
{
    options.AddConsole();
    options.AddDebug();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
    // Custom error handling for 404 and 403
    app.UseStatusCodePagesWithRedirects("/Error/{0}");
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Use authentication and authorization
app.UseAuthentication();
app.UseAuthorization();

// Enable session middleware
app.UseSession();

// Routing setup
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Register}/{id?}"); // Default controller and action

app.Run();
