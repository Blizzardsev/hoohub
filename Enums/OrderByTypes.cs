using System.ComponentModel;

namespace hoohub.Enums
{
    /// <summary>
    /// The types by which items such as comics can be ordered for display.
    /// </summary>
    public enum OrderByTypes
    {
        [Description("Ascending")]
        Ascending,

        [Description("Descending")]
        Descending
    }
}
