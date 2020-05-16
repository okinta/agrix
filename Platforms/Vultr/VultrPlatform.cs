using agrix.Configuration;
using agrix.Extensions;
using agrix.Platforms.Vultr.Provisioning;
using Server = agrix.Configuration.Server;
using System;
using Vultr.API;
using YamlDotNet.RepresentationModel;

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
        /// Loads infrastructure configuration from the given YAML.
        /// </summary>
        /// <param name="yaml">The YAML to load configuration from.</param>
        /// <returns>The infrastructure configuration loaded from the given YAML.</returns>
        public Infrastructure Load(YamlMappingNode node)
        {
            var config = new VultrAgrixConfig();
            var infrastructure = new Infrastructure();
            foreach (var item in node.Children)
            {
                if (item.Key.GetTag() == "platform") continue;
                else if (item.Key.GetTag() == "servers")
                {
                    infrastructure.AddItems(config.LoadServers(item.Value));
                }
                else if (item.Key.GetTag() == "scripts")
                {
                    infrastructure.AddItems(config.LoadScripts(item.Value));
                }
                else if (item.Key.GetTag() == "firewalls")
                {
                    infrastructure.AddItems(config.LoadFirewalls(item.Value));
                }
                else
                {
                    throw new ArgumentException(
                        string.Format("Unknown tag {0}", item.Key.GetTag()));
                }
            }

            return infrastructure;
        }

        /// <summary>
        /// Provisions infrastructure referencing the given configuration.
        /// </summary>
        /// <param name="server">The Infrastructure configuration to provision.</param>
        /// <param name="dryrun">Whether or not this is a dryrun. If set to true then
        /// provision commands will not be sent to the platform and instead messaging
        /// will be outputted describing what would be done.</param>
        public void Provision(Infrastructure infrastructure, bool dryrun = false)
        {
            foreach (var type in infrastructure.Types)
            {
                if (type == typeof(Firewall))
                {
                    foreach (var firewall in infrastructure.GetItems<Firewall>())
                    {
                        var provisioner = new VultrFirewallProvisioner(Client);
                        provisioner.Provision(firewall, dryrun);
                    }
                }
                else if (type == typeof(Script))
                {
                    foreach (var script in infrastructure.GetItems<Script>())
                    {
                        var provisioner = new VultrScriptProvisioner(Client);
                        provisioner.Provision(script, dryrun);
                    }
                }
                else if (type == typeof(Server))
                {
                    foreach (var server in infrastructure.GetItems<Server>())
                    {
                        var provisioner = new VultrServerProvisioner(Client);
                        provisioner.Provision(server, dryrun);
                    }
                }
                else
                {
                    throw new ArgumentException("unknown type");
                }
            }
            throw new NotImplementedException();
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
