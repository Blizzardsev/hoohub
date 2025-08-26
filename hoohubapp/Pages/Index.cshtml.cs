using DeviceDetectorNET;
using hoohub.Data;
using hoohub.Enums;
using hoohub.Requests.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace hoohub.Pages
{
    [AllowAnonymous]
    public class IndexModel : PageModel
    {
        private readonly HooHubContext _hooContext;
        private readonly SignInManager<HooHubUser> _signInManager;
        private readonly UserManager<HooHubUser> _userManager;

        /// <summary>
        /// The comic to render on the page.<br/>
        /// By default this is today's comic, but depending on request could be a specific comic or a comic based on an index.
        /// </summary>
        public Comic? Comic { get; private set; }

        /// <summary>
        /// Whether the comic has been previously liked from the current IP address.
        /// </summary>
        public bool ComicIsLiked { get; private set; }

        /// <summary>
        /// The ID of the next comic chronologically relative to the one currently being displayed.
        /// </summary>
        public string? NextComicId { get; private set; }

        /// <summary>
        /// The ID of the previous comic chronologically relative to the one currently being displayed.
        /// </summary>
        public string? PreviousComicId { get; private set; }

        /// <summary>
        /// Whether or not the app is considered to be in Night Mode.<br/>
        /// Some assets may need replacement based on this.
        /// </summary>
        public bool IsNightMode { get; private set; } = false;

        /// <summary>
        /// Whether to display the comic in the full view mode on loading the page, for instance if the user is on mobile and swiping left/right.
        /// </summary>
        public bool FullScreenDisplay { get; private set; } = false;

        /// <summary>
        /// Whether the request is coming from a mobile client or not.
        /// </summary>
        public bool IsMobileClient { get; private set; } = true;

        /// <summary>
        /// Initialises a new instance of the <see cref="IndexModel"/> class.
        /// </summary>
        /// <param name="hooContext">Injected app context.</param>
        /// <param name="signInManager">Injected <see cref="SignInManager{TUser}"/>.</param>
        /// <param name="userManager">Injected <see cref="UserManager{TUser}"/>.</param>
        public IndexModel(
            HooHubContext hooContext,
            SignInManager<HooHubUser> signInManager,
            UserManager<HooHubUser> userManager)
        {
            _hooContext = hooContext;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        /// <summary>
        /// Returns the main page.</br>
        /// If a comic ID is specified in the request (E.G if going forward/backward), load the associated comic if possible for rendering.<br/>
        /// Otherwise, default to today's comic.
        /// </summary>
        /// <param name="comic">Optional comic GUID to render.</param>
        /// <returns>The main page.</returns>
        public async Task<IActionResult> OnGetAsync(string comic = "", bool fullScreen = false)
        {
            try
            {
                // Redirect if necessary
                var redirectString = await GetOfflineRedirectAsync();
                if (!string.IsNullOrWhiteSpace(redirectString))
                {
                    return RedirectToPage(redirectString);
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

                // Determine the comic to render - if we have a GUID, try to retrieve it. Otherwise, just load the most recent, I guess
                Comic? comicToDisplay = null;
                if (!string.IsNullOrEmpty(comic))
                {
                    comicToDisplay = await _hooContext.Comics
                        .Include(comicItem => comicItem.UploadedBy)
                        .Include(comicItem => comicItem.ComicLikes)
                        .AsSplitQuery()
                        .SingleOrDefaultAsync(comicItem => comicItem.Id == comic && !comicItem.IsHidden);
                }

                if (comicToDisplay == null)
                {
                    comicToDisplay = await _hooContext.Comics
                        .Include(comic => comic.UploadedBy)
                        .Include(comic => comic.ComicLikes)
                        .AsSplitQuery()
                        .Where(comic => !comic.IsHidden)
                        .OrderByDescending(comic => comic.ComicNumber).FirstOrDefaultAsync();
                }

                var nextPreviousComicIds = GetNextPreviousComicIds(comicToDisplay, _hooContext.Comics
                    .OrderByDescending(comic => comic.ComicNumber)
                    .Where(comic => !comic.IsHidden).ToList());

                NextComicId = nextPreviousComicIds.Item1;
                PreviousComicId = nextPreviousComicIds.Item2;
                Comic = comicToDisplay;
                FullScreenDisplay = fullScreen;

                var currentUser = await _userManager.GetUserAsync(User);
                string? userGuid = currentUser != null
                    ? currentUser.Id
                    : Request.Cookies["uniqueId"];
                ComicIsLiked = !string.IsNullOrWhiteSpace(userGuid) && Comic.ComicLikes.Any(comicItem => comicItem.UserGuid == userGuid);

                // Some extra text for mobile devices, so check client
                var deviceDetector = new DeviceDetector(userAgent: Request.Headers["User-Agent"]);
                deviceDetector.Parse();
                IsMobileClient = deviceDetector.IsMobile();

                return Page();
            }
            catch (Exception exception)
            {
                await _hooContext.Events.AddAsync(new Event(
                    eventType: EventTypes.Error,
                    details: $"Failed to load comic GUID {comic}: {exception.Message}",
                    stackTrace: JsonConvert.SerializeObject(value: exception.StackTrace, formatting: Formatting.Indented)));
                await _hooContext.SaveChangesAsync();
                return RedirectToPage("./Error");
            }
        }

        /// <summary>
        /// Returns the main page, selecting a random comic to render in the process.
        /// </summary>
        /// <returns>The main page.</returns>
        public async Task<IActionResult> OnGetRandomComicAsync(string currentComic = "")
        {
			try
			{
                var redirectString = await GetOfflineRedirectAsync();
                if (!string.IsNullOrWhiteSpace(redirectString))
                {
                    return RedirectToPage(redirectString);
                }

                var otherComics = await _hooContext.Comics
                    .Include(comic => comic.UploadedBy)
                    .Include(comic => comic.ComicLikes)
                    .AsSplitQuery()
                    .Where(comic => !comic.IsHidden && comic.Id != currentComic)
                    .OrderByDescending(comic => comic.ComicNumber)
                    .ToListAsync();

                if (otherComics.Count == 0)
                {
                    // We have no unique comics, so we either have none at all, or just one at the moment
                    var allComics = await _hooContext.Comics
                        .Include(comic => comic.UploadedBy)
                        .Include(comic => comic.ComicLikes)
                        .AsSplitQuery()
                        .Where(comic => !comic.IsHidden)
                        .OrderByDescending(comic => comic.ComicNumber)
                        .ToListAsync();

                    if (allComics.FirstOrDefault() == null)
                    {
                        // We have no cats Kathleen!
                        return RedirectToPage("./Error");
                    }

                    NextComicId = string.Empty;
                    PreviousComicId = string.Empty;
                    Comic = allComics.First();

                    // We only have one comic, so return that
                    return Page();
                }

                var randomComic = otherComics.ElementAt(new Random().Next(0, otherComics.Count));
                var nextPreviousComicIds = GetNextPreviousComicIds(randomComic, otherComics);
                NextComicId = nextPreviousComicIds.Item1;
                PreviousComicId = nextPreviousComicIds.Item2;
                Comic = randomComic;
                ComicIsLiked = Comic.ComicLikes.Any(comicItem => comicItem.IpAddress == Request.HttpContext.Connection.RemoteIpAddress.ToString());

                return Page();
            }
			catch (Exception exception)
			{
                await _hooContext.Events.AddAsync(new Event(
                eventType: EventTypes.Error,
                    details: $"Failed to load random comic GUID: {exception.Message}",
                    stackTrace: JsonConvert.SerializeObject(value: exception.StackTrace, formatting: Formatting.Indented)));
                await _hooContext.SaveChangesAsync();
                return RedirectToPage("./Error");
            }
		}

        /// <summary>
        /// Returns the main page, selecting the first (earliest) comic (based on comic number) to render in the process.
        /// </summary>
        /// <returns>The main page.</returns>
        public async Task<IActionResult> OnGetFirstComicAsync()
        {
            try
            {
                var redirectString = await GetOfflineRedirectAsync();
                if (!string.IsNullOrWhiteSpace(redirectString))
                {
                    return RedirectToPage(redirectString);
                }

                var allComics = await _hooContext.Comics
                    .Include(comic => comic.UploadedBy)
                    .Include(comic => comic.ComicLikes)
                    .AsSplitQuery()
                    .Where(comic => !comic.IsHidden)
                    .OrderByDescending(comic => comic.ComicNumber)
                    .ToListAsync();
                var firstComic = allComics.First();
                var nextPreviousComicIds = GetNextPreviousComicIds(firstComic, allComics);
                NextComicId = nextPreviousComicIds.Item1;
                PreviousComicId = nextPreviousComicIds.Item2;
                Comic = firstComic;
                ComicIsLiked = Comic.ComicLikes.Any(comicItem => comicItem.IpAddress == Request.HttpContext.Connection.RemoteIpAddress.ToString());

                return Page();
            }
            catch (Exception exception)
            {
                await _hooContext.Events.AddAsync(new Event(
                eventType: EventTypes.Error,
                    details: $"Failed to load first comic: {exception.Message}",
                    stackTrace: JsonConvert.SerializeObject(value: exception.StackTrace, formatting: Formatting.Indented)));
                await _hooContext.SaveChangesAsync();
                return RedirectToPage("./Error");
            }
        }

        /// <summary>
        /// Returns the main page, selecting the last (latest) comic (based on comic number) to render in the process.
        /// </summary>
        /// <returns>The main page.</returns>
        public async Task<IActionResult> OnGetLastComicAsync()
        {
            try
            {
                var redirectString = await GetOfflineRedirectAsync();
                if (!string.IsNullOrWhiteSpace(redirectString))
                {
                    return RedirectToPage(redirectString);
                }

                var allComics = await _hooContext.Comics
                    .Include(comic => comic.UploadedBy)
                    .Include(comic => comic.ComicLikes)
                    .AsSplitQuery()
                    .Where(comic => !comic.IsHidden)
                    .OrderByDescending(comic => comic.ComicNumber)
                    .ToListAsync();
                var lastComic = allComics.Last();
                var nextPreviousComicIds = GetNextPreviousComicIds(lastComic, allComics);
                NextComicId = nextPreviousComicIds.Item1;
                PreviousComicId = nextPreviousComicIds.Item2;
                Comic = lastComic;
                ComicIsLiked = Comic.ComicLikes.Any(comicItem => comicItem.IpAddress == Request.HttpContext.Connection.RemoteIpAddress.ToString());

                return Page();
            }
            catch (Exception exception)
            {
                await _hooContext.Events.AddAsync(new Event(
                    eventType: EventTypes.Error,
                    details: $"Failed to load last comic: {exception.Message}",
                    stackTrace: JsonConvert.SerializeObject(value: exception.StackTrace, formatting: Formatting.Indented)));
                await _hooContext.SaveChangesAsync();
                return RedirectToPage("./Error");
            }
        }

        /// <summary>
        /// Toggles the Night Mode function of the app.<br/>
        /// Night mode features a softer theme ideal for night browsing.
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnGetToggleNightModeAsync(string returnUrl = null)
        {
            try
            {
                string? nightModeSetting = Request.Cookies["nightMode"];
                if (string.IsNullOrWhiteSpace(nightModeSetting))
                {
                    Response.Cookies.Append("nightMode", "true");
                }
                else
                {
                    Response.Cookies.Append("nightMode", Request.Cookies["nightMode"] == "true" ? "false" : "true");
                }

                return Url.IsLocalUrl(returnUrl) ? LocalRedirect(returnUrl) : RedirectToPage("/Index");
            }
            catch (Exception exception)
            {
                await _hooContext.Events.AddAsync(new Event(
                    eventType: EventTypes.Error,
                    details: $"Failed to switch themes: {exception.Message}",
                    stackTrace: JsonConvert.SerializeObject(value: exception.StackTrace, formatting: Formatting.Indented)));
                await _hooContext.SaveChangesAsync();
                return RedirectToPage("./Error");
            }
        }

        /// <summary>
        /// Attempts to toggle the liked state of the given comic for the current user GUID.<br/>
        /// If no <see cref="ComicLike"/> exists for the comic and the current user GUID (either user ID if signed in, or cookie value as guest), a new one is created.<br/>
        /// Otherwise, the existing like is deleted.<br/>
        /// The count of likes is then returned.<br/>
        /// Finally, returns a <see cref="JsonResult"/> representing the result of the request.
        /// </summary>
        /// <param name="comicGuid">The ID of the comic to toggle the liked state for.</param>
        /// <returns><see cref="JsonResult"/> representing the result of the request.</returns>
        public async Task<JsonResult> OnPatchComicLikedAsync(string comicGuid)
        {
            try
            {
                var comic = await _hooContext.Comics
                    .Include(comicItem => comicItem.ComicLikes)
                    .AsSplitQuery()
                    .SingleOrDefaultAsync(comicItem => comicItem.Id == comicGuid);
                if (comic == null)
                {
                    return new JsonResult(new BaseResult(
                        success: false,
                        message: $"Comic GUID {comicGuid} not found"));
                }

                // If signed in, we can link the like to the current user: otherwise, we have a guest: use the unique GUID from cookies
                var currentUser = _userManager.GetUserAsync(User).Result;
                string? userGuid = currentUser != null
                    ? currentUser.Id
                    : Request.Cookies["uniqueId"];

                if (string.IsNullOrWhiteSpace(userGuid))
                {
                    // No unregistered user GUID; create it
                    userGuid = Guid.NewGuid().ToString();
                    Response.Cookies.Append("uniqueId", userGuid);
                }

                var existingLike = await _hooContext.ComicLikes
                    .Include(comicLike => comicLike.Comic)
                    .SingleOrDefaultAsync(comicLike => comicLike.Comic.Id == comic.Id && comicLike.UserGuid == userGuid);
                
                if (existingLike == null)
                {
                    // No existing like - create it
                    await _hooContext.ComicLikes.AddAsync(new ComicLike(
                        comic: comic,
                        ipAddress: Request.HttpContext.Connection.RemoteIpAddress.ToString(),
                        userGuid: userGuid));

                    await _hooContext.Events.AddAsync(new Event(
                        eventType: EventTypes.ComicLikeCreated,
                        details: $"Comic GUID {comicGuid} was hearted by {(currentUser != null ? currentUser.GetEventLogString() : $"Guest user (GUID: {userGuid})")}"));
                }
                else
                {
                    // Existing like - remove it
                    _hooContext.ComicLikes.Remove(existingLike);
                    await _hooContext.Events.AddAsync(new Event(
                        eventType: EventTypes.ComicLikeDeleted,
                        details: $"Comic GUID {comicGuid} was unhearted by {(currentUser != null ? currentUser.GetEventLogString() : $"Guest user (GUID: {userGuid})")}"));
                }

                await _hooContext.SaveChangesAsync(); // Force a save so the upcoming count is current
                return new JsonResult(new ComicLikeResult(
                    success: true,
                    likeCount: _hooContext.ComicLikes
                        .Include(comicLike => comicLike.Comic)
                        .Where(comicLike => comicLike.Comic.Id == comicGuid).Count(),
                    wasLiked: existingLike == null));
            }
            catch (Exception exception)
            {
                await _hooContext.Events.AddAsync(new Event(
                    eventType: EventTypes.Error,
                    details: $"Failed to update heart status for comic GUID {comicGuid}: {exception.Message}",
                    stackTrace: JsonConvert.SerializeObject(value: exception.StackTrace, formatting: Formatting.Indented)));
                await _hooContext.SaveChangesAsync();
                return new JsonResult(new BaseResult(success: false));
            }
        }

        /// <summary>
        /// Fetches the IDs of the comics preceding and following the current comic, given a list of comics, if they exist.<br/>
        /// Non-existent IDs for the next/previous comics (if the current comic is already the first or last) are instead empty strings.
        /// </summary>
        /// <param name="currentComic">The current <see cref="Comic"/> being viewed.</param>
        /// <param name="comics">The list of <see cref="Comic"/> items to query.</param>
        /// <returns><see cref="Tuple{string, string}"/> consisting of the next comic ID and previous comic ID respectively.</returns>
        private static Tuple<string, string> GetNextPreviousComicIds(Comic currentComic, List<Comic> comics)
        {
            var currentIndex = comics.IndexOf(currentComic);
            var previousComicId = currentIndex + 1 <= comics.Count - 1 ? comics[currentIndex + 1].Id : string.Empty;
            var nextComicId  = currentIndex - 1 >= 0 ? comics[currentIndex - 1].Id : string.Empty;

            return new Tuple<string, string>(nextComicId, previousComicId);
        }

        /// <summary>
        /// Returns a redirection string if the site is considered closed based on settings, or an empty string otherwise.
        /// </summary>
        /// <returns>Redirection string if the site is considered closed, or an empty string otherwise.</returns>
        public async Task<string> GetOfflineRedirectAsync()
        {
            var settings = await _hooContext.Settings.FirstOrDefaultAsync();
            if (settings != null)
            {
                if (!settings.PublicAccessEnabled && !_signInManager.IsSignedIn(User))
                {
                    return "./Offline";
                }
            }

            return string.Empty;
        }
    }
}