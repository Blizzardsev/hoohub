namespace hoohub.Data
{
    public class Comic
    {
        /// <summary>
        /// 
        /// </summary>
        public string Id { get; init; }

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
        public DateTime PublishDate { get; init; }

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

        public Comic()
        {
        }

        public Comic(string comicTitle, string comicNumber, string comicDescription, byte[] imageData, List<string> tags, bool isHidden)
        {
            Id = Guid.NewGuid().ToString();
            ComicNumber = comicNumber;
            ComicTitle = comicTitle;
            ComicDescription = comicDescription;
            PublishDate = DateTime.Now;
            ImageData = imageData;
            Tags = string.Join(",", tags);
            IsHidden = isHidden;
        }

        public string GetComicDisplayName() => $"{ComicNumber} | {ComicTitle}";

        public string GetImageAsBase64String() => Convert.ToBase64String(ImageData);
    }
}
