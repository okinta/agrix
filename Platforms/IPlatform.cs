using agrix.Configuration;

namespace agrix.Platforms
{
    internal interface IPlatform
    {
        /// <summary>
        /// Provisions a server using the given configuration.
        /// </summary>
        /// <param name="server">The server configuration to use for provisioning.</param>
        public void Provision(Server server);
    }
}
