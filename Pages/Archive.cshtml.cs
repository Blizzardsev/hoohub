using hoohub.Data;
using hoohub.Requests.Data;
using hoohub.Requests.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace hoohub.Pages
{
    [AllowAnonymous]
    public class ArchiveModel : PageModel
    {
        private readonly HooHubContext _hooContext;

        [DataType(DataType.Text)]
        [MaxLength(200)]
        [Display(Prompt = "enter one or more tags")]
        [RegularExpression("^[a-zA-Z-' ,]+$")]
        public string TagInput { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hooContext"></param>
        public ArchiveModel(HooHubContext hooContext)
        {
            _hooContext = hooContext;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnGet()
        {
            try
            {
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
        /// 
        /// </summary>
        /// <param name="startAtComic"></param>
        /// <returns></returns>
        public async Task<JsonResult> OnGetComics(string startAtComic = "")
        {
            try
            {
                var allComics = _hooContext.Comics
                    .Where(comic => !comic.IsHidden)
                    .OrderByDescending(comic => comic.ComicNumber)
                    .ToList();
                var startIndex = allComics.Any(comic => comic.Id == startAtComic)
                    ? allComics.IndexOf(allComics.First(comic => comic.Id == startAtComic))
                    : 0;

                return new JsonResult(new ArchiveResult(
                    success: true,
                    archiveComicData: allComics
                        .Skip(startIndex)
                        .Take(20)
                        .Select(comic => new ArchiveComicData(comic))
                        .ToList()));
            }
            catch (Exception exception)
            {
                await _hooContext.Events.AddAsync(new Event(
                eventType: Enums.EventTypes.Error,
                    details: $"Failed to load comic archive for starting comic GUID {startAtComic}: {exception.Message}",
                    stackTrace: JsonConvert.SerializeObject(value: exception.StackTrace, formatting: Formatting.Indented)));
                return new JsonResult(new BaseResult(success: false));
            }
        }
    }
}
