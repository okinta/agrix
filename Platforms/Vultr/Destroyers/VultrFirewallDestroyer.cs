using agrix.Configuration;
using agrix.Extensions;
using System;
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
            Console.WriteLine("Destroying firewall");
            ConsoleX.WriteLine("name", firewall.Name);

            var firewallGroupId = Client.Firewall.GetExistingFirewall(firewall.Name);

            if (!string.IsNullOrEmpty(firewallGroupId))
            {
                Console.WriteLine("Firewall {0} with ID {1} exists",
                    firewall.Name, firewallGroupId);

                var existingRules =
                    Client.Firewall.GetExistingRules(firewallGroupId);
                foreach (var firewallRule in existingRules)
                    Client.Firewall.DeleteRule(
                        firewall.Name, firewallGroupId, firewallRule.Value, dryrun);

                if (!dryrun)
                {
                    Client.Firewall.DeleteFirewallGroup(firewallGroupId);
                    Console.WriteLine("Deleted firewall {0}", firewall.Name);
                }
            }
            else
                Console.WriteLine("Firewall {0} doesn't exist", firewall.Name);

            Console.WriteLine("---");
        }
    }
}
