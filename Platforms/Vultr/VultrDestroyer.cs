using System;
using Vultr.API;

namespace agrix.Platforms.Vultr
{
    /// <summary>
    /// Base class for provisioning Vultr infrastructure.
    /// </summary>
    internal abstract class VultrDestroyer<T> : IDestroyer<T>
    {
        /// <summary>
        /// Gets the VultrClient instance to use to communicate with Vultr API.
        /// </summary>
        protected VultrClient Client { get; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="client">The VultrClient to use to destroy infrastructure.</param>
        protected VultrDestroyer(VultrClient client)
        {
            Client = client ?? throw new ArgumentNullException(
                nameof(client), "client must not be null");
        }

        /// <summary>
        /// Provisions new Vultr infrastructure script.
        /// </summary>
        /// <param name="infrastructure">The configuration to reference during
        /// provisioning.</param>
        /// <param name="dryrun">Whether or not this is a dryrun. If set to true then
        /// provision commands will not be sent to the platform and instead messaging
        /// will be outputted describing what would be done.</param>
        public abstract void Destroy(T infrastructure, bool dryrun = false);
    }
}
