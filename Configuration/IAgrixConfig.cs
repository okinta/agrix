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
        public IList<Server> LoadServers(YamlMappingNode node);
    }
}
