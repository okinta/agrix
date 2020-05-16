using agrix.Extensions;
using System.Linq;
using System;
using YamlDotNet.RepresentationModel;

namespace agrix.Configuration.Parsers
{
    /// <summary>
    /// Parses a server configuration.
    /// </summary>
    internal class ServerParser
    {
        /// <summary>
        /// Creates a Server instance from a YAML configuration.
        /// </summary>
        /// <param name="node">The YAML configuration to parse.</param>
        /// <returns>The Server instance parsed from the given YAML.</returns>
        public virtual Server Parse(YamlNode node)
        {
            if (node.NodeType != YamlNodeType.Mapping)
                throw new InvalidCastException(
                    string.Format("server item must be a mapping type (line {0}",
                        node.Start.Line));

            var serverItem = (YamlMappingNode)node;
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

            return new Server(
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
            );
        }
    }
}
