using Microsoft.AspNetCore.Identity;
using System.Drawing;

namespace hoohub.Data
{
    /// <summary>
    /// Represents an authorised HooHub user.
    /// </summary>
    public class HooHubUser : IdentityUser
    {
        /// <summary>
        /// Whether the user account is considered disabled; disabled accounts cannot log in.
        /// </summary>
        public bool IsDisabled { get; set; } = false;

        /// <summary>
        /// The date and time the user last logged in.
        /// </summary>
        public DateTime LastLoginDate { get; set; } = DateTime.UtcNow.AddYears(-99);

        /// <summary>
        /// The preferred handle/display name of the user.
        /// </summary>
        public string Handle { get; set; } = string.Empty;

        /// <summary>
        /// The IP address from where the user last logged in.
        /// </summary>
        public string LastLoginIpAddress { get; set; } = string.Empty;
        
        /// <summary>
        /// Whether this is the first time the user is logging in; a password reset should be forced in this case.
        /// </summary>
        public bool FirstLogin { get; set; } = true;

        /// <summary>
        /// The display picture of the user.
        /// </summary>
        public byte[] DisplayPicture { get; set; } = (byte[])new ImageConverter().ConvertTo(Properties.Resources.hoo_pfp, typeof(byte[]));

        /// <summary>
        /// Default constructor.
        /// Assigns a default profile picture.
        /// </summary>
        public HooHubUser()
        {
            if (DisplayPicture == null || DisplayPicture.Length == 0)
            {
                DisplayPicture = (byte[])new ImageConverter().ConvertTo(Properties.Resources.hoo_pfp, typeof(byte[]));
            }
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="HooHubUser"/> class.
        /// </summary>
        /// <param name="handle">The handle to set.</param>
        public HooHubUser(string handle)
        {
            Id = Guid.NewGuid().ToString();
            Handle = handle;
        }

        /// <summary>
        /// Returns the handle and ID of this user in the format <see cref="Handle"/> (Guid: <see cref="Guid"/>).
        /// </summary>
        /// <returns>The handle and ID of this user in the format <see cref="Handle"/> (Guid: <see cref="Guid"/>).</returns>
        public string GetEventLogString() => $"{Handle} (GUID: {Id})";
    }
}
