using hoohub.Requests.Data;

namespace hoohub.Requests.Results
{
    /// <summary>
    /// Represents the result of an attempt to fetch the list of comics for management.
    /// </summary>
    public class ManageComicsListResult : BaseResult
    {
        /// <summary>
        /// The list of comics for management.
        /// </summary>
        public List<ManageComicsListData> ManageComicsListData { get; init; }

        /// <summary>
        /// Initialises a new instance of the <see cref="ManageComicsListResult"/> class.
        /// </summary>
        /// <param name="success">The success state to set.</param>
        /// <param name="manageComicsListData">The list of <see cref="Data.ManageComicsListData"/> items to set.</param>
        /// <param name="message">Optional message to set.</param>
        public ManageComicsListResult(bool success, List<ManageComicsListData> manageComicsListData, string message = "") : base(success, message)
        {
            Success = success;
            ManageComicsListData = manageComicsListData;
            Message = message;
        }
    }
}