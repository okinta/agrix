using MockHttpServer;
using System.Collections.Generic;
using System.Net;
using System;

namespace tests
{
    /// <summary>
    /// Allows counting of calls to a MockHttpHandler.
    /// </summary>
    internal class CustomMockHttpHandler
    {
        /// <summary>
        /// Gets the number of times the MockHttpHandler was called.
        /// </summary>
        public int Called { get; private set; } = 0;

        /// <summary>
        /// Gets the URL of the MockHttpHandler.
        /// </summary>
        public string URL { get; }

        /// <summary>
        /// Gets the HTTP method of the MockHttpHandler.
        /// </summary>
        public string HttpMethod { get; }

        /// <summary>
        /// Gets the MockHttpHandler instance.
        /// </summary>
        public MockHttpHandler MockHttpHandler { get; }

        /// <summary>
        /// Gets the MockHttpHandler handler function.
        /// </summary>
        public Func<
            HttpListenerRequest,
            HttpListenerResponse,
            Dictionary<string, string>, string> HandlerFunction { get; }

        /// <summary>
        /// Instantiates the instance.
        /// </summary>
        /// <param name="url">The URL to mock.</param>
        /// <param name="httpMethod">The HTTP method to mock.</param>
        /// <param name="handlerFunction">The function to call when a mock request is
        /// received.</param>
        public CustomMockHttpHandler(string url, string httpMethod,
            Func<
                HttpListenerRequest,
                HttpListenerResponse,
                Dictionary<string, string>, string> handlerFunction)
        {
            URL = url;
            HttpMethod = httpMethod;
            HandlerFunction = handlerFunction;

            MockHttpHandler = new MockHttpHandler(
                url, httpMethod, HandlerFunctionWithCounter);
        }

        private string HandlerFunctionWithCounter(
            HttpListenerRequest req,
            HttpListenerResponse rsp,
            Dictionary<string, string> prm)
        {
            Called += 1;
            return HandlerFunction(req, rsp, prm);
        }
    }

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
