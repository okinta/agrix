using agrix.Exceptions;
using System.Collections.Generic;
using System;
using YamlDotNet.RepresentationModel;

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
        /// <exception cref="KnownKeyNotFoundException">If <paramref name="required"/>
        /// is true and the key is not present.</exception>
        public static string GetKey(this YamlMappingNode node, string name,
            string defaultValue = "", bool required = false)
        {
            try
            {
                return (string)node.Children[new YamlScalarNode(name)];
            }
            catch (KeyNotFoundException e)
            {
                if (required) throw new KnownKeyNotFoundException<string>(name, e);
                return defaultValue;
            }
        }

        /// <summary>
        /// Gets an integer key from the node.
        /// </summary>
        /// <param name="node">The node to retrieve the key from.</param>
        /// <param name="name">The name of the key to retrieve.</param>
        /// <returns>The key retrieved from the node.</returns>
        /// <exception cref="KnownKeyNotFoundException">If the key is not
        /// found.</exception>
        /// <exception cref="InvalidCastException">If the value is not an
        /// integer.</exception>
        public static int GetInt(this YamlMappingNode node, string name)
        {
            string value;
            try
            {
                value = (string)node.Children[new YamlScalarNode(name)];
            }
            catch (KeyNotFoundException e)
            {
                throw new KnownKeyNotFoundException<string>(name, e);
            }

            if (!int.TryParse(value, out int result))
                throw new InvalidCastException("{0} is not an integer");

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
        public static int GetInt(this YamlMappingNode node, string name, int defaultValue)
        {
            string value;

            try
            {
                value = (string)node.Children[new YamlScalarNode(name)];
            }
            catch (KeyNotFoundException)
            {
                return defaultValue;
            }

            if (!int.TryParse(value, out int result))
                throw new InvalidCastException("{0} is not an integer");

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
        public static bool GetBool(this YamlMappingNode node, string name,
            bool defaultValue = false)
        {
            string value;
            try
            {
                value = (string)node.Children[new YamlScalarNode(name)];
            }
            catch (KeyNotFoundException)
            {
                return defaultValue;
            }

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
                        string.Format("{0} is not a boolean", value));
            }
        }

        /// <summary>
        /// Gets a child node from a node.
        /// </summary>
        /// <param name="node">The node to retrieve from.</param>
        /// <param name="name">The name of the key to retrieve.</param>
        /// <returns>The child node retrieved.</returns>
        /// <exception cref="KnownKeyNotFoundException">If the key is not
        /// found.</exception>
        /// <exception cref="InvalidCastException">If the node is not a mapping
        /// node.</exception>
        public static YamlMappingNode GetMapping(this YamlMappingNode node, string name)
        {
            try
            {
                return (YamlMappingNode)node.Children[new YamlScalarNode(name)];
            }
            catch (KeyNotFoundException e)
            {
                throw new KnownKeyNotFoundException<string>(name, e.Message, e);
            }
            catch (InvalidCastException e)
            {
                throw new InvalidCastException(
                    string.Format("{0} is not a mapping node", name), e);
            }
        }

        /// <summary>
        /// Gets a list of strings from a node.
        /// </summary>
        /// <param name="node">The node to retrieve the list from.</param>
        /// <param name="name">The name of the key to retrieve.</param>
        /// <returns>The list retrieved from the node. An empty list if not
        /// present.</returns>
        public static IList<string> GetList(this YamlMappingNode node, string name)
        {
            var list = new List<string>();

            YamlSequenceNode sequence;
            try
            {
                sequence = (YamlSequenceNode)node.Children[new YamlScalarNode(name)];
            }
            catch (KeyNotFoundException)
            {
                return list;
            }

            foreach (string value in sequence)
            {
                list.Add(value);
            }

            return list;
        }
    }
}
