using agrix.Platforms.Vultr;

namespace tests
{
    /// <summary>
    /// Describes methods to mock Vultr API requests.
    /// </summary>
    internal class MockVultrRequests : MockRequests
    {
        /// <summary>
        /// Gets the VultrPlatform instance that has its requests routed to the mock
        /// server.
        /// </summary>
        public VultrPlatform Platform { get; }

        /// <summary>
        /// Instantiates a new instance. Starts the mock HTTP server.
        /// </summary>
        /// <param name="handlers"></param>
        /// <exception cref="ArgumentNullException">If <paramref name="handlers"/> is
        /// null.</exception>
        public MockVultrRequests(params CustomMockHttpHandler[] handlers) :
            base(handlers)
        {
            Platform = new VultrPlatform("abc123", URL);
        }
    }
}
