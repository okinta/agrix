using agrix.Configuration.Parsers;
using agrix.Exceptions;
using agrix.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text.RegularExpressions;
using System;
using YamlDotNet.RepresentationModel;

namespace agrix.Configuration
{
    /// <summary>
    /// Describes methods for the IAgrixConfig to load configuration from YAML. Can be
    /// used as a starting point to customize loading of configurations for individual
    /// platforms.
    /// </summary>
    internal abstract class AgrixConfig : IAgrixConfig
    {
        /// <summary>
        /// Loads Server configurations from YAML.
        /// </summary>
        /// <param name="node">The YAML node to load configuration from.</param>
        /// <returns>The list of Server configurations loaded from the given
        /// YAML.</returns>
        /// <exception cref="ArgumentException">If the servers property is not a
        /// list.</exception>
        /// <exception cref="KnownKeyNotFoundException{string}">If a required key is not
        /// present in the YAML.</exception>
        /// <exception cref="InvalidCastException">If a YAML key is in an invalid
        /// format.</exception>
        public IList<Server> LoadServers(YamlMappingNode node)
        {
            var servers = new List<Server>();
            var serverItems = node.GetSequence("servers", required: false);

            // If servers are empty, return an empty list
            if (serverItems is null) return servers;

            var parser = new ServerParser();

            foreach (var serverItemNode in serverItems)
                servers.Add(parser.Parse(serverItemNode));

            return servers;
        }

        /// <summary>
        /// Loads Script configurations from YAML.
        /// </summary>
        /// <param name="node">The YAML node to load configuration from.</param>
        /// <returns>The list of Script configurations loaded from the given
        /// YAML.</returns>
        /// <exception cref="ArgumentException">If the scripts property is not a
        /// list.</exception>
        /// <exception cref="KnownKeyNotFoundException{string}">If a required key is not
        /// present in the YAML.</exception>
        /// <exception cref="InvalidCastException">If a YAML key is in an invalid
        /// format.</exception>
        public IList<Script> LoadScripts(YamlMappingNode node)
        {
            var scripts = new List<Script>();
            var scriptItems = node.GetSequence("scripts", required: false);

            // If scripts are empty, return an empty list
            if (scriptItems is null) return scripts;

            var parser = new ScriptParser();

            foreach (var scriptItemNode in scriptItems)
                scripts.Add(parser.Parse(scriptItemNode));

            return scripts;
        }

        /// <summary>
        /// Loads Firewall configurations from YAML.
        /// </summary>
        /// <param name="node">The YAML node to load configuration from.</param>
        /// <returns>The list of Firwall configurations loaded from the given
        /// YAML.</returns>
        /// <exception cref="ArgumentException">If the scripts property is not a
        /// list.</exception>
        /// <exception cref="KnownKeyNotFoundException{string}">If a required key is not
        /// present in the YAML.</exception>
        /// <exception cref="InvalidCastException">If a YAML key is in an invalid
        /// format.</exception>
        public IList<Firewall> LoadFirewalls(YamlMappingNode node)
        {
            var firwalls = new List<Firewall>();
            var firwallItems = node.GetSequence("firewalls", required: false);

            // If firwalls are empty, return an empty list
            if (firwallItems is null) return firwalls;

            foreach (var firwallItemNode in firwallItems)
            {
                if (firwallItemNode.NodeType != YamlNodeType.Mapping)
                    throw new InvalidCastException(
                        string.Format("script item must be a mapping type (line {0}",
                            firwallItemNode.Start.Line));

                var firewallItem = (YamlMappingNode)firwallItemNode;
                var name = firewallItem.GetKey("name");

                var rules = new List<FirewallRule>();
                foreach (var childNode in firewallItem.GetSequence("rules", required: true))
                {
                    if (childNode.NodeType != YamlNodeType.Mapping)
                        throw new ArgumentException(string.Format(
                            "rule must be a mapping node (line {0})",
                            childNode.Start.Line));

                    var ruleNode = (YamlMappingNode)childNode;
                    var protocol = GetProtocol(ruleNode);
                    var source = GetSource(ruleNode);
                    var ports = GetPorts(ruleNode);

                    if (!string.IsNullOrEmpty(source.Subnet))
                        rules.Add(new FirewallRule(
                            source.IPType, protocol, ports, source.Subnet, source.SubnetSize
                        ));

                    else
                        rules.Add(new FirewallRule(
                            source.IPType, protocol, ports, source.Source
                        ));
                }

                firwalls.Add(new Firewall(name, rules));
            }

            return firwalls;
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
