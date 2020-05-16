using agrix.Configuration;
using agrix.Platforms.Vultr.Provisioning;
using Server = agrix.Configuration.Server;
using System.Collections.Generic;
using System;
using Vultr.API;

namespace agrix.Platforms.Vultr
{
    internal delegate void Provisioner<T>(T firewall, bool dryrun = false);

    /// <summary>
    /// Describes methods to communicate with the Vultr platform.
    /// </summary>
    internal class VultrPlatform : Platform
    {
        private VultrClient Client { get; }

        private Dictionary<Type, Action<object, bool>> KnownProvisioners { get; }
            = new Dictionary<Type, Action<object, bool>>();

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

            AddProvisioner<Firewall>(new VultrFirewallProvisioner(Client).Provision);
            AddProvisioner<Script>(new VultrScriptProvisioner(Client).Provision);
            AddProvisioner<Server>(new VultrServerProvisioner(Client).Provision);
        }

        /// <summary>
        /// Adds a provisioner to create infrastructure from a configuration.
        /// </summary>
        /// <typeparam name="T">The type of configuration the provisioner
        /// provisions.</typeparam>
        /// <param name="provisioner">The provisioner to add.</param>
        protected void AddProvisioner<T>(Provisioner<T> provisioner)
        {
            KnownProvisioners[typeof(T)] = (item, dryrun) => provisioner((T)item, dryrun);
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
            foreach (var type in infrastructure.Types)
            {
                foreach (var item in infrastructure.GetItems(type))
                {
                    if (!KnownProvisioners.TryGetValue(type, out var action))
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
