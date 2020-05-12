using agrix.Extensions;
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
    }
}
