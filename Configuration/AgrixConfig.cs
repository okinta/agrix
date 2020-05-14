using agrix.Exceptions;
using agrix.Extensions;
using System.Collections.Generic;
using System.Linq;
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
            var serverItems = node.Children[new YamlScalarNode("servers")];

            // If servers are empty, return an empty list
            if (serverItems.NodeType == YamlNodeType.Scalar &&
                string.IsNullOrEmpty((string)serverItems)) return servers;

            if (serverItems.NodeType != YamlNodeType.Sequence)
                throw new ArgumentException(
                    string.Format(
                        "servers property (line {0}) must be a list",
                        serverItems.Start.Line)
                    , "config");

            foreach (YamlMappingNode serverItem in (YamlSequenceNode)serverItems)
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
                    userData: serverItem.GetJSON("userdata"),
                    sshKeys: serverItem.GetList("ssh-keys").ToArray()
                ));
            }

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
            var scriptItems = node.Children[new YamlScalarNode("servers")];

            // If servers are empty, return an empty list
            if (scriptItems.NodeType == YamlNodeType.Scalar &&
                string.IsNullOrEmpty((string)scriptItems)) return scripts;

            if (scriptItems.NodeType != YamlNodeType.Sequence)
                throw new ArgumentException("scripts property must be a list", "config");

            foreach (YamlMappingNode scriptItem in (YamlSequenceNode)scriptItems)
            {
                scripts.Add(new Script(
                    name: scriptItem.GetKey("name", required: true),
                    type: scriptItem.GetKey("type", required: true),
                    content: scriptItem.GetKey("content", required: true)
                ));
            }

            return scripts;
        }
    }
}
