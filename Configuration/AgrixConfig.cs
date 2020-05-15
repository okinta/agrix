using agrix.Exceptions;
using agrix.Extensions;
using System.Collections.Generic;
using System.Linq;
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

            foreach (var serverItemNode in serverItems)
            {
                if (serverItemNode.NodeType != YamlNodeType.Mapping)
                    throw new InvalidCastException(
                        string.Format("server item must be a mapping type (line {0}",
                            serverItemNode.Start.Line));

                var serverItem = (YamlMappingNode)serverItemNode;
                var osMapping = serverItem.GetMapping("os");
                var os = new OperatingSystem(
                    app: osMapping.GetKey("app"),
                    iso: osMapping.GetKey("iso"),
                    name: osMapping.GetKey("name")
                );

                var planMapping = serverItem.GetMapping("plan");
                var plan = new Plan(
                    cpu: planMapping.GetInt("cpu"),
                    memory: planMapping.GetInt("memory"),
                    type: planMapping.GetKey("type", required: true)
                );

                servers.Add(new Server(
                    os: os,
                    plan: plan,
                    region: serverItem.GetKey("region", required: true),
                    privateNetworking: serverItem.GetBool("private-networking", false),
                    firewall: serverItem.GetKey("firewall"),
                    label: serverItem.GetKey("label"),
                    startupScript: serverItem.GetKey("startup-script"),
                    tag: serverItem.GetKey("tag"),
                    userData: serverItem.GetJSON("userdata"),
                    sshKeys: serverItem.GetList("ssh-keys").ToArray()
                ));
            }

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

            foreach (var scriptItemNode in scriptItems)
            {
                if (scriptItemNode.NodeType != YamlNodeType.Mapping)
                    throw new InvalidCastException(
                        string.Format("script item must be a mapping type (line {0}",
                            scriptItemNode.Start.Line));

                var scriptItem = (YamlMappingNode)scriptItemNode;
                var typeName = scriptItem.GetKey("type", required: true);
                var type = (typeName.ToLower()) switch
                {
                    "boot" => ScriptType.Boot,
                    "pxe" => ScriptType.PXE,
                    _ => throw new ArgumentException(string.Format(
                        "{0} is not a known type (line {1})",
                        typeName, scriptItem.GetNode("type").Start.Line)),
                };

                scripts.Add(new Script(
                    name: scriptItem.GetKey("name", required: true),
                    type: type,
                    content: scriptItem.GetKey("content", required: true)
                ));
            }

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
                    var ipTypeName = ruleNode.GetKey("ip-type", required: true);
                    var ipType = (ipTypeName.ToLower()) switch
                    {
                        "v4" => IPType.V4,
                        "v6" => IPType.v6,
                        _ => throw new ArgumentException(string.Format(
                            "{0} is not a known IP type (line {1})",
                                ipTypeName, ruleNode.GetNode("ip-type").Start.Line))
                    };

                    var protocolName = ruleNode.GetKey("protocol", required: true);
                    var protocol = (protocolName.ToLower()) switch
                    {
                        "udp" => Protocol.UDP,
                        "tcp" => Protocol.TCP,
                        "gre" => Protocol.GRE,
                        "icmp" => Protocol.ICMP,
                        _ => throw new ArgumentException(string.Format(
                            "{0} is ot a known protocol (line {1})",
                                protocolName, ruleNode.GetNode("protocol").Start.Line))
                    };

                    var source = ruleNode.GetKey("source", required: true);
                    if (source != "cloudflare"
                        && !Regex.IsMatch(source, @"[0-9]+\.[0-9]+\.[0-9]+\.[0-9]+/[0-9]+"))
                        throw new ArgumentException(string.Format(
                            "Either port or ports need to be set in rules (line {0}",
                            ruleNode.GetNode("source").Start.Line));

                    var portField = ruleNode.GetInt("port", 0);
                    var portsField = ruleNode.GetKey("ports");

                    if (portField == 0 && string.IsNullOrEmpty(portsField))
                        throw new ArgumentException(string.Format(
                            "Either port or ports need to be set in rules (line {0})",
                            ruleNode.Start.Line));

                    if (portField != 0 && !string.IsNullOrEmpty(portsField))
                        throw new ArgumentException(string.Format(
                            "Set either port or ports property, not both (line {0} and {1})",
                            ruleNode.GetNode("port").Start.Line,
                            ruleNode.GetNode("ports").Start.Line));

                    if (portField < 0)
                        throw new ArgumentOutOfRangeException(
                            string.Format("Port must be larger than 0",
                            ruleNode.GetNode("port").Start.Line));

                    List<int> ports = new List<int>();

                    if (portField > 0)
                        ports.Add(portField);

                    else
                    {
                        var parsedPorts = ParsePorts(portsField);
                        if (parsedPorts is null)
                            throw new ArgumentException(string.Format(
                                "Cannot parse ports property (line {0})",
                                ruleNode.GetNode("ports").Start.Line));

                        ports = new List<int>(parsedPorts);
                    }

                    rules.Add(new FirewallRule(
                        ipType, protocol, source, ports
                    ));
                }

                firwalls.Add(new Firewall(name, rules));
            }

            return firwalls;
        }

        private static IEnumerable<int> ParsePorts(string portsProperty)
        {
            var splitPorts = portsProperty.Split('-');
            if (splitPorts.Count() != 2)
                splitPorts = portsProperty.Split(':');

            if (splitPorts.Count() != 2) return null;

            if (!int.TryParse(splitPorts[0].Trim(), out int portStart)) return null;

            if (!int.TryParse(splitPorts[1].Trim(), out int portEnd)) return null;

            if (portEnd <= portStart) return null;

            return Enumerable.Range(portStart, portEnd);
        }
    }
}
