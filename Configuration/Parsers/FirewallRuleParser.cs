using agrix.Extensions;
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
                throw new ArgumentException(
                    $"rule must be a mapping node (line {node.Start.Line})");

            var ruleNode = (YamlMappingNode)node;
            var protocol = GetProtocol(ruleNode);
            var source = GetSource(ruleNode);
            var ports = GetPorts(ruleNode);

            if (!string.IsNullOrEmpty(source.Subnet))
                return new FirewallRule(
                    source.IpType, protocol, ports, source.Subnet, source.SubnetSize
                );

            return new FirewallRule(
                source.IpType, protocol, ports, source.Source
            );
        }

        private static Protocol GetProtocol(YamlMappingNode node)
        {
            var protocolName = node.GetKey("protocol", required: true);
            var line = node.GetNode("protocol").Start.Line;
            return protocolName.ToLower() switch
            {
                "udp" => Protocol.UDP,
                "tcp" => Protocol.TCP,
                "gre" => Protocol.GRE,
                "icmp" => Protocol.ICMP,
                _ => throw new ArgumentException(
                    $"{protocolName} is not a known protocol (line {line})")
            };
        }

        private static SourceResult GetSource(YamlMappingNode node)
        {
            var source = node.GetKey("source", required: true);
            var line = node.GetNode("source").Start.Line;
            if (!source.Contains('/'))
            {
                if (source.ToLower() == "cloudflare")
                    return new SourceResult("cloudflare");
            }
            else
            {
                var split = source.Split('/');
                if (split.Length != 2)
                    throw new ArgumentException(
                        $"{source} is not a known source (line {line})");

                if (!int.TryParse(split[1], out var size))
                    throw new ArgumentException(
                        $"{source} invalid subnet size (line {line})");

                var ip = split[0];
                if (IPAddress.TryParse(ip, out var address))
                {
                    return address.AddressFamily switch
                    {
                        AddressFamily.InterNetwork =>
                        new SourceResult(IpType.V4, ip, size),
                        AddressFamily.InterNetworkV6 =>
                        new SourceResult(IpType.V6, ip, size),
                        _ => throw new ArgumentException(
                            $"{address.AddressFamily} is not a supported IP "
                            + $"type (line {line})")
                    };
                }
            }

            throw new ArgumentException(
                $"{source} is not a known source (line {line})");
        }

        private static string GetPorts(YamlMappingNode node)
        {
            if (!string.IsNullOrEmpty(node.GetKey("port"))
                && !string.IsNullOrEmpty(node.GetKey("ports")))
                throw new ArgumentException(
                    "Set either port or ports property, not both (line " +
                    $"{node.GetNode("port").Start.Line} and " +
                    $"{node.GetNode("ports").Start.Line})");

            var portsProperty = "port";
            var ports = node.GetKey("port");
            if (string.IsNullOrEmpty(ports))
            {
                ports = node.GetKey("ports");
                portsProperty = "ports";
            }

            var portsPropertyLine = node.GetNode(portsProperty).Start.Line;

            if (string.IsNullOrEmpty(ports))
                throw new ArgumentException(
                    $"Port need to be set in rules (line {node.Start.Line})");

            if (int.TryParse(ports, out var port))
                return port.ToString();

            var matches = Regex.Match(
                ports, "([0-9]+) *[:-]{1,2} *([0-9]+)");
            if (!matches.Success)
                throw new ArgumentException(
                    $"Cannot parse ports property (line {portsPropertyLine})");

            var portStart = int.Parse(matches.Groups[1].Value);
            var portEnd = int.Parse(matches.Groups[2].Value);

            if (portStart >= portEnd)
                throw new ArgumentException(
                    $"Is port range in reverse? (line {portsPropertyLine})");

            return $"{portStart}:{portEnd}";
        }
    }

    internal readonly struct SourceResult
    {
        public IpType IpType { get; }
        public string Source { get; }
        public string Subnet { get; }
        public int SubnetSize { get; }

        public SourceResult(string source)
        {
            IpType = IpType.V4;
            Source = source;
            Subnet = "";
            SubnetSize = 0;
        }

        public SourceResult(IpType ipType, string subnet, int subnetSize)
        {
            IpType = ipType;
            Source = "";
            Subnet = subnet;
            SubnetSize = subnetSize;
        }
    }
}
