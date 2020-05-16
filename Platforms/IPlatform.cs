using agrix.Configuration;

namespace agrix.Platforms
{
    /// <summary>
    /// Describes an interface for communicating with a platform.
    /// </summary>
    internal interface IPlatform
    {
        /// <summary>
        /// The IAgrixConfig to use to load configuration from YAML.
        /// </summary>
        public IAgrixConfig AgrixConfig { get; }

        /// <summary>
        /// Provisions a Server using the given configuration.
        /// </summary>
        /// <param name="server">The Server configuration to provision.</param>
        /// <param name="dryrun">Whether or not this is a dryrun. If set to true then
        /// provision commands will not be sent to the platform and instead messaging
        /// will be outputted describing what would be done.</param>
        public void Provision(Server server, bool dryrun = false);

        /// <summary>
        /// Provisions a Script using the given configuration.
        /// </summary>
        /// <param name="script">The Script configuration to provision.</param>
        /// <param name="dryrun">Whether or not this is a dryrun. If set to true then
        /// provision commands will not be sent to the platform and instead messaging
        /// will be outputted describing what would be done.</param>
        public void Provision(Script script, bool dryrun = false);

        /// <summary>
        /// Provisions a Firewall using the given configuration.
        /// </summary>
        /// <param name="firewall">The Firewall configuration to provision.</param>
        /// <param name="dryrun">Whether or not this is a dryrun. If set to true then
        /// provision commands will not be sent to the platform and instead messaging
        /// will be outputted describing what would be done.</param>
        public void Provision(Firewall firewall, bool dryrun = false);

        /// <summary>
        /// Tests the connection. Throws an exception if the connection is invalid.
        /// </summary>
        public void TestConnection();
    }
}
