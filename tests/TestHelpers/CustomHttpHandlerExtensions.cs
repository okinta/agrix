using MockHttpServer;
using System.Collections.Generic;

namespace tests.TestHelpers
{
    /// <summary>
    /// Provides an extension method to convert CustomMockHttpHandler instances
    /// to MockHttpHandler for use by MockServer.
    /// </summary>
    internal static class CustomHttpHandlerExtensions
    {
        /// <summary>
        /// Converts to a collection of MockHttpHandler instances.
        /// </summary>
        /// <param name="handlers">The handlers to convert.</param>
        /// <returns>The converted instances.</returns>
        public static IEnumerable<MockHttpHandler> GetMockHttpHandlers(
            this IEnumerable<CustomMockHttpHandler> handlers)
        {
            var newHandlers = new List<MockHttpHandler>();
            foreach (var handler in handlers)
                newHandlers.Add(handler.MockHttpHandler);

            return newHandlers;
        }
    }
}
