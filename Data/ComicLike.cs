namespace hoohub.Data
{
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
        /// The IP address associated with this like.<br/>
        /// Each IP address is limited to a single like per comic.
        /// </summary>
        public string IpAddress { get; init; } = string.Empty;

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
        public ComicLike(Comic comic, string ipAddress)
        {
            Comic = comic;
            IpAddress = ipAddress;
        }
    }
}