using hoohub.Requests.Data;

namespace hoohub.Requests.Results
{
    public class ArchiveResult : BaseResult
    {
        public List<ArchiveComicData> ArchiveComicData { get; set; }

        public ArchiveResult(bool success, List<ArchiveComicData> archiveComicData, string message = "") : base(success, message)
        {
            ArchiveComicData = archiveComicData;
        }
    }
}
