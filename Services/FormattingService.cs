using hoohub.Properties;
using System.Drawing;
using System.Drawing.Imaging;

namespace hoohub.Services
{
    public class FormattingService
    {
        /// <summary>
        /// Given a <see cref="DateTime"/> object, returns a formatted representation based on format declared in
        /// <see cref="Resources.DatetimeFormat"/>.
        /// </summary>
        /// <param name="dateTime">The datetime to return a string for.</param>
        /// <returns>Formatted datetime string.</returns> 
        public static string GetDateTimeAsString(DateTime dateTime, bool includeTime = true) => includeTime
            ? dateTime.ToString(Resources.DatetimeFormat)
            : dateTime.ToString(Resources.DateFormat);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static string GetBitmapAsBase64String(Bitmap bitmap)
        {
            using var memoryStream = new MemoryStream();
            bitmap.Save(stream: memoryStream, format: ImageFormat.Png);
            return Convert.ToBase64String(memoryStream.ToArray());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static byte[] GetIFormFileAsBytes(IFormFile file)
        {
            using var memoryStream = new MemoryStream();
            file.CopyTo(memoryStream);
            return memoryStream.ToArray();
        }
    }
}
