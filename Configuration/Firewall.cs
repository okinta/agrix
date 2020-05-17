using System.Collections.Generic;
using System.Linq;
using System;

namespace agrix.Configuration
{
    /// <summary>
    /// Represents a firewall configuration.
    /// </summary>
    internal readonly struct Firewall
    {
        /// <summary>
        /// Gets the name of the firewall.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the list of the firewall rules.
        /// </summary>
        public IReadOnlyList<FirewallRule> Rules { get; }

        /// <summary>
        /// Creates a new firewall configuration.
        /// </summary>
        /// <param name="name">The name of the firewall.</param>
        /// <param name="rules">The list of firewall rules.</param>
        /// <exception cref="ArgumentNullException">If any of the arguments are null or
        /// empty.</exception>
        public Firewall(string name, IEnumerable<FirewallRule> rules)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(
                    nameof(name), "Firewall name cannot be empty");

            var firewallRules = rules as FirewallRule[] ?? rules.ToArray();
            if (rules == null || !firewallRules.Any())
                throw new ArgumentNullException(
                    nameof(rules), "Firewall rules cannot be empty");

            Name = name;
            Rules = new List<FirewallRule>(firewallRules);
        }
    }
}
