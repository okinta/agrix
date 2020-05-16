﻿using agrix.Configuration;
using agrix.Exceptions;
using agrix.Platforms;
using System.Collections.Generic;
using System.IO;
using System;
using YamlDotNet.RepresentationModel;

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
            IPlatform platform;
            try
            {
                platform = InfrastructureConfiguration.LoadPlatform(YAML, ApiKey);
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
                InfrastructureConfiguration.LoadScripts(YAML, platform.AgrixConfig);
            }
            catch (KnownKeyNotFoundException<string> e)
            {
                throw new AgrixValidationException(
                    string.Format("scripts validation error: {0}", e.Message), e);
            }

            try
            {
                InfrastructureConfiguration.LoadFirewalls(YAML, platform.AgrixConfig);
            }
            catch (KnownKeyNotFoundException<string> e)
            {
                throw new AgrixValidationException(
                    string.Format("firewalls validation error: {0}", e.Message), e);
            }

            try
            {
                InfrastructureConfiguration.LoadServers(YAML, platform.AgrixConfig);
            }
            catch (KnownKeyNotFoundException<string> e)
            {
                throw new AgrixValidationException(
                    string.Format("servers validation error: {0}", e.Message), e);
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
            Validate();

            var platform = InfrastructureConfiguration.LoadPlatform(YAML, ApiKey);
            var scripts = InfrastructureConfiguration.LoadScripts(
                YAML, platform.AgrixConfig);
            var firewalls = InfrastructureConfiguration.LoadFirewalls(
                YAML, platform.AgrixConfig);
            var servers = InfrastructureConfiguration.LoadServers(
                YAML, platform.AgrixConfig);

            foreach (var script in scripts)
                platform.Provision(script, dryrun);

            foreach (var firewall in firewalls)
                platform.Provision(firewall, dryrun);

            foreach (var server in servers)
                platform.Provision(server, dryrun);
        }
    }
}
