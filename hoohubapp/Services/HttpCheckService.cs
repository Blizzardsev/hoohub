using hoohub.Configuration;

namespace hoohub.Services
{
    /// <summary>
    /// Service for checking HTTP requests for authenticity.
    /// </summary>
    public class HttpCheckService
    {
        /// <summary>
        /// Evaluates the supplied request user agent and IP address against the current configuration and block list to determine whether both are valid.
        /// </summary>
        /// <param name="appSettings"><see cref="AppSettings"/> instance to reference configuration from.</param>
        /// <param name="request"><see cref="HttpRequest"/> from which to check the user agent and IP address.</param>
        /// <returns>True if the user agent and IP address are acceptable, otherwise false.</returns>
        public static bool IsValidUserAgentAndIpAddress(AppSettings appSettings, HttpRequest request)
        {
            var userAgent = request.Headers["User-Agent"].ToString().ToLower();
            var ipAddress = request.HttpContext.Connection.RemoteIpAddress.ToString();

            // User agent checks - split so we can debug
            if (string.IsNullOrWhiteSpace(userAgent))
            {
                return false;
            }
            if (appSettings.BlockedUserAgents.Any(blockedUserAgent => blockedUserAgent.ToLower().Contains(userAgent)))
            {
                return false;
            }
            if (userAgent.Length < 40)
            {
                return false;
            }
            if (userAgent.Length >= 195)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(ipAddress) || appSettings.BlockedIpAddressRange.Any(blockedIpAddress => blockedIpAddress.IpAddress == ipAddress))
            {
                return false;
            }

            return true;
        }
    }
}