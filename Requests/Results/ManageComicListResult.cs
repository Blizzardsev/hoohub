using hoohub.Requests.Data;

namespace hoohub.Requests.Results
{
    /// <summary>
    /// 
    /// </summary>
    public class ManageComicListResult : BaseResult
    {
        /// <summary>
        /// 
        /// </summary>
        public List<ManageComicListData> ManageComicListData { get; init; } 

        /// <summary>
        /// 
        /// </summary>
        /// <param name="success"></param>
        /// <param name="manageComicListData"></param>
        /// <param name="message"></param>
        public ManageComicListResult(bool success, List<ManageComicListData> manageComicListData, string message = "") : base(success, message)
        {
            Success = success;
            ManageComicListData = manageComicListData;
            Message = message;
        }
    }
}
