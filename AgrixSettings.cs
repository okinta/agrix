using agrix.Platforms;
using System.Reflection;
using System;

namespace agrix
{
    /// <summary>
    /// Describes settings for configuring the Agrix class.
    /// </summary>
    internal class AgrixSettings
    {
        /// <summary>
        /// Gets the API key to use for communicating with the platform.
        /// </summary>
        public string ApiKey { get; }

        /// <summary>
        /// Gets the API url to use for communicating with the platform.
        /// </summary>
        public string ApiUrl { get; }

        /// <summary>
        /// Gets the Assembly to search within for the platforms.
        /// </summary>
        public Assembly Assembly { get; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="apiKey">The platform API key to use for communicating with
        /// the platform.</param>
        /// <param name="apiUrl">The optional platform API URL to use for communicating
        /// with the platform.</param>
        /// <param name="assembly">The optional Assembly to search within for
        /// platforms. Defaults to this assembly.</param>
        /// <exception cref="ArgumentNullException">If <param name="apiKey">is null or
        /// empty.</param></exception>
        public AgrixSettings(string apiKey, string apiUrl = null, Assembly assembly = null)
        {
            if (string.IsNullOrEmpty(apiKey))
                throw new ArgumentNullException(
                    nameof(apiKey), "API key must not be empty");

            ApiKey = apiKey;
            ApiUrl = apiUrl;
            Assembly = assembly ?? Assembly.GetAssembly(typeof(IPlatform));
        }
    }
}
