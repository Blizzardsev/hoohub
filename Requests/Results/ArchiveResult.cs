using hoohub.Requests.Data;

namespace hoohub.Requests.Results
{
    /// <summary>
    /// Represents the result of an attempt to fetch a list of archived comics.
    /// </summary>
    public class ArchiveResult : BaseResult
    {
        /// <summary>
        /// Whether or not this slice is the end of the results.
        /// </summary>
        public bool EndOfResults {  get; set; }

        /// <summary>
        /// The (capped) list of archived comics for presentation.
        /// </summary>
        public List<ArchiveComicData> ArchiveComicData { get; set; }

        /// <summary>
        /// Initialises a new instance of the <see cref="ArchiveResult"/> class.
        /// </summary>
        /// <param name="success">The success state to set.</param>
        /// <param name="endOfResults">The end of results state to set.</param>
        /// <param name="archiveComicData">The list of <see cref="ArchiveComicData"/> to set.</param>
        /// <param name="message">Optional message to set.</param>
        public ArchiveResult(bool success, bool endOfResults, List<ArchiveComicData> archiveComicData = null, string message = "") : base(success, message)
        {
            EndOfResults = endOfResults;
            ArchiveComicData = archiveComicData;
        }
    }
}
