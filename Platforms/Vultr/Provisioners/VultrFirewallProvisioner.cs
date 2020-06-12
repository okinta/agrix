using agrix.Configuration;
using agrix.Extensions;
using FirewallRule = Vultr.API.Models.FirewallRule;
using System.Collections.Generic;
using System.Linq;
using System;
using Vultr.API;

namespace agrix.Platforms.Vultr.Provisioners
{
    /// <summary>
    /// Provisions Vultr firewalls.
    /// </summary>
    internal class VultrFirewallProvisioner : VultrProvisioner<Firewall>
    {
        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="client">The VultrClient to use to provision firewalls.</param>
        public VultrFirewallProvisioner(VultrClient client) : base(client) { }

        /// <summary>
        /// Provisions a new Vultr firewall.
        /// </summary>
        /// <param name="firewall">The configuration to reference to provision the
        /// firewall.</param>
        /// <param name="dryrun">Whether or not this is a dryrun. If set to true then
        /// provision commands will not be sent to the platform and instead messaging
        /// will be outputted describing what would be done.</param>
        public override void Provision(Firewall firewall, bool dryrun = false)
        {
            Console.WriteLine("Creating firewall");
            ConsoleX.WriteLine("name", firewall.Name);

            var firewallGroupId = Client.Firewall.GetExistingFirewall(firewall.Name);

            if (!string.IsNullOrEmpty(firewallGroupId))
            {
                Console.WriteLine("Firewall {0} with ID {1} already exists",
                    firewall.Name, firewallGroupId);

                var existingRules =
                    Client.Firewall.GetExistingRules(firewallGroupId);
                CreateRules(firewall, firewallGroupId, dryrun, existingRules);
                DeleteOldRules(firewall, firewallGroupId, existingRules, dryrun);
            }
            else
                CreateRules(firewall, CreateNewFirewallGroup(firewall, dryrun), dryrun);

            Console.WriteLine("---");
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
            IReadOnlyDictionary<string, FirewallRule> existingRules = null)
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

            Console.WriteLine("--");
        }

        private void DeleteOldRules(
            Firewall firewall,
            string firewallGroupId,
            IReadOnlyDictionary<string, FirewallRule> existingRules,
            bool dryrun)
        {
            foreach (var firewallRule in
                existingRules.Where(r =>
                    !DoesRuleExist(firewall.Rules, r.Value)))
                Client.Firewall.DeleteRule(
                    firewall.Name, firewallGroupId, firewallRule.Value, dryrun);
        }

        private static bool DoesRuleExist(
            IReadOnlyDictionary<string, FirewallRule> vultrRules,
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
