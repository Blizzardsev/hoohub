// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using hoohub.Data;
using hoohub.Enums;
using hoohub.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using RequiredAttribute = System.ComponentModel.DataAnnotations.RequiredAttribute;

namespace hoohub.Areas.Identity.Pages.Account
{
    public class LoginWith2faModel : PageModel
    {
        public readonly HooHubContext _context;
        private readonly SignInManager<HooHubUser> _signInManager;
        private readonly UserManager<HooHubUser> _userManager;
        private readonly SmtpService _smtpService;

        /// <summary>
        /// Initialises the <see cref="LoginWith2faModel"/> class.
        /// </summary>
        /// <param name="context">Injected app context.</param>
        /// <param name="signInManager">Injected <see cref="SignInManager{TUser}"/> service.</param>
        /// <param name="userManager">Injected <see cref="UserManager{TUser}"/> service.</param>
        /// <param name="smtpService">Injected <see cref="SmtpService"/> service.</param>
        public LoginWith2faModel(
            HooHubContext context,
            SignInManager<HooHubUser> signInManager,
            UserManager<HooHubUser> userManager,
            SmtpService smtpService)
        {
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;
            _smtpService = smtpService;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        /// <summary>
        /// Input validation model.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            /// The 2FA code the user must supply.
            /// </summary>
            [Required(ErrorMessage = "Two-factor code must be provided", AllowEmptyStrings = false)]
            [DataType(DataType.Text)]
            [MaxLength(6)]
            [Display(Name = "Check your email for your two-factor code.", Prompt = "123456")]
            public string TwoFactorCode { get; set; }
        }

        /// <summary>
        /// Returns the two-factor request page.
        /// </summary>
        /// <param name="returnUrl">URL for redirection, from the page the user was originally attempting to access if they were logged out.</param>
        /// <returns>The two-factor request page.</returns>
        public async Task<IActionResult> OnGetAsync(string returnUrl = null)
        {
            HooHubUser user = null;
            try
            {
                // Ensure the user has gone through the username & password screen first
                user = await _signInManager.GetTwoFactorAuthenticationUserAsync();

                if (user == null)
                {
                    return RedirectToPage("../Index");
                }

                ReturnUrl = returnUrl;
                _smtpService.SendEmail(
                    name: user.Handle,
                    address: user.Email,
                    subject: "Your Hoo-factor code",
                    body: TemplateService.GetTemplateSubstitutions(
                        template: Properties.Resources.TwoFactorCodeTemplate,
                        substitutions: new Dictionary<string, string>()
                        {
                            { "{toName}", user.Handle },
                            { "{twoFactorCode}", await _userManager.GenerateTwoFactorTokenAsync(user, TokenOptions.DefaultEmailProvider) }
                        }));

                _context.Events.Add(new Event(
                    eventType: EventTypes.TwoFactorCodeIssued,
                    details: $"Two-factor code email was sent to user {user.GetEventLogString()}."));

                await _context.SaveChangesAsync();
                return Page();
            }
            catch (Exception exception)
            {
                _context.Events.Add(new Event(
                    eventType: EventTypes.Error,
                    details: $"Failed to send two-factor code email to {(user == null ? "unknown user" : user.GetEventLogString())}. Error: {exception.Message}, trace: {exception.StackTrace}"));

                await _context.SaveChangesAsync();
                return RedirectToPage("/Error", new { area = "" });
            }
        }

        /// <summary>
        /// Attempts to validate the given two-factor code.<br/>
        /// If correct, proceeds and redirects the user onwards to the originally requested page, or the home page otherwise.
        /// If incorrect, returns to the previous page to allow a retry.
        /// </summary>
        /// <param name="returnUrl">URL for redirection, from the page the user was originally attempting to access if they were logged out.</param>
        /// <returns>The originally requested page or home page if successful; otherwise the two-factor request page.</returns>
        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            try
            {
                returnUrl ??= Url.Content("~/");

                if (!ModelState.IsValid)
                {
                    return Page();
                }

                var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
                if (user == null)
                {
                    return RedirectToPage("../Index");
                }

                var result = await _signInManager.TwoFactorSignInAsync(
                    provider: TokenOptions.DefaultEmailProvider,
                    code: Input.TwoFactorCode,
                    isPersistent: true,
                    rememberClient: true);

                if (result.Succeeded)
                {
                    user.AccessFailedCount = 0;
                    user.LastLoginDate = DateTime.UtcNow;
                    user.LastLoginIpAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString();

                    _context.Events.Add(new Event(
                        eventType: EventTypes.UserLoggedIn,
                        details: $"User {user.GetEventLogString()} logged in."));

                    await _context.SaveChangesAsync();
                    return Url.IsLocalUrl(returnUrl) ? LocalRedirect(returnUrl) : RedirectToPage("./");
                }
                else if (result.IsLockedOut)
                {
                    _context.Events.Add(new Event(
                        eventType: EventTypes.UserLockedOut,
                        details: $"User {user.GetEventLogString()} locked out; 2FA attempts exceeded."));

                    await _context.SaveChangesAsync();
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    ModelState.AddModelError("error", "Invalid authenticator code.");
                    return Page();
                }
            }
            catch (Exception exception)
            {
                ModelState.AddModelError("error", "Failed to process 2FA request. Please try again.");
                await _context.Events.AddAsync(new Event(
                    eventType: EventTypes.Error,
                    details: $"Failed to process 2FA request: {exception.Message} | Stacktrace: {exception.StackTrace}"));

                await _context.SaveChangesAsync();
            }

            return Page();
        }
    }
}