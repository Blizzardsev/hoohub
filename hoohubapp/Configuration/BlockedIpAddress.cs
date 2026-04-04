namespace hoohub.Configuration
{
    public class BlockedIpAddress
    {
        public string IpAddress { get; private set; }
        public DateTimeOffset BlockedDate { get; private set; }

        public BlockedIpAddress(string ipAddress, DateTimeOffset blockedDate)
        {
            IpAddress = ipAddress;
            BlockedDate = blockedDate;
        }
    }
}