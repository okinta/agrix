using agrix.Configuration.Parsers;
using agrix.Exceptions;
using agrix.Extensions;
using System.Collections.Generic;
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

            var parser = new FirewallParser();

            foreach (var firwallItemNode in firwallItems)
                firwalls.Add(parser.Parse(firwallItemNode));

            return firwalls;
        }
    }
}
