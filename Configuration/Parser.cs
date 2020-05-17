using System.Collections.Generic;
using System.Linq;
using System;
using YamlDotNet.RepresentationModel;

namespace agrix.Configuration
{
    /// <summary>
    /// Describes methods to load configuration from YAML.
    /// </summary>
    internal class Parser : IParser
    {
        /// <summary>
        /// Loads an instance from a YAML configuration.
        /// </summary>
        /// <typeparam name="T">The type of instance to load.</typeparam>
        /// <param name="name">The name of the node to load instances from.</param>
        /// <param name="node">The YAML node to load configuration from.</param>
        /// <param name="parse">The delegate to use to parse the configuration.</param>
        /// <returns>The list of parsed configurations loaded from the given YAML.</returns>
        /// <exception cref="ArgumentNullException">If any arguments are null.</exception>
        public virtual IList<T> Load<T>(string name, YamlNode node, Parse<T> parse)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(
                    nameof(name), "name must not be empty");

            if (node is null)
                throw new ArgumentNullException(
                    nameof(node), "node must not be empty");

            if (parse is null)
                throw new ArgumentNullException(
                    nameof(parse), "parse must not be empty");

            var items = new List<T>();

            // If items are empty, return an empty list
            if (node.NodeType == YamlNodeType.Scalar
                && string.IsNullOrEmpty(((YamlScalarNode)node).Value))
                return items;

            if (node.NodeType != YamlNodeType.Sequence)
                throw new ArgumentException("node",
                    $"{name} must be a sequence (line {node.Start.Line})");

            items.AddRange(from nodeItem in (YamlSequenceNode) node select parse(nodeItem));
            return items;
        }
    }
}
