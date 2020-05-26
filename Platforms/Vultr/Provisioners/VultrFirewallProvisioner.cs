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

            var firewallGroupId = GetExistingFirewall(firewall.Name);

            if (!string.IsNullOrEmpty(firewallGroupId))
            {
                Console.WriteLine("Firewall {0} with ID {1} already exists",
                    firewall.Name, firewallGroupId);

                var existingRules =
                    GetExistingRules(firewallGroupId);
                CreateRules(firewall, firewallGroupId, dryrun, existingRules);
                DeleteOldRules(firewall, firewallGroupId, existingRules, dryrun);
            }
            else
                CreateRules(firewall, CreateNewFirewallGroup(firewall, dryrun), dryrun);
        }

        private string GetExistingFirewall(string name)
        {
            var existingFirewalls = Client.Firewall.GetFirewallGroups();
            bool Predicate(KeyValuePair<string, FirewallGroup> existingFirewall) =>
                existingFirewall.Value.description == name;

            if (existingFirewalls.FirewallGroups == null ||
                !existingFirewalls.FirewallGroups.Exists(Predicate)) return "";

            var (existingFirewallGroupId, _) =
                existingFirewalls.FirewallGroups.Single(Predicate);
            return existingFirewallGroupId;
        }

        private Dictionary<string, FirewallRule> GetExistingRules(
            string firewallGroupId)
        {
            var existingRules =
                Client.Firewall.GetFirewallRules(
                        firewallGroupId, "in", "v4")
                    .FirewallRules;

            foreach (var (firewallId, firewallRule) in
                Client.Firewall.GetFirewallRules(
                        firewallGroupId, "in", "v6")
                    .FirewallRules)
                existingRules[firewallId] = firewallRule;

            return existingRules;
        }

        private string CreateNewFirewallGroup(Firewall firewall, bool dryrun)
        {
            if (dryrun) return "";

            var result = Client.Firewall.CreateFirewallGroup(firewall.Name);
            var firewallGroupId = result.FirewallGroup.FIREWALLGROUPID;
            Console.WriteLine("Created firewall with ID {0}", firewallGroupId);
            return firewallGroupId;
        }

        private void CreateRules(
            Firewall firewall,
            string firewallGroupId,
            bool dryrun,
            Dictionary<string, FirewallRule> existingRules = null)
        {
            foreach (var rule in firewall.Rules)
            {
                if (existingRules == null || !DoesRuleExist(existingRules, rule))
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
            ConsoleX.WriteLine("port", rule.Ports);
            ConsoleX.WriteLine("protocol", protocol);
            ConsoleX.WriteLine("source", rule.Source);
            ConsoleX.WriteLine("subnet", rule.Subnet);
            ConsoleX.WriteLine("subnet_size", rule.SubnetSize);

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

        private void DeleteOldRules(
            Firewall firewall,
            string firewallGroupId,
            Dictionary<string, FirewallRule> existingRules,
            bool dryrun)
        {
            foreach (var firewallRule in
                existingRules.Where(r =>
                    !DoesRuleExist(firewall.Rules, r.Value)))
                DeleteRule(
                    firewall.Name, firewallGroupId, firewallRule.Value, dryrun);
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
            ConsoleX.WriteLine("source", rule.source);

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
                && f.Value.subnet_size == rule.SubnetSize
                && f.Value.source == rule.Source);
        }

        private static bool DoesRuleExist(
            IEnumerable<Configuration.FirewallRule> firewallRules,
            FirewallRule rule)
        {
            return firewallRules.Any(f =>
                f.Ports == rule.port
                && f.Protocol.ToString().ToLower() == rule.protocol
                && f.Subnet == rule.subnet
                && f.SubnetSize == rule.subnet_size
                && f.Source == rule.source);
        }
    }
}
