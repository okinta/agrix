using agrix.Configuration;
using Vultr.API;

namespace agrix.Platforms.Vultr.Destroyers
{
    internal class VultrServerDestroyer : VultrDestroyer<Server>
    {
        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="client">The VultrClient to use to destroy servers.</param>
        public VultrServerDestroyer(VultrClient client) : base(client)
        {
        }

        /// <summary>
        /// Destroys an existing Vultr server.
        /// </summary>
        /// <param name="server">The configuration to reference to destroy the
        /// server.</param>
        /// <param name="dryrun">Whether or not this is a dryrun. If set to true then
        /// provision commands will not be sent to the platform and instead messaging
        /// will be outputted describing what would be done.</param>
        public override void Destroy(Server server, bool dryrun = false)
        {
            throw new System.NotImplementedException();
        }
    }
}
