using hoohub.Data;
using hoohub.Requests.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace hoohub.Pages.Manage
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly HooHubContext _hooContext;

        public IndexModel(HooHubContext hooContext)
        {

        }

        public class ManageInput
        {
            public class NewComic()
            {
                public string ComicNumber { get; set; }

                public string ComicTitle { get; set; }

                public string ComicDescription { get; set; }

                public string Tags { get; set; }

                public bool IsHidden { get; set; }
            }

            public class ManageComic()
            {

            }

            public class ManageSite()
            {

            }

            public NewComic NewComicInput;
            public ManageComic ManageComicInput;
            public ManageSite ManageSiteInput;

            public ManageInput()
            {
                NewComicInput = new NewComic();
                ManageComicInput = new ManageComic();
                ManageSiteInput = new ManageSite();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void OnGet()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="comicNumber"></param>
        /// <param name="comicTitle"></param>
        /// <param name="comicDescription"></param>
        /// <param name="imageData"></param>
        /// <param name="tags"></param>
        /// <param name="isHidden"></param>
        /// <returns></returns>
        public async Task<JsonResult> OnPostComicAsync(
            string comicNumber,
            string comicTitle,
            string comicDescription,
            byte[] imageData,
            string tags,
            bool isHidden)
        {
            try
            {
                await _hooContext.Comics.AddAsync(new Comic(
                    comicNumber: comicNumber,
                    comicTitle: comicTitle,
                    comicDescription: comicDescription,
                    imageData: imageData,
                    tags: tags,
                    isHidden: isHidden));
                await _hooContext.SaveChangesAsync();
                return new JsonResult(new BaseResult(success: true));
            }
            catch (Exception exception)
            {
                await _hooContext.Events.AddAsync(new Event(
                    eventType: Enums.EventTypes.Error,
                    details: $"Failed to create new comic: {exception.Message}",
                    stackTrace: JsonConvert.SerializeObject(value: exception.StackTrace, formatting: Formatting.Indented)));
                return new JsonResult(new BaseResult(success: false));
            }
        }

        public async Task<JsonResult> OnPutComicAsync(
            string comicGuid,
            string comicNumber,
            string comicTitle,
            string comicDescription,
            byte[] imageData,
            string tags,
            bool isHidden)
        {
            try
            {
                throw new NotImplementedException("hootbye");
                return new JsonResult(new BaseResult(success: true));
            }
            catch (Exception exception)
            {
                await _hooContext.Events.AddAsync(new Event(
                    eventType: Enums.EventTypes.Error,
                    details: $"Failed to update comic GUID {comicGuid}: {exception.Message}",
                    stackTrace: JsonConvert.SerializeObject(value: exception.StackTrace, formatting: Formatting.Indented)));
                return new JsonResult(new BaseResult(success: false));
            }
        }
    }
}
