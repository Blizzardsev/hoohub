using hoohub.Configuration;
using hoohub.Data;
using hoohub.Enums;
using hoohub.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Drawing;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSession(options => {
    options.IdleTimeout = TimeSpan.FromMinutes(30);
});

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddWebOptimizer(pipeline =>
{
    pipeline.MinifyJsFiles("js/*");
    pipeline.MinifyCssFiles("css/*");
    //pipeline.MinifyHtmlFiles("html/*");
});

// Identity scaffolding
builder.Services.AddDbContext<HooHubContext>();
builder.Services.AddDefaultIdentity<HooHubUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
    options.User.RequireUniqueEmail = true;
}).AddRoles<IdentityRole>().AddEntityFrameworkStores<HooHubContext>();
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.ExpireTimeSpan = TimeSpan.FromHours(7);
    options.SlidingExpiration = true;
});

builder.Services.AddAntiforgery(o => o.HeaderName = "XSRF-TOKEN");

var appSettings = new AppSettings();
builder.Configuration.GetSection("AppSettings").Bind(appSettings);
builder.Services.Add(new ServiceDescriptor(typeof(AppSettings), appSettings));

var accountSettings = new List<AccountSettings>();
builder.Configuration.GetSection("AccountSettings").Bind(accountSettings);

var smtpSettings = new SmtpSettings();
builder.Configuration.GetSection("SmtpSettings").Bind(smtpSettings);
builder.Services.Add(new ServiceDescriptor(typeof(SmtpService), new SmtpService(smtpSettings)));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseWebOptimizer();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.UseEndpoints(endpoints =>
{
    // Prevent users accessing pages we don't use as part of Identity, that are bundled in the framework
    var disabledRoutes = new[]
    {
        "Identity/Account/AccessDenied",
        "Identity/Account/Lockout",
        "Identity/Account/StatusMessage",
        "Identity/Account/ConfirmEmailChange",
        "Identity/Account/Manage/Layout",
        "Identity/Account/Manage/ChangePassword",
        "Identity/Account/Manage/DownloadPersonalData",
        "Identity/Account/Manage/ExternalLogins",
        "Identity/Account/Manage/PersonalData",
        "Identity/Account/Manage/ShowRecoveryCodes",
        "Identity/Account/RegisterConfirmation",
        "Identity/Account/ExternalLogin",
        "Identity/Account/LoginWithRecoveryCode",
        "Identity/Account/Manage/ManageNav",
        "Identity/Account/Manage/DeletePersonalData",
        "Identity/Account/Manage/Email",
        "Identity/Account/Manage/Login",
        "Identity/Account/Manage/GenerateRecoveryCodes",
        "Identity/Account/Manage/ResetAuthenticator",
        "Identity/Account/Manage/TwoFactorAuthentication",
        "Identity/Account/ResendEmailConfirmation",
        "Identity/Account/ConfirmEmail",
        "Identity/Account/Manage",
        "Identity/Account/Manage/StatusMessage",
        "Identity/Account/Manage/Disable2fa",
        "Identity/Account/Manage/EnableAuthenticator",
        "Identity/Account/Manage/Index",
        "Identity/Account/Manage/SetPassword",
        "Identity/Account/Register",
        "Identity/Account/ForgotPassword",
        "Identity/Account/ForgotPasswordConfirmation",
        "Identity/Account/Login",
        "Identity/Account/LoginWith2fa",
        "Identity/Account/Logout",
        "Identity/Account/ResetPassword",
        "Identity/Account/ResetPasswordConfirmation",
        "Identity/Account/Lockout",
    };
    foreach (var route in disabledRoutes)
    {
        endpoints.MapGet(route, async (context) =>
        {
            context.Response.StatusCode = 404;
        });
    }
});

using (var _scope = app.Services.CreateScope())
{
	var _hooContext = _scope.ServiceProvider.GetRequiredService<HooHubContext>();
	_hooContext.Database.Migrate();

    if (!_hooContext.Comics.Any() && Debugger.IsAttached)
    {
        // Default comic
        _hooContext.Comics.Add(new Comic(
            comicTitle: "Directions",
            comicNumber: "000",
            comicDescription: "",
            imageData: (byte[])new ImageConverter().ConvertTo(hoohub.Properties.Resources.default_comic, typeof(byte[])),
            tags: new List<string>(),
            isHidden: false));
        _hooContext.SaveChanges();
    }

    using var userStore = _scope.ServiceProvider.GetService<IUserStore<HooHubUser>>();
    using var emailStore = (IUserEmailStore<HooHubUser>)userStore;
    using var userManager = _scope.ServiceProvider.GetService<UserManager<HooHubUser>>();

    if (!_hooContext.Users.Any())
    {
        foreach (var settings in accountSettings)
        {
            if (!_hooContext.Users.AsEnumerable().Any(user => string.Equals(user.Email, settings.Email, StringComparison.OrdinalIgnoreCase)))
            {
                var user = Activator.CreateInstance<HooHubUser>();
                user.Id = Guid.NewGuid().ToString();
                user.Handle = settings.Handle;
                user.TwoFactorEnabled = false;
                user.IsDisabled = false;
                user.TwoFactorEnabled = true;

                userStore.SetUserNameAsync(
                    user: user,
                    userName: settings.Email,
                    cancellationToken: CancellationToken.None).Wait();

                emailStore.SetEmailAsync(
                    user: user,
                    email: settings.Email,
                    cancellationToken: CancellationToken.None).Wait();

                var result = userManager.CreateAsync(
                    user: user,
                    password: $"!!HooHoo{Guid.NewGuid()}").Result;

                if (result.Succeeded)
                {
                    userManager.ConfirmEmailAsync(user, userManager.GenerateEmailConfirmationTokenAsync(user).Result).Wait();

                    _hooContext.Events.Add(new Event(
                        eventType: EventTypes.UserCreated,
                        details: $"User {settings.Handle} created."));
                }
                else
                {
                    var errorString = string.Empty;
                    foreach (var error in result.Errors)
                    {
                        errorString += $"{error.Description}{Environment.NewLine}";
                    }
                    _hooContext.Events.Add(new Event(
                        eventType: EventTypes.Error,
                        details: $"Failed to create root user: {errorString}"));
                }
            }
        }
    }

    _hooContext.SaveChanges();
}

app.Run();
