using hoohub.Enums;
using hoohub.Properties;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;

namespace hoohub.Services
{
    /// <summary>
    /// Helper class for formatting-related functionality.
    /// </summary>
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
        /// Given an <see cref="IFormFile"/>, returns it in byte array format.
        /// </summary>
        /// <param name="file">The <see cref="IFormFile"/> to convert.</param>
        /// <returns>The given file in byte array format.</returns>
        public static byte[] GetIFormFileAsBytes(IFormFile file)
        {
            using var memoryStream = new MemoryStream();
            file.CopyTo(memoryStream);
            return memoryStream.ToArray();
        }

        /// <summary>
        /// Returns the Description for a given enum value, if it exists.
        /// Otherwise, return the enum's implementation of .ToString().
        /// </summary>
        /// <param name="value">The enum value to return the description at tribute value for.</param>
        /// <returns>Enum description, if it exists. Otherwise, the enum's implementation of .ToString().</returns>
        public static string GetEnumDescription(Enum value)
        {
            var fieldInfo = value.GetType().GetField(value.ToString());
            if (fieldInfo == null)
            {
                return string.Empty;
            }
            var attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : value.ToString();
        }

        /// <summary>
        /// Returns a list of sort options for use in a filter.
        /// </summary>
        /// <param name="defaultOrder">The <see cref="OrderByTypes"/> value that should be selected by default.</param>
        /// <returns><see cref="SelectList"/> of options as described by <see cref="OrderByTypes"/>.</returns>
        public static SelectList GetOrderBySelectListWithDefault(OrderByTypes defaultOrder)
        {
            return new SelectList(items: new List<SelectListItem>() {
                new SelectListItem(text: GetEnumDescription(OrderByTypes.Ascending), value: ((int)OrderByTypes.Ascending).ToString()),
                new SelectListItem(text: GetEnumDescription(OrderByTypes.Descending), value: ((int)OrderByTypes.Descending).ToString())
            }, dataValueField: "Value", dataTextField: "Text", selectedValue: ((int)defaultOrder).ToString());
        }

        /// <summary>
        /// Given a boolean expression, returns the string representation of that expression as Yes or No.
        /// </summary>
        /// <param name="expression">The expression to evaluate.</param>
        /// <returns>Yes if the expression is True, otherwise No.</returns>
        public static string GetBooleanAsYesNoString(bool expression) => expression ? "Yes" : "No";
    }
}
