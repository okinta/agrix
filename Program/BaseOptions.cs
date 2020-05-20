using CommandLine;

namespace agrix.Program
{
    /// <summary>
    /// The base CLI options for the program.
    /// </summary>
    internal abstract class BaseOptions
    {
        [Value(0, MetaName = "filename", HelpText = "The agrix config file to load")]
        public string Filename { get; set; }

        [Option('k', Constants.ApiKeyArgument,
            HelpText = "The API key to use for communicating with the platform. Pulls " +
                "from the environment variable " + Constants.EnvPlatformApiKey +
                " if not provided.")]
        public string ApiKey { get; set; }

        [Option('u', Constants.ApiUrlArgument,
            HelpText = "The API URL to use for communicating wit the platform. Pulls" +
                       "from the environment variable " + Constants.EnvPlatformApiUrl +
                       "if not provided.")]
        public string ApiUrl { get; set; }
    }
}
