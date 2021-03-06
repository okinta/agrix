﻿using System;

namespace agrix.Configuration
{
    /// <summary>
    /// Represents a type of IP.
    /// </summary>
    internal enum IpType
    {
        V4,
        V6
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
    internal readonly struct FirewallRule
    {
        /// <summary>
        /// Gets the type of IP this firewall rule is for.
        /// </summary>
        public IpType IpType { get; }

        /// <summary>
        /// Gets the protocol this firewall rule is for.
        /// </summary>
        public Protocol Protocol { get; }

        /// <summary>
        /// Gets the subnet this firewall rule applies to.
        /// </summary>
        public string Subnet { get; }

        /// <summary>
        /// Gets the size of the subnet this firewall rule applies to.
        /// </summary>
        public int SubnetSize { get; }

        /// <summary>
        /// Gets the source that's allowed through this firewall rule.
        /// </summary>
        public string Source { get; }

        /// <summary>
        /// Gets the ports affected by this firewall rule.
        /// </summary>
        public string Ports { get; }

        /// <summary>
        /// Creates a new firewall source rule.
        /// </summary>
        /// <param name="ipType">The type of IP the firewall rule applies to.</param>
        /// <param name="protocol">The type of protocol the firewall rule applies
        /// to.</param>
        /// <param name="ports">The ports affected by the firewall rule.</param>
        /// <param name="source">The source that's allowed through the firewall
        /// rule.</param>
        /// <exception cref="ArgumentNullException">If any arguments are null or
        /// empty.</exception>
        public FirewallRule(
            IpType ipType, Protocol protocol, string ports, string source)
        {
            if (string.IsNullOrWhiteSpace(ports))
                throw new ArgumentNullException(
                    nameof(ports), "Firewall rule ports must not be empty");

            if (string.IsNullOrWhiteSpace(source))
                throw new ArgumentNullException(
                    nameof(source), "Firewall rule source must not be empty");

            IpType = ipType;
            Ports = ports;
            Protocol = protocol;
            Source = source;
            Subnet = "0.0.0.0";
            SubnetSize = 0;
        }

        /// <summary>
        /// Creates a new firewall subnet rule.
        /// </summary>
        /// <param name="ipType">The type of IP the firewall rule applies to.</param>
        /// <param name="protocol">The type of protocol the firewall rule applies
        /// to.</param>
        /// <param name="ports">The ports affected by the firewall rule.</param>
        /// <param name="subnet">The subnet the firewall rule applies to.</param>
        /// <param name="subnetSize">The size of the subnet the firewall rule applies
        /// to.</param>
        /// <exception cref="ArgumentNullException">If any arguments are null or
        /// empty.</exception>
        public FirewallRule(
            IpType ipType, Protocol protocol, string ports, string subnet, int subnetSize)
        {
            if (string.IsNullOrWhiteSpace(ports))
                throw new ArgumentNullException(
                    nameof(ports), "Firewall rule ports must not be empty");

            if (string.IsNullOrWhiteSpace(subnet))
                throw new ArgumentNullException(
                    nameof(subnet), "Firewall rule source must not be empty");

            IpType = ipType;
            Ports = ports;
            Protocol = protocol;
            Source = "";
            Subnet = subnet;
            SubnetSize = subnetSize;
        }
    }
}
