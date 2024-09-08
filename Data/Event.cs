using hoohub.Enums;

namespace hoohub.Data
{
    public class Event
    {
        /// <summary>
        /// 
        /// </summary>
        public string Id { get; init; } = Guid.NewGuid().ToString();

        /// <summary>
        /// 
        /// </summary>
        public DateTime CreatedDate { get; init; } = DateTime.UtcNow;

        /// <summary>
        /// 
        /// </summary>
        public EventTypes EventType { get; init; } = EventTypes.Unknown;

        /// <summary>
        /// 
        /// </summary>
        public string Details { get; init; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        public string StackTrace { get; init; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        public Event()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="details"></param>
        /// <param name="stackTrace"></param>
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
