using agrix.Configuration;
using System.Collections.Generic;
using YamlDotNet.RepresentationModel;

namespace agrix.Platform
{
    /// <summary>
    /// Represents a platform that can be configured via code.
    /// </summary>
    internal interface IPlatform
    {
        /// <summary>
        /// Loads all the server configurations from the given YAML.
        /// </summary>
        /// <param name="config">The YAML list to load servers from.</param>
        /// <returns>The list of Server configurations loaded from the given
        /// YAML.</returns>
        public IList<Server> LoadServers(YamlSequenceNode serverItems);
    }
}
