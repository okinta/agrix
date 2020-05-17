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
                    $"server item must be a mapping type (line {node.Start.Line}");

            var serverItem = (YamlMappingNode)node;
            var osMapping = serverItem.GetMapping("os");
            var os = new OperatingSystem(
                osMapping.GetKey("app"),
                osMapping.GetKey("iso"),
                osMapping.GetKey("name")
            );

            var planMapping = serverItem.GetMapping("plan");
            var plan = new Plan(
                planMapping.GetInt("cpu"),
                planMapping.GetInt("memory"),
                planMapping.GetKey("type", required: true)
            );

            return new Server(
                os,
                plan,
                serverItem.GetKey("region", required: true),
                serverItem.GetBool("private-networking"),
                serverItem.GetKey("firewall"),
                serverItem.GetKey("label"),
                serverItem.GetKey("startup-script"),
                serverItem.GetKey("tag"),
                serverItem.GetJson("userdata"),
                serverItem.GetList("ssh-keys").ToArray()
            );
        }
    }
}
