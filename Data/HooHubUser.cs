using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace hoohub.Data
{
    public class HooHubUser : IdentityUser
    {
        /// <summary>
        /// Concurrency token.
        /// </summary>
        [Timestamp]
        public byte[] Version { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Guid { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsDisabled { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime LastLoginDate { get; set; } = new DateTime(1900, 1, 1);

        /// <summary>
        /// 
        /// </summary>
        public string Handle { get; set; }

        /// <summary>
        /// Whether this user has logged in before and set their password.
        /// </summary>
        public bool FirstLogin { get; set; } = false;

        public HooHubUser()
        {
        }

        public HooHubUser(string handle)
        {
            Guid = System.Guid.NewGuid().ToString();
            Handle = handle;
        }
    }
}
