using hoohub.Data;
using System.Web;

namespace hoohub.Requests.Data
{
    /// <summary>
    /// 
    /// </summary>
    public class ArchiveComicData
    {
        /// <summary>
        /// 
        /// </summary>
        public string Guid { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string DisplayName { get; init; }

        /// <summary>
        /// 
        /// </summary>
        public string DisplayPublishDate { get; init; }

        /// <summary>
        /// 
        /// </summary>
        public string DisplayTags { get; init; }

        /// <summary>
        /// 
        /// </summary>
        public string Description { get; init; }

        /// <summary>
        /// 
        /// </summary>
        public byte[] ImageData { get; init; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="comic"></param>
        public ArchiveComicData(Comic comic) 
        {
            Guid = comic.Id;
            DisplayName = HttpUtility.HtmlEncode(comic.GetComicDisplayName());
            DisplayPublishDate = comic.PublishDate.ToLocalTime().ToString("dddd, dd | MM | yyyy");
            DisplayTags = string.IsNullOrWhiteSpace(comic.Tags)
                ? "(No tags)"
                : HttpUtility.HtmlEncode(comic.Tags.Replace(",", ", "));
            Description = string.IsNullOrWhiteSpace(comic.ComicDescription)
                ? "(No description)"
                : HttpUtility.HtmlEncode(comic.ComicDescription);
            ImageData = comic.ImageData;
        }
    }
}