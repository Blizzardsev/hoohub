using hoohub.Data;
using hoohub.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Drawing;

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

        public async Task<IActionResult> OnGetAsync(string comic = "")
        {
            try
            {
                Comic? comicToDisplay = null;
                if (!string.IsNullOrEmpty(comic))
                {
                    comicToDisplay = await _hooContext.Comics.SingleOrDefaultAsync(comicItem => comicItem.Guid == comic && !comicItem.IsHidden);
                }

                if (comicToDisplay == null)
                {
                    comicToDisplay = await _hooContext.Comics.OrderByDescending(comic => comic.ComicNumber).FirstOrDefaultAsync();
                }

                Comic = comicToDisplay;
                return Page();
            }
            catch (Exception exception)
            {
                await _hooContext.Events.AddAsync(new Event(
                    eventType: Enums.EventTypes.Error,
                    details: $"Failed to load comic GUID {comic}: {exception.Message}",
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
