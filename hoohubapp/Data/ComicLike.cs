namespace hoohub.Data
{
    /// <summary>
    /// A like/heart against a comic for feedback purposes.
    /// </summary>
    public class ComicLike
    {
        /// <summary>
        /// Uniquely identifies this comic.
        /// </summary>
        public string Id { get; init; } = Guid.NewGuid().ToString();

        /// <summary>
        /// The comic associated with this like.
        /// </summary>
        public Comic Comic { get; init; }

        /// <summary>
        /// The date this like was created.
        /// </summary>
        public DateTime CreatedDate { get; init; } = DateTime.UtcNow;

        /// <summary>
        /// The IP address associated with this like.
        /// </summary>
        public string IpAddress { get; init; } = string.Empty;

        /// <summary>
        /// The user GUID associated with this like.<br/>
        /// Users that aren't logged in are assigned a GUID via cookie, which is used to link likes.
        /// </summary>
        public string UserGuid { get; init; } = string.Empty;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ComicLike()
        {
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="ComicLike"/> class.
        /// </summary>
        /// <param name="comic">The <see cref="Comic"/> to set.</param>
        /// <param name="ipAddress">The IP address to set.</param>
        /// <param name="userGuid">The user GUID to set.</param>
        public ComicLike(Comic comic, string ipAddress, string userGuid)
        {
            Comic = comic;
            IpAddress = ipAddress;
            UserGuid = userGuid;
        }
    }
}