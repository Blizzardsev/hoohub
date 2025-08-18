namespace hoohub.Requests.Results
{
    /// <summary>
    /// Base result object representing the result of a request for data/an action.
    /// </summary>
    public class BaseResult
    {
        /// <summary>
        /// Whether the request was successful.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Optional request message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Initialises a new instance of the <see cref="BaseResult"/> class/
        /// </summary>
        /// <param name="success">The success state to set.</param>
        /// <param name="message">Optional message to set.</param>
        public BaseResult(bool success, string message = "")
        {
            Success = success;
            Message = message;
        }
    }
}