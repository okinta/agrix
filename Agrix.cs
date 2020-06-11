using agrix.Configuration;
using agrix.Exceptions;
using agrix.Extensions;
using agrix.Platforms;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System;
using YamlDotNet.RepresentationModel;

namespace agrix
{
    /// <summary>
    /// Provides an interface to convert code to infrastructure.
    /// </summary>
    internal class Agrix
    {
        private YamlStream _yaml;

        /// <summary>
        /// The YAML configuration to process, as a string.
        /// </summary>
        private string Configuration { get; }

        /// <summary>
        /// The YAML configuration to process, as a YamlStream.
        /// </summary>
        private YamlStream Yaml
        {
            get
            {
                if (!(_yaml is null)) return _yaml;
                _yaml = new YamlStream();
                _yaml.Load(new StringReader(Configuration));

                return _yaml;
            }
        }

        private string ApiKey { get; }
        private string ApiUrl { get; }
        private Assembly Assembly { get; }

        /// <summary>
        /// Provisions/destroys infrastructure.
        /// </summary>
        /// <param name="platform">The loaded IPlatform instance.</param>
        /// <param name="infrastructure">The infrastructure configuration to
        /// provision/destroy.</param>
        /// <param name="dryrun">Whether or not this is a dryrun.</param>
        private delegate void Provision(
            IPlatform platform, Infrastructure infrastructure, bool dryrun);

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="configuration">The YAML configuration to process.</param>
        /// <param name="settings">The settings to use to configure the instance.</param>
        /// <exception cref="ArgumentNullException">If any arguments are null or
        /// empty.</exception>
        public Agrix(string configuration, AgrixSettings settings)
        {
            if (string.IsNullOrEmpty(configuration))
                throw new ArgumentNullException(
                    nameof(configuration), "Configuration must not be empty");

            if (settings is null)
                throw new ArgumentNullException(
                    nameof(settings), "Settings must not be empty");

            ApiKey = settings.ApiKey;
            ApiUrl = settings.ApiUrl;
            Assembly = settings.Assembly;
            Configuration = configuration;
        }

        /// <summary>
        /// Loads the platform configuration from the given YAML.
        /// </summary>
        /// <returns>The platform configuration loaded from the given YAML.</returns>
        /// <exception cref="KeyNotFoundException">If the platform key is not present
        /// inside the configuration..</exception>
        /// <exception cref="ArgumentException">If the platform is not
        /// supported.</exception>
        public IPlatform LoadPlatform()
        {
            var root = Yaml.GetRootNode();
            var platformName = root.GetKey("platform", required: true);
            var availablePlatforms = GetAvailablePlatforms(Assembly);
            var platformSettings = new PlatformSettings(ApiKey, ApiUrl);

            if (availablePlatforms.TryGetValue(platformName, out var platform))
                return platform(platformSettings);

            var line = root.GetNode("platform").Start.Line;
            var availablePlatformsText = string.Join(
                ", ", availablePlatforms.Keys);
            throw new ArgumentException(
                $"Unknown platform {platformName} (line {line}). Available "
                + $"platforms are: {availablePlatformsText}");

        }

        /// <summary>
        /// Validates the YAML configuration to ensure correctness.
        /// </summary>
        /// <exception cref="AgrixValidationException">If there is a validation
        /// error.</exception>
        public void Validate()
        {
            IPlatform platform;
            try
            {
                platform = LoadPlatform();
            }
            catch (KeyNotFoundException e)
            {
                throw new AgrixValidationException("platform key is missing", e);
            }
            catch (ArgumentException e)
            {
                throw new AgrixValidationException(e.Message, e);
            }

            try
            {
                platform.TestConnection();
            }
            catch (Exception e)
            {
                throw new AgrixValidationException(e.Message, e);
            }

            try
            {
                platform.Load(Yaml.GetRootNode());
            }
            catch (KnownKeyNotFoundException<string> e)
            {
                throw new AgrixValidationException(
                    $"scripts validation error: {e.Message}", e);
            }
        }

        /// <summary>
        /// Builds infrastructure from the configuration.
        /// </summary>
        /// <param name="dryrun">Whether or not this is a dryrun. If set to true then
        /// provision commands will not be sent to the platform and instead messaging
        /// will be outputted describing what would be done.</param>
        public void Process(bool dryrun)
        {
            Process(
                (p, i, d) => p.Provision(i, d), dryrun);
        }

        /// <summary>
        /// Destroys infrastructure defined in the configuration.
        /// </summary>
        /// <param name="dryrun">Whether or not this is a dryrun. If set to true then
        /// provision commands will not be sent to the platform and instead messaging
        /// will be outputted describing what would be done.</param>
        public void Destroy(bool dryrun)
        {
            Process(
                (p, i, d) => p.Destroy(i, d), dryrun);
        }

        /// <summary>
        /// Loads all the registered platforms.
        /// </summary>
        /// <param name="assembly">The Assembly to search within.</param>
        /// <returns>The collection of registered platforms with their associated
        /// keys.</returns>
        private static Dictionary<string, CreatePlatform> GetAvailablePlatforms(
            Assembly assembly)
        {
            var platforms = new Dictionary<string, CreatePlatform>();

            foreach (var type in assembly.GetTypes())
            {
                if (!typeof(IPlatform).IsAssignableFrom(type)) continue;

                var attributes = type.GetCustomAttributes(
                    typeof(PlatformAttribute));
                var enumerable = attributes as Attribute[]
                    ?? attributes.ToArray();
                if (!enumerable.Any())
                    continue;

                var platformAttribute = (PlatformAttribute)enumerable[0];

                if (string.IsNullOrWhiteSpace(platformAttribute.Tag))
                    throw new InvalidOperationException(
                        $"PlatformAttribute tag cannot be empty for {type}");

                if (platforms.ContainsKey(platformAttribute.Tag))
                {
                    var inUseBy = platforms[platformAttribute.Tag];
                    throw new InvalidOperationException(
                        $"{platformAttribute.Tag} is already in use by "
                        + $"{inUseBy}. Cannot be used by {type}.");
                }

                platforms[platformAttribute.Tag] =
                    (CreatePlatform)Delegate.CreateDelegate(
                        typeof(CreatePlatform), type, platformAttribute.Create);
            }

            return platforms;
        }

        /// <summary>
        /// Provisions/destroys infrastructure.
        /// </summary>
        /// <param name="provision">The delegate to call after the infrastructure
        /// configuration has been loaded.</param>
        /// <param name="dryrun">Whether or not this is a dryrun.</param>
        private void Process(Provision provision, bool dryrun)
        {
            Validate();

            var platform = LoadPlatform();
            var infrastructure = platform.Load(Yaml.GetRootNode());
            provision(platform, infrastructure, dryrun);
        }
    }
}
