using Server = agrix.Configuration.Server;
using System.Collections.Generic;
using System.Linq;
using System;
using Vultr.API.Models.Responses;
using Vultr.API;

namespace agrix.Platforms.Vultr
{
    /// <summary>
    /// Describes methods to communicate with the Vultr platform.
    /// </summary>
    internal class VultrPlatform : IPlatform
    {
        private string ApiKey { get; }
        private VultrClient Client { get; }

        /// <summary>
        /// Instantiates the instance.
        /// </summary>
        /// <param name="apiKey">The API key to use for communicating with Vultr.</param>
        public VultrPlatform(string apiKey)
        {
            ApiKey = apiKey;
            Client = new VultrClient(ApiKey);
        }

        /// <summary>
        /// Provisions a server using the given configuration.
        /// </summary>
        /// <param name="server">The server configuration to use for provisioning.</param>
        public void Provision(Server server)
        {
            var os = GetOS(server);
            Client.Server.CreateServer(
                DCID: GetRegionID(server.Region),
                VPSPLANID: GetPlanID(server.Plan),
                OSID: os.OSID,
                ISOID: os.ISOID?.ToString(),
                SCRIPTID: os.SCRIPTID,
                SNAPSHOTID: os.SNAPSHOTID?.ToString(),
                enable_private_network: server.PrivateNetworking,
                label: server.Label,
                APPID: os.APPID,
                userdata: server.Label,
                notify_activate: false,
                tag: server.Tag
            );
        }

        /// <summary>
        /// Tests the connection. Throws an exception if the connection is invalid.
        /// </summary>
        public void TestConnection()
        {
            Client.Account.GetInfo();
        }

        /// <summary>
        /// Gets the ID of the given region.
        /// </summary>
        /// <param name="name">The name of the region to retrieve the ID for.</param>
        /// <returns>The ID of the given region.</returns>
        /// <exception cref="ArgumentException">If the region cannot be found.</exception>
        public int GetRegionID(string name)
        {
            var regions = Client.Region.GetRegions();

            KeyValuePair<int, Region> region;
            try
            {
                region = regions.Regions.Single(region => region.Value.name == name);
            }
            catch (InvalidOperationException e)
            {
                throw new ArgumentException(
                    string.Format("Cannot find region called {0}", name), "name", e);
            }

            return region.Key;
        }

        /// <summary>
        /// Gets the ID of the given plan.
        /// </summary>
        /// <param name="name">The name of the plan to retrieve the ID for.</param>
        /// <returns>The ID of the given plan.</returns>
        /// <exception cref="ArgumentException">If the plan cannot be found.</exception>
        public int GetPlanID(Configuration.Plan plan)
        {
            var plans = Client.Plan.GetPlans();

            foreach (var vultrPlan in plans.Plans)
            {
                if (int.Parse(vultrPlan.Value.vcpu_count) == plan.CPU
                    && int.Parse(vultrPlan.Value.ram) == plan.Memory
                    && vultrPlan.Value.plan_type == plan.Type)
                {
                    return vultrPlan.Key;
                }
            }

            throw new ArgumentException(
                string.Format("Cannot find plan {0}", plan), "plan");
        }

        /// <summary>
        /// Gets the Operating System configuration for the Server.
        /// </summary>
        /// <param name="server">The Server to extract the Operating System
        /// configuration for.</param>
        /// <returns>The Operating System configuration for the Server.</returns>
        /// <exception cref="ArgumentException">If the Server Operating System is
        /// misconfigured.</exception>
        public OS GetOS(Server server)
        {
            if (!string.IsNullOrEmpty(server.OS.App))
            {
                if (!string.IsNullOrEmpty(server.OS.Name))
                {
                    throw new ArgumentException(
                        "OS.Name must be empty if OS.App is set", "server");
                }

                if (!string.IsNullOrEmpty(server.OS.ISO))
                {
                    throw new ArgumentException(
                        "OS.ISO must be empty if OS.App is set", "server");
                }

                if (!string.IsNullOrEmpty(server.StartupScript))
                {
                    throw new ArgumentException(
                        "OS.StartupScript must be empty if OS.App is set", "server");
                }

                return OS.CreateApp(server.OS.App, Client);
            }

            if (!string.IsNullOrEmpty(server.OS.ISO))
            {
                if (!string.IsNullOrEmpty(server.OS.Name))
                {
                    throw new ArgumentException(
                        "OS.Name must be empty if OS.ISO is set", "server");
                }

                if (!string.IsNullOrEmpty(server.StartupScript))
                {
                    throw new ArgumentException(
                        "OS.StartupScript must be empty if OS.ISO is set", "server");
                }

                return OS.CreateISO(server.OS.ISO, Client);
            }

            if (string.IsNullOrEmpty(server.OS.Name))
            {
                throw new ArgumentException(
                    "OS.App, OS.ISO or OS.Name must be set", "server");
            }

            if (!string.IsNullOrEmpty(server.StartupScript))
            {
                return OS.CreateScript(server.OS.Name, server.StartupScript, Client);
            }
            else
            {
                return OS.CreateOS(server.OS.Name, Client);
            }
        }
    }
}
