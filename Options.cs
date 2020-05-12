using CommandLine;

namespace agrix
{
    /// <summary>
    /// The CLI options for the program.
    /// </summary>
    internal class Options
    {
        [Value(0, MetaName = "filename", HelpText = "The agrix config file to load")]
        public string Filename { get; set; }

        [Option('v', "validate", Default = false,
            HelpText = "Flag for validating but not processing the configuration")]
        public bool Validate { get; set; }

        [Option('k', Constants.ApiKeyArgument,
            HelpText = "The API key to use for communicating with the platform. Pulls from the environment variable " + Constants.EnvPlatformApiKey + " if not provided.")]
        public string ApiKey { get; set; }
    }
}
