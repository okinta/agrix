using System;

namespace agrix.Platforms
{
    /// <summary>
    /// Attribute that is applied to a class to indicate that it is a platform that can
    /// parse and provision configuration.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    internal class PlatformAttribute : Attribute
    {
        /// <summary>
        /// Gets the tag associated with the platform.
        /// </summary>
        public string Tag { get; }

        /// <summary>
        /// Registers a new platform.
        /// </summary>
        /// <param name="tag">The tag associated with the platform.</param>
        public PlatformAttribute(string tag)
        {
            if (string.IsNullOrWhiteSpace(tag))
                throw new ArgumentNullException("tag", "tag cannot be empty");

            Tag = tag;
        }
    }
}
