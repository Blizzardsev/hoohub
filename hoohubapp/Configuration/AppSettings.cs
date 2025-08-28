namespace hoohub.Configuration
{
    /// <summary>
    /// Application-wide configuration.
    /// Update via appsettings for appropriate environment.
    /// </summary>
    public sealed class AppSettings
    {
        /// <summary>
        /// The list of IP addresses that are considered trusted locations, requiring no 2FA.
        /// </summary>
        public string[] TrustedLocations { get; set; } = [];

        /// <summary>
        /// The list of user agents that should be blocked from accessing main site content.<br/>
        /// Any user agent containing this text will be served a dummy page.
        /// </summary>
        public string[] BlockedUserAgents { get; set; } = [];

        /// <summary>
        /// The email address of the artist to assign credit for.
        /// </summary>
        public string ArtistCredit { get; set; } = string.Empty;

        /// <summary>
        /// The email address of the writer to assign credit for.
        /// </summary>
        public string WriterCredit { get; set; } = string.Empty;

        /// <summary>
        /// The social media link of the artist to assign credit for.
        /// </summary>
        public string ArtistSocial { get; set; } = string.Empty;

        /// <summary>
        /// The social media link of the artist to assign credit for.
        /// </summary>
        public string WriterSocial { get; set; } = string.Empty;

        /// <summary>
        /// The base site URL to use in situations where constructing the URL from a request/context is not available.
        /// </summary>
        public string SiteBaseUrl { get; set; } = string.Empty;

        /// <summary>
        /// The email address of the user who is considered to be the site administrator.<br/>
        /// The administrator cannot be managed by other users.
        /// </summary>
        public string SiteAdmin { get; set; } = string.Empty;

        /// <summary>
        /// Basic blacklist; not persisted on app restart.
        /// </summary>
        public List<string> BlockedIpAddressRange = [
            "::ffff:10.244.5.106"
        ];
    }
}