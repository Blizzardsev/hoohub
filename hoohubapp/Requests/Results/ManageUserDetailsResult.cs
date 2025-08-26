using hoohub.Data;

namespace hoohub.Requests.Results
{
    /// <summary>
    /// Represents the result of an attempt to fetch the complete set of details of a user for management.
    /// </summary>
    public class ManageUserDetailsResult : BaseResult
    {
        /// <summary>
        /// Uniquely identifies the user.
        /// </summary>
        public string Guid { get; init; }

        /// <summary>
        /// The user's currently chosen handle.
        /// </summary>
        public string Handle { get; init; }

        /// <summary>
        /// The user's currently chosen social media URL.
        /// </summary>
        public string SocialLink { get; init; }

        /// <summary>
        /// The locked state of the user.
        /// </summary>
        public bool AccountIsLocked { get; init; }

        /// <summary>
        /// The disabled state of the user.
        /// </summary>
        public bool AccountIsDisabled { get; init; }

        /// <summary>
        /// The date the user last logged in, if it exists.
        /// </summary>
        public DateTime? LastLoginDate { get; init; }

        /// <summary>
        /// The address the user last logged in from.
        /// </summary>
        public string LastLoginIpAddress { get; init; }

        /// <summary>
        /// The user display picture to render.
        /// </summary>
        public byte[] DisplayPictureData { get; init; }

        /// <summary>
        /// Initialises a new instance of the <see cref="ManageUserDetailsResult"/> class.
        /// </summary>
        /// <param name="success">The success state to set.</param>
        /// <param name="user">Base <see cref="HooHubUser"/> to derive attributes from.</param>
        /// <param name="message">Optional message to set.</param>
        public ManageUserDetailsResult(bool success, HooHubUser user, string message = "") : base(success, message)
        {
            Success = success;
            Guid = user.Id;
            Handle = user.Handle;
            SocialLink = user.SocialLink;
            AccountIsLocked = user.LockoutEnd > DateTime.UtcNow;
            AccountIsDisabled = user.IsDisabled;
            LastLoginDate = user.FirstLogin ? null : user.LastLoginDate;
            LastLoginIpAddress = string.IsNullOrWhiteSpace(user.LastLoginIpAddress) ? "N/A" : user.LastLoginIpAddress;
            DisplayPictureData = user.DisplayPicture;
            Message = message;
        }
    }
}