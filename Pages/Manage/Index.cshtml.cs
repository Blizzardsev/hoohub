using hoohub.Data;
using hoohub.Requests.Results;
using hoohub.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace hoohub.Pages.Manage
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly HooHubContext _hooContext;
        private readonly UserManager<HooHubUser> _userManager;
        private readonly string[] _comicUploadFileExtensions = ["png", "jpg", "jpeg"];

        [BindProperty]
        public ManageInput ManageInputModel { get; set; } = new ManageInput();

        public string DisplayPictureOnLoad { get; set; }

        public IndexModel(HooHubContext hooContext, UserManager<HooHubUser> userManager)
        {
            _hooContext = hooContext;
            _userManager = userManager;
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
                [RegularExpression("^[a-zA-Z0-9]{1,64}(?:,\\s*[a-zA-Z0-9]{1,64})*$", ErrorMessage = "Tags must be comma-separated, with no symbols or whitespace")]
                public string? Tags { get; set; } = string.Empty;

                [Display(Name = "Hidden")]
                [Required]
                public bool IsHidden { get; set; } = false;
            }

            public class ManageComic()
            {
                [Required(AllowEmptyStrings = false, ErrorMessage = "Comic GUID must be provided")]
                [MinLength(36)]
                [MaxLength(36)]
                public string ComicGuid { get; set; }

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
                [RegularExpression("^[a-zA-Z0-9]{1,64}(?:,\\s*[a-zA-Z0-9]{1,64})*$", ErrorMessage = "Tags must be comma-separated, with no symbols or whitespace")]
                public string? Tags { get; set; } = string.Empty;

                [Display(Name = "Hidden")]
                [Required]
                public bool IsHidden { get; set; } = false;
            }

            public class ManageSite()
            {

            }

            public class ManageMe()
            {
                [Display(Name = "Handle", Prompt = "hoo")]
                [Required(AllowEmptyStrings = false, ErrorMessage = "Handle must be provided")]
                [MinLength(1)]
                [MaxLength(10)]
                [RegularExpression("^[A-Za-z]+$", ErrorMessage = "Handle cannot contain numbers, symbols or whitespace")]
                public string Handle { get; set; }

                [Required(ErrorMessage = "Profile must have a picture")]
                [MinLength(1)]
                [MaxLength(1)]
                public IFormFile ImageData { get; set; }
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
            var currentUser = _userManager.GetUserAsync(User).Result;
            DisplayPictureOnLoad = Convert.ToBase64String(currentUser.DisplayPicture);
            ManageInputModel.ManageMeInput.Handle = currentUser.Handle;
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

                await _hooContext.SaveChangesAsync();
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
                        .OrderByDescending(comic => comic.ComicNumber)
                        .Select(comic => new Requests.Data.ManageComicListData(comic))
                        .ToList()));
            }
            catch (Exception exception) 
            {
                await _hooContext.Events.AddAsync(new Event(
                    eventType: Enums.EventTypes.Error,
                    details: $"Failed to load comics list: {exception.Message}",
                    stackTrace: JsonConvert.SerializeObject(value: exception.StackTrace, formatting: Formatting.Indented)));

                await _hooContext.SaveChangesAsync();
                return new JsonResult(new BaseResult(success: false));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="comicGuid"></param>
        /// <returns></returns>
        public async Task<JsonResult> OnGetManageComicDetailsAsync(string comicGuid)
        {
            try
            {
                var comic = await _hooContext.Comics.SingleOrDefaultAsync(comic => comic.Id == comicGuid);
                if (comic == null)
                {
                    return new JsonResult(new BaseResult(success: false, message: $"Comic GUID {comicGuid} not found"));
                }

                return new JsonResult(new ManageComicDetailsResult(success: true, comic: comic));
            }
            catch (Exception exception)
            {
                await _hooContext.Events.AddAsync(new Event(
                    eventType: Enums.EventTypes.Error,
                    details: $"Failed to load details for comic GUID {comicGuid}: {exception.Message}",
                    stackTrace: JsonConvert.SerializeObject(value: exception.StackTrace, formatting: Formatting.Indented)));

                await _hooContext.SaveChangesAsync();
                return new JsonResult(new BaseResult(success: false, message: "Failed to load comic details"));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="comicGuid"></param>
        /// <param name="comicNumber"></param>
        /// <param name="comicTitle"></param>
        /// <param name="comicDescription"></param>
        /// <param name="tags"></param>
        /// <param name="isHidden"></param>
        /// <param name="imageData"></param>
        /// <returns></returns>
        public async Task<JsonResult> OnPatchComicAsync(
            string comicGuid,
            string comicNumber,
            string comicTitle,
            string comicDescription,
            string tags,
            bool isHidden,
            IFormFile imageData = null)
        {
            try
            {
                var comic = await _hooContext.Comics.SingleOrDefaultAsync(comic => comic.Id == comicGuid);
                if (comic == null)
                {
                    return new JsonResult(new BaseResult(success: false, message: $"Comic GUID {comicGuid} not found"));
                }

                if (comicNumber != comic.ComicNumber && _hooContext.Comics.Any(comic => comic.Id != comicGuid && comic.ComicNumber == comicNumber))
                {
                    return new JsonResult(new BaseResult(
                        success: false,
                        message: $"Comic {comicNumber} already exists"));
                }

                if ((comic.ImageData == null || comic.ImageData.Length == 0) && imageData == null)
                {
                    return new JsonResult(new BaseResult(
                        success: false,
                        message: $"A comic image file is required"));
                }

                if (imageData != null && !_comicUploadFileExtensions.Contains(imageData.FileName.Split(".").Last()))
                {
                    return new JsonResult(new BaseResult(
                        success: false,
                        message: $"Comics must be uploaded in jpg or png format"));
                }

                comic.ComicNumber = comicNumber;
                comic.ComicTitle = comicTitle;
                comic.ComicDescription = comicDescription;
                comic.ImageData = imageData != null 
                    ? FormattingService.GetIFormFileAsBytes(imageData) 
                    : comic.ImageData;
                comic.Tags = tags;
                comic.IsHidden = isHidden;

                await _hooContext.Events.AddAsync(new Event(
                    eventType: Enums.EventTypes.ComicUpdated,
                    details: $"Comic GUID {comic.Id} was updated by ${_userManager.GetUserAsync(User).Result.GetEventLogString()}"));
                await _hooContext.SaveChangesAsync();

                return new JsonResult(new BaseResult(success: true));
            }
            catch (Exception exception)
            {
                await _hooContext.Events.AddAsync(new Event(
                    eventType: Enums.EventTypes.Error,
                    details: $"Failed to update comic GUID {comicGuid}: {exception.Message}",
                    stackTrace: JsonConvert.SerializeObject(value: exception.StackTrace, formatting: Formatting.Indented)));
                await _hooContext.SaveChangesAsync();
                return new JsonResult(new BaseResult(success: false));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="imageData"></param>
        /// <returns></returns>
        public async Task<JsonResult> OnPatchMeAsync(string handle, IFormFile imageData = null)
        {
            var user = await _userManager.GetUserAsync(User);
            try
            {
                if ((user.DisplayPicture == null || user.DisplayPicture.Length == 0) && imageData == null)
                {
                    return new JsonResult(new BaseResult(
                        success: false,
                        message: $"A profile picture is required"));
                }

                if (imageData != null && !_comicUploadFileExtensions.Contains(imageData.FileName.Split(".").Last()))
                {
                    return new JsonResult(new BaseResult(
                        success: false,
                        message: $"Profile pictures must be uploaded in jpg or png format"));
                }

                user.Handle = handle;
                user.DisplayPicture = imageData != null
                    ? FormattingService.GetIFormFileAsBytes(imageData)
                    : user.DisplayPicture;

                await _hooContext.Events.AddAsync(new Event(
                    eventType: Enums.EventTypes.UserUpdated,
                    details: $"{user.Handle} updated their profile."));
                await _hooContext.SaveChangesAsync();

                return new JsonResult(new BaseResult(success: true));
            }
            catch (Exception exception)
            {
                await _hooContext.Events.AddAsync(new Event(
                    eventType: Enums.EventTypes.Error,
                    details: $"Failed to update profile for {user.GetEventLogString()}: {exception.Message}",
                    stackTrace: JsonConvert.SerializeObject(value: exception.StackTrace, formatting: Formatting.Indented)));
                await _hooContext.SaveChangesAsync();
                return new JsonResult(new BaseResult(success: false));
            }
        }
    }
}
