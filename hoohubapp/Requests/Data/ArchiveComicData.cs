using hoohub.Data;
using System.Web;

namespace hoohub.Requests.Data
{
    /// <summary>
    /// Represents requested archived comic information for display to the user.
    /// </summary>
    public class ArchiveComicData
    {
        /// <summary>
        /// Uniquely identifies the comic.
        /// </summary>
        public string Guid { get; set; }

        /// <summary>
        /// The name of the comic to display.
        /// </summary>
        public string DisplayName { get; init; }

        /// <summary>
        /// The published date of the comic to display.
        /// </summary>
        public DateTime? DisplayPublishDate { get; init; }

        /// <summary>
        /// The tags of the comic to display.
        /// </summary>
        public string DisplayTags { get; init; }

        /// <summary>
        /// The description of the comic to display.
        /// </summary>
        public string Description { get; init; }

        /// <summary>
        /// The alt description of the comic to display for accessibility.
        /// </summary>
        public string AltDescription { get; init; }

        /// <summary>
        /// The image data of the comic to render.
        /// </summary>
        public byte[] ImageData { get; init; }

        /// <summary>
        /// Initialises a new instance of the <see cref="ArchiveComicData"/> class.
        /// </summary>
        /// <param name="comic">Base <see cref="Comic"/> to derive attributes from.</param>
        public ArchiveComicData(Comic comic) 
        {
            Guid = comic.Id;
            DisplayName = HttpUtility.HtmlEncode(comic.GetComicDisplayName());
            DisplayPublishDate = comic.PublishDate.HasValue ? comic.PublishDate.Value : null;
            DisplayTags = comic.GetComicDisplayTags();
            Description = comic.GetComicDisplayDescription();
            AltDescription = comic.GetComicDisplayAltDescription();
            ImageData = comic.ImageData;
        }
    }
}