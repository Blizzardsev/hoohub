using hoohub.Data;
using hoohub.Migrations;

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
        /// The comic alt description for display to the user.
        /// </summary>
        public string ComicAltDescription { get; init; }

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
        public DateTime? DisplayPublished { get; init; }

        /// <summary>
        /// The date the comic was last modified, and who by.
        /// </summary>
        public DateTime? DisplayLastModified { get; init; }

        /// <summary>
        /// The handle of the user who last modified the  comic.
        /// </summary>
        public string LastModifiedHandle { get; init; }

        /// <summary>
        /// The amount of <see cref="ComicLike"/> items associated with this comic.
        /// </summary>
        public int LikeCount { get; init; } = 0;

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
            ComicAltDescription = comic.ComicAltDescription;
            ImageData = comic.ImageData;
            Tags = comic.Tags;
            IsHidden = comic.IsHidden;
            ScheduledDate = comic.ScheduledDate;
            PublishDate = comic.PublishDate;
            DisplayPublished = comic.PublishDate.HasValue ? PublishDate : null;
            DisplayLastModified = comic.LastModifiedDate;
            LastModifiedHandle = comic.LastEditedBy == null ? "Unknown" : comic.LastEditedBy.Handle;
            Message = message;
            LikeCount = comic.ComicLikes.Count;
        }
    }
}
