using hoohub.Configuration;
using hoohub.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.RateLimiting;

namespace hoohub.Pages
{
    [AllowAnonymous]
    [DisableRateLimiting]
    public class RateLimitedModel : PageModel
    {
        private readonly AppSettings _appSettings;

        /// <summary>
        /// Whether or not the app is considered to be in Night Mode.<br/>
        /// Some assets may need replacement based on this.
        /// </summary>
        public bool IsNightMode { get; private set; } = false;

        /// <summary>
        /// Initialises a new instance of the <see cref="OfflineModel"/> class.
        /// </summary>
        /// <param name="appSettings">Injected <see cref="AppSettings"/>.</param>
        public RateLimitedModel(AppSettings appSettings)
        {
            _appSettings = appSettings;
        }

        /// <summary>
        /// Returns the rate limited page.
        /// </summary>
        public async Task<IActionResult> OnGetAsync()
        {
            if (!HttpCheckService.IsValidUserAgentAndIpAddress(_appSettings, Request))
            {
                return NotFound("Sorry, we could not process your request: please try again later.");
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