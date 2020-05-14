using System;

namespace agrix.Configuration
{
    /// <summary>
    /// Represents a stored script.
    /// </summary>
    internal struct Script
    {
        /// <summary>
        /// The name of the script.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The type of script.
        /// </summary>
        public string Type { get; }

        /// <summary>
        /// The contents of the script.
        /// </summary>
        public string Content { get; }

        /// <summary>
        /// Creates a new Script configuration.
        /// </summary>
        /// <param name="name">The name of the script.</param>
        /// <param name="type">The type of script.</param>
        /// <param name="content">The script content.</param>
        /// <exception cref="ArgumentNullException">If any argument is null or
        /// empty.</exception>
        public Script(string name, string type, string content)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(
                    "name", "Script name must not be empty");

            if (string.IsNullOrEmpty(type))
                throw new ArgumentNullException(
                    "name", "Script type must not be empty");

            if (string.IsNullOrEmpty(content))
                throw new ArgumentNullException(
                    "name", "Script content must not be empty");

            Name = name;
            Type = type;
            Content = content;
        }
    }
}
