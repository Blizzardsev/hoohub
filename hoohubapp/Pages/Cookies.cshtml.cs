using hoohub.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace hoohub.Pages
{
    [AllowAnonymous]
    public class CookiesModel : PageModel
    {
        private readonly HooHubContext _hooContext;
        private readonly SignInManager<HooHubUser> _signInManager;

        /// <summary>
        /// Initialises a new instance of the <see cref="CookiesModel"/> class.
        /// </summary>
        /// <param name="hooContext">Injected app context.</param>
        /// <param name="signInManager">Injected <see cref="SignInManager{TUser}"/>.</param>
        public CookiesModel(
            HooHubContext hooContext,
            SignInManager<HooHubUser> signInManager)
        {
            _hooContext = hooContext;
            _signInManager = signInManager;
        }

        /// <summary>
        /// Returns the about us page.
        /// </summary>
        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                var settings = await _hooContext.Settings.FirstOrDefaultAsync();
                if (settings != null)
                {
                    if (!settings.PublicAccessEnabled && !_signInManager.IsSignedIn(User))
                    {
                        return RedirectToPage("./Offline");
                    }
                }
                return Page();
            }
            catch (Exception exception)
            {
                await _hooContext.Events.AddAsync(new Event(
                    eventType: Enums.EventTypes.Error,
                    details: $"Failed to load cookies page: {exception.Message}",
                    stackTrace: JsonConvert.SerializeObject(value: exception.StackTrace, formatting: Formatting.Indented)));
                await _hooContext.SaveChangesAsync();
                return RedirectToPage("./Error");
            }
        }
    }
}
