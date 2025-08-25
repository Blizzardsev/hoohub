using hoohub.Data;
using System.Web;

namespace hoohub.Requests.Data
{
    /// <summary>
    /// Represents requested simple user data to display to the user when fetching the list of existing users for management.
    /// </summary>
    public class ManageUsersListData
    {
        /// <summary>
        /// Uniquely identifies the user.
        /// </summary>
        public string Guid { get; set; }

        /// <summary>
        /// The email address of the user to display.
        /// </summary>
        public string DisplayEmail { get; init; }

        /// <summary>
        /// Initialises a new instance of the <see cref="ManageUsersListData"/> class.
        /// </summary>
        /// <param name="user">Base <see cref="HooHubUser"/> to derive attributes from.</param>
        public ManageUsersListData(HooHubUser user)
        {
            Guid = user.Id;
            DisplayEmail = HttpUtility.HtmlEncode(user.Email);
        }
    }
}