using System.ComponentModel.DataAnnotations;

namespace hoohub.Data
{
    public class Comic
    {
        [Timestamp]
        public byte[] Version { get; set; }
        public string Guid { get; init; }
        public string ComicNumber { get; init; }
        public string ComicTitle { get; init; }
        public string ComicDescription { get; init; }
        public DateTime PublishDate { get; init; }
        public byte[] ImageData { get; init; }
        public string Tags { get; init; }
        public bool IsHidden { get; init; }

        public Comic()
        {
        }

        public Comic(string comicTitle, string comicNumber, string comicDescription, byte[] imageData, List<string> tags, bool isHidden)
        {
            Guid = System.Guid.NewGuid().ToString();
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
