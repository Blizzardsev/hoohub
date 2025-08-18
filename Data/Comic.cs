namespace hoohub.Data
{
    public class Comic
    {
        /// <summary>
        /// Uniquely identifies this comic.
        /// </summary>
        public string Id { get; init; } = Guid.NewGuid().ToString();

        /// <summary>
        /// The number of the comic for presentation in combination with the title; this also determines how the comic is ordered.<br/>
        /// Comic numbers are always given in a three digit format with leading zeroes.
        /// </summary>
        public string ComicNumber { get; set; } = string.Empty;

        /// <summary>
        /// The title of the comic for presentation in combination with the number.
        /// </summary>
        public string ComicTitle { get; set; } = string.Empty;

        /// <summary>
        /// Supplementary comic description.
        /// </summary>
        public string ComicDescription { get; set; } = string.Empty;

        /// <summary>
        /// The date the comic was published.
        /// </summary>
        public DateTime PublishDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// The actual image file data associated with the comic.
        /// </summary>
        public byte[] ImageData { get; set; } = new byte[0];

        /// <summary>
        /// Comma-separated tag list associated with the comic. This should be a maximum of three tags for consistency with the comic number.
        /// </summary>
        public string Tags { get; set; } = string.Empty;

        /// <summary>
        /// Whether the comic should be considered hidden from presentation and not available to users.
        /// </summary>
        public bool IsHidden { get; set; } = false;

        /// <summary>
        /// The scheduled date for this comic to be published, if any. Upon reaching the scheduled date, the comic is unhidden.
        /// </summary>
        public DateTime? ScheduledDate { get; set; } = null;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Comic()
        {
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="Comic"/>.
        /// </summary>
        /// <param name="comicTitle">The comic title to set.</param>
        /// <param name="comicNumber">The comic number to set.</param>
        /// <param name="comicDescription">The comic description to set.</param>
        /// <param name="imageData">The comic image data to set.</param>
        /// <param name="tags">The comic tags to set.</param>
        /// <param name="isHidden">The hidden state to set.</param>
        /// <param name="scheduledDate">The optional scheduled date to set.</param>
        public Comic(
            string comicTitle, 
            string comicNumber, 
            string comicDescription, 
            byte[] imageData, 
            string tags,
            bool isHidden,
            DateTime? scheduledDate = null)
        {
            ComicNumber = comicNumber;
            ComicTitle = comicTitle;
            ComicDescription = comicDescription;
            if (scheduledDate == null)
            {
                PublishDate = DateTime.UtcNow;
            }
            ImageData = imageData;
            Tags = string.Join(",", tags);
            IsHidden = isHidden;
            ScheduledDate = scheduledDate;
        }

        /// <summary>
        /// Returns the formatted name to use when presenting the comic for titles consisting of both the <see cref="ComicNumber"/> and <see cref="ComicTitle"/>, etc - E.G 001 | MyComic
        /// </summary>
        /// <returns>The formatted name to use when presenting the comic.</returns>
        public string GetComicDisplayName() => $"{ComicNumber} | {ComicTitle}";

        /// <summary>
        /// Returns the <see cref="ImageData"/> of the comic as a Base64 string for presentation in image elements or for downloads.
        /// </summary>
        /// <returns>The <see cref="ImageData"/> of the comic as a Base64 string.</returns>
        public string GetImageAsBase64String() => Convert.ToBase64String(ImageData);

        /// <summary>
        /// Returns whether the comic tags contain the given comma-separated search terms.<br/>
        /// If one or more tags match the search terms, returns true. Otherwise, returns false.
        /// </summary>
        /// <param name="searchTerms">The comma-seperated search terms to check the tags for.</param>
        /// <returns>True if one or more tags match the search terms, otherwise false.</returns>
        public bool GetTagsContainsTerms(string searchTerms)
        {
            if (string.IsNullOrWhiteSpace(Tags))
            {
                return false;
            }

            foreach (var tag in Tags.Split(','))
            {
                if (searchTerms.Contains(value: tag, comparisonType: StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns whether the <see cref="ComicTitle"/> contains the given search term.
        /// </summary>
        /// <param name="searchTerms">The search term to check the name for.</param>
        /// <returns>True if the <see cref="ComicTitle"/> contains the search term, otherwise false.</returns>
        public bool GetComicTitleContainsTerms(string searchTerms) => ComicTitle.Contains(value: searchTerms, comparisonType: StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// Returns whether the <see cref="ComicNumber"/> contains the given search term.
        /// </summary>
        /// <param name="searchTerms">The search term to check the number for.</param>
        /// <returns>True if the <see cref="ComicNumber"/> contains the search term, otherwise false.</returns>
        public bool GetComicNumberContainsTerms(string searchTerms) => ComicNumber.Contains(value: searchTerms, comparisonType: StringComparison.OrdinalIgnoreCase);
    }
}
