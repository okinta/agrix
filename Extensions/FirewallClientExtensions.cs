using System.Collections.Generic;
using System.Linq;
using System;
using Vultr.API.Clients;
using Vultr.API.Models;

namespace agrix.Extensions
{
    /// <summary>
    /// Extends the FirewallClient class.
    /// </summary>
    internal static class FirewallClientExtensions
    {
        /// <summary>
        /// Gets the ID of an existing firewall.
        /// </summary>
        /// <param name="client">The FirewallClient to use to obtain the firewall
        /// ID.</param>
        /// <param name="name">The name of the firewall to obtain the ID for.</param>
        /// <returns>The firewall ID if it exists. Returns an empty string if no firewall
        /// exists with the given <paramref name="name"/>.</returns>
        public static string GetExistingFirewall(this FirewallClient client, string name)
        {
            var existingFirewalls = client.GetFirewallGroups();
            bool Predicate(KeyValuePair<string, FirewallGroup> existingFirewall) =>
                existingFirewall.Value.description == name;

            if (existingFirewalls.FirewallGroups == null ||
                !existingFirewalls.FirewallGroups.Exists(Predicate)) return "";

            var (existingFirewallGroupId, _) =
                existingFirewalls.FirewallGroups.Single(Predicate);
            return existingFirewallGroupId;
        }

        /// <summary>
        /// Gets the collection of existing rules for a firewall.
        /// </summary>
        /// <param name="client">The FirewallClient to use to obtain the firewall
        /// rules for.</param>
        /// <param name="firewallGroupId">The ID of the firewall to obtain the rules
        /// for.</param>
        /// <returns>The collection of firewall rules.</returns>
        public static IReadOnlyDictionary<string, FirewallRule> GetExistingRules(
            this FirewallClient client, string firewallGroupId)
        {
            var existingRules =
                client.GetFirewallRules(
                        firewallGroupId, "in", "v4")
                    .FirewallRules;

            foreach (var (firewallId, firewallRule) in
                client.GetFirewallRules(
                        firewallGroupId, "in", "v6")
                    .FirewallRules)
                existingRules[firewallId] = firewallRule;

            return existingRules;
        }

        /// <summary>
        /// Deletes an existing firewall rule.
        /// </summary>
        /// <param name="client">The FirewallClient to use to delete the firewall
        /// rule.</param>
        /// <param name="firewallName">The name of the firewall the rule belongs
        /// to.</param>
        /// <param name="firewallGroupId">The ID of the firewall the rule belongs
        /// to.</param>
        /// <param name="rule">The rule to delete.</param>
        /// <param name="dryrun">Whether or not this is a dryrun. If set to true then
        /// provision commands will not be sent to the platform and instead messaging
        /// will be outputted describing what would be done.</param>
        public static void DeleteRule(
            this FirewallClient client,
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
                client.DeleteFirewallRule(firewallGroupId, rule.rulenumber);

            Console.WriteLine("--");
        }
    }
}
