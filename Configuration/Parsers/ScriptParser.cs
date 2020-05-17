using agrix.Extensions;
using System;
using YamlDotNet.RepresentationModel;

namespace agrix.Configuration.Parsers
{
    /// <summary>
    /// Parses a script configuration.
    /// </summary>
    internal class ScriptParser
    {
        /// <summary>
        /// Creates a Script instance from a YAML configuration.
        /// </summary>
        /// <param name="node">The YAML configuration to parse.</param>
        /// <returns>The Script instance parsed from the given YAML.</returns>
        public virtual Script Parse(YamlNode node)
        {
            if (node.NodeType != YamlNodeType.Mapping)
                throw new InvalidCastException(
                    $"script item must be a mapping type (line {node.Start.Line}");

            var scriptItem = (YamlMappingNode)node;
            var typeName = scriptItem.GetKey("type", required: true);
            var type = (typeName.ToLower()) switch
            {
                "boot" => ScriptType.Boot,
                "pxe" => ScriptType.PXE,
                _ => throw new ArgumentException(
                    $"{typeName} is not a known type (line " +
                    $"{scriptItem.GetNode("type").Start.Line})")
            };

            return new Script(
                scriptItem.GetKey("name", required: true),
                type,
                scriptItem.GetKey("content", required: true)
            );
        }
    }
}
