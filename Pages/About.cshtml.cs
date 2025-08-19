using hoohub.Configuration;
using hoohub.Data;
using hoohub.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace hoohub.Pages
{
    public class AboutModel : PageModel
    {
        private readonly HooHubContext _hooContext;
        private readonly AppSettings _appSettings;

        /// <summary>
        /// 
        /// </summary>
        public HooHubUser ArtistCreditUser { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public HooHubUser WriterCreditUser { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hooContext"></param>
        public AboutModel(HooHubContext hooContext, AppSettings appSettings)
        {
            _hooContext = hooContext;
            _appSettings = appSettings;
        }

        /// <summary>
        /// 
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
