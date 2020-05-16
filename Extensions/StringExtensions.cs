using System.Text;
using System;

namespace agrix.Extensions
{
    /// <summary>
    /// Extends the string class.
    /// </summary>
    internal static class StringExtensions
    {
        /// <summary>
        /// Encodes a string to base64.
        /// </summary>
        /// <param name="plainText">The string to encode.</param>
        /// <returns>The base64 encoded string.</returns>
        public static string Base64Encode(this string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }
    }
}
