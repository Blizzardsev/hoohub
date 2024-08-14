using hoohub.Data;

namespace hoohub.Requests.Data
{
    public class ArchiveComicData
    {
        public string Guid { get; set; }
        public string DisplayName { get; init; }
        public string DisplayPublishDate { get; init; }
        public string Tags { get; init; }
        public string Description { get; init; }
        public byte[] ImageData { get; init; }

        public ArchiveComicData(Comic comic) 
        {
            Guid = comic.Id;
            DisplayName = comic.GetComicDisplayName();
            DisplayPublishDate = comic.PublishDate.ToString("dddd, dd | MM | yyyy");
            Tags = comic.Tags;
            Description = comic.ComicDescription;
            ImageData = comic.ImageData;
        }
    }
}