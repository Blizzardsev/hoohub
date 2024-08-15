using System.Linq;

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

        /// <summary>
        /// 
        /// </summary>
        public Comic()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="comicTitle"></param>
        /// <param name="comicNumber"></param>
        /// <param name="comicDescription"></param>
        /// <param name="imageData"></param>
        /// <param name="tags"></param>
        /// <param name="isHidden"></param>
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetComicDisplayName() => $"{ComicNumber} | {ComicTitle}";

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetImageAsBase64String() => Convert.ToBase64String(ImageData);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchTerms"></param>
        /// <returns></returns>
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
        /// 
        /// </summary>
        /// <param name="searchTerms"></param>
        /// <returns></returns>
        public bool GetComicNameContainsTerms(string searchTerms) => ComicTitle.Contains(value: searchTerms, comparisonType: StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchTerms"></param>
        /// <returns></returns>
        public bool GetComicNumberContainsTerms(string searchTerms) => ComicNumber.Contains(value: searchTerms, comparisonType: StringComparison.OrdinalIgnoreCase);
    }
}
