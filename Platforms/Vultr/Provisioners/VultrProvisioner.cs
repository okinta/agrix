using System;
using Vultr.API;

namespace agrix.Platforms.Vultr.Provisioners
{
    /// <summary>
    /// Base class for provisioning Vultr infrastructure.
    /// </summary>
    internal abstract class VultrProvisioner
    {
        /// <summary>
        /// Gets the VultrClient instance to use to commuicate with Vultr API.
        /// </summary>
        protected VultrClient Client { get; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="client">The VultrClient to use to provision servers.</param>
        public VultrProvisioner(VultrClient client)
        {
            if (client is null)
                throw new ArgumentNullException("client", "client must not be null");

            Client = client;
        }
    }
}
