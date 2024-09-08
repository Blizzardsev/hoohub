namespace hoohub.Configuration
{
    /// <summary>
    /// Binding class for SMTP configuration.
    /// </summary>
    public sealed class SmtpSettings
    {
        /// <summary>
        /// The host address to use for SMTP activity.
        /// </summary>
        public string Host { get; set; } = string.Empty;

        /// <summary>
        /// The network port to use for SMTP activity.
        /// </summary>
        public int Port { get; set; } = -1;

        /// <summary>
        /// The sender name to use for SMTP activity.
        /// </summary>
        public string SenderName { get; set; } = string.Empty;

        /// <summary>
        /// The sender address to use for SMTP activity.
        /// </summary>
        public string SenderAddress { get; set; } = string.Empty;

        /// <summary>
        /// The username to use as part of the SMTP credentials.z
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// The password to use as part of the SMTP credentials.
        /// </summary>
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// The domain to use as part of the SMTP credentials.
        /// </summary>c
        public string Domain { get; set; } = string.Empty;

        /// <summary>
        /// Whether to use SSL for the connection to the target SMTP server.
        /// </summary>
        public bool EnableSsl { get; set; } = true;

        /// <summary>
        /// Whether to send emails or not globally. This will prevent 2FA logins, so an account must have 2FA disabled to login if this is enabled.
        /// </summary>
        public bool EnableEmails { get; set; } = true;
    }
}