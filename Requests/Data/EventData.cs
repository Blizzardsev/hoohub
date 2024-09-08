using hoohub.Data;
using hoohub.Services;

namespace hoohub.Requests.Data
{
    /// <summary>
    /// 
    /// </summary>
    public class EventData
    {
        /// <summary>
        /// 
        /// </summary>
        public string Guid { get; init; }

        /// <summary>
        /// 
        /// </summary>
        public string DisplayCreatedDate { get; init; }

        /// <summary>
        /// 
        /// </summary>
        public string DisplayEventType { get; init; }

        /// <summary>
        /// 
        /// </summary>
        public string Details { get; init; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name=""></param>
        public EventData(Event eventItem)
        {
            Guid = eventItem.Id;
            DisplayCreatedDate = FormattingService.GetDateTimeAsString(eventItem.CreatedDate.ToLocalTime());
            DisplayEventType = FormattingService.GetEnumDescription(eventItem.EventType);
            Details = eventItem.Details;
        }
    }
}