﻿using System.Collections.Generic;
using System.Linq;
using System;

namespace agrix.Configuration
{
    /// <summary>
    /// Represents a firewall configuration.
    /// </summary>
    internal struct Firewall
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
                throw new ArgumentNullException("name", "Firewall name cannot be empty");

            if (rules == null || rules.Count() == 0)
                throw new ArgumentNullException("rules", "Firewall rules cannot be empty");

            Name = name;
            Rules = new List<FirewallRule>(rules);
        }
    }
}
