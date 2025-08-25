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
        public List<string> TrustedLocations { get; set; } = new List<string>();

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
    }
}