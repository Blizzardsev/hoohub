using hoohub.Configuration;
using hoohub.Data;
using hoohub.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace hoohub.Pages
{
    [AllowAnonymous]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [IgnoreAntiforgeryToken]
    public class OfflineModel : PageModel
    {
        private readonly HooHubContext _context;
        private readonly SignInManager<HooHubUser> _signInManager;
        private readonly AppSettings _appSettings;

        /// <summary>
        /// Whether or not the app is considered to be in Night Mode.<br/>
        /// Some assets may need replacement based on this.
        /// </summary>
        public bool IsNightMode { get; private set; } = false;

        /// <summary>
        /// Initialises a new instance of the <see cref="OfflineModel"/> class.
        /// </summary>
        /// <param name="context">Injected app context.</param>
        /// <param name="signInManager">Injected <see cref="SignInManager{TUser}"/>.</param>
        /// <param name="appSettings">Injected <see cref="AppSettings"/>.</param>
        public OfflineModel(HooHubContext context, SignInManager<HooHubUser> signInManager, AppSettings appSettings)
        {
            _context = context;
            _signInManager = signInManager;
            _appSettings = appSettings;
        }

        /// <summary>
        /// Returns the offline page.
        /// </summary>
        public async Task<IActionResult> OnGetAsync()
        {
            if (!HttpCheckService.IsValidUserAgentAndIpAddress(_appSettings, Request))
            {
                return NotFound("Sorry, we could not process your request: please try again later.");
            }

            var settings = _context.Settings.FirstOrDefault();
            if (settings.PublicAccessEnabled)
            {
                return RedirectToPage("./Index");
            }

            string? nightModeSetting = Request.Cookies["nightMode"];
            if (string.IsNullOrWhiteSpace(nightModeSetting))
            {
                Response.Cookies.Append("nightMode", "false");
                IsNightMode = false;
            }
            else
            {
                IsNightMode = Request.Cookies["nightMode"] == "true";
            }

            return Page();
        }
    }
}