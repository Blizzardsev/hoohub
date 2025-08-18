using hoohub.Data;

namespace hoohub.Requests.Results
{
    /// <summary>
    /// Represents the result of an attempt to fetch the current user data for profile management/changes.
    /// </summary>
    public class ManageMeResult
    {
        /// <summary>
        /// The handle of the current user for modification.
        /// </summary>
        public string Handle { get; set; }

        /// <summary>
        /// The image data of the current user's profile picture for modification.
        /// </summary>
        public byte[] ImageData { get; set; }

        /// <summary>
        /// Initialises a new instance of the <see cref="ManageMeResult"/> class.
        /// </summary>
        /// <param name="user">Base <see cref="HooHubUser"/> to derive attributes from.</param>
        public ManageMeResult(HooHubUser user) 
        {
            Handle = user.Handle;
            ImageData = user.DisplayPicture;
        }
    }
}
