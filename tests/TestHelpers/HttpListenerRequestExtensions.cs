﻿using System.IO;
using System.Net;

namespace tests.TestHelpers
{
    /// <summary>
    /// Extends the HttpListenerRequest class.
    /// </summary>
    internal static class HttpListenerRequestExtensions
    {
        /// <summary>
        /// Gets the content of the request as a string.
        /// </summary>
        /// <param name="req">The instance to retrieve the content from.</param>
        /// <returns>The content of the request as a string.</returns>
        public static string GetContent(this HttpListenerRequest req)
        {
            using var reader = new StreamReader(req.InputStream);
            return reader.ReadToEnd();
        }
    }
}
