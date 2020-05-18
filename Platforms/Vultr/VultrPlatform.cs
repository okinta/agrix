using agrix.Configuration;
using agrix.Platforms.Vultr.Provisioners;
using Server = agrix.Configuration.Server;
using System;
using Vultr.API;

namespace agrix.Platforms.Vultr
{
    /// <summary>
    /// Describes methods to communicate with the Vultr platform.
    /// </summary>
    [Platform("vultr", nameof(CreateVultrPlatform))]
    internal class VultrPlatform : Platform
    {
        private VultrClient Client { get; }

        /// <summary>
        /// Instantiates the instance and overrides the Vultr API URL.
        /// </summary>
        /// <param name="apiKey">The API key to use for communicating with Vultr.</param>
        /// <param name="apiUrl">The API URL for Vultr. Set this to override the
        /// Vultr API endpoint (e.g. for testing).</param>
        /// <exception cref="ArgumentNullException">If <param name="apiKey"> is null or
        /// empty.</param></exception>
        public VultrPlatform(string apiKey, string apiUrl)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
                throw new ArgumentNullException(
                    nameof(apiKey), "apiKey cannot be provided");

            Client = string.IsNullOrEmpty(apiUrl) ?
                new VultrClient(apiKey) : new VultrClient(apiKey, apiUrl);

            AddProvisioner<Firewall>(new VultrFirewallProvisioner(Client).Provision);
            AddProvisioner<Script>(new VultrScriptProvisioner(Client).Provision);
            AddProvisioner<Server>(new VultrServerProvisioner(Client).Provision);
        }

        /// <summary>
        /// Instantiates an IPlatform instance to configure and provision Vultr services.
        /// </summary>
        /// <param name="apiKey">The API key to use for communicating with Vultr.</param>
        /// <param name="apiUrl">The API URL for Vultr. Set this to override the
        /// Vultr API endpoint (e.g. for testing).</param>
        /// <returns>The instantiated IPlatform instance.</returns>
        /// <exception cref="ArgumentNullException">If <param name="apiKey"> is null or
        /// empty.</param></exception>
        public static IPlatform CreateVultrPlatform(string apiKey, string apiUrl)
        {
            return new VultrPlatform(apiKey, apiUrl);
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
