using hoohub.Enums;

namespace hoohub.Data
{
    /// <summary>
    /// Represents the settings of the app for use in access control, behaviour, etc.
    /// </summary>
    public class HooHubSettings
    {
        /// <summary>
        /// Uniquely identifies this settings instance.
        /// </summary>
        public string Id { get; init; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Whether the app is accessible to non-authorised users.<br/>
        /// If false, non-authorised users will be redirected to a downtime page.
        /// </summary>
        public bool PublicAccessEnabled { get; set; } = true;

        /// <summary>
        /// The accessibility of the Archive page to users.
        /// </summary>
        public AccessTypes ArchiveAccess { get; set; } = AccessTypes.AllUsers;

        /// <summary>
        /// The maximum amount of comics that can be fetched per scroll when going through the archive.
        /// </summary>
        public int ArchiveMaximumComicsPerFetch { get; set; } = 5;

        /// <summary>
        /// The maximum number of events to return when going through the management menu.
        /// </summary>
        public int ManageEventsMaximumHistory { get; set; } = 1000;

        /// <summary>
        /// 
        /// </summary>
        public TimeOnly ScheduledComicReleaseTime { get; set; } = new TimeOnly(hour: 12, minute: 00);

        /// <summary>
        /// The date/time the comic was last updated.
        /// </summary>
        public DateTime LastModifiedDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public HooHubSettings()
        {
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="HooHubSettings"/> class.
        /// </summary>
        /// <param name="publicAccessEnabled">The public access enabled state to set.</param>
        /// <param name="archiveAccess">The archive access state to set.</param>
        /// <param name="archiveMaximumComicsPerFetch">The archive maximum comics per fetch value to set.</param>
        /// <param name="manageEventsMaximumHistory">The manage events maximum history value to set.</param>
        public HooHubSettings(
            bool publicAccessEnabled,
            AccessTypes archiveAccess,
            int archiveMaximumComicsPerFetch,
            int manageEventsMaximumHistory)
        {
            PublicAccessEnabled = publicAccessEnabled;
            ArchiveAccess = archiveAccess;
            ArchiveMaximumComicsPerFetch = archiveMaximumComicsPerFetch;
            ManageEventsMaximumHistory = manageEventsMaximumHistory;
            LastModifiedDate = DateTime.UtcNow;
        }
    }
}