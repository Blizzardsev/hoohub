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
        [Display(Prompt = "enter tags, number or the name of a comic")]
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
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<JsonResult> OnGetComics(
            string startAtComic = "",
            string query = "")
        {
            try
            {
                var allComics = _hooContext.Comics
                    .AsEnumerable()
                    .Where(comic => !comic.IsHidden)
                    .OrderByDescending(comic => comic.ComicNumber)
                    .ToList();

                if (!string.IsNullOrWhiteSpace(query))
                {
                    allComics = allComics
                        .AsEnumerable()
                        .Where(comic => comic.GetComicNameContainsTerms(query) || comic.GetComicNumberContainsTerms(query) || comic.GetTagsContainsTerms(query))
                        .ToList();
                    if (allComics.Count == 0)
                    {
                        return new JsonResult(new ArchiveResult(
                            success: true,
                            endOfResults: true));
                    }
                }

                var startFromComic = allComics.FirstOrDefault(comic => comic.Id == startAtComic);
                var startIndex = startFromComic != null
                    ? allComics.IndexOf(startFromComic)
                    : -1;

                var archiveComics = allComics.Skip(startIndex + 1).Take(20);
                return new JsonResult(new ArchiveResult(
                    success: true,
                    endOfResults: archiveComics.Last().Id == allComics.Last().Id,
                    archiveComicData: archiveComics.Select(comic => new ArchiveComicData(comic)).ToList()));
            }
            catch (Exception exception)
            {
                await _hooContext.Events.AddAsync(new Event(
                    eventType: Enums.EventTypes.Error,
                    details: $"Failed to load comic archive for starting comic GUID {startAtComic}: {exception.Message}",
                    stackTrace: JsonConvert.SerializeObject(value: exception.StackTrace, formatting: Formatting.Indented)));
                await _hooContext.SaveChangesAsync();
                return new JsonResult(new BaseResult(success: false));
            }
        }
    }
}
