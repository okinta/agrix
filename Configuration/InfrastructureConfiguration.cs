using agrix.Extensions;
using agrix.Platforms.Vultr;
using agrix.Platforms;
using System.Collections.Generic;
using System.Linq;
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
            var servers = new List<Server>();

            if (config.Documents.Count == 0)
            {
                throw new ArgumentException(
                    "config", "YAML config must have a document root");
            }

            var mapping = (YamlMappingNode)config.Documents[0].RootNode;
            var serverItems = (YamlSequenceNode)mapping.Children[
                new YamlScalarNode("servers")];
            
            foreach (YamlMappingNode serverItem in serverItems)
            {
                var osMapping = serverItem.GetMapping("os");
                var os = new OperatingSystem(
                    app: osMapping.GetKey("app"),
                    iso: osMapping.GetKey("iso"),
                    name: osMapping.GetKey("name")
                );

                var planMapping = serverItem.GetMapping("plan");
                var plan = new Plan(
                    cpu: planMapping.GetInt("cpu"),
                    memory: planMapping.GetInt("memory"),
                    type: planMapping.GetKey("type", required: true)
                );

                servers.Add(new Server(
                    os: os,
                    plan: plan,
                    region: serverItem.GetKey("region", required: true),
                    privateNetworking: serverItem.GetBool("private-networking", false),
                    firewall: serverItem.GetKey("firewall"),
                    label: serverItem.GetKey("label"),
                    startupScript: serverItem.GetKey("startup-script"),
                    tag: serverItem.GetKey("tag"),
                    userData: serverItem.GetKey("userdata"),
                    sshKeys: serverItem.GetList("ssh-keys").ToArray()
                ));
            }

            return servers;
        }

        /// <summary>
        /// Loads the platform configuration from the given YAML.
        /// </summary>
        /// <param name="config">The YAML to load the platform configuration from.</param>
        /// <param name="apiKey">The API key to use for communicating with the
        /// platform.</param>
        /// <returns>The platform configuration loaded from the given YAML.</returns>
        public static IPlatform LoadPlatform(YamlStream config, string apiKey)
        {
            if (config.Documents.Count == 0)
            {
                throw new ArgumentException(
                    "config", "YAML config must have a document root");
            }

            var mapping = (YamlMappingNode)config.Documents[0].RootNode;
            var platform = mapping.GetKey("platform", required: true);

            return platform switch
            {
                "vultr" => new VultrPlatform(apiKey),
                _ => throw new ArgumentException(
                    string.Format("unknown platform: {0}", platform), "config"),
            };
        }
    }
}
