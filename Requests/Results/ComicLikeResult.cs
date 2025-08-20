namespace hoohub.Requests.Results
{
    /// <summary>
    /// Represents the result of an attempt to like or unlike a comic.
    /// </summary>
    public class ComicLikeResult : BaseResult
    {
        /// <summary>
        /// The new current like count of the comic that was liked/unliked.
        /// </summary>
        public int LikeCount { get; init; }

        /// <summary>
        /// Whether the action that was performed resulted in the comic being liked.<br/>
        /// If false, the action resulted in the comic being unliked.
        /// </summary>
        public bool WasLiked { get; init; }
        
        /// <summary>
        /// Initialises a new instance of the <see cref="ComicLikeResult"/> class.
        /// </summary>
        /// <param name="success">The success state to set.</param>
        /// <param name="likeCount">The like count to set.</param>
        /// <param name="wasLiked">The liked status to set.</param>
        /// <param name="message">Optional message to set.</param>
        public ComicLikeResult(
            bool success, 
            int likeCount,
            bool wasLiked,
            string message = "") : base(success, message)
        {
            Success = success;
            LikeCount = likeCount;
            WasLiked = wasLiked;
            Message = message;
        }
    }
}