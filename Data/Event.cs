using hoohub.Enums;

namespace hoohub.Data
{
    public class Event
    {
        /// <summary>
        /// 
        /// </summary>
        public string Id { get; init; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime CreatedDate { get; init; }

        /// <summary>
        /// 
        /// </summary>
        public EventTypes EventType { get; init; }

        /// <summary>
        /// 
        /// </summary>
        public string Details { get; init; }

        /// <summary>
        /// 
        /// </summary>
        public string StackTrace { get; init; }

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
            Id = Guid.NewGuid().ToString();
            CreatedDate = DateTime.Now;
            EventType = eventType;
            Details = details;
            StackTrace = stackTrace;
        }

        /// <summary>
        /// Returns a <see cref="TimeSpan"/> representing the age of this event.
        /// </summary>
        /// <returns><see cref="TimeSpan"/> representing the age of this event.</returns>
        public TimeSpan GetAge() => DateTime.Now - CreatedDate;
    }
}
