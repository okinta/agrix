﻿using agrix.Configuration;
using YamlDotNet.RepresentationModel;

namespace agrix.Platforms
{
    /// <summary>
    /// Describes an interface for communicating with a platform.
    /// </summary>
    internal interface IPlatform
    {
        /// <summary>
        /// Loads infrastructure configuration from the given YAML.
        /// </summary>
        /// <param name="yaml">The YAML to load configuration from.</param>
        /// <returns>The infrastructure configuration loaded from the given YAML.</returns>
        public Infrastructure Load(YamlMappingNode yaml);

        /// <summary>
        /// Provisions infrastructure referencing the given configuration.
        /// </summary>
        /// <param name="infrastructure">The Infrastructure configuration to
        /// provision.</param>
        /// <param name="dryrun">Whether or not this is a dryrun. If set to true then
        /// provision commands will not be sent to the platform and instead messaging
        /// will be outputted describing what would be done.</param>
        public void Provision(Infrastructure infrastructure, bool dryrun = false);

        /// <summary>
        /// Destroys infrastructure referencing the given configuration.
        /// </summary>
        /// <param name="infrastructure">The Infrastructure configuration to
        /// provision.</param>
        /// <param name="dryrun">Whether or not this is a dryrun. If set to true then
        /// provision commands will not be sent to the platform and instead messaging
        /// will be outputted describing what would be done.</param>
        public void Destroy(Infrastructure infrastructure, bool dryrun = false);

        /// <summary>
        /// Tests the connection. Throws an exception if the connection is invalid.
        /// </summary>
        public void TestConnection();
    }
}
