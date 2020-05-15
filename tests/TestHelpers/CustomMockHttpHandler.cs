using MockHttpServer;
using System.Collections.Generic;
using System.Net;
using System;

namespace tests.TestHelpers
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
        /// Creates a HTTP GET mock request that returns an empty response.
        /// </summary>
        /// <param name="url">The URL to mock.</param>
        public CustomMockHttpHandler(string url) : this(url, "") { }

        /// <summary>
        /// Creates a HTTP GET mock request that returns the given string.
        /// </summary>
        /// <param name="url">The URL to mock.</param>
        /// <param name="response">The response to return when a mock request is
        /// received.</param>
        public CustomMockHttpHandler(string url, string response) :
            this(url, response, HttpMethods.GET) { }

        /// <summary>
        /// Creates a HTTP mock request that returns the given string.
        /// </summary>
        /// <param name="url">The URL to mock.</param>
        /// <param name="response">The response to return when a mock request is
        /// received.</param>
        /// <param name="httpMethod">The HTTP method to mock.</param>
        public CustomMockHttpHandler(string url, string response, HttpMethods httpMethod)
        {
            URL = url;
            HttpMethod = httpMethod.ToString();
            HandlerFunction = (req, rsp, prm) => response;

            MockHttpHandler = new MockHttpHandler(
                url, HttpMethod, HandlerFunctionWithCounter);
        }

        /// <summary>
        /// Creates a HTTP POST mock request that calls the given function when a request
        /// is received.
        /// </summary>
        /// <param name="url">The URL to mock.</param>
        /// <param name="handlerFunction">The function to call when a mock request is
        /// received.</param>
        public CustomMockHttpHandler(string url,
            Func<
                HttpListenerRequest,
                HttpListenerResponse,
                Dictionary<string, string>, string> handlerFunction) :
            this(url, handlerFunction, HttpMethods.POST) { }

        /// <summary>
        /// Creates a HTTP mock request that calls the given function when a request is
        /// received.
        /// </summary>
        /// <param name="url">The URL to mock.</param>
        /// <param name="handlerFunction">The function to call when a mock request is
        /// received.</param>
        /// <param name="httpMethod">The HTTP method to mock.</param>
        public CustomMockHttpHandler(string url,
            Func<
                HttpListenerRequest,
                HttpListenerResponse,
                Dictionary<string, string>, string> handlerFunction,
            HttpMethods httpMethod)
        {
            URL = url;
            HttpMethod = httpMethod.ToString();
            HandlerFunction = handlerFunction;

            MockHttpHandler = new MockHttpHandler(
                url, HttpMethod, HandlerFunctionWithCounter);
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
}
