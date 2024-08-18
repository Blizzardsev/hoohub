using hoohub.Requests.Data;

namespace hoohub.Requests.Results
{
    public class ArchiveResult : BaseResult
    {
        public bool EndOfResults {  get; set; }

        public List<ArchiveComicData> ArchiveComicData { get; set; }

        public ArchiveResult(bool success, bool endOfResults, List<ArchiveComicData> archiveComicData = null, string message = "") : base(success, message)
        {
            EndOfResults = endOfResults;
            ArchiveComicData = archiveComicData;
        }
    }
}
