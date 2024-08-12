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
    }
}