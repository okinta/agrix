using agrix.Configuration;
using Vultr.API;

namespace agrix.Platforms.Vultr.Destroyers
{
    /// <summary>
    /// Destroys Vultr firewalls.
    /// </summary>
    internal class VultrFirewallDestroyer : VultrDestroyer<Firewall>
    {
        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="client">The VultrClient to use to destroy firewalls.</param>
        public VultrFirewallDestroyer(VultrClient client) : base(client)
        {
        }

        /// <summary>
        /// Destroys an existing Vultr firewall.
        /// </summary>
        /// <param name="firewall">The configuration to reference to destroy the
        /// firewall.</param>
        /// <param name="dryrun">Whether or not this is a dryrun. If set to true then
        /// provision commands will not be sent to the platform and instead messaging
        /// will be outputted describing what would be done.</param>
        public override void Destroy(Firewall firewall, bool dryrun = false)
        {
            throw new System.NotImplementedException();
        }
    }
}
