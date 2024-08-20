using hoohub.Data;

namespace hoohub.Requests.Results
{
    /// <summary>
    /// 
    /// </summary>
    public class ManageMeResult
    {
        /// <summary>
        /// 
        /// </summary>
        public string Handle { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public byte[] ImageData { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        public ManageMeResult(HooHubUser user) 
        {
            Handle = user.Handle;
            ImageData = user.DisplayPicture;
        }
    }
}
