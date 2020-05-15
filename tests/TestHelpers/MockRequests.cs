using MockHttpServer;
using System;

namespace tests.TestHelpers
{
    /// <summary>
    /// Describes methods to help mock HTTP requests.
    /// </summary>
    internal class MockRequests : IDisposable
    {
        private const int TestPort = 8873;
        private const string TestURL = "http://localhost:8873/";

        /// <summary>
        /// Gets the mocked URL that requests should be sent to.
        /// </summary>
        public string URL { get; }

        private CustomMockHttpHandler[] Handlers { get; }
        private MockServer MockServer { get; }

        /// <summary>
        /// Instantiates a new instance. Starts the mock HTTP server.
        /// </summary>
        /// <param name="handlers"></param>
        /// <exception cref="ArgumentNullException">If <paramref name="handlers"/> is
        /// null.</exception>
        public MockRequests(params CustomMockHttpHandler[] handlers)
        {
            if (handlers is null)
                throw new ArgumentNullException("handlers", "handlers must not be null");

            Handlers = handlers;
            MockServer = new MockServer(TestPort, handlers.GetMockHttpHandlers());
            URL = TestURL;
        }

        /// <summary>
        /// Retrieves a CustomMockHttpHandler instance passed into the constructor.
        /// </summary>
        /// <param name="index">The index to retrieve.</param>
        /// <returns>The CustomMockHttpHandler instance at the specified index.</returns>
        public CustomMockHttpHandler this[int index] { get { return Handlers[index]; } }

        /// <summary>
        /// Stops the mock HTTP server.
        /// </summary>
        public void Dispose()
        {
            MockServer.Dispose();
        }

        /// <summary>
        /// Asserts that all requests have been called exactly once.
        /// </summary>
        /// <exception cref="RequestNotCalledException">If a request was not
        /// called.</exception>
        /// <exception cref="RequestCalledTooOftenException">If a request was called
        /// more than once.</exception>
        public void AssertAllCalledOnce()
        {
            foreach (var handler in Handlers)
            {
                if (handler.Called == 0)
                    throw new RequestNotCalledException(
                        URL, string.Format("{0} was not called", URL));

                if (handler.Called > 1)
                    throw new RequestCalledTooOftenException(
                        URL, 1, handler.Called, string.Format(
                            "{0} was only expected to be called once. Instead, was called {1} times",
                            URL, handler.Called));
            }
        }
    }
}
