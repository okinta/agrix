using agrix.Platforms;
using System.Reflection;

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
        public AgrixSettings(string apiKey, string apiUrl = null, Assembly assembly = null)
        {
            ApiKey = apiKey;
            ApiUrl = apiUrl;
            Assembly = assembly ?? Assembly.GetAssembly(typeof(IPlatform));
        }
    }
}
