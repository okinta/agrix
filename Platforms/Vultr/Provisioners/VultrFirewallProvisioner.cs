using System;
using System.Collections.Generic;
using System.Linq;
using agrix.Configuration;
using agrix.Extensions;
using Vultr.API;
using Vultr.API.Models;

namespace agrix.Platforms.Vultr.Provisioners
{
    /// <summary>
    /// Provisions Vultr firewalls.
    /// </summary>
    internal class VultrFirewallProvisioner : VultrProvisioner
    {
        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="client">The VultrClient to use to provision servers.</param>
        public VultrFirewallProvisioner(VultrClient client) : base(client) { }

        /// <summary>
        /// Provisions a new Vultr firewall.
        /// </summary>
        /// <param name="firewall">The configuration to reference to provision the
        /// firewall.</param>
        /// <param name="dryrun">Whether or not this is a dryrun. If set to true then
        /// provision commands will not be sent to the platform and instead messaging
        /// will be outputted describing what would be done.</param>
        public void Provision(Firewall firewall, bool dryrun = false)
        {
            Console.WriteLine("Creating firewall");
            ConsoleX.WriteLine("name", firewall.Name);

            var existingFirewalls = Client.Firewall.GetFirewallGroups();

            bool Predicate(KeyValuePair<string, FirewallGroup> existingFirewall) =>
                existingFirewall.Value.description == firewall.Name;
            if (existingFirewalls.FirewallGroups != null
                && existingFirewalls.FirewallGroups.Exists(Predicate))
            {
                var existingFirewall = existingFirewalls.FirewallGroups.Single(Predicate);
                Console.WriteLine("Firewall {0} with ID {1} already exists",
                    firewall.Name, existingFirewall.Key);
            }
            else
            {
                var firewallGroupId = "";

                if (!dryrun)
                {
                    var result = Client.Firewall.CreateFirewallGroup(firewall.Name);
                    firewallGroupId = result.FirewallGroup.FIREWALLGROUPID;
                    Console.WriteLine("Created firewall with ID {0}", firewallGroupId);
                }

                foreach (var rule in firewall.Rules)
                {
                    var ipType = rule.IpType.ToString().ToLower();
                    var protocol = rule.Protocol.ToString().ToLower();

                    Console.WriteLine("Creating firewall rule for {0}", firewall.Name);
                    ConsoleX.WriteLine("ip_type", ipType);
                    ConsoleX.WriteLine("protocol", protocol);
                    ConsoleX.WriteLine("subnet", rule.Subnet);
                    ConsoleX.WriteLine("subnet_size", rule.SubnetSize);
                    ConsoleX.WriteLine("port", rule.Ports);
                    ConsoleX.WriteLine("source", rule.Source);

                    if (!dryrun)
                    {
                        Client.Firewall.CreateFirewallRule(
                            firewallGroupId,
                            ip_type: ipType,
                            protocol: protocol,
                            subnet: rule.Subnet,
                            subnet_size: rule.SubnetSize,
                            port: rule.Ports,
                            source: rule.Source);
                    }
                }
            }
        }
    }
}
