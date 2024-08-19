using hoohub.Data;
using hoohub.Requests.Results;
using hoohub.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace hoohub.Pages.Manage
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly HooHubContext _hooContext;
        private readonly string[] _comicUploadFileExtensions = ["png", "jpg", "jpeg"];

        [BindProperty]
        public ManageInput ManageInputModel { get; set; } = new ManageInput();

        public IndexModel(HooHubContext hooContext)
        {
            _hooContext = hooContext;
        }

        public class ManageInput
        {
            public class NewComic()
            {
                [Required(ErrorMessage = "Comic must have an image")]
                [MinLength(1)]
                [MaxLength(1)]
                public IFormFile ImageData { get; set; }

                [Display(Name = "Comic number", Prompt = "000")]
                [Required(AllowEmptyStrings = false, ErrorMessage = "Comic must have a comic number")]
                [DataType(DataType.Text)]
                [MinLength(3)]
                [MaxLength(3)]
                [RegularExpression("^((00[0-9])|(0[0-9][0-9])|([0-9][0-9][0-9]))$", ErrorMessage = "Comic number must be in the correct format (E.G 001)")]
                public string ComicNumber { get; set; }

                [Display(Name = "Comic title", Prompt = "hate everything")]
                [Required(AllowEmptyStrings = false, ErrorMessage = "Comic must have a title")]
                [DataType(DataType.Text)]
                [MinLength(1)]
                [MaxLength(30)]
                public string ComicTitle { get; set; }

                [Display(Name = "Comic description", Prompt = "hate everything")]
                [MaxLength(100)]
                [DataType(DataType.Text)]
                public string? ComicDescription { get; set; } = string.Empty;

                [Display(Name = "Tags", Prompt = "hoo,hate,everything")]
                [MaxLength(100)]
                [DataType(DataType.Text)]
                public string? Tags { get; set; } = string.Empty;

                [Display(Name = "Hidden")]
                [Required]
                public bool IsHidden { get; set; } = false;
            }

            public class ManageComic()
            {

            }

            public class ManageSite()
            {

            }

            public class ManageMe()
            {
                
            }

            public NewComic NewComicInput;
            public ManageComic ManageComicInput;
            public ManageSite ManageSiteInput;
            public ManageMe ManageMeInput;

            public ManageInput()
            {
                NewComicInput = new NewComic();
                ManageComicInput = new ManageComic();
                ManageSiteInput = new ManageSite();
                ManageMeInput = new ManageMe();
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
            IFormFile imageData,
            string tags,
            bool isHidden)
        {
            try
            {
                if (_hooContext.Comics.Any(comic => comic.ComicNumber == comicNumber))
                {
                    return new JsonResult(new BaseResult(
                        success: false,
                        message: $"Comic {comicNumber} already exists"));
                }

                if (imageData == null)
                {
                    return new JsonResult(new BaseResult(
                        success: false,
                        message: $"A comic image file is required"));
                }

                if (!_comicUploadFileExtensions.Contains(imageData.FileName.Split(".").Last()))
                {
                    return new JsonResult(new BaseResult(
                        success: false,
                        message: $"Comics must be uploaded in jpg or png format"));
                }

                var newComic = new Comic(
                    comicNumber: comicNumber,
                    comicTitle: comicTitle,
                    comicDescription: string.IsNullOrWhiteSpace(comicDescription) ? string.Empty : comicDescription,
                    imageData: FormattingService.GetIFormFileAsBytes(imageData),
                    tags: string.IsNullOrWhiteSpace(tags) ? string.Empty : tags,
                    isHidden: isHidden);
                await _hooContext.Comics.AddAsync(newComic);
                await _hooContext.Events.AddAsync(new Event(
                    eventType: Enums.EventTypes.ComicCreated,
                    details: $"Comic GUID {newComic.Id} created."));
                await _hooContext.SaveChangesAsync();

                return new JsonResult(new BaseResult(success: true));
            }
            catch (Exception exception)
            {
                await _hooContext.Events.AddAsync(new Event(
                    eventType: Enums.EventTypes.Error,
                    details: $"Failed to create new comic: {exception.Message}",
                    stackTrace: JsonConvert.SerializeObject(value: exception.StackTrace, formatting: Formatting.Indented)));
                return new JsonResult(new BaseResult(
                    success: false,
                    message: "Failed to create new comic: please try again later"));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<JsonResult> OnGetManageComicsListAsync()
        {
            try
            {
                return new JsonResult(new ManageComicListResult(
                    success: true,
                    manageComicListData: _hooContext.Comics
                        .Select(comic => new Requests.Data.ManageComicListData(comic))
                        .ToList()));
            }
            catch (Exception exception) 
            {
                await _hooContext.Events.AddAsync(new Event(
                    eventType: Enums.EventTypes.Error,
                    details: $"Failed to load comics list: {exception.Message}",
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
