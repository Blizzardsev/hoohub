using hoohub.Data;
using hoohub.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace hoohub.Areas.Identity.Pages.Account
{
    public class ResetPasswordModel : PageModel
    {
        private readonly HooHubContext _context;
        private readonly UserManager<HooHubUser> _userManager;
        private readonly SignInManager<HooHubUser> _signInManager;

        public ResetPasswordModel(HooHubContext context, UserManager<HooHubUser> userManager, SignInManager<HooHubUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required(AllowEmptyStrings = false)]
            [Display(Prompt = "hate@everything.hoo")]
            public string Login { get; set; }

            [Required(ErrorMessage = "Passwords must be provided, match and meet strength requirements.", AllowEmptyStrings = false)]
            [DataType(DataType.Password)]
            [MinLength(10)]
            // Must contain at least one capital letter, one lowercase character, and one special character
            [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*[@#$%^&+=!.]).*$", ErrorMessage = "Password does not meet strength requirement.")]
            [Display(Prompt = "password")]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Prompt = "confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            public string Code { get; set; }
        }

        public async Task<IActionResult> OnGet(string code = null)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return RedirectToPage("/Error");
            }
            else
            {
                Input = new InputModel
                {
                    Code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code))
                };
                return Page();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Page();
                }

                var user = await _userManager.FindByNameAsync(Input.Login);
                if (user == null)
                {
                    // Don't reveal that the user does not exist
                    await _context.Events.AddAsync(new Event(
                        eventType: EventTypes.UserPasswordResetRequested,
                        details: $"A password reset was requested for {Input.Login}, but no account with this login exists."));
                    await _context.SaveChangesAsync();

                    ModelState.AddModelError("error", $"Sorry, we couldn't find an account for {Input.Login}.");
                    return Page();
                }

                var passwordResetResult = await _userManager.ResetPasswordAsync(user, Input.Code, Input.Password);
                if (passwordResetResult.Succeeded)
                {
                    await _context.Events.AddAsync(new Event(
                        eventType: EventTypes.UserPasswordReset,
                        details: $"User {user.GetEventLogString()} password reset successfully."));
                    user.FirstLogin = false;
                    user.LastLoginIpAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString();

                    await _context.SaveChangesAsync();
                    await _signInManager.SignInAsync(user, isPersistent: true);

                    return RedirectToPage("./ResetPasswordConfirmation");
                }

                foreach (var error in passwordResetResult.Errors)
                {
                    ModelState.AddModelError("error", error.Description);
                }
            }
            catch (Exception exception)
            {
                ModelState.AddModelError("error", "Failed to process reset request: please try again.");
                await _context.Events.AddAsync(new Event(
                    eventType: EventTypes.Error,
                    details: $"Failed to process password reset request: {exception.Message} | Stacktrace: {exception.StackTrace}"));

                await _context.SaveChangesAsync();
            }

            return Page();
        }
    }
}
