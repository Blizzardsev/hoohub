// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using hoohub.Configuration;
using hoohub.Data;
using hoohub.Enums;
using hoohub.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace hoohub.Areas.Identity.Pages.Account
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly HooHubContext _context;
        private readonly UserManager<HooHubUser> _userManager;
        private readonly SmtpService _smtpService;
        private readonly AppSettings _appSettings;


        /// <summary>
        /// Initialises a new instance of the <see cref="ForgotPasswordModel"/>.
        /// </summary>
        /// <param name="context">Injected app context.</param>
        /// <param name="userManager">Injected <see cref="UserManager{TUser}"/> instance.</param>
        /// <param name="smtpService">Injected <see cref="SmtpService"/> instance.</param>
        /// <param name="appSettings">Injected <see cref="AppSettings"/>.</param>
        public ForgotPasswordModel(
            HooHubContext context, 
            UserManager<HooHubUser> userManager, 
            SmtpService smtpService,
            AppSettings appSettings)
        {
            _context = context;
            _userManager = userManager;
            _smtpService = smtpService;
            _appSettings = appSettings;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public bool IsFirstTimeLogin { get; set; }

        /// <summary>
        /// Input validation models.
        /// </summary>
        public class InputModel
        {
            [Required(ErrorMessage = "Email address must be provided", AllowEmptyStrings = false)]
            [Display(Prompt = "hate@everything.hoo")]
            public string Login { get; set; }
        }

        /// <summary>
        /// Returns the forgotten password reset request page.
        /// </summary>
        /// <param name="email">The email address to prepopulate the <see cref="InputModel.Login"/> for.</param>
        /// <returns>The forgotten password reset request page.</returns>
        public async Task<IActionResult> OnGetAsync(string? email = null)
        {
            if (_appSettings.BlockedUserAgents.Contains(Request.Headers["User-Agent"].ToString().ToLower())
                || _appSettings.BlockedIpAddressRange.Contains(Request.HttpContext.Connection.RemoteIpAddress.ToString()))
            {
                return NotFound("Sorry, we could not process your request: please try again later.");
            }

            Input = new InputModel
            {
                Login = string.IsNullOrWhiteSpace(email) ? string.Empty : email
            };
            if (!string.IsNullOrEmpty(email))
            {
                var user = await _userManager.FindByNameAsync(Input.Login);
                IsFirstTimeLogin = user == null ? false : user.FirstLogin;
            }

            return Page();
        }

        /// <summary>
        /// Attempts to initiate the password reset process for the given <see cref="InputModel.Login"/>, if a corresponding <see cref="HooHubUser"/> exists.
        /// </summary>
        /// <returns>The password reset confirmation page if the process was initiated successfully, or returns this page if the account does not exist.</returns>
        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(Input.Login);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    await _context.Events.AddAsync(new Event(
                        eventType: EventTypes.UserPasswordResetRequested,
                        details: $"A password reset was requested for {Input.Login}, but no account with this login exists."));
                    await _context.SaveChangesAsync();

                    ModelState.AddModelError("error", $"Sorry, we couldn't find an account for {Input.Login}.");
                    return Page();
                }

                // Generate the password reset URL
                var code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(await _userManager.GeneratePasswordResetTokenAsync(user)));
                var callbackUrl = Url.Page(
                    "/Account/ResetPassword",
                    pageHandler: null,
                    values: new { code },
                    protocol: Request.Scheme);

                // Send email, redirect
                try
                {
                    _smtpService.SendEmail(
                        name: user.Handle,
                        address: user.Email,
                        subject: "Reset your password",
                        body: TemplateService.GetTemplateSubstitutions(
                            template: Properties.Resources.UserPasswordResetTemplate,
                            substitutions: new Dictionary<string, string>()
                            {
                                { "{toName}", user.Handle},
                                { "{buttonUrl}", callbackUrl }
                            }));

                    _context.Events.Add(new Event(
                        eventType: EventTypes.UserPasswordResetRequested,
                        details: $"Password reset email was sent to user {user.GetEventLogString()}."));

                    await _context.SaveChangesAsync();
                    return RedirectToPage("./ForgotPasswordConfirmation");
                }
                catch (Exception exception)
                {
                    _context.Events.Add(new Event(
                        eventType: EventTypes.UserPasswordResetRequested,
                        details: $"Failed to send password reset email to user {user.GetEventLogString()}. Error: {exception.Message} | Stacktrace: {exception.StackTrace}"));

                    await _context.SaveChangesAsync();
                    return RedirectToPage("/Error", new { area = "" });
                }
            }

            return Page();
        }
    }
}
