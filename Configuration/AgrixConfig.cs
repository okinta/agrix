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
            var serverItems = node.GetSequence("servers", required: false);

            // If servers are empty, return an empty list
            if (serverItems is null) return servers;

            foreach (var serverItemNode in serverItems)
            {
                if (serverItemNode.NodeType != YamlNodeType.Mapping)
                    throw new InvalidCastException(
                        string.Format("server item must be a mapping type (line {0}",
                            serverItemNode.Start.Line));

                var serverItem = (YamlMappingNode)serverItemNode;
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
            var scriptItems = node.GetSequence("scripts", required: false);

            // If scripts are empty, return an empty list
            if (scriptItems is null) return scripts;

            foreach (var scriptItemNode in scriptItems)
            {
                if (scriptItemNode.NodeType != YamlNodeType.Mapping)
                    throw new InvalidCastException(
                        string.Format("script item must be a mapping type (line {0}",
                            scriptItemNode.Start.Line));

                var scriptItem = (YamlMappingNode)scriptItemNode;
                var typeName = scriptItem.GetKey("type", required: true);
                var type = (typeName.ToLower()) switch
                {
                    "boot" => ScriptType.Boot,
                    "pxe" => ScriptType.PXE,
                    _ => throw new ArgumentException(string.Format(
                        "{0} is not a known type (line {1})",
                        typeName, scriptItem.GetNode("type").Start.Line)),
                };

                scripts.Add(new Script(
                    name: scriptItem.GetKey("name", required: true),
                    type: type,
                    content: scriptItem.GetKey("content", required: true)
                ));
            }

            return scripts;
        }
    }
}
