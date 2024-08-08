// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

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

        public ForgotPasswordModel(HooHubContext context, UserManager<HooHubUser> userManager, SmtpService smtpService)
        {
            _context = context;
            _userManager = userManager;
            _smtpService = smtpService;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public bool IsFirstTimeLogin { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Email address must be provided", AllowEmptyStrings = false)]
            [Display(Prompt = "hate@everything.hoo")]
            public string Login { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(string? email = null)
        {
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
                    values: new { area = "Identity", code },
                    protocol: Request.Scheme);

                // Send email, redirect
                try
                {
                    _smtpService.SendEmail(
                        name: user.Handle,
                        address: user.Email,
                        subject: "Reset your password",
                        body: TemplateService.GetTemplateSubstitutions(
                            template: "", // TODO: Email template
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
