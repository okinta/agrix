using agrix.Configuration;
using System;

namespace agrix.Platforms
{
    internal class Vultr : IPlatform
    {
        /// <summary>
        /// Provisions a server using the given configuration.
        /// </summary>
        /// <param name="server">The server configuration to use for provisioning.</param>
        public void Provision(Server server)
        {
            throw new NotImplementedException();
        }
    }
}
