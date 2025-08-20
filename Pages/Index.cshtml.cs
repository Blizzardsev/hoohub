using hoohub.Data;
using Microsoft.AspNetCore.Authorization;
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

        /// <summary>
        /// The comic to render on the page.<br/>
        /// By default this is today's comic, but depending on request could be a specific comic or a comic based on an index.
        /// </summary>
        public Comic? Comic { get; private set; }

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
        /// Initialises a new instance of the <see cref="IndexModel"/> class.
        /// </summary>
        /// <param name="hooContext">Injected app context.</param>
        public IndexModel(HooHubContext hooContext)
        {
            _hooContext = hooContext;
        }

        /// <summary>
        /// Returns the main page.</br>
        /// If a comic ID is specified in the request (E.G if going forward/backward), load the associated comic if possible for rendering.<br/>
        /// Otherwise, default to today's comic.
        /// </summary>
        /// <param name="comic">Optional comic GUID to render.</param>
        /// <returns>The main page.</returns>
        public async Task<IActionResult> OnGetAsync(string comic = "")
        {
            try
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

                Comic? comicToDisplay = null;
                if (!string.IsNullOrEmpty(comic))
                {
                    comicToDisplay = await _hooContext.Comics
                        .Include(comicItem => comicItem.UploadedBy)
                        .AsSplitQuery()
                        .SingleOrDefaultAsync(comicItem => comicItem.Id == comic && !comicItem.IsHidden);
                }

                if (comicToDisplay == null)
                {
                    comicToDisplay = await _hooContext.Comics
                        .Include(comic => comic.UploadedBy)
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

                return Page();
            }
            catch (Exception exception)
            {
                await _hooContext.Events.AddAsync(new Event(
                    eventType: Enums.EventTypes.Error,
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
                var allComics = await _hooContext.Comics
                    .Include(comic => comic.UploadedBy)
                    .AsSplitQuery()
                    .Where(comic => !comic.IsHidden && comic.Id != currentComic)
                    .OrderByDescending(comic => comic.ComicNumber)
                    .ToListAsync();
				var randomComic = allComics.ElementAt(new Random().Next(0, allComics.Count));
                var nextPreviousComicIds = GetNextPreviousComicIds(randomComic, allComics);
                NextComicId = nextPreviousComicIds.Item1;
                PreviousComicId = nextPreviousComicIds.Item2;
                Comic = randomComic;
                return Page();
            }
			catch (Exception exception)
			{
                await _hooContext.Events.AddAsync(new Event(
                eventType: Enums.EventTypes.Error,
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
                var allComics = await _hooContext.Comics
                    .Include(comic => comic.UploadedBy)
                    .AsSplitQuery()
                    .Where(comic => !comic.IsHidden)
                    .OrderByDescending(comic => comic.ComicNumber)
                    .ToListAsync();
                var firstComic = allComics.First();
                var nextPreviousComicIds = GetNextPreviousComicIds(firstComic, allComics);
                NextComicId = nextPreviousComicIds.Item1;
                PreviousComicId = nextPreviousComicIds.Item2;
                Comic = firstComic;
                return Page();
            }
            catch (Exception exception)
            {
                await _hooContext.Events.AddAsync(new Event(
                eventType: Enums.EventTypes.Error,
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
                var allComics = await _hooContext.Comics
                    .Include(comic => comic.UploadedBy)
                    .AsSplitQuery()
                    .Where(comic => !comic.IsHidden)
                    .OrderByDescending(comic => comic.ComicNumber)
                    .ToListAsync();
                var lastComic = allComics.Last();
                var nextPreviousComicIds = GetNextPreviousComicIds(lastComic, allComics);
                NextComicId = nextPreviousComicIds.Item1;
                PreviousComicId = nextPreviousComicIds.Item2;
                Comic = lastComic;
                return Page();
            }
            catch (Exception exception)
            {
                await _hooContext.Events.AddAsync(new Event(
                    eventType: Enums.EventTypes.Error,
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
        public async Task<IActionResult> OnGetToggleNightModeAsync()
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

                return RedirectToPage();
            }
            catch (Exception exception)
            {
                await _hooContext.Events.AddAsync(new Event(
                    eventType: Enums.EventTypes.Error,
                    details: $"Failed to switch themes: {exception.Message}",
                    stackTrace: JsonConvert.SerializeObject(value: exception.StackTrace, formatting: Formatting.Indented)));
                await _hooContext.SaveChangesAsync();
                return RedirectToPage("./Error");
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
    }
}
