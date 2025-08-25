using hoohub.Data;
using hoohub.Services;

namespace hoohub.Requests.Data
{
    /// <summary>
    /// Represents requested event data to display to the user. 
    /// </summary>
    public class EventData
    {
        /// <summary>
        /// Uniquely identifies the event.
        /// </summary>
        public string Guid { get; init; }

        /// <summary>
        /// The created date of the event to display to the user.
        /// </summary>
        public DateTime DisplayCreatedDate { get; init; }

        /// <summary>
        /// The event type description to display to the user.
        /// </summary>
        public string DisplayEventType { get; init; }

        /// <summary>
        /// The details of the event to display to the user, if any.
        /// </summary>
        public string Details { get; init; }

        /// <summary>
        /// Initialises a new instance of the <see cref="EventData"/> class.
        /// </summary>
        /// <param name="eventItem">Base <see cref="Event"/> to derive attributes from.</param>
        public EventData(Event eventItem)
        {
            Guid = eventItem.Id;
            DisplayCreatedDate = eventItem.CreatedDate;
            DisplayEventType = FormattingService.GetEnumDescription(eventItem.EventType);
            Details = eventItem.Details;
        }
    }
}