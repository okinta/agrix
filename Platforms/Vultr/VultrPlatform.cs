using agrix.Configuration;
using agrix.Platforms.Vultr.Provisioners;
using Server = agrix.Configuration.Server;
using Vultr.API;

namespace agrix.Platforms.Vultr
{
    /// <summary>
    /// Describes methods to communicate with the Vultr platform.
    /// </summary>
    [Platform("vultr")]
    internal class VultrPlatform : Platform
    {
        private VultrClient Client { get; }

        /// <summary>
        /// Instantiates the instance.
        /// </summary>
        /// <param name="apiKey">The API key to use for communicating with Vultr.</param>
        public VultrPlatform(string apiKey) : this(apiKey, null) { }

        /// <summary>
        /// Instantiates the instance and overrides the Vultr API URL.
        /// </summary>
        /// <param name="apiKey">The API key to use for communicating with Vultr.</param>
        /// <param name="apiUrl">The API URL for Vultr. Set this to override the
        /// Vultr API endpoint (e.g. for testing).</param>
        public VultrPlatform(string apiKey, string apiUrl)
        {
            Client = string.IsNullOrEmpty(apiUrl) ?
                new VultrClient(apiKey) : new VultrClient(apiKey, apiUrl);

            AddProvisioner<Firewall>(new VultrFirewallProvisioner(Client).Provision);
            AddProvisioner<Script>(new VultrScriptProvisioner(Client).Provision);
            AddProvisioner<Server>(new VultrServerProvisioner(Client).Provision);
        }

        /// <summary>
        /// Tests the connection. Throws an exception if the connection is invalid.
        /// </summary>
        public override void TestConnection()
        {
            Client.Account.GetInfo();
        }
    }
}
