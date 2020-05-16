using agrix.Extensions;
using Server = agrix.Configuration.Server;
using System.Collections.Generic;
using System.Linq;
using System;
using Vultr.API.Models;
using Vultr.API;

namespace agrix.Platforms.Vultr.Provisioning
{
    /// <summary>
    /// Provisions Vultr servers.
    /// </summary>
    internal class VultrServerProvisioner : VultrProvisioner
    {
        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="client">The VultrClient to use to provision servers.</param>
        public VultrServerProvisioner(VultrClient client) : base(client) { }

        /// <summary>
        /// Provisions a new Vultr server.
        /// </summary>
        /// <param name="server">The configuration to reference to provision the
        /// server.</param>
        /// <param name="dryrun">Whether or not this is a dryrun. If set to true then
        /// provision commands will not be sent to the platform and instead messaging
        /// will be outputted describing what would be done.</param>
        public void Provision(Server server, bool dryrun = false)
        {
            var os = GetOS(server);
            var DCID = GetRegionID(server.Region);
            var VPSPLANID = GetPlanID(server.Plan);
            var OSID = os.OSID;
            var ISOID = os.ISOID?.ToString();
            var SCRIPTID = os.SCRIPTID;
            var SNAPSHOTID = os.SNAPSHOTID?.ToString();
            var enable_private_network = server.PrivateNetworking;
            var label = server.Label;
            var APPID = os.APPID;
            var userdata = server.UserData.Base64Encode();
            var notify_activate = false;
            var tag = server.Tag;

            Console.WriteLine("Provisioning server");
            ConsoleX.WriteLine("DCID", DCID);
            ConsoleX.WriteLine("VPSPLANID", VPSPLANID);
            ConsoleX.WriteLine("OSID", OSID);
            ConsoleX.WriteLine("ISOID", ISOID);
            ConsoleX.WriteLine("SCRIPTID", SCRIPTID);
            ConsoleX.WriteLine("SNAPSHOTID", SNAPSHOTID);
            ConsoleX.WriteLine("enable_private_network", enable_private_network);
            ConsoleX.WriteLine("label", label);
            ConsoleX.WriteLine("APPID", APPID);
            ConsoleX.WriteLine("userdata", userdata);
            ConsoleX.WriteLine("notify_activate", notify_activate);
            ConsoleX.WriteLine("tag", tag);

            var existingServers = Client.Server.GetServers();
            bool predicate(KeyValuePair<
                string, global::Vultr.API.Models.Server> existingServer) =>
                    existingServer.Value.DCID == DCID.ToString()
                    && existingServer.Value.label == label;

            if (existingServers.Servers != null
                && existingServers.Servers.Exists(predicate))
            {
                var existingServer = existingServers.Servers.Single(predicate);
                Console.WriteLine("Server with label {0} in DCID {1} already exists",
                    label, DCID);

                var vultrServer = new global::Vultr.API.Models.Server()
                {
                    OSID = OSID.ToString(),
                    tag = tag,
                    label = label,
                    VPSPLANID = VPSPLANID.ToString(),
                    APPID = APPID?.ToString(),
                    DCID = DCID.ToString()
                };

                // TODO: Check if userdata has changed

                var subid = int.Parse(existingServer.Key);
                if (!vultrServer.IsEquivalent(existingServer.Value))
                {
                    Console.WriteLine("Server is different. Deleting server {0}", subid);

                    if (!dryrun)
                    {
                        Client.Server.DestroyServer(subid);
                        Console.WriteLine("Deleted server {0}", subid);
                    }
                }
                else
                {
                    Console.WriteLine("Server {0} is the same. Moving on.", subid);
                    return;
                }
            }

            if (!dryrun)
            {
                var result = Client.Server.CreateServer(
                    DCID: DCID,
                    VPSPLANID: VPSPLANID,
                    OSID: OSID,
                    ISOID: ISOID,
                    SCRIPTID: SCRIPTID,
                    SNAPSHOTID: SNAPSHOTID,
                    enable_private_network: enable_private_network,
                    label: label,
                    APPID: APPID,
                    userdata: userdata,
                    notify_activate: notify_activate,
                    tag: tag
                );

                Console.WriteLine("Provisioned server with ID {0}", result.Server.SUBID);
            }
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
        public VultrOS GetOS(Server server)
        {
            if (!string.IsNullOrEmpty(server.OS.App))
            {
                if (!string.IsNullOrEmpty(server.OS.Name))
                    throw new ArgumentException(
                        "OS.Name must be empty if OS.App is set", "server");

                if (!string.IsNullOrEmpty(server.OS.ISO))
                    throw new ArgumentException(
                        "OS.ISO must be empty if OS.App is set", "server");

                if (!string.IsNullOrEmpty(server.StartupScript))
                    throw new ArgumentException(
                        "OS.StartupScript must be empty if OS.App is set", "server");

                return VultrOS.CreateApp(server.OS.App, Client);
            }

            if (!string.IsNullOrEmpty(server.OS.ISO))
            {
                if (!string.IsNullOrEmpty(server.OS.Name))
                    throw new ArgumentException(
                        "OS.Name must be empty if OS.ISO is set", "server");

                if (!string.IsNullOrEmpty(server.StartupScript))
                    throw new ArgumentException(
                        "OS.StartupScript must be empty if OS.ISO is set", "server");

                return VultrOS.CreateISO(server.OS.ISO, Client);
            }

            if (string.IsNullOrEmpty(server.OS.Name))
                throw new ArgumentException(
                    "OS.App, OS.ISO or OS.Name must be set", "server");

            if (!string.IsNullOrEmpty(server.StartupScript))
                return VultrOS.CreateScript(server.OS.Name, server.StartupScript, Client);

            else
                return VultrOS.CreateOS(server.OS.Name, Client);
        }
    }
}
