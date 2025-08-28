using DeviceDetectorNET;
using hoohub.Configuration;
using hoohub.Data;
using hoohub.Requests.Data;
using hoohub.Requests.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace hoohub.Pages
{
    [AllowAnonymous]
    public class ArchiveModel : PageModel
    {
        private readonly HooHubContext _hooContext;
        private readonly SignInManager<HooHubUser> _signInManager;
        private readonly AppSettings _appSettings;

        [DataType(DataType.Text)]
        [MaxLength(200)]
        [Display(Prompt = "enter tags, number or the name of a comic")]
        [RegularExpression("^[a-zA-Z-' ,]+$")]
        public string TagInput { get; set; }

        /// <summary>
        /// Whether or not the app is considered to be in Night Mode.<br/>
        /// Some assets may need replacement based on this.
        /// </summary>
        public bool IsNightMode { get; private set; } = false;

        /// <summary>
        /// Whether the request is coming from a mobile client or not.
        /// </summary>
        public bool IsMobileClient { get; private set; } = true;

        /// <summary>
        /// Initialises a new instance of the <see cref="ArchiveModel"/> page.
        /// </summary>
        /// <param name="hooContext">Injected app context.</param>
        /// <param name="signInManager">Injected <see cref="SignInManager{TUser}"/>.</param>
        /// <param name="appSettings">Injected <see cref="AppSettings"/>.</param>
        public ArchiveModel(HooHubContext hooContext, SignInManager<HooHubUser> signInManager, AppSettings appSettings)
        {
            _hooContext = hooContext;
            _signInManager = signInManager;
            _appSettings = appSettings;
        }

        /// <summary>
        /// Returns the Archive page, or the Offline page depending on current access configuration.
        /// </summary>
        /// <returns>The Archive page, or the Offline page depending on current access configuration.</returns>
        public async Task<IActionResult> OnGet()
        {
            try
            {
                if (_appSettings.BlockedUserAgents.Contains(Request.Headers["User-Agent"].ToString().ToLower())
                    || _appSettings.BlockedIpAddressRange.Contains(Request.HttpContext.Connection.RemoteIpAddress.ToString()))
                {
                    return NotFound("Sorry, we could not process your request: please try again later.");
                }

                var settings = _hooContext.Settings.FirstOrDefault();
                if (settings != null)
                {
                    if (!settings.PublicAccessEnabled && !_signInManager.IsSignedIn(User))
                    {
                        return RedirectToPage("./Offline");
                    }
                    if (settings.ArchiveAccess == Enums.AccessTypes.None
                        || (settings.ArchiveAccess == Enums.AccessTypes.RegisteredUsers && !_signInManager.IsSignedIn(User))
                        || (settings.ArchiveAccess == Enums.AccessTypes.AuthorisedUsers && !_signInManager.IsSignedIn(User)))
                    {
                        return RedirectToPage("./Index");
                    }
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

                // Some extra text for mobile devices, so check client
                var deviceDetector = new DeviceDetector(userAgent: Request.Headers["User-Agent"]);
                deviceDetector.Parse();
                IsMobileClient = deviceDetector.IsMobile();

                return Page();
            }
            catch (Exception exception)
            {
                await _hooContext.Events.AddAsync(new Event(
                eventType: Enums.EventTypes.Error,
                    details: $"Failed to load comic archive: {exception.Message}",
                    stackTrace: JsonConvert.SerializeObject(value: exception.StackTrace, formatting: Formatting.Indented)));
                await _hooContext.SaveChangesAsync();
                return RedirectToPage("./Error");
            }
        }

        /// <summary>
        /// Attempts to fetch a selection of comics from the archive, depending on any query, as well as caps on archive querying according to the app settinbgs.<br/>
        /// Returns a <see cref="JsonResult"/> representing the result of the request.
        /// </summary>
        /// <param name="startAtComic">Optional comic GUID to begin from, for example if the user is scrolling through the full list.</param>
        /// <param name="query">Optionasl query for filtering the resulting comics, for example tags.</param>
        /// <returns><see cref="JsonResult"/> representing the result of the request.</returns>
        public async Task<JsonResult> OnGetComics(
            string startAtComic = "",
            string query = "")
        {
            try
            {
                if (_appSettings.BlockedUserAgents.Contains(Request.Headers["User-Agent"].ToString().ToLower())
                    || _appSettings.BlockedIpAddressRange.Contains(Request.HttpContext.Connection.RemoteIpAddress.ToString()))
                {
                    return new JsonResult(new BaseResult(
                        success: false,
                        message: "Sorry, we could not process your request: please try again later."));
                }

                var allComics = _hooContext.Comics
                    .AsEnumerable()
                    .Where(comic => !comic.IsHidden)
                    .OrderByDescending(comic => comic.ComicNumber)
                    .ToList();

                if (!string.IsNullOrWhiteSpace(query))
                {
                    allComics = allComics
                        .AsEnumerable()
                        .Where(comic => comic.GetComicTitleContainsTerms(query) || comic.GetComicNumberContainsTerms(query) || comic.GetTagsContainsTerms(query))
                        .ToList();
                    if (allComics.Count == 0)
                    {
                        return new JsonResult(new ArchiveResult(
                            success: true,
                            endOfResults: true,
                            archiveComicData: new List<ArchiveComicData>()));
                    }
                }

                var startFromComic = allComics.FirstOrDefault(comic => comic.Id == startAtComic);
                var startIndex = startFromComic != null
                    ? allComics.IndexOf(startFromComic)
                    : -1;

                var settings = await _hooContext.Settings.FirstOrDefaultAsync();
                var takeComics = settings != null ? settings.ArchiveMaximumComicsPerFetch : 20;
                var archiveComics = allComics.Skip(startIndex + 1).Take(takeComics);

                return new JsonResult(new ArchiveResult(
                    success: true,
                    endOfResults: archiveComics.Count() == 0 || archiveComics.Last().Id == allComics.Last().Id,
                    archiveComicData: archiveComics.Count() == 0 
                        ? new List<ArchiveComicData>()
                        : archiveComics.Select(comic => new ArchiveComicData(comic)).ToList()));
            }
            catch (Exception exception)
            {
                await _hooContext.Events.AddAsync(new Event(
                    eventType: Enums.EventTypes.Error,
                    details: $"Failed to load comic archive for starting comic GUID {startAtComic}: {exception.Message}",
                    stackTrace: JsonConvert.SerializeObject(value: exception.StackTrace, formatting: Formatting.Indented)));
                await _hooContext.SaveChangesAsync();
                return new JsonResult(new BaseResult(success: false, message: exception.Message));
            }
        }
    }
}
