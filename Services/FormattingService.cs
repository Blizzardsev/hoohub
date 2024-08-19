using System.Drawing;
using System.Drawing.Imaging;

namespace hoohub.Services
{
    public class FormattingService
    {
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
