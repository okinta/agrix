using agrix.Configuration;
using System.IO;
using System;
using YamlDotNet.RepresentationModel;
using System.Collections.Generic;
using agrix.Exceptions;

namespace agrix
{
    /// <summary>
    /// Provides an interface to convert code to infrastructure.
    /// </summary>
    internal class Agrix
    {
        private YamlStream yaml;

        /// <summary>
        /// The YAML configuration to process, as a string.
        /// </summary>
        public string Configuration { get; }

        /// <summary>
        /// The YAML configuration to process, as a YamlStream.
        /// </summary>
        public YamlStream YAML
        {
            get
            {
                if (yaml is null)
                {
                    yaml = new YamlStream();
                    yaml.Load(new StringReader(Configuration));
                }

                return yaml;
            }
        }

        private string ApiKey { get; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="configuration">The YAML configuration to process.</param>
        /// <param name="apiKey">The platform API key to use for communicating with
        /// the platform.</param>
        public Agrix(string configuration, string apiKey)
        {
            if (string.IsNullOrEmpty(configuration))
            {
                throw new ArgumentNullException(
                    "configuration", "Configuration must not be empty");
            }

            if (string.IsNullOrEmpty(apiKey))
            {
                throw new ArgumentNullException("apiKey", "API key must not be empty");
            }

            ApiKey = apiKey;
            Configuration = configuration;
        }

        /// <summary>
        /// Validates the YAML configuration to ensure correctness.
        /// </summary>
        /// <exception cref="AgrixValidationException">If there is a validation
        /// error.</exception>
        public void Validate()
        {
            try
            {
                InfrastructureConfiguration.LoadPlatform(YAML, ApiKey);
            }
            catch (KeyNotFoundException e)
            {
                throw new AgrixValidationException("platform key is missing", e);
            }
            catch (ArgumentException e)
            {
                throw new AgrixValidationException(e.Message, e);
            }

            InfrastructureConfiguration.LoadServers(YAML);
        }

        /// <summary>
        /// Builds infrastructure from the configuration.
        /// </summary>
        public void Process()
        {
            var platform = InfrastructureConfiguration.LoadPlatform(YAML, ApiKey);
            var servers = InfrastructureConfiguration.LoadServers(YAML);

            foreach (var server in servers)
            {
                platform.Provision(server);
            }
        }
    }
}
