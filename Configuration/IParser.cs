using System.Collections.Generic;
using System;
using YamlDotNet.RepresentationModel;

namespace agrix.Configuration
{
    /// <summary>
    /// Creates an instance from a YAML configuration.
    /// </summary>
    /// <typeparam name="T">The type of instance to create.</typeparam>
    /// <param name="node">The YAML configuration to parse.</param>
    /// <returns>The instance parsed from the given YAML.</returns>
    internal delegate T Parse<out T>(YamlNode node);

    /// <summary>
    /// Describes methods to load configuration from YAML.
    /// </summary>
    internal interface IParser
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
        public IList<T> Load<T>(string name, YamlNode node, Parse<T> parse);
    }
}
