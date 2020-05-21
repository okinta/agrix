using agrix.Extensions;
using System;
using System.Linq;
using YamlDotNet.RepresentationModel;

namespace agrix.Configuration.Parsers
{
    /// <summary>
    /// Parses a firewall configuration.
    /// </summary>
    internal class FirewallParser
    {
        /// <summary>
        /// Delegate used to create a FirewallRule instance from a YAML configuration. Can
        /// be overridden in subclasses.
        /// </summary>
        protected Parse<FirewallRule> ParseRule = new FirewallRuleParser().Parse;

        /// <summary>
        /// Creates a Firewall instance from a YAML configuration.
        /// </summary>
        /// <param name="node">The YAML configuration to parse.</param>
        /// <returns>The Firewall instance parsed from the given YAML.</returns>
        public virtual Firewall Parse(YamlNode node)
        {
            if (node.NodeType != YamlNodeType.Mapping)
                throw new InvalidCastException(
                    $"script item must be a mapping type (line {node.Start.Line}");

            var firewallItem = (YamlMappingNode)node;
            var name = firewallItem.GetKey("name");

            var rules = firewallItem.GetSequence("rules")
                .Select(childNode => ParseRule(childNode)).ToList();
            return new Firewall(name, rules.ToArray());
        }
    }
}
