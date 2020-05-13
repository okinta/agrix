using agrix.Configuration;

namespace agrix.Platforms
{
    /// <summary>
    /// Describes an interface for communicating with a platform.
    /// </summary>
    internal interface IPlatform
    {
        /// <summary>
        /// Provisions a server using the given configuration.
        /// </summary>
        /// <param name="server">The server configuration to use for provisioning.</param>
        public void Provision(Server server);

        /// <summary>
        /// Tests the connection. Throws an exception if the connection is invalid.
        /// </summary>
        public void TestConnection();
    }
}
