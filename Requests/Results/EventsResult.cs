using hoohub.Requests.Data;

namespace hoohub.Requests.Results
{
    /// <summary>
    /// Represents the result of an attempt to fetch a list of event items.
    /// </summary>
    public class EventsResult : BaseResult
    {
        /// <summary>
        /// The list of event data items to display.
        /// </summary>
        public List<EventData> EventData { get; init; }

        /// <summary>
        /// Initialises a new instance of the <see cref="EventsResult"/> class.
        /// </summary>
        /// <param name="success">The success state to set.</param>
        /// <param name="eventData">The list of <see cref="EventData"/> to set.</param>
        /// <param name="message">Optional message to set.</param>
        public EventsResult(bool success, List<EventData> eventData, string message = "") : base(success, message)
        {
            Success = success;
            EventData = eventData;
            Message = message;
        }
    }
}