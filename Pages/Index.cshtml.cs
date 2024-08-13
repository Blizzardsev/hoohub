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
        public Comic? Comic { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string NextComicId { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public string PreviousComicID { get; private set; }

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
                Comic? comicToDisplay = null;
                if (!string.IsNullOrEmpty(comic))
                {
                    comicToDisplay = await _hooContext.Comics.SingleOrDefaultAsync(comicItem => comicItem.Id == comic && !comicItem.IsHidden);
                }

                if (comicToDisplay == null)
                {
                    comicToDisplay = await _hooContext.Comics
                        .Where(comic => !comic.IsHidden)
                        .OrderByDescending(comic => comic.ComicNumber).FirstOrDefaultAsync();
                }

                var nextPreviousComicIds = GetNextPreviousComicIds(comicToDisplay, _hooContext.Comics.Where(comic => !comic.IsHidden).ToList());
                NextComicId = nextPreviousComicIds.Item1;
                PreviousComicID = nextPreviousComicIds.Item2;
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
        public async Task<IActionResult> OnGetRandomAsync()
        {
			try
			{
                var allComics = await _hooContext.Comics.Where(comic => !comic.IsHidden).ToListAsync();
				var randomComic = await _hooContext.Comics.ElementAtAsync(new Random().Next(0, allComics.Count - 1));
                var nextPreviousComicIds = GetNextPreviousComicIds(randomComic, _hooContext.Comics.Where(comic => !comic.IsHidden).ToList());
                NextComicId = nextPreviousComicIds.Item1;
                PreviousComicID = nextPreviousComicIds.Item2;
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
        /// <param name="currentComic"></param>
        /// <param name="comics"></param>
        /// <returns></returns>
        private Tuple<string, string> GetNextPreviousComicIds(Comic currentComic, List<Comic> comics)
        {
            var currentIndex = comics.IndexOf(currentComic);
            var nextComicId = comics.IndexOf(comics.Last()) > currentIndex ? comics[currentIndex++].Id : string.Empty;
            var previousComicId = 0 < currentIndex ? comics[currentIndex--].Id : string.Empty;

            return new Tuple<string, string>(
                nextComicId,
                previousComicId);
        }
    }
}
