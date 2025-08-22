using hoohub.Data;
using hoohub.Enums;
using hoohub.Requests.Data;
using hoohub.Requests.Results;
using hoohub.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis;
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
        private readonly string[] _comicUploadFileExtensions = ["png", "jpg", "jpeg", "gif"];

        [BindProperty]
        public ManageInput ManageInputModel { get; set; } = new ManageInput();

        /// <summary>
        /// The display picture to load when the Manage Me tab is opened.
        /// </summary>
        public string DisplayPictureOnLoad { get; set; } = string.Empty;

        /// <summary>
        /// Whether or not the app is considered to be in Night Mode.<br/>
        /// Some assets may need replacement based on this.
        /// </summary>
        public bool IsNightMode { get; private set; } = false;

        /// <summary>
        /// Initialises a new instance of the <see cref="IndexModel"/> class.
        /// </summary>
        /// <param name="hooContext">Injected app context.</param>
        /// <param name="userManager">Injected <see cref="UserManager{TUser}"/>.</param>
        public IndexModel(HooHubContext hooContext, UserManager<HooHubUser> userManager)
        {
            _hooContext = hooContext;
            _userManager = userManager;
        }

        /// <summary>
        /// Input validation models.
        /// </summary>
        public class ManageInput
        {
            public class NewComic
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
                [RegularExpression("^(((00[0-9])|(0[0-9][0-9])|([0-9][0-9][0-9]))(.[0-9])?)$", ErrorMessage = "Comic number must be in the correct format (E.G 001)")]
                public string ComicNumber { get; set; } = string.Empty;

                [Display(Name = "Comic title", Prompt = "hate everything")]
                [Required(AllowEmptyStrings = false, ErrorMessage = "Comic must have a title")]
                [DataType(DataType.Text)]
                [MinLength(1)]
                [MaxLength(30)]
                public string ComicTitle { get; set; } = string.Empty;

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

                [Display(Name = "Scheduled post")]
                [Required]
                public bool IsScheduled { get; set; } = false;

                [Display(Name = "Schedule for")]
                [DataType(DataType.DateTime)]
                public DateTime ScheduleFor { get; set; } = DateTime.UtcNow;

                public NewComic()
                {
                    var utcNow = DateTime.UtcNow;
                    ScheduleFor = new DateTime(
                        year: utcNow.Year,
                        month: utcNow.Month,
                        day: utcNow.Day + 1,
                        hour: 16,
                        minute: 00,
                        second: 00).ToUniversalTime();
                }
            }

            public class ManageComic()
            {
                [Required(AllowEmptyStrings = false, ErrorMessage = "Comic GUID must be provided")]
                [MinLength(36)]
                [MaxLength(36)]
                public string ComicGuid { get; set; } = string.Empty;

                [Required(ErrorMessage = "Comic must have an image")]
                [MinLength(1)]
                [MaxLength(1)]
                public IFormFile ImageData { get; set; }

                [Display(Name = "Comic number", Prompt = "000")]
                [Required(AllowEmptyStrings = false, ErrorMessage = "Comic must have a comic number")]
                [DataType(DataType.Text)]
                [MinLength(3)]
                [MaxLength(3)]
                [RegularExpression("^(((00[0-9])|(0[0-9][0-9])|([0-9][0-9][0-9]))(.[0-9])?)$", ErrorMessage = "Comic number must be in the correct format (E.G 001)")]
                public string ComicNumber { get; set; } = string.Empty;

                [Display(Name = "Comic title", Prompt = "hate everything")]
                [Required(AllowEmptyStrings = false, ErrorMessage = "Comic must have a title")]
                [DataType(DataType.Text)]
                [MinLength(1)]
                [MaxLength(30)]
                public string ComicTitle { get; set; } = string.Empty;

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

                [Display(Name = "Scheduled post")]
                [Required]
                public bool IsScheduled { get; set; } = false;

                [Display(Name = "Schedule for")]
                [DataType(DataType.DateTime)]
                public DateTime ScheduleFor { get; set; } = DateTime.UtcNow;
            }

            public class ManageMe()
            {
                [Display(Name = "Handle", Prompt = "hoo")]
                [Required(AllowEmptyStrings = false, ErrorMessage = "Handle must be provided")]
                [MinLength(1)]
                [MaxLength(30)]
                [RegularExpression("^[A-Za-z]+$", ErrorMessage = "Handle cannot contain numbers, symbols or whitespace")]
                public string Handle { get; set; }

                [Display(Name = "Social media URL", Prompt = "https://twitter.com")]
                [MaxLength(255)]
                [RegularExpression("^(https?:\\/\\/)?([\\w\\-]+\\.)+[\\w\\-]+(\\/[\\w\\-.,@?^=%&:/~+#]*)?$", ErrorMessage = "Handle must be a valid URL")]
                public string SocialLink { get; set; }

                [Required(ErrorMessage = "Profile must have a picture")]
                [MinLength(1)]
                [MaxLength(1)]
                public IFormFile ImageData { get; set; }
            }

            public NewComic NewComicInput;
            public ManageComic ManageComicInput;
            public ManageMe ManageMeInput;

            public ManageInput()
            {
                NewComicInput = new NewComic();
                ManageComicInput = new ManageComic();
                ManageMeInput = new ManageMe();
            }
        }

        /// <summary>
        /// Returns the page and populates the input validation models for any data that should be pre-filled (E.G user handle).
        /// </summary>
        public async Task<IActionResult> OnGet()
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

                var currentUser = _userManager.GetUserAsync(User).Result;
                if (currentUser == null)
                {
                    return RedirectToPage("/Index");
                }

                DisplayPictureOnLoad = Convert.ToBase64String(currentUser.DisplayPicture);
                ManageInputModel.ManageMeInput.Handle = currentUser.Handle;
                ManageInputModel.ManageMeInput.SocialLink = currentUser.SocialLink;
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
        /// Attempts to post a new comic, creating it and returning a <see cref="JsonResult"/> representing the result of the request.
        /// </summary>
        /// <param name="comicNumber">The number of the new comic to set. This must be unique.</param>
        /// <param name="comicTitle">The title of the new comic to set.</param>
        /// <param name="comicDescription">The description of the new comic to set.</param>
        /// <param name="imageData">The content of the new comic to set.</param>
        /// <param name="tags">The optional tags of the new comic to set.</param>
        /// <param name="isHidden">The hidden state to the new comic to set; hidden comics are not visible publicly.</param>
        /// <param name="isScheduled">The scheduled state of the new comic to set; scheduled comics are not visible until the scheduled date is met.</param>
        /// <param name="scheduleFor">The scheduled date of the new comic to set; scheduled comics are not visible until the scheduled date is met.</param>
        /// <returns><see cref="JsonResult"/> representing the result of the request.</returns>
        public async Task<JsonResult> OnPostComicAsync(
            string comicNumber,
            string comicTitle,
            string comicDescription,
            IFormFile imageData,
            string tags,
            bool isHidden,
            bool isScheduled,
            DateTime? scheduleFor)
        {
            try
            {
                var currentUser = _userManager.GetUserAsync(User).Result;
                if (currentUser == null)
                {
                    return new JsonResult(new BaseResult(
                        success: false,
                        message: $"You are not authorised to perform this action"));
                }

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

                if (isScheduled && scheduleFor.HasValue && DateTime.UtcNow > scheduleFor.Value.ToUniversalTime())
                {
                    return new JsonResult(new BaseResult(
                        success: false,
                        message: $"Cannot schedule for a date in the past: current time is {FormattingService.GetDateTimeAsString(DateTime.UtcNow)}"));
                }

                if (isScheduled && !scheduleFor.HasValue)
                {
                    return new JsonResult(new BaseResult(
                        success: false,
                        message: "Scheduled comics must have a date and/or time"));
                }

                if (isScheduled)
                {
                    // Make sure hidden if scheduled
                    isHidden = true;
                }

                var newComic = new Comic(
                    comicNumber: comicNumber,
                    comicTitle: comicTitle,
                    comicDescription: string.IsNullOrWhiteSpace(comicDescription) ? string.Empty : comicDescription,
                    imageData: FormattingService.GetIFormFileAsBytes(imageData),
                    tags: string.IsNullOrWhiteSpace(tags) ? string.Empty : tags,
                    isHidden: isScheduled ? true : isHidden,
                    uploadedBy: currentUser,
                    scheduledDate: scheduleFor != null && isScheduled ? scheduleFor.Value.ToUniversalTime() : null,
                    wasPublished: !isHidden && !isScheduled);

                await _hooContext.Comics.AddAsync(newComic);
                await _hooContext.Events.AddAsync(new Event(
                    eventType: EventTypes.ComicCreated,
                    details: $"Comic GUID {newComic.Id} created by {currentUser.GetEventLogString()}{(isScheduled ? $" and scheduled for {FormattingService.GetDateTimeAsString(scheduleFor.Value.ToUniversalTime())}" : string.Empty)}."));

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
        /// Attempts to fetch a list of all comics for selection in the management menu, returning a <see cref="JsonResult"/> representing the result of the request.
        /// </summary>
        /// <returns><see cref="JsonResult"/> representing the result of the request.</returns>
        public async Task<JsonResult> OnGetManageComicsListAsync()
        {
            try
            {
                return new JsonResult(new ManageComicListResult(
                    success: true,
                    manageComicListData: _hooContext.Comics
                        .OrderByDescending(comic => comic.ComicNumber)
                        .Select(comic => new ManageComicListData(comic))
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
        /// Attempts to fetch the details of a specific comic for display/editing in the manage comic menu, returning a <see cref="JsonResult"/> representing the result of the request.
        /// </summary>
        /// <param name="comicGuid">The GUID of the <see cref="Comic"/> to fetch details for.</param>
        /// <returns><see cref="JsonResult"/> representing the result of the request.</returns>
        public async Task<JsonResult> OnGetManageComicDetailsAsync(string comicGuid)
        {
            try
            {
                var comic = await _hooContext.Comics
                    .Include(comic => comic.LastEditedBy)
                    .Include(comic => comic.ComicLikes)
                    .AsSplitQuery()
                    .SingleOrDefaultAsync(comic => comic.Id == comicGuid);
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
        /// Attempts to update an existing comic, returning a <see cref="JsonResult"/> representing the result of the request.
        /// </summary>
        /// <param name="comicGuid">The GUID of the comic to update.</param>
        /// <param name="comicNumber">The number of the comic to set. This must be unique.</param>
        /// <param name="comicTitle">The title of the comic to set.</param>
        /// <param name="comicDescription">The description of the comic to set.</param>
        /// <param name="imageData">The content of the comic to set.</param>
        /// <param name="tags">The optional tags of the comic to set.</param>
        /// <param name="isHidden">The hidden state to the comic to set; hidden comics are not visible publicly.</param>
        /// <param name="isScheduled">The scheduled state of the comic to set; scheduled comics are not visible until the scheduled date is met.</param>
        /// <param name="scheduleFor">The scheduled date of the comic to set; scheduled comics are not visible until the scheduled date is met.</param>
        /// <returns><see cref="JsonResult"/> representing the result of the request.</returns>
        public async Task<JsonResult> OnPatchComicAsync(
            string comicGuid,
            string comicNumber,
            string comicTitle,
            string comicDescription,
            IFormFile imageData,
            string tags,
            bool isHidden,
            bool isScheduled,
            DateTime? scheduleFor)
        {
            try
            {
                var currentUser = _userManager.GetUserAsync(User).Result;
                if (currentUser == null)
                {
                    return new JsonResult(new BaseResult(
                        success: false,
                        message: $"You are not authorised to perform this action"));
                }

                var comic = await _hooContext.Comics.SingleOrDefaultAsync(comic => comic.Id == comicGuid);
                if (comic == null)
                {
                    return new JsonResult(new BaseResult(
                        success: false,
                        message: $"Comic GUID {comicGuid} not found"));
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

                if (isScheduled && scheduleFor.HasValue && DateTime.UtcNow > scheduleFor.Value.ToUniversalTime())
                {
                    return new JsonResult(new BaseResult(
                        success: false,
                        message: $"Cannot schedule for a date in the past: current time is {FormattingService.GetDateTimeAsString(DateTime.UtcNow)}"));
                }

                if (isScheduled && !scheduleFor.HasValue)
                {
                    return new JsonResult(new BaseResult(
                        success: false,
                        message: "Scheduled comics must have a date and/or time"));
                }

                if (isScheduled)
                {
                    // Make sure hidden if scheduled
                    isHidden = true;
                }

                var comicNumberChange = comic.ComicNumber != comicNumber ? $"{comic.ComicNumber} -> {comicNumber}" : "(Unchanged)";
                var comicTitleChange = comic.ComicTitle != comicTitle ? $"{comic.ComicTitle} -> {comicTitle}" : "(Unchanged)";
                var comicDescriptionChange = comic.ComicDescription != comicDescription ? $"{comic.ComicDescription} -> {comicDescription}" : "(Unchanged)";
                var comicTagsChange = comic.Tags != tags ? $"{comic.Tags} -> {tags}" : "(Unchanged)";
                var comicHiddenChange = comic.IsHidden != isHidden 
                    ? $"{FormattingService.GetBooleanAsYesNoString(comic.IsHidden)} -> {FormattingService.GetBooleanAsYesNoString(isHidden)}"
                    : "(Unchanged)";
                var comicScheduledChange = comic.ScheduledDate != scheduleFor 
                    ? $"{(comic.ScheduledDate.HasValue ? FormattingService.GetDateTimeAsString(comic.ScheduledDate.Value) : "N/A")} -> {(scheduleFor.HasValue ? FormattingService.GetDateTimeAsString(scheduleFor.Value) : "N/A")}"
                    : "(Unchanged)";

                comic.LastModifiedDate = DateTime.UtcNow;
                comic.ComicNumber = comicNumber;
                comic.ComicTitle = comicTitle;
                comic.ComicDescription = comicDescription;
                if (imageData != null )
                {
                    comic.ImageData = FormattingService.GetIFormFileAsBytes(imageData);
                }
                comic.Tags = tags;
                comic.IsHidden = isHidden;
                comic.LastEditedBy = currentUser;

                if (!isScheduled && comic.ScheduledDate.HasValue)
                {
                    // Scheduled date was removed
                    comic.PublishDate = DateTime.UtcNow;
                    comic.ScheduledDate = null;
                }
                else if (isScheduled && scheduleFor.HasValue)
                {
                    // Scheduled date was added
                    comic.PublishDate = null;
                    comic.ScheduledDate = scheduleFor.Value;
                }

                await _hooContext.Events.AddAsync(new Event(
                    eventType: EventTypes.ComicUpdated,
                    details: $"Comic GUID {comic.Id} was updated by {currentUser.GetEventLogString()}:" +
                        "\n---" +
                        $"\nComic number: {comicNumberChange}" +
                        $"\nTitle: {comicTitleChange}" +
                        $"\nDescription: {comicDescriptionChange}" +
                        $"\nTags: {comicTagsChange}" +
                        $"\nHidden: {comicHiddenChange}" +
                        $"\nScheduled date: {comicScheduledChange}"));
                await _hooContext.SaveChangesAsync();

                return new JsonResult(new BaseResult(success: true));
            }
            catch (Exception exception)
            {
                await _hooContext.Events.AddAsync(new Event(
                    eventType: EventTypes.Error,
                    details: $"Failed to update comic GUID {comicGuid}: {exception.Message}",
                    stackTrace: JsonConvert.SerializeObject(value: exception.StackTrace, formatting: Formatting.Indented)));
                await _hooContext.SaveChangesAsync();
                return new JsonResult(new BaseResult(success: false));
            }
        }

        /// <summary>
        /// Attempts to update the current user, returning a <see cref="JsonResult"/> representing the result of the request.
        /// </summary>
        /// <param name="handle">The handle of the user to set.</param>
        /// <param name="socialLink">The preferred social media URL of the user to set.</param>
        /// <param name="imageData">Optional replacement profile picture of the user to set.</param>
        /// <returns><see cref="JsonResult"/> representing the result of the request.</returns>
        public async Task<JsonResult> OnPatchMeAsync(
            string handle, 
            string socialLink,
            IFormFile? imageData = null)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            try
            {
                if (currentUser == null)
                {
                    return new JsonResult(new BaseResult(
                        success: false,
                        message: $"You are not authorised to perform this action"));
                }

                if ((currentUser.DisplayPicture == null || currentUser.DisplayPicture.Length == 0) && imageData == null)
                {
                    return new JsonResult(new BaseResult(
                        success: false,
                        message: "A profile picture is required"));
                }

                if (imageData != null && !_comicUploadFileExtensions.Contains(imageData.FileName.Split(".").Last()))
                {
                    return new JsonResult(new BaseResult(
                        success: false,
                        message: $"Profile pictures must be uploaded in jpg, png or gif format"));
                }

                var handleChange = currentUser.Handle != handle ? $"{currentUser.Handle} -> {handle}" : "(Unchanged)";
                var socialChange = currentUser.SocialLink != socialLink ? $"{currentUser.SocialLink} -> {socialLink}" : "(Unchanged)";
                var profilePictureChange = "(Unchanged)";

                currentUser.Handle = handle;
                currentUser.SocialLink = socialLink;
                if (imageData != null)
                {
                    profilePictureChange = $"Current -> {imageData.FileName}";
                    currentUser.DisplayPicture = FormattingService.GetIFormFileAsBytes(imageData);
                }

                await _hooContext.Events.AddAsync(new Event(
                    eventType: EventTypes.UserUpdated,
                    details: $"{currentUser.GetEventLogString()} updated their profile:" +
                        "\n---" +
                        $"\nHandle: {handleChange}" +
                        $"\nSocial media URL: {socialChange}" +
                        $"\nProfile picture: {profilePictureChange}"));
                await _hooContext.SaveChangesAsync();

                return new JsonResult(new BaseResult(success: true));
            }
            catch (Exception exception)
            {
                await _hooContext.Events.AddAsync(new Event(
                    eventType: Enums.EventTypes.Error,
                    details: $"Failed to update profile for {currentUser.GetEventLogString()}: {exception.Message}",
                    stackTrace: JsonConvert.SerializeObject(value: exception.StackTrace, formatting: Formatting.Indented)));
                await _hooContext.SaveChangesAsync();
                return new JsonResult(new BaseResult(success: false));
            }
        }

        /// <summary>
        /// Attempts to fetch a list of all events for viewing in the management menu, returning a <see cref="JsonResult"/> representing the result of the request.
        /// </summary>
        /// <returns><see cref="JsonResult"/> representing the result of the request</returns>
        public async Task<JsonResult> OnGetEventsAsync()
        {
            try
            {
                var events = _hooContext.Events
                    .AsNoTracking()
                    .OrderByDescending(eventItem => eventItem.CreatedDate)
                    .Take(10000)
                    .AsEnumerable();

                return new JsonResult(new EventsResult(
                    success: true, 
                    eventData: events.Select(eventItem => new EventData(eventItem)).ToList()));
            }
            catch (Exception exception)
            {
                await _hooContext.Events.AddAsync(new Event(
                    eventType: EventTypes.Error,
                    details: $"Failed to fetch log entries: {exception.Message}",
                    stackTrace: JsonConvert.SerializeObject(value: exception.StackTrace, formatting: Formatting.Indented)));
                await _hooContext.SaveChangesAsync();
                return new JsonResult(new BaseResult(success: false));
            }
        }
    }
}