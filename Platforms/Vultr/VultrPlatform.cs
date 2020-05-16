using agrix.Configuration;
using agrix.Platforms.Vultr.Provisioning;
using Server = agrix.Configuration.Server;
using Vultr.API;

namespace agrix.Platforms.Vultr
{
    /// <summary>
    /// Describes methods to communicate with the Vultr platform.
    /// </summary>
    internal class VultrPlatform : IPlatform
    {
        /// <summary>
        /// The IAgrixConfig to use to load configuration from YAML.
        /// </summary>
        public IAgrixConfig AgrixConfig { get; } = new VultrAgrixConfig();

        private VultrClient Client { get; }

        /// <summary>
        /// Instantiates the instance.
        /// </summary>
        /// <param name="apiKey">The API key to use for communicating with Vultr.</param>
        /// <param name="apiURL">The optional API URL for Vultr. Set this to override
        /// the Vultr API endpoint (e.g. for testing).</param>
        public VultrPlatform(string apiKey, string apiURL = null)
        {
            Client = string.IsNullOrEmpty(apiURL) ?
                new VultrClient(apiKey) : new VultrClient(apiKey, apiURL);
        }

        /// <summary>
        /// Provisions a Server using the given configuration.
        /// </summary>
        /// <param name="server">The Server configuration to provision.</param>
        /// <param name="dryrun">Whether or not this is a dryrun. If set to true then
        /// provision commands will not be sent to the platform and instead messaging
        /// will be outputted describing what would be done.</param>
        public void Provision(Server server, bool dryrun = false)
        {
            new VultrServerProvisioner(Client).Provision(server, dryrun);
        }

        /// <summary>
        /// Provisions a Script using the given configuration.
        /// </summary>
        /// <param name="script">The Script configuration to provision.</param>
        /// <param name="dryrun">Whether or not this is a dryrun. If set to true then
        /// provision commands will not be sent to the platform and instead messaging
        /// will be outputted describing what would be done.</param>
        public void Provision(Script script, bool dryrun = false)
        {
            new VultrScriptProvisioner(Client).Provision(script, dryrun);
        }

        /// <summary>
        /// Tests the connection. Throws an exception if the connection is invalid.
        /// </summary>
        public void TestConnection()
        {
            Client.Account.GetInfo();
        }
    }
}
