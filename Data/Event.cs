using hoohub.Enums;

namespace hoohub.Data
{
    /// <summary>
    /// Represents a loggable application event.
    /// </summary>
    public class Event
    {
        /// <summary>
        /// Uniquely identifies this event.
        /// </summary>
        public string Id { get; init; } = Guid.NewGuid().ToString();

        /// <summary>
        /// The date and time the event was created.
        /// </summary>
        public DateTime CreatedDate { get; init; } = DateTime.UtcNow;

        /// <summary>
        /// The <see cref="EventTypes"/> type of the event.
        /// </summary>
        public EventTypes EventType { get; init; } = EventTypes.Unknown;

        /// <summary>
        /// The details of the event and any contextual information.
        /// </summary>
        public string Details { get; init; } = string.Empty;

        /// <summary>
        /// Optional stack trace associated with the event.
        /// </summary>
        public string StackTrace { get; init; } = string.Empty;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Event()
        {
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="Event"/> class.
        /// </summary>
        /// <param name="eventType">The <see cref="EventType"/> to set.</param>
        /// <param name="details">The event details to set.</param>
        /// <param name="stackTrace">Optional stack trace to set.</param>
        public Event(EventTypes eventType, string details = "", string stackTrace = "")
        {
            EventType = eventType;
            Details = details;
            StackTrace = stackTrace;
        }

        /// <summary>
        /// Returns a <see cref="TimeSpan"/> representing the age of this event.
        /// </summary>
        /// <returns><see cref="TimeSpan"/> representing the age of this event.</returns>
        public TimeSpan GetAge() => DateTime.UtcNow - CreatedDate;
    }
}
