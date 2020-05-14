﻿using agrix.Exceptions;
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
        /// Gets a key from the node.
        /// </summary>
        /// <param name="node">The node to retrieve the key from.</param>
        /// <param name="name">The name of the key to retrieve.</param>
        /// <param name="defaultValue">The value to return if the key is not
        /// present. Defaults to an empty string.</param>
        /// <param name="required">Whether or not the key is required. Defaults to
        /// false.</param>
        /// <returns>The key retrieved from the node.</returns>
        /// <exception cref="KnownKeyNotFoundException{string}">If
        /// <paramref name="required"/> is true and the key is not present.</exception>
        /// <exception cref="InvalidCastException">If the value is not a
        /// string.</exception>
        public static string GetKey(this YamlMappingNode node, string name,
            string defaultValue = "", bool required = false)
        {
            YamlNode childNode;
            try
            {
                childNode = node.Children[new YamlScalarNode(name)];
            }
            catch (KeyNotFoundException)
            {
                if (required)
                    throw new KnownKeyNotFoundException<string>(name,
                        string.Format("{0} not found (line {1})", name, node.Start.Line));

                return defaultValue;
            }

            if (childNode.NodeType != YamlNodeType.Scalar)
                throw new InvalidCastException(
                    string.Format("{0} is not a string (line {1})",
                        name, childNode.Start.Line));

            return (string)childNode;
        }

        /// <summary>
        /// Gets an integer key from the node.
        /// </summary>
        /// <param name="node">The node to retrieve the key from.</param>
        /// <param name="name">The name of the key to retrieve.</param>
        /// <returns>The key retrieved from the node.</returns>
        /// <exception cref="KnownKeyNotFoundException{string}">If the key is not
        /// found.</exception>
        /// <exception cref="InvalidCastException">If the value is not an
        /// integer.</exception>
        public static int GetInt(this YamlMappingNode node, string name)
        {
            YamlNode childNode;
            try
            {
                childNode = node.Children[new YamlScalarNode(name)];
            }
            catch (KeyNotFoundException)
            {
                throw new KnownKeyNotFoundException<string>(name,
                    string.Format("{0} not found (line {1})", name, node.Start.Line));
            }

            if (childNode.NodeType != YamlNodeType.Scalar)
                throw new InvalidCastException(
                    string.Format("{0} is not an integer (line {1})",
                        name, childNode.Start.Line));

            if (!int.TryParse((string)childNode, out int result))
                throw new InvalidCastException(
                    string.Format("{0} is not an integer (line {1}",
                        name, childNode.Start.Line));

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
                childNode = node.Children[new YamlScalarNode(name)];
            }
            catch (KeyNotFoundException)
            {
                return defaultValue;
            }

            if (childNode.NodeType != YamlNodeType.Scalar)
                throw new InvalidCastException(
                    string.Format("{0} is not an integer (line {1})",
                        name, childNode.Start.Line));

            if (!int.TryParse((string)childNode, out int result))
                throw new InvalidCastException(
                    string.Format("{0} is not an integer (line {1}",
                        name, childNode.Start.Line));

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
                childNode = node.Children[new YamlScalarNode(name)];
            }
            catch (KeyNotFoundException)
            {
                return defaultValue;
            }

            if (childNode.NodeType != YamlNodeType.Scalar)
                throw new InvalidCastException(
                    string.Format("{0} is not a boolean (line {1})",
                        name, childNode.Start.Line));

            var value = (string)childNode;
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
                        string.Format("{0} is not a boolean (line {1}",
                            value, childNode.Start.Line));
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
        /// <exception cref="KnownKeyNotFoundException{string}">If
        /// <paramref name="required"/> is true and the key is not present.</exception>
        public static string GetJSON(this YamlMappingNode node,
            string name, string defaultValue = "", bool required = false)
        {
            YamlNode jsonNode;
            try
            {
                jsonNode = node.Children[new YamlScalarNode(name)];
            }
            catch (KeyNotFoundException)
            {
                if (required)
                    throw new KnownKeyNotFoundException<string>(name,
                        string.Format("{0} not found (line {1})",
                            name, node.Start.Line));

                return defaultValue;
            }

            return jsonNode.NodeType == YamlNodeType.Scalar ?
                (string)jsonNode : jsonNode.ToJSON();
        }

        /// <summary>
        /// Converts a YamlNode to a JSON string.
        /// </summary>
        /// <param name="node">The YamlNode instance to convert.</param>
        /// <returns>The converted JSON string.</returns>
        public static string ToJSON(this YamlNode node)
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
            return serializer.Serialize(yamlObject).Trim();
        }

        /// <summary>
        /// Gets a child mapping node from a node.
        /// </summary>
        /// <param name="node">The node to retrieve from.</param>
        /// <param name="name">The name of the key to retrieve.</param>
        /// <returns>The child node retrieved.</returns>
        /// <exception cref="KnownKeyNotFoundException{string}">If the key is not
        /// found.</exception>
        /// <exception cref="InvalidCastException">If the node is not a mapping
        /// node.</exception>
        public static YamlMappingNode GetMapping(this YamlMappingNode node, string name)
        {
            YamlNode childNode;
            try
            {
                childNode = node.Children[new YamlScalarNode(name)];
            }
            catch (KeyNotFoundException)
            {
                throw new KnownKeyNotFoundException<string>(name,
                    string.Format("{0} not found (line {1})", name, node.Start.Line));
            }

            if (childNode.NodeType != YamlNodeType.Mapping)
                throw new InvalidCastException(
                    string.Format("{0} is not a mapping node (line {1})",
                        name, childNode.Start.Line));

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
        /// <exception cref="KnownKeyNotFoundException{string}">If
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
                childNode = node.Children[new YamlScalarNode(name)];
            }
            catch (KeyNotFoundException)
            {
                if (required)
                    throw new KnownKeyNotFoundException<string>(name,
                        string.Format("{0} not found (line {1})", name, node.Start.Line));

                return null;
            }

            // If the node isn't required, is present but empty, return null.
            if (required == false
                && childNode.NodeType == YamlNodeType.Scalar
                && string.IsNullOrEmpty((string)childNode))
                return null;

            if (childNode.NodeType != YamlNodeType.Sequence)
                throw new InvalidCastException(
                    string.Format("{0} is not a sequence node (line {1})",
                        name, childNode.Start.Line));

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
                childNode = node.Children[new YamlScalarNode(name)];
            }
            catch (KeyNotFoundException)
            {
                return list;
            }

            if (childNode.NodeType != YamlNodeType.Sequence)
                throw new InvalidCastException(
                    string.Format("{0} is not a sequence node (line {1})",
                        name, childNode.Start.Line));

            foreach (string value in (YamlSequenceNode)childNode)
                list.Add(value);

            return list;
        }
    }
}
