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
        /// 
        /// </summary>
        public Comic? Comic { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public string? NextComicId { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public string? PreviousComicId { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsNightMode { get; private set; } = false;

        /// <summary>
        /// 
        /// </summary>
        public HooHubUser ComicUploader { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hooContext"></param>
        public IndexModel(HooHubContext hooContext)
        {
            _hooContext = hooContext;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="comic"></param>
        /// <returns></returns>
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
                        .SingleOrDefaultAsync(comicItem => comicItem.Id == comic && !comicItem.IsHidden);
                }

                if (comicToDisplay == null)
                {
                    comicToDisplay = await _hooContext.Comics
                        .Include(comic => comic.UploadedBy)
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
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnGetRandomComicAsync(string currentComic = "")
        {
			try
			{
                var allComics = await _hooContext.Comics
                    .Include(comic => comic.UploadedBy)
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
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnGetFirstComicAsync()
        {
            try
            {
                var allComics = await _hooContext.Comics
                    .Include(comic => comic.UploadedBy)
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
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnGetLastComicAsync()
        {
            try
            {
                var allComics = await _hooContext.Comics
                    .Include(comic => comic.UploadedBy)
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
        /// 
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
        /// 
        /// </summary>
        /// <param name="currentComic"></param>
        /// <param name="comics"></param>
        /// <returns></returns>
        private Tuple<string, string> GetNextPreviousComicIds(Comic currentComic, List<Comic> comics)
        {
            var currentIndex = comics.IndexOf(currentComic);
            var previousComicId = currentIndex + 1 <= comics.Count - 1 ? comics[currentIndex + 1].Id : string.Empty;
            var nextComicId  = currentIndex - 1 >= 0 ? comics[currentIndex - 1].Id : string.Empty;

            return new Tuple<string, string>(nextComicId, previousComicId);
        }
    }
}
