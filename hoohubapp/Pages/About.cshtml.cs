using hoohub.Configuration;
using hoohub.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace hoohub.Pages
{
    [AllowAnonymous]
    public class AboutModel : PageModel
    {
        private readonly HooHubContext _hooContext;
        private readonly AppSettings _appSettings;

        /// <summary>
        /// The <see cref="HooHubUser"/> corresponding to the artist to credit and link on the page.
        /// </summary>
        public HooHubUser ArtistCreditUser { get; private set; }

        /// <summary>
        /// The <see cref="HooHubUser"/> corresponding to the writer to credit and link on the page.
        /// </summary>
        public HooHubUser WriterCreditUser { get; private set; }

        /// <summary>
        /// Initialises a new instance of the <see cref="AboutModel"/> class.
        /// </summary>
        /// <param name="hooContext">Injected app context.</param>
        /// <param name="appSettings">Injected app settings.</param>
        public AboutModel(HooHubContext hooContext, AppSettings appSettings)
        {
            _hooContext = hooContext;
            _appSettings = appSettings;
        }

        /// <summary>
        /// Returns the about us page.
        /// </summary>
        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                ArtistCreditUser = _hooContext.Users
                    .AsEnumerable()
                    .First(user => string.Equals(user.Email, _appSettings.ArtistCredit, StringComparison.OrdinalIgnoreCase));
                WriterCreditUser = _hooContext.Users
                    .AsEnumerable()
                    .First(user => string.Equals(user.Email, _appSettings.WriterCredit, StringComparison.OrdinalIgnoreCase));

                return Page();
            }
            catch (Exception exception)
            {
                await _hooContext.Events.AddAsync(new Event(
                    eventType: Enums.EventTypes.Error,
                    details: $"Failed to load about page: {exception.Message}",
                    stackTrace: JsonConvert.SerializeObject(value: exception.StackTrace, formatting: Formatting.Indented)));
                await _hooContext.SaveChangesAsync();
                return RedirectToPage("./Error");
            }
        }
    }
}
