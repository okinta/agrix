namespace agrix.Platforms
{
    /// <summary>
    /// Describes settings to use to configure a IPlatform instance.
    /// </summary>
    internal readonly struct PlatformSettings
    {
        /// <summary>
        /// Gets the API key to use to communicate with the platform.
        /// </summary>
        public string ApiKey { get; }

        /// <summary>
        /// Gets the API URL to use for communicating with the platform.
        /// </summary>
        public string ApiUrl { get; }

        /// <summary>
        /// Creates new platform settings.
        /// </summary>
        /// <param name="apiKey">The API key to use to communicate with the
        /// platform.</param>
        /// <param name="apiUrl">The API URl to use for communicating with the
        /// platform.</param>
        public PlatformSettings(string apiKey, string apiUrl)
        {
            ApiKey = apiKey;
            ApiUrl = apiUrl;
        }
    }
}
