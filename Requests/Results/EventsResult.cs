using hoohub.Requests.Data;

namespace hoohub.Requests.Results
{
    public class EventsResult : BaseResult
    {
        public List<EventData> EventData { get; init; }

        public EventsResult(bool success, List<EventData> eventData, string message = "") : base(success, message)
        {
            Success = success;
            EventData = eventData;
            Message = message;
        }
    }
}