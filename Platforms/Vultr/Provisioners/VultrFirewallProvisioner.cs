using agrix.Configuration;
using agrix.Extensions;
using FirewallRule = Vultr.API.Models.FirewallRule;
using System.Collections.Generic;
using System.Linq;
using System;
using Vultr.API.Models;
using Vultr.API;

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
                var (existingFirewallGroupId, _) =
                    existingFirewalls.FirewallGroups.Single(Predicate);
                Console.WriteLine("Firewall {0} with ID {1} already exists",
                    firewall.Name, existingFirewallGroupId);

                var existingRules =
                    Client.Firewall.GetFirewallRules(
                        existingFirewallGroupId, "in", "v4")
                    .FirewallRules;

                foreach (var (firewallId, firewallRule) in
                    Client.Firewall.GetFirewallRules(
                    existingFirewallGroupId, "in", "v6")
                    .FirewallRules)
                    existingRules[firewallId] = firewallRule;

                foreach (var rule in firewall.Rules)
                {
                    if (!DoesRuleExist(existingRules, rule))
                        CreateRule(firewall.Name, existingFirewallGroupId, rule, dryrun);
                }

                foreach (var (_, firewallRule) in existingRules)
                {
                    if (!DoesRuleExist(firewall.Rules, firewallRule))
                        DeleteRule(
                            firewall.Name, existingFirewallGroupId, firewallRule, dryrun);
                }
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
                    CreateRule(firewall.Name, firewallGroupId, rule, dryrun);
            }
        }

        private void CreateRule(
            string firewallName,
            string firewallGroupId,
            Configuration.FirewallRule rule,
            bool dryrun)
        {
            var ipType = rule.IpType.ToString().ToLower();
            var protocol = rule.Protocol.ToString().ToLower();

            Console.WriteLine("Creating firewall rule for {0}", firewallName);
            ConsoleX.WriteLine("ip_type", ipType);
            ConsoleX.WriteLine("protocol", protocol);
            ConsoleX.WriteLine("subnet", rule.Subnet);
            ConsoleX.WriteLine("subnet_size", rule.SubnetSize);
            ConsoleX.WriteLine("port", rule.Ports);
            ConsoleX.WriteLine("source", rule.Source);

            if (!dryrun)
                Client.Firewall.CreateFirewallRule(
                    firewallGroupId,
                    ip_type: ipType,
                    protocol: protocol,
                    subnet: rule.Subnet,
                    subnet_size: rule.SubnetSize,
                    port: rule.Ports,
                    source: rule.Source);
        }

        private void DeleteRule(
            string firewallName,
            string firewallGroupId,
            FirewallRule rule,
            bool dryrun)
        {
            Console.WriteLine("Deleting existing firewall rule for {0}", firewallName);
            ConsoleX.WriteLine("port", rule.port);
            ConsoleX.WriteLine("port", rule.rulenumber);
            ConsoleX.WriteLine("port", rule.subnet);
            ConsoleX.WriteLine("port", rule.subnet_size);
            ConsoleX.WriteLine("protocol", rule.protocol);

            if (!dryrun)
                Client.Firewall.DeleteFirewallRule(
                    firewallGroupId, rule.rulenumber);
        }

        private static bool DoesRuleExist(
            Dictionary<string, FirewallRule> vultrRules,
            Configuration.FirewallRule rule)
        {
            return vultrRules.Any(f =>
                f.Value.port == rule.Ports
                && f.Value.protocol == rule.Protocol.ToString().ToLower()
                && f.Value.subnet == rule.Subnet
                && f.Value.subnet_size == rule.SubnetSize);
        }

        private static bool DoesRuleExist(
            IEnumerable<Configuration.FirewallRule> firewallRules,
            FirewallRule rule)
        {
            return firewallRules.Any(f =>
                f.Ports == rule.port
                && f.Protocol.ToString().ToLower() == rule.protocol
                && f.Subnet == rule.subnet
                && f.SubnetSize == rule.subnet_size);
        }
    }
}
