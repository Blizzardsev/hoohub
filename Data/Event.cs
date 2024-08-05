using hoohub.Enums;

namespace hoohub.Data
{
    public class Event
    {
        public string Guid { get; init; }
        public DateTime CreatedDate { get; init; }
        public EventTypes EventType { get; init; }
        public string Details { get; init; }
        public string StackTrace { get; init; }

        public Event()
        {
        }

        public Event(EventTypes eventType, string details = "", string stackTrace = "")
        {
            Guid = System.Guid.NewGuid().ToString();
            CreatedDate = DateTime.Now;
            EventType = eventType;
            Details = details;
            StackTrace = stackTrace;
        }
    }
}
