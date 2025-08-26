using hoohub.Requests.Data;

namespace hoohub.Requests.Results
{
    /// <summary>
    /// Represents the result of an attempt to fetch the list of users for management.
    /// </summary>
    public class ManageUsersListResult : BaseResult
    {
        /// <summary>
        /// The list of users for management.
        /// </summary>
        public List<ManageUsersListData> ManageUsersListData { get; init; }

        /// <summary>
        /// Initialises a new instance of the <see cref="ManageUsersListResult"/> class.
        /// </summary>
        /// <param name="success">The success state to set.</param>
        /// <param name="manageUsersListData">The list of <see cref="ManageUsersListData"/> items to set.</param>
        /// <param name="message">Optional message to set.</param>
        public ManageUsersListResult(bool success, List<ManageUsersListData> manageUsersListData, string message = "") : base(success, message)
        {
            Success = success;
            ManageUsersListData = manageUsersListData;
            Message = message;
        }
    }
}