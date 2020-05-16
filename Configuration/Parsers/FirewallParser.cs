using agrix.Extensions;
using System.Collections.Generic;
using System;
using YamlDotNet.RepresentationModel;

namespace agrix.Configuration.Parsers
{
    /// <summary>
    /// Creates a FirewallRule instance from a YAML configuration.
    /// </summary>
    /// <param name="node">The YAML configuration to parse.</param>
    /// <returns>The FirewallRule instance parsed from the given YAML.</returns>
    internal delegate FirewallRule ParseFirewallRule(YamlNode node);

    /// <summary>
    /// Parses a firewall configuration.
    /// </summary>
    internal class FirewallParser
    {
        /// <summary>
        /// Delegate used to create a FirewallRule instance from a YAML configuration. Can
        /// be overridden in subclasses.
        /// </summary>
        protected ParseFirewallRule ParseRule = new FirewallRuleParser().Parse;

        /// <summary>
        /// Creates a Firewall instance from a YAML configuration.
        /// </summary>
        /// <param name="node">The YAML configuration to parse.</param>
        /// <returns>The Firewall instance parsed from the given YAML.</returns>
        public virtual Firewall Parse(YamlNode node)
        {
            if (node.NodeType != YamlNodeType.Mapping)
                throw new InvalidCastException(
                    string.Format("script item must be a mapping type (line {0}",
                        node.Start.Line));

            var firewallItem = (YamlMappingNode)node;
            var name = firewallItem.GetKey("name");

            var rules = new List<FirewallRule>();

            foreach (var childNode in firewallItem.GetSequence("rules", required: true))
                rules.Add(ParseRule(childNode));

            return new Firewall(name, rules);
        }
    }
}
