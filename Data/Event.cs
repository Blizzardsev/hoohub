using hoohub.Enums;
using System.ComponentModel.DataAnnotations;

namespace hoohub.Data
{
    public class Event
    {
        /// <summary>
        /// 
        /// </summary>
        public string Guid { get; init; }

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
            Guid = System.Guid.NewGuid().ToString();
            CreatedDate = DateTime.Now;
            EventType = eventType;
            Details = details;
            StackTrace = stackTrace;
        }
    }
}
