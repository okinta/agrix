using agrix.Exceptions;
using System.Collections.Generic;
using System.IO;
using System;
using YamlDotNet.RepresentationModel;
using YamlDotNet.Serialization;

namespace agrix.Extensions
{
    /// <summary>
    /// Extends YAML classes.
    /// </summary>
    internal static class YamlExtensions
    {
        /// <summary>
        /// Gets the root mapping node in the YAML document.
        /// </summary>
        /// <param name="yamlConfig">The YAML document to get the root node from.</param>
        /// <returns>The root mapping node in the YAML document.</returns>
        public static YamlMappingNode GetRootNode(this YamlStream yamlConfig)
        {
            if (yamlConfig is null)
                throw new ArgumentNullException(
                    nameof(yamlConfig), "must not be null");

            if (yamlConfig.Documents.Count == 0)
                throw new ArgumentException(
                    "YAML config must have a document root", nameof(yamlConfig));

            return (YamlMappingNode)yamlConfig.Documents[0].RootNode;
        }

        /// <summary>
        /// Gets the tag (name) of the node.
        /// </summary>
        /// <param name="node">The node to get the tag for.</param>
        /// <returns>The tag of the node.</returns>
        public static string GetTag(this YamlNode node)
        {
            return node.NodeType == YamlNodeType.Scalar ?
                ((YamlScalarNode)node).Value : node.Tag;
        }

        /// <summary>
        /// Gets a child node.
        /// </summary>
        /// <param name="node">The node to retrieve the child from.</param>
        /// <param name="name">The name of the child node to retrieve.</param>
        /// <returns>The child node.</returns>
        /// <exception cref="KnownKeyNotFoundException{T}">If the child node doesn't
        /// exist.</exception>
        public static YamlNode GetNode(this YamlMappingNode node, string name)
        {
            try
            {
                return node.Children[new YamlScalarNode(name)];
            }
            catch (KeyNotFoundException)
            {
                throw new KnownKeyNotFoundException<string>(
                    name, $"{name} not found (line {node.Start.Line})");
            }
        }

        /// <summary>
        /// Gets a key from the node.
        /// </summary>
        /// <param name="node">The node to retrieve the key from.</param>
        /// <param name="name">The name of the key to retrieve.</param>
        /// <param name="defaultValue">The value to return if the key is not
        /// present. Defaults to an empty string.</param>
        /// <param name="required">Whether or not the key is required. Defaults to
        /// false.</param>
        /// <returns>The key retrieved from the node.</returns>
        /// <exception cref="KnownKeyNotFoundException{T}">If
        /// <paramref name="required"/> is true and the key is not present.</exception>
        /// <exception cref="InvalidCastException">If the value is not a
        /// string.</exception>
        public static string GetKey(this YamlMappingNode node, string name,
            string defaultValue = "", bool required = false)
        {
            YamlNode childNode;
            try
            {
                childNode = node.GetNode(name);
            }
            catch (KnownKeyNotFoundException<string>)
            {
                if (required) throw;
                return defaultValue;
            }

            if (childNode.NodeType != YamlNodeType.Scalar)
                throw new InvalidCastException(
                    $"{name} is not a string (line {childNode.Start.Line})");

            return (string)childNode;
        }

        /// <summary>
        /// Gets an integer key from the node.
        /// </summary>
        /// <param name="node">The node to retrieve the key from.</param>
        /// <param name="name">The name of the key to retrieve.</param>
        /// <returns>The key retrieved from the node.</returns>
        /// <exception cref="KnownKeyNotFoundException{T}">If the key is not
        /// found.</exception>
        /// <exception cref="InvalidCastException">If the value is not an
        /// integer.</exception>
        public static int GetInt(this YamlMappingNode node, string name)
        {
            var childNode = node.GetNode(name);

            if (childNode.NodeType != YamlNodeType.Scalar)
                throw new InvalidCastException(
                    $"{name} is not an integer (line {childNode.Start.Line})");

            if (!int.TryParse((string)childNode, out int result))
                throw new InvalidCastException(
                    $"{name} is not an integer (line {childNode.Start.Line}");

            return result;
        }

        /// <summary>
        /// Gets an integer key from the node.
        /// </summary>
        /// <param name="node">The node to retrieve the key from.</param>
        /// <param name="name">The name of the key to retrieve.</param>
        /// <param name="defaultValue">The value to return if the key is not
        /// present.</param>
        /// <returns>The key retrieved from the node.</returns>
        /// <exception cref="InvalidCastException">If the value is not an
        /// integer.</exception>
        public static int GetInt(this YamlMappingNode node, string name, int defaultValue)
        {
            YamlNode childNode;

            try
            {
                childNode = node.GetNode(name);
            }
            catch (KnownKeyNotFoundException<string>)
            {
                return defaultValue;
            }

            if (childNode.NodeType != YamlNodeType.Scalar)
                throw new InvalidCastException(
                    $"{name} is not an integer (line {childNode.Start.Line})");

            if (!int.TryParse((string)childNode, out int result))
                throw new InvalidCastException(
                    $"{name} is not an integer (line {childNode.Start.Line}");

            return result;
        }

        /// <summary>
        /// Gets a boolean key from the node. Follows the specification outlined here:
        /// https://yaml.org/type/bool.html
        /// </summary>
        /// <param name="node">The node to retrieve the key from.</param>
        /// <param name="name">The name of the key to retrieve.</param>
        /// <param name="defaultValue">The value to return if the key is not
        /// present. Defaults to false.</param>
        /// <returns>The key retrieved from the node.</returns>
        /// <exception cref="InvalidCastException">If the value is not a
        /// boolean.</exception>
        public static bool GetBool(this YamlMappingNode node, string name,
            bool defaultValue = false)
        {
            YamlNode childNode;
            try
            {
                childNode = node.GetNode(name);
            }
            catch (KnownKeyNotFoundException<string>)
            {
                return defaultValue;
            }

            if (childNode.NodeType != YamlNodeType.Scalar)
                throw new InvalidCastException(
                    $"{name} is not a boolean (line {childNode.Start.Line})");

            var value = (string)childNode ?? "";
            switch (value.ToLower())
            {
                case "on":
                case "true":
                case "y":
                case "yes":
                    return true;

                case "false":
                case "n":
                case "no":
                case "off":
                    return false;

                default:
                    throw new InvalidCastException(
                        $"{value} is not a boolean (line {childNode.Start.Line}");
            }
        }

        /// <summary>
        /// Gets a JSON key from the node.
        /// </summary>
        /// <param name="node">The node to retrieve the key from.</param>
        /// <param name="name">The name of the key to retrieve.</param>
        /// <param name="defaultValue">The value to return if the key is not
        /// present. Defaults to an empty string.</param>
        /// <param name="required">Whether or not the key is required. Defaults to
        /// false.</param>
        /// <returns>The key retrieved from the node.</returns>
        /// <exception cref="KnownKeyNotFoundException{T}">If
        /// <paramref name="required"/> is true and the key is not present.</exception>
        public static string GetJson(this YamlMappingNode node,
            string name, string defaultValue = "", bool required = false)
        {
            YamlNode jsonNode;
            try
            {
                jsonNode = node.GetNode(name);
            }
            catch (KnownKeyNotFoundException<string>)
            {
                if (required) throw;
                return defaultValue;
            }

            return jsonNode.NodeType == YamlNodeType.Scalar ?
                (string)jsonNode : jsonNode.ToJson();
        }

        /// <summary>
        /// Converts a YamlNode to a JSON string.
        /// </summary>
        /// <param name="node">The YamlNode instance to convert.</param>
        /// <returns>The converted JSON string.</returns>
        public static string ToJson(this YamlNode node)
        {
            var stream = new YamlStream { new YamlDocument(node) };
            using var writer = new StringWriter();
            stream.Save(writer);

            using var reader = new StringReader(writer.ToString());
            var deserializer = new Deserializer();
            var yamlObject = deserializer.Deserialize(reader);
            var serializer = new SerializerBuilder()
                .JsonCompatible()
                .Build();
            return serializer.Serialize(yamlObject ?? "").Trim();
        }

        /// <summary>
        /// Gets a child mapping node from a node.
        /// </summary>
        /// <param name="node">The node to retrieve from.</param>
        /// <param name="name">The name of the key to retrieve.</param>
        /// <returns>The child node retrieved.</returns>
        /// <exception cref="KnownKeyNotFoundException{T}">If the key is not
        /// found.</exception>
        /// <exception cref="InvalidCastException">If the node is not a mapping
        /// node.</exception>
        public static YamlMappingNode GetMapping(this YamlMappingNode node, string name)
        {
            var childNode = node.GetNode(name);

            if (childNode.NodeType != YamlNodeType.Mapping)
                throw new InvalidCastException(
                    $"{name} is not a mapping node (line {childNode.Start.Line})");

            return (YamlMappingNode)childNode;
        }

        /// <summary>
        /// Gets a child sequence from a node.
        /// </summary>
        /// <param name="node">The node to retrieve from.</param>
        /// <param name="name">The name of the key to retrieve.</param>
        /// <param name="required">Whether or not the key is required. Defaults to
        /// true.</param>
        /// <returns>The child node retrieved.</returns>
        /// <returns>The YamlSequenceNode child node. Or null if
        /// <paramref name="required"/> is false and the child node doesn't
        /// exist.</returns>
        /// <exception cref="KnownKeyNotFoundException{T}">If
        /// <paramref name="required"/> is true and the child node doesn't
        /// exist.</exception>
        /// <exception cref="InvalidCastException">If the child node is not a sequence
        /// node.</exception>
        public static YamlSequenceNode GetSequence(
            this YamlMappingNode node, string name, bool required = true)
        {
            YamlNode childNode;
            try
            {
                childNode = node.GetNode(name);
            }
            catch (KnownKeyNotFoundException<string>)
            {
                if (required) throw;
                return null;
            }

            // If the node isn't required, is present but empty, return null.
            if (required == false
                && childNode.NodeType == YamlNodeType.Scalar
                && string.IsNullOrEmpty((string)childNode))
                return null;

            if (childNode.NodeType != YamlNodeType.Sequence)
                throw new InvalidCastException(
                    $"{name} is not a sequence node (line {childNode.Start.Line})");

            return (YamlSequenceNode)childNode;
        }

        /// <summary>
        /// Gets a list of strings from a node.
        /// </summary>
        /// <param name="node">The node to retrieve the list from.</param>
        /// <param name="name">The name of the key to retrieve.</param>
        /// <returns>The list retrieved from the node. An empty list if not
        /// present.</returns>
        /// <exception cref="InvalidCastException">If the value is not a
        /// list.</exception>
        public static IList<string> GetList(this YamlMappingNode node, string name)
        {
            var list = new List<string>();

            YamlNode childNode;
            try
            {
                childNode = node.GetNode(name);
            }
            catch (KnownKeyNotFoundException<string>)
            {
                return list;
            }

            if (childNode.NodeType != YamlNodeType.Sequence)
                throw new InvalidCastException(
                    $"{name} is not a sequence node (line {childNode.Start.Line})");

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (string value in (YamlSequenceNode)childNode)
                list.Add(value);

            return list;
        }
    }
}
