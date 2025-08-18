using hoohub.Data;
using System.Web;

namespace hoohub.Requests.Data
{
    /// <summary>
    /// Represents requested simple comic data to display to the user when fetching the list of existing comics for management.
    /// </summary>
    public class ManageComicListData
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
        /// Initialises a new instance of the <see cref="ManageComicListData"/> class.
        /// </summary>
        /// <param name="comic">Base <see cref="Comic"/> to derive attributes from.</param>
        public ManageComicListData(Comic comic)
        {
            Guid = comic.Id;
            DisplayName = HttpUtility.HtmlEncode(comic.GetComicDisplayName());
        }
    }
}