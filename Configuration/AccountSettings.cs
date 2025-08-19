namespace hoohub.Configuration
{
    /// <summary>
    /// Defines the basic settings for an account to be created from the appsettings file.
    /// </summary>
    public class AccountSettings
    {
        /// <summary>
        /// The email address to use for the new account.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// The handle (display name) to use for the new account.
        /// </summary>
        public string Handle { get; set; } = string.Empty;

        /// <summary>
        /// The preferred social media link to use for the new account.
        /// </summary>
        public string SocialLink { get; set; } = string.Empty;
    }
}