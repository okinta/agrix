using agrix.Configuration.Parsers;
using agrix.Exceptions;
using agrix.Extensions;
using System.Collections.Generic;
using System;
using YamlDotNet.RepresentationModel;

namespace agrix.Configuration
{
    /// <summary>
    /// Creates an instance from a YAML configuration.
    /// </summary>
    /// <typeparam name="T">The type of instance to create.</typeparam>
    /// <param name="node">The YAML configuration to parse.</param>
    /// <returns>The instance parsed from the given YAML.</returns>
    internal delegate T Parse<T>(YamlNode node);

    /// <summary>
    /// Describes methods for the IAgrixConfig to load configuration from YAML. Can be
    /// used as a starting point to customize loading of configurations for individual
    /// platforms.
    /// </summary>
    internal abstract class AgrixConfig : IAgrixConfig
    {
        /// <summary>
        /// Delegate used to create a Firewall instance from a YAML configuration. Can
        /// be overridden in subclasses.
        /// </summary>
        protected Parse<Firewall> ParseFirewall { get; set; } = new FirewallParser().Parse;

        /// <summary>
        /// Delegate used to create a Script instance from a YAML configuration. Can
        /// be overridden in subclasses.
        /// </summary>
        protected Parse<Script> ParseScript { get; set; } = new ScriptParser().Parse;

        /// <summary>
        /// Delegate used to create a Server instance from a YAML configuration. Can
        /// be overridden in subclasses.
        /// </summary>
        protected Parse<Server> ParseServer { get; set; } = new ServerParser().Parse;

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
        public virtual IList<Server> LoadServers(YamlMappingNode node)
        {
            return Load("servers", node, ParseServer);
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
        public virtual IList<Script> LoadScripts(YamlMappingNode node)
        {
            return Load("scripts", node, ParseScript);
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
        public virtual IList<Firewall> LoadFirewalls(YamlMappingNode node)
        {
            return Load("firewalls", node, ParseFirewall);
        }

        /// <summary>
        /// Loads an instance from a YAML configuration.
        /// </summary>
        /// <typeparam name="T">The type of instance to load.</typeparam>
        /// <param name="name">The name of the node to load instances from.</param>
        /// <param name="node">The YAML node to load configuration from.</param>
        /// <param name="parse">The delegate to use to parse the configuration.</param>
        /// <returns>The list of parsed configurations loaded from the given YAML.</returns>
        /// <exception cref="ArgumentNullException">If any arguments are null.</exception>
        protected virtual IList<T> Load<T>(string name, YamlMappingNode node, Parse<T> parse)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name", "name must not be empty");

            if (node is null)
                throw new ArgumentNullException("node", "node must not be empty");

            if (parse is null)
                throw new ArgumentNullException("parse", "parse must not be empty");

            var items = new List<T>();
            var nodeItems = node.GetSequence(name, required: false);

            // If servers are empty, return an empty list
            if (nodeItems is null) return items;

            foreach (var nodeItem in nodeItems)
                items.Add(parse(nodeItem));

            return items;
        }
    }
}
