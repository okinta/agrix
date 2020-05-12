using agrix.Configuration;
using System;
using Vultr.API;

namespace agrix.Platforms
{
    /// <summary>
    /// Describes methods to communicate with the Vultr platform.
    /// </summary>
    internal class Vultr : IPlatform
    {
        private string ApiKey { get; }

        /// <summary>
        /// Instantiates the instance.
        /// </summary>
        /// <param name="apiKey">The API key to use for communicating with Vultr.</param>
        public Vultr(string apiKey)
        {
            ApiKey = apiKey;
        }

        /// <summary>
        /// Provisions a server using the given configuration.
        /// </summary>
        /// <param name="server">The server configuration to use for provisioning.</param>
        public void Provision(Server server)
        {
            var client = new VultrClient(ApiKey);
            throw new NotImplementedException();
        }
    }
}
