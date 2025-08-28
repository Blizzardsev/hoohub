// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using hoohub.Configuration;
using hoohub.Data;
using hoohub.Enums;
using hoohub.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace hoohub.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly HooHubContext _context;
        private readonly SignInManager<HooHubUser> _signInManager;
        private readonly AppSettings _appSettings;

        public LoginModel(
            HooHubContext context,
            SignInManager<HooHubUser> signInManager,
            AppSettings appSettings)
        {
            _context = context;
            _signInManager = signInManager;
            _appSettings = appSettings;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Input validation model.
        /// </summary>
        public class InputModel
        {
            [Required(AllowEmptyStrings = false)]
            [Display(Prompt = "hate@everything.hoo")]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [Display(Prompt = "password")]
            public string Password { get; set; }
        }

        /// <summary>
        /// Returns the Login page.
        /// </summary>
        /// <param name="returnUrl">The page to return the user to after logging in, or attempting to log in.</param>
        /// <returns>The login page.</returns>
        public async Task<IActionResult> OnGetAsync(string returnUrl = null)
        {
            if (!HttpCheckService.IsValidUserAgentAndIpAddress(_appSettings, Request))
            {
                return NotFound("Sorry, we could not process your request: please try again later.");
            }

            if (_signInManager.IsSignedIn(User))
            {
                ReturnUrl = string.Empty;
                return RedirectToPage("../Index");
            }

            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ErrorMessage = ErrorMessage;
            }

            returnUrl ??= Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            ReturnUrl = returnUrl;

            if (HttpContext.Session.GetInt32("locationChange") != null && HttpContext.Session.GetInt32("locationChange") == 1)
            {
                ErrorMessage = "Your location has changed from your last login. Please log in again.";
                HttpContext.Session.SetInt32("locationChange", 0);
            }

            return Page();
        }

        /// <summary>
        /// Attempts to log in the user, and direct them to the home page.<br/>
        /// If two-factor is required, then the user is directed to the challenge page instead.<br/>
        /// Excess login attempts prompt a user lockout - this can be undone by an Admin.
        /// </summary>
        /// <param name="returnUrl">The page to return the user to after logging in, or attempting to log in.</param>
        /// <returns></returns>
        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            try
            {
                var currentIpAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString();
                returnUrl ??= Url.Content("~/");

                if (ModelState.IsValid)
                {
                    var result = await _signInManager.PasswordSignInAsync(
                        userName: Input.Email,
                        password: Input.Password,
                        isPersistent: true,
                        lockoutOnFailure: true);
                    var user = await _signInManager.UserManager.FindByNameAsync(Input.Email);

                    if (user == null)
                    {
                        ErrorMessage = $"Invalid username and/or password";

                        await _context.Events.AddAsync(new Event(
                            eventType: EventTypes.UserLoginAttempt,
                            details: $"A login was attempted for {Input.Email}, but no account with this login exists."));

                        await _context.SaveChangesAsync();
                        return Page();
                    }

                    if (user.IsDisabled)
                    {
                        ErrorMessage = "Invalid username and/or password.";

                        await _context.Events.AddAsync(new Event(
                            eventType: EventTypes.UserLoginAttempt,
                            details: $"Disabled or deleted user {user.GetEventLogString()} attempted login."));

                        await _context.SaveChangesAsync();
                        return Page();
                    }

                    if (result.IsLockedOut)
                    {
                        ErrorMessage = "Too many login attempts. Please try again later.";

                        await _context.Events.AddAsync(new Event(
                            eventType: EventTypes.UserLoginAttempt,
                            details: $"Locked out user {user.GetEventLogString()} attempted login."));

                        await _context.SaveChangesAsync();
                        return Page();
                    }

                    if (!result.Succeeded && user.FirstLogin)
                    {
                        ErrorMessage = "Please set your password.";

                        await _context.Events.AddAsync(new Event(
                            eventType: EventTypes.UserLoginAttempt,
                            details: $"User {user.GetEventLogString()} attempted login, but requires a password to be set."));

                        await _context.SaveChangesAsync();
                        return RedirectToPage("./ForgotPassword", new
                        {
                            user.Email
                        });
                    }

                    if (result.Succeeded)
                    {
                        if (user.LastLoginIpAddress != currentIpAddress && !_appSettings.TrustedLocations.Contains(currentIpAddress) && user.TwoFactorEnabled)
                        {
                            ErrorMessage = "Your location has changed from your last login. Please log in again.";

                            _context.Events.Add(new Event(
                                eventType: EventTypes.UserLoggedIn,
                                details: $"User {user.GetEventLogString()} IP address changed from {user.LastLoginIpAddress} to {currentIpAddress}; requiring fresh sign-in for 2FA."));

                            await _signInManager.ForgetTwoFactorClientAsync(); // This only procs on the next request, so we have to redirect back to same page to force relog with 2FA
                            await _context.SaveChangesAsync();
                            return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl });
                        }

                        user.AccessFailedCount = 0;
                        user.LastLoginDate = DateTime.UtcNow;
                        user.LastLoginIpAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString();

                        await _context.Events.AddAsync(new Event(
                            eventType: EventTypes.UserLoggedIn,
                            details: $"User {user.GetEventLogString()} logged in at {user.LastLoginIpAddress}."));

                        await _context.SaveChangesAsync();
                        return Url.IsLocalUrl(returnUrl) ? LocalRedirect(returnUrl) : RedirectToPage("./");
                    }

                    if (result.RequiresTwoFactor)
                    {
                        if (_appSettings.TrustedLocations.Contains(currentIpAddress))
                        {
                            await _signInManager.SignInAsync(user: user, isPersistent: true);

                            _context.Events.Add(new Event(
                                eventType: EventTypes.UserLockedOut,
                                details: $"User {user.GetEventLogString()} logged in at safe address {user.LastLoginIpAddress}; 2FA bypassed."));

                            user.AccessFailedCount = 0;
                            user.LastLoginDate = DateTime.UtcNow;
                            user.LastLoginIpAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString();

                            await _context.SaveChangesAsync();
                            return Url.IsLocalUrl(returnUrl) ? LocalRedirect(returnUrl) : RedirectToPage("./");
                        }

                        await _context.Events.AddAsync(new Event(
                            eventType: EventTypes.TwoFactorChallengeIssued,
                            details: $"User {user.GetEventLogString()} challenged for two-factor."));

                        await _context.SaveChangesAsync();
                        return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl });
                    }

                    if (user.AccessFailedCount >= 3)
                    {
                        ErrorMessage = "Too many login attempts. Please try again later.";
                        if (user.LockoutEnd == null || user.LockoutEnd < DateTime.UtcNow)
                        {
                            user.LockoutEnd = DateTime.UtcNow.AddMinutes(10);
                            await _context.Events.AddAsync(new Event(
                                eventType: EventTypes.UserLockedOut,
                                details: $"User {user.GetEventLogString()} locked out; login attempts exceeded."));
                        }
                        else
                        {
                            await _context.Events.AddAsync(new Event(
                                eventType: EventTypes.UserLockedOut,
                                details: $"User {user.GetEventLogString()} attempted login, but is still locked out."));
                        }

                        await _context.SaveChangesAsync();
                        return Page();
                    }
                    else
                    {
                        ErrorMessage = "Invalid username and/or password.";
                        await _context.Events.AddAsync(new Event(
                            eventType: EventTypes.UserLoginAttempt,
                            details: $"User {user.GetEventLogString()} attempted login."));

                        await _context.SaveChangesAsync();
                        return Page();
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorMessage = "Failed to process login request. Please try again.";
                await _context.Events.AddAsync(new Event(
                    eventType: EventTypes.Error,
                    details: $"Failed to process login: {exception.Message} | Stacktrace: {exception.StackTrace}"));

                await _context.SaveChangesAsync();
            }

            return Page();
        }
    }
}