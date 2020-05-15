using MockHttpServer;
using System;

namespace tests
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
    }
}
