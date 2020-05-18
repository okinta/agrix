using System;

namespace agrix.Platforms
{
    /// <summary>
    /// Instantiates a new IPlatform instance.
    /// </summary>
    /// <param name="apiKey">The API key to use to communicate with the platform.</param>
    /// <param name="apiUrl">The API URL to use for communicating with the
    /// platform.</param>
    /// <returns>The instantiated IPlatform instance.</returns>
    internal delegate IPlatform CreatePlatform(string apiKey, string apiUrl);

    /// <summary>
    /// Attribute that is applied to a class to indicate that it is a platform that can
    /// parse and provision configuration.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    internal class PlatformAttribute : Attribute
    {
        /// <summary>
        /// Gets the tag associated with the platform.
        /// </summary>
        public string Tag { get; }

        /// <summary>
        /// Gets the name of the static method used to instantiate the platform.
        /// </summary>
        public string Create { get; }

        /// <summary>
        /// Registers a new platform.
        /// </summary>
        /// <param name="tag">The tag associated with the platform.</param>
        /// <param name="create">The name of the static CreatePlatform delegate to use
        /// to instantiate the platform.</param>
        /// <exception cref="ArgumentNullException">If any arguments are null or
        /// empty.</exception>
        public PlatformAttribute(string tag, string create)
        {
            if (string.IsNullOrWhiteSpace(tag))
                throw new ArgumentNullException(nameof(tag), "tag cannot be empty");

            if (string.IsNullOrWhiteSpace(create))
                throw new ArgumentNullException(
                    nameof(create), "create cannot be empty");

            Tag = tag;
            Create = create;
        }
    }
}
