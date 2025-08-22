using hoohub.Requests.Data;

namespace hoohub.Requests.Results
{
    /// <summary>
    /// Represents the result of an attempt to fetch the list of comics for management.
    /// </summary>
    public class ManageComicListResult : BaseResult
    {
        /// <summary>
        /// The list of comics for management.
        /// </summary>
        public List<ManageComicListData> ManageComicListData { get; init; } 

        /// <summary>
        /// Initialises a new instance of the <see cref="ManageComicListResult"/> class.
        /// </summary>
        /// <param name="success">The success state to set.</param>
        /// <param name="manageComicListData">The list of <see cref="ManageComicListData"/> items to set.</param>
        /// <param name="message">Optional message to set.</param>
        public ManageComicListResult(bool success, List<ManageComicListData> manageComicListData, string message = "") : base(success, message)
        {
            Success = success;
            ManageComicListData = manageComicListData;
            Message = message;
        }
    }
}