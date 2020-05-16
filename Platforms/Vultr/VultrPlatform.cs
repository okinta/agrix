using agrix.Configuration;
using agrix.Platforms.Vultr.Provisioning;
using Server = agrix.Configuration.Server;
using System.Collections.Generic;
using System;
using Vultr.API;

namespace agrix.Platforms.Vultr
{
    /// <summary>
    /// Describes methods to communicate with the Vultr platform.
    /// </summary>
    internal class VultrPlatform : Platform
    {
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
        /// Provisions infrastructure referencing the given configuration.
        /// </summary>
        /// <param name="server">The Infrastructure configuration to provision.</param>
        /// <param name="dryrun">Whether or not this is a dryrun. If set to true then
        /// provision commands will not be sent to the platform and instead messaging
        /// will be outputted describing what would be done.</param>
        public override void Provision(Infrastructure infrastructure, bool dryrun = false)
        {
            var mapping = new Dictionary<Type, Action<object, bool>>
            {
                [typeof(Firewall)] = (firewall, dryrun) =>
                {
                    var provisioner = new VultrFirewallProvisioner(Client);
                    provisioner.Provision((Firewall)firewall, dryrun);
                },

                [typeof(Server)] = (server, dryrun) =>
                {
                    var provisioner = new VultrServerProvisioner(Client);
                    provisioner.Provision((Server)server, dryrun);
                },

                [typeof(Script)] = (script, dryrun) =>
                {
                    var provisioner = new VultrScriptProvisioner(Client);
                    provisioner.Provision((Script)script, dryrun);
                }
            };

            foreach (var type in infrastructure.Types)
            {
                foreach (var item in infrastructure.GetItems(type))
                {
                    if (!mapping.TryGetValue(type, out var action))
                        throw new ArgumentException(string.Format(
                            "Unknown item type {0}", type));

                    action(item, dryrun);
                }
            }
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
