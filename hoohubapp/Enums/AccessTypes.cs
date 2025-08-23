using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace hoohub.Enums
{
    /// <summary>
    /// Access level types for users, from least access/priveleges to most.
    /// </summary>
    public enum AccessTypes
    {
        [Description("No users")]
        [Display(Name = "No users")] // Nobody, not even administrators
        None,

        [Description("All users")]
        [Display(Name = "All users")] // Anybody
        AllUsers,

        [Description("Registered and authorised users only")]
        [Display(Name = "Registered and authorised users only")] // Only registered accounts and above (NOT IN USE)
        RegisteredUsers,

        [Description("Authorised users only")]
        [Display(Name = "Authorised users only")] // Only authorised (admin) accounts
        AuthorisedUsers
    }
}