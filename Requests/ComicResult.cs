using hoohub.Data;

namespace hoohub.Requests
{
    public class ComicResult : BaseResult
	{
		public string DisplayName { get; init; }
		public string DisplayPublishDate { get; init; }
		public string Description { get; init; }
		public byte[] ImageData { get; init; }

		public ComicResult(bool success, Comic comic, string message = "") : base(success, message)
		{
			Success = success;
			DisplayName = comic.GetComicDisplayName();
			DisplayPublishDate = comic.PublishDate.ToString("dddd, dd | MM | yyyy");
			Description = comic.ComicDescription;
			ImageData = comic.ImageData;
			Message = message;
		}
	}
}
