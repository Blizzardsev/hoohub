using hoohub.Data;

namespace hoohub.Requests.Results
{
    /// <summary>
    /// 
    /// </summary>
    public class ManageComicDetailsResult : BaseResult
    {
        /// <summary>
        /// 
        /// </summary>
        public string Guid { get; init; }

        /// <summary>
        /// 
        /// </summary>
        public string ComicNumber { get; init; }

        /// <summary>
        /// 
        /// </summary>
        public string ComicTitle { get; init; }

        /// <summary>
        /// 
        /// </summary>
        public string ComicDescription { get; init; }

        /// <summary>
        /// 
        /// </summary>
        public byte[] ImageData { get; init; }

        /// <summary>
        /// 
        /// </summary>
        public string Tags { get; init; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsHidden { get; init; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime? ScheduledDate { get; init; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime? PublishDate { get; init; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="success"></param>
        /// <param name="comic"></param>
        /// <param name="message"></param>
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
            Message = message;
        }
    }
}
