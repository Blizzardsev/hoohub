using hoohub.Data;
using hoohub.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace hoohub.Pages
{
    [AllowAnonymous]
    public class IndexModel : PageModel
    {
        private readonly HooHubContext _hooContext;
        public Comic? Comic { get; set; }

        public IndexModel(HooHubContext hooContext)
        {
            _hooContext = hooContext;
        }

        public async Task<IActionResult> OnGetAsync(string comicGuid = "")
        {
            try
            {
                Comic? comic = null;
                if (!string.IsNullOrEmpty(comicGuid))
                {
                    comic = await _hooContext.Comics.SingleOrDefaultAsync(comic => comic.Guid == comicGuid && !comic.IsHidden);
                }

                if (comic == null)
                {
                    //comic = await _hooContext.Comics.OrderByDescending(comic => comic.ComicNumber).FirstOrDefaultAsync();
                    comic = new Comic(
                        comicTitle: "Directions",
                        comicNumber: "000",
                        comicDescription: "",
                        imageData: System.IO.File.ReadAllBytes("C:\\Users\\Blizz\\Pictures\\Misc\\Cute Stuff\\Commissions and Gifts\\RaionArt\\HooDoodles\\000.png"),
                        tags: new List<string>(),
                        isHidden: false);
                }

                Comic = comic;
                return Page();
            }
            catch (Exception exception)
            {
                await _hooContext.Events.AddAsync(new Event(
                    eventType: Enums.EventTypes.Error,
                    details: $"Failed to load comic GUID {comicGuid}: {exception.Message}",
                    stackTrace: JsonConvert.SerializeObject(value: exception.StackTrace, formatting: Formatting.Indented)));
                return RedirectToPage("./Error");
            }
        }

        public async Task<JsonResult> OnGetRandomAsync()
        {
			try
			{
                var comicsCount = await _hooContext.Comics.Where(comic => !comic.IsHidden).CountAsync();
				var randomComic = await _hooContext.Comics.ElementAtAsync(new Random().Next(0, comicsCount - 1));
                return new JsonResult(new ComicResult(
                    success: true,
                    comic: randomComic));
			}
			catch (Exception exception)
			{
				await _hooContext.Events.AddAsync(new Event(
					eventType: Enums.EventTypes.Error,
					details: $"Failed to load random comic: {exception.Message}",
					stackTrace: JsonConvert.SerializeObject(value: exception.StackTrace, formatting: Formatting.Indented)));
                return new JsonResult(new BaseResult(
                    success: false,
                    message: "Failed to load comic: please try again later."));
			}
		}

        public async Task OnGetNextAsync()
        {
            // TODO
            throw new NotImplementedException();
        }

        public async Task OnGetPreviousAsync()
        {
            // TODO
            throw new NotImplementedException();
        }
    }
}
