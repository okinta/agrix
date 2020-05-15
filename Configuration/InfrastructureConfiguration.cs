using agrix.Extensions;
using agrix.Platforms.Vultr;
using agrix.Platforms;
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
        /// Loads the platform configuration from the given YAML.
        /// </summary>
        /// <param name="config">The YAML to load the platform configuration from.</param>
        /// <param name="apiKey">The API key to use for communicating with the
        /// platform.</param>
        /// <returns>The platform configuration loaded from the given YAML.</returns>
        /// <exception cref="KeyNotFoundException">If the platform key is not present
        /// inside <paramref name="config"/>.</exception>
        /// <exception cref="ArgumentException">If the platform is not
        /// supported.</exception>
        public static IPlatform LoadPlatform(YamlStream config, string apiKey)
        {
            if (config.Documents.Count == 0)
                throw new ArgumentException(
                    "config", "YAML config must have a document root");

            var mapping = (YamlMappingNode)config.Documents[0].RootNode;
            var platform = mapping.GetKey("platform", required: true);

            return platform switch
            {
                "vultr" => new VultrPlatform(apiKey),
                _ => throw new ArgumentException(
                    string.Format("Unknown platform: {0}", platform), "config"),
            };
        }

        /// <summary>
        /// Loads all the server configurations from the given YAML.
        /// </summary>
        /// <param name="yamlConfig">The YAML to load servers from.</param>
        /// <param name="agrixConfig">The IAgrixConfig instance to use to load the YAML
        /// config.</param>
        /// <returns>The list of Server configurations loaded from the given
        /// YAML.</returns>
        /// <exception cref="ArgumentException">If the configuration is
        /// invalid.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="yamlConfig"/> or
        /// <paramref name="agrixConfig"/> are null</exception>
        public static IList<Server> LoadServers(
            YamlStream yamlConfig, IAgrixConfig agrixConfig)
        {
            if (agrixConfig is null)
                throw new ArgumentNullException("agrixConfig", "must not be null");

            return agrixConfig.LoadServers(yamlConfig.GetRootNode());
        }

        /// <summary>
        /// Loads all the script configurations from the given YAML.
        /// </summary>
        /// <param name="yamlConfig">The YAML to load scripts from.</param>
        /// <param name="agrixConfig">The IAgrixConfig instance to use to load the YAML
        /// config.</param>
        /// <returns>The list of Script configurations loaded from the given
        /// YAML.</returns>
        /// <exception cref="ArgumentException">If the configuration is
        /// invalid.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="yamlConfig"/> or
        /// <paramref name="agrixConfig"/> are null</exception>
        public static IList<Script> LoadScripts(
            YamlStream yamlConfig, IAgrixConfig agrixConfig)
        {
            if (agrixConfig is null)
                throw new ArgumentNullException("agrixConfig", "must not be null");

            return agrixConfig.LoadScripts(yamlConfig.GetRootNode());
        }

        /// <summary>
        /// Loads all the firewall configurations from the given YAML.
        /// </summary>
        /// <param name="yamlConfig">The YAML to load firewalls from.</param>
        /// <param name="agrixConfig">The IAgrixConfig instance to use to load the YAML
        /// config.</param>
        /// <returns>The list of Firewall configurations loaded from the given
        /// YAML</returns>
        /// <exception cref="ArgumentException">If the configuration is
        /// invalid.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="yamlConfig"/> or
        /// <paramref name="agrixConfig"/> are null</exception>
        public static IList<Firewall> LoadFirewalls(
            YamlStream yamlConfig, IAgrixConfig agrixConfig)
        {
            if (agrixConfig is null)
                throw new ArgumentNullException("agrixConfig", "must not be null");

            return agrixConfig.LoadFirewalls(yamlConfig.GetRootNode());
        }

        private static YamlMappingNode GetRootNode(this YamlStream yamlConfig)
        {
            if (yamlConfig is null)
                throw new ArgumentNullException("yamlConfig", "must not be null");

            if (yamlConfig.Documents.Count == 0)
                throw new ArgumentException(
                    "config", "YAML config must have a document root");

            return (YamlMappingNode)yamlConfig.Documents[0].RootNode;
        }
    }
}
