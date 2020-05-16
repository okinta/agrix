using agrix.Configuration.Parsers;
using agrix.Exceptions;
using agrix.Extensions;
using System.Collections.Generic;
using System;
using YamlDotNet.RepresentationModel;

namespace agrix.Configuration
{
    /// <summary>
    /// Creates a Firewall instance from a YAML configuration.
    /// </summary>
    /// <param name="node">The YAML configuration to parse.</param>
    /// <returns>The Firewall instance parsed from the given YAML.</returns>
    internal delegate Firewall ParseFirewall(YamlNode node);

    /// <summary>
    /// Creates a Script instance from a YAML configuration.
    /// </summary>
    /// <param name="node">The YAML configuration to parse.</param>
    /// <returns>The Script instance parsed from the given YAML.</returns>
    internal delegate Script ParseScript(YamlNode node);

    /// <summary>
    /// Creates a Server instance from a YAML configuration.
    /// </summary>
    /// <param name="node">The YAML configuration to parse.</param>
    /// <returns>The Server instance parsed from the given YAML.</returns>
    internal delegate Server ParseServer(YamlNode node);

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
        protected ParseFirewall ParseFirewall { get; set; } = new FirewallParser().Parse;

        /// <summary>
        /// Delegate used to create a Script instance from a YAML configuration. Can
        /// be overridden in subclasses.
        /// </summary>
        protected ParseScript ParseScript { get; set; } = new ScriptParser().Parse;

        /// <summary>
        /// Delegate used to create a Server instance from a YAML configuration. Can
        /// be overridden in subclasses.
        /// </summary>
        protected ParseServer ParseServer { get; set; } = new ServerParser().Parse;

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
            var servers = new List<Server>();
            var serverItems = node.GetSequence("servers", required: false);

            // If servers are empty, return an empty list
            if (serverItems is null) return servers;

            foreach (var serverItemNode in serverItems)
                servers.Add(ParseServer(serverItemNode));

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
        public virtual IList<Script> LoadScripts(YamlMappingNode node)
        {
            var scripts = new List<Script>();
            var scriptItems = node.GetSequence("scripts", required: false);

            // If scripts are empty, return an empty list
            if (scriptItems is null) return scripts;

            foreach (var scriptItemNode in scriptItems)
                scripts.Add(ParseScript(scriptItemNode));

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
        public virtual IList<Firewall> LoadFirewalls(YamlMappingNode node)
        {
            var firwalls = new List<Firewall>();
            var firwallItems = node.GetSequence("firewalls", required: false);

            // If firwalls are empty, return an empty list
            if (firwallItems is null) return firwalls;

            foreach (var firwallItemNode in firwallItems)
                firwalls.Add(ParseFirewall(firwallItemNode));

            return firwalls;
        }
    }
}
