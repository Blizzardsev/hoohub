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
        /// <summary>
        /// Whether or not the app is considered to be in Night Mode.<br/>
        /// Some assets may need replacement based on this.
        /// </summary>
        public bool IsNightMode { get; private set; } = false;

        /// <summary>
        /// Returns the rate limited page.
        /// </summary>
        public async Task<IActionResult> OnGetAsync()
        {
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