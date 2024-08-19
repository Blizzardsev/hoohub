using hoohub.Data;

namespace hoohub.Requests.Data
{
    /// <summary>
    /// 
    /// </summary>
    public class ManageComicListData
    {
        /// <summary>
        /// 
        /// </summary>
        public string Guid { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string DisplayName { get; init; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="comic"></param>
        public ManageComicListData(Comic comic)
        {
            Guid = comic.Id;
            DisplayName = comic.GetComicDisplayName();
        }
    }
}