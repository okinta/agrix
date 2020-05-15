using System.Collections.Generic;
using System;

namespace agrix.Configuration
{
    /// <summary>
    /// Represents a type of IP.
    /// </summary>
    internal enum IPType
    {
        V4,
        v6
    }

    /// <summary>
    /// Represents a type of protocol.
    /// </summary>
    internal enum Protocol
    {
        ICMP,
        TCP,
        UDP,
        GRE
    }

    /// <summary>
    /// Represents a firewall rule configuration.
    /// </summary>
    internal struct FirewallRule
    {
        /// <summary>
        /// Gets the type of IP this firewall rule is for.
        /// </summary>
        public IPType IPType { get; }

        /// <summary>
        /// Gets the protocol this firewall rule is for.
        /// </summary>
        public Protocol Protocol { get; }

        /// <summary>
        /// Gets the source that's allowed through this firewall rule.
        /// </summary>
        public string Source { get; }

        /// <summary>
        /// Gets the ports affected by this firewall rule.
        /// </summary>
        public IReadOnlyList<int> Ports { get; }

        /// <summary>
        /// Creates a new firewall rule.
        /// </summary>
        /// <param name="ipType">The type of IP the firewall rule applies to.</param>
        /// <param name="protocol">The type of protocol the firewall rule applies
        /// to.</param>
        /// <param name="source">The source that's allowed through the firewall
        /// rule.</param>
        /// <param name="ports">The ports affected by the firewall rule.</param>
        /// <exception cref="ArgumentNullException">If any arguments are null or
        /// empty.</exception>
        public FirewallRule(
            IPType ipType, Protocol protocol, string source, IEnumerable<int> ports)
        {
            if (string.IsNullOrWhiteSpace(source))
                throw new ArgumentNullException(
                    "source", "Firewall rule source must not be empty");

            IPType = ipType;
            Protocol = protocol;
            Source = source;
            Ports = new List<int>(ports);
        }
    }
}
