using hoohub.Data;
using hoohub.Services;

namespace hoohub.Requests.Results
{
    /// <summary>
    /// Represents the result of an attempt to fetch the complete set of details of a comic for management.
    /// </summary>
    public class ManageComicDetailsResult : BaseResult
    {
        /// <summary>
        /// Uniquely identifies the comic.
        /// </summary>
        public string Guid { get; init; }

        /// <summary>
        /// The comic number for display to the user.
        /// </summary>
        public string ComicNumber { get; init; }

        /// <summary>
        /// The comic title for display to the user.
        /// </summary>
        public string ComicTitle { get; init; }

        /// <summary>
        /// The comic description for display to the user.
        /// </summary>
        public string ComicDescription { get; init; }

        /// <summary>
        /// The comic image to render.
        /// </summary>
        public byte[] ImageData { get; init; }

        /// <summary>
        /// The comic tags for display to the user.
        /// </summary>
        public string Tags { get; init; }

        /// <summary>
        /// The comic's hidden state.
        /// </summary>
        public bool IsHidden { get; init; }

        /// <summary>
        /// The comic's optional scheduled date.
        /// </summary>
        public DateTime? ScheduledDate { get; init; }

        /// <summary>
        /// The comic's optional published date.
        /// </summary>
        public DateTime? PublishDate { get; init; }

        /// <summary>
        /// The date the comic was published, if it exists.
        /// </summary>
        public string DisplayPublished { get; init; }

        /// <summary>
        /// The date the comic was last modified, and who by.
        /// </summary>
        public string DisplayLastModified { get; init; }

        /// <summary>
        /// Initialises a new instance of the <see cref="ManageComicDetailsResult"/> class.
        /// </summary>
        /// <param name="success">The success state to set.</param>
        /// <param name="comic">Base <see cref="Comic"/> to derive attributes from.</param>
        /// <param name="message">Optional message to set.</param>
        public ManageComicDetailsResult(bool success, Comic comic, string message = "") : base(success, message)
        {
            Success = success;
            Guid = comic.Id;
            ComicNumber = comic.ComicNumber;
            ComicTitle = comic.ComicTitle;
            ComicDescription = comic.ComicDescription;
            ImageData = comic.ImageData;
            Tags = comic.Tags;
            IsHidden = comic.IsHidden;
            ScheduledDate = comic.ScheduledDate;
            PublishDate = comic.PublishDate;
            DisplayPublished = comic.PublishDate.HasValue ? FormattingService.GetDateTimeAsString(PublishDate.Value) : "N/A";
            DisplayLastModified = $"{FormattingService.GetDateTimeAsString(comic.LastModifiedDate)}\nby {(comic.LastEditedBy == null ? "Unknown" : comic.LastEditedBy.Handle)}";
            Message = message;
        }
    }
}
