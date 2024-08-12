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

        public Event()
        {
        }

        public Event(EventTypes eventType, string details = "", string stackTrace = "")
        {
            Id = Guid.NewGuid().ToString();
            CreatedDate = DateTime.Now;
            EventType = eventType;
            Details = details;
            StackTrace = stackTrace;
        }
    }
}
