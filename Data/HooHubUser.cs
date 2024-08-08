using Microsoft.AspNetCore.Identity;
using System.Drawing;

namespace hoohub.Data
{
    public class HooHubUser : IdentityUser
    {
        /// <summary>
        /// 
        /// </summary>
        public string Guid { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsDisabled { get; set; } = false;

        /// <summary>
        /// 
        /// </summary>
        public DateTime LastLoginDate { get; set; } = new DateTime(1900, 1, 1);

        /// <summary>
        /// 
        /// </summary>
        public string Handle { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        public string LastLoginIpAddress { get; set; } = string.Empty;
        
        /// <summary>
        /// 
        /// </summary>
        public bool FirstLogin { get; set; } = true;

        /// <summary>
        /// 
        /// </summary>
        public byte[] DisplayPicture { get; set; } = (byte[])new ImageConverter().ConvertTo(Properties.Resources.default_pfp, typeof(byte[]));

        public HooHubUser()
        {
        }

        public HooHubUser(string handle)
        {
            Guid = System.Guid.NewGuid().ToString();
            Handle = handle;
        }

        /// <summary>
        /// Returns the handle and ID of this user in the format <see cref="Handle"/> (Guid: <see cref="Guid"/>).
        /// </summary>
        /// <returns>The handle and ID of this user in the format <see cref="Handle"/> (Guid: <see cref="Guid"/>).</returns>
        public string GetEventLogString() => $"{Handle} (Guid: {Id})";
    }
}
