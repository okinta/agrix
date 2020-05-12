using agrix.Extensions;
using agrix.Platform;
using System.Collections.Generic;
using System;
using YamlDotNet.RepresentationModel;

namespace agrix.Configuration
{
    /// <summary>
    /// Represents infrastructure to be initialized.
    /// </summary>
    internal static class InfrastructureConfiguration
    {
        /// <summary>
        /// Loads all the server configurations from the given YAML.
        /// </summary>
        /// <param name="config">The YAML to load servers from.</param>
        /// <returns>The list of Server configurations loaded from the given
        /// YAML.</returns>
        public static IList<Server> LoadServers(YamlStream config)
        {
            var mapping = (YamlMappingNode)config.Documents[0].RootNode;
            var platformName = mapping.GetKey("platform", required: true);

            var platform = platformName switch
            {
                "vultr" => new Vultr(),
                _ => throw new ArgumentException(
                    string.Format("unknown platform: {0}", platformName), "config"
                ),
            };

            var serverItems = (YamlSequenceNode)mapping.Children[
                new YamlScalarNode("servers")];
            return platform.LoadServers(serverItems);
        }
    }
}
