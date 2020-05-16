using agrix.Extensions;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text.RegularExpressions;
using System;
using YamlDotNet.RepresentationModel;

namespace agrix.Configuration.Parsers
{
    /// <summary>
    /// Parses a firewall rule configuration.
    /// </summary>
    internal class FirewallRuleParser
    {
        /// <summary>
        /// Creates a FirewallRule instance from a YAML configuration.
        /// </summary>
        /// <param name="node">The YAML configuration to parse.</param>
        /// <returns>The FirewallRule instance parsed from the given YAML.</returns>
        public virtual FirewallRule Parse(YamlNode node)
        {
            if (node.NodeType != YamlNodeType.Mapping)
                throw new ArgumentException(string.Format(
                    "rule must be a mapping node (line {0})",
                    node.Start.Line));

            var ruleNode = (YamlMappingNode)node;
            var protocol = GetProtocol(ruleNode);
            var source = GetSource(ruleNode);
            var ports = GetPorts(ruleNode);

            if (!string.IsNullOrEmpty(source.Subnet))
                return new FirewallRule(
                    source.IPType, protocol, ports, source.Subnet, source.SubnetSize
                );

            else
                return new FirewallRule(
                    source.IPType, protocol, ports, source.Source
                );
        }

        private static Protocol GetProtocol(YamlMappingNode node)
        {
            var protocolName = node.GetKey("protocol", required: true);
            return protocolName.ToLower() switch
            {
                "udp" => Protocol.UDP,
                "tcp" => Protocol.TCP,
                "gre" => Protocol.GRE,
                "icmp" => Protocol.ICMP,
                _ => throw new ArgumentException(string.Format(
                    "{0} is not a known protocol (line {1})",
                        protocolName, node.GetNode("protocol").Start.Line))
            };
        }

        private static SourceResult GetSource(YamlMappingNode node)
        {
            var source = node.GetKey("source", required: true);
            if (!source.Contains('/'))
            {
                if (source.ToLower() == "cloudflare")
                    return new SourceResult("cloudflare");
            }
            else
            {
                var split = source.Split('/');
                if (split.Count() == 2)
                {
                    var ip = split[0];

                    if (!int.TryParse(split[1], out int size))
                        throw new ArgumentException(string.Format(
                            "{0} invalid subnet size (line {1})",
                            source, node.GetNode("source").Start.Line));

                    if (IPAddress.TryParse(ip, out IPAddress address))
                    {
                        return address.AddressFamily switch
                        {
                            AddressFamily.InterNetwork =>
                                new SourceResult(IPType.V4, ip, size),
                            AddressFamily.InterNetworkV6 =>
                                new SourceResult(IPType.v6, ip, size),
                            _ => throw new ArgumentException(string.Format(
                               "{0} is not a supported IP type (line {1})",
                               address.AddressFamily.ToString(),
                               node.GetNode("source").Start.Line))
                        };
                    }
                }
            }

            throw new ArgumentException(string.Format(
                "{0} is not a known source (line {1})",
                    source, node.GetNode("source").Start.Line));
        }

        private static string GetPorts(YamlMappingNode node)
        {
            if (!string.IsNullOrEmpty(node.GetKey("port"))
                && !string.IsNullOrEmpty(node.GetKey("ports")))
                throw new ArgumentException(string.Format(
                    "Set either port or ports property, not both (line {0} and {1})",
                    node.GetNode("port").Start.Line,
                    node.GetNode("ports").Start.Line));

            var portsProperty = "port";
            var ports = node.GetKey("port");
            if (string.IsNullOrEmpty(ports))
            {
                ports = node.GetKey("ports");
                portsProperty = "ports";
            }

            var portsPropertyLine = node.GetNode(portsProperty).Start.Line;

            if (string.IsNullOrEmpty(ports))
                throw new ArgumentException(string.Format(
                    "Port need to be set in rules (line {0})",
                    node.Start.Line));

            if (int.TryParse(ports, out int port))
                return port.ToString();

            var matches = Regex.Match(ports, "([0-9]+) *[:-]{1,2} *([0-9]+)");
            if (!matches.Success)
                throw new ArgumentException(string.Format(
                    "Cannot parse ports property (line {0})", portsPropertyLine));

            var portStart = int.Parse(matches.Groups[1].Value);
            var portEnd = int.Parse(matches.Groups[2].Value);

            if (portStart >= portEnd)
                throw new ArgumentException(string.Format(
                    "Is port range in reverse? (line {0})", portsPropertyLine));

            return string.Format("{0}:{1}", portStart, portEnd);
        }
    }

    internal struct SourceResult
    {
        public IPType IPType { get; }
        public string Source { get; }
        public string Subnet { get; }
        public int SubnetSize { get; }

        public SourceResult(string source)
        {
            IPType = IPType.V4;
            Source = source;
            Subnet = "";
            SubnetSize = 0;
        }

        public SourceResult(IPType ipType, string subnet, int subnetSize)
        {
            IPType = ipType;
            Source = "";
            Subnet = subnet;
            SubnetSize = subnetSize;
        }
    }
}
