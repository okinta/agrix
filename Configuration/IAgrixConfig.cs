using System.Collections.Generic;
using YamlDotNet.RepresentationModel;

namespace agrix.Configuration
{
    /// <summary>
    /// Describes an interface to load configuration from YAML.
    /// </summary>
    internal interface IAgrixConfig
    {
        /// <summary>
        /// Loads Server configurations from YAML.
        /// </summary>
        /// <param name="node">The YAML node to load configuration from.</param>
        /// <returns>The list of Server configurations loaded from the given
        /// YAML.</returns>
        public IList<Server> LoadServers(YamlNode node);

        /// <summary>
        /// Loads Script configurations from YAML.
        /// </summary>
        /// <param name="node">The YAML node to load configuration from.</param>
        /// <returns>The list of Script configurations loaded from the given
        /// YAML.</returns>
        public IList<Script> LoadScripts(YamlNode node);

        /// <summary>
        /// Loads Firewall configurations from YAML.
        /// </summary>
        /// <param name="node">The YAML node to load configuration from.</param>
        /// <returns>The list of Firewall configurations loaded from the given
        /// YAML.</returns>
        public IList<Firewall> LoadFirewalls(YamlNode node);
    }
}
