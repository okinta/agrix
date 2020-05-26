using agrix.Extensions;
using Server = agrix.Configuration.Server;
using System.Collections.Generic;
using System.Linq;
using System;
using Vultr.API.Models;
using Vultr.API;

namespace agrix.Platforms.Vultr.Provisioners
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
            var os = GetOs(server);
            var dcId = GetRegionId(server.Region);
            var vpsPlanId = GetPlanId(server.Plan);
            var osId = os.OsId;
            var isoId = os.IsoId?.ToString();
            var scriptId = os.ScriptId;
            var snapshotId = os.SnapshotId;
            var enablePrivateNetwork = server.PrivateNetworking;
            var label = server.Label;
            var appId = os.Appid;
            var userdata = server.UserData.Base64Encode();
            const bool notifyActivate = false;
            var tag = server.Tag;

            Console.WriteLine("Provisioning server");
            ConsoleX.WriteLine("DCID", dcId);
            ConsoleX.WriteLine("VPSPLANID", vpsPlanId);
            ConsoleX.WriteLine("OSID", osId);
            ConsoleX.WriteLine("ISOID", isoId);
            ConsoleX.WriteLine("SCRIPTID", scriptId);
            ConsoleX.WriteLine("SNAPSHOTID", snapshotId);
            ConsoleX.WriteLine("enable_private_network", enablePrivateNetwork);
            ConsoleX.WriteLine("label", label);
            ConsoleX.WriteLine("APPID", appId);
            ConsoleX.WriteLine("userdata", server.UserData);
            ConsoleX.WriteLine("notify_activate", notifyActivate);
            ConsoleX.WriteLine("tag", tag);

            var existingServers = Client.Server.GetServers();
            bool Predicate(KeyValuePair<
                string, global::Vultr.API.Models.Server> existingServer) =>
                    existingServer.Value.DCID == dcId.ToString()
                    && existingServer.Value.label == label;

            if (existingServers.Servers != null
                && existingServers.Servers.Exists(Predicate))
            {
                var (id, existingServer) = existingServers.Servers.Single(Predicate);
                Console.WriteLine("Server with label {0} in DCID {1} already exists",
                    label, dcId);

                var vultrServer = new global::Vultr.API.Models.Server()
                {
                    OSID = osId.ToString(),
                    tag = tag,
                    label = label,
                    VPSPLANID = vpsPlanId.ToString(),
                    APPID = appId?.ToString(),
                    DCID = dcId.ToString()
                };

                // TODO: Check if userdata has changed

                var subId = int.Parse(id);
                if (!vultrServer.IsEquivalent(existingServer))
                {
                    Console.WriteLine("Server is different. Deleting server {0}", subId);

                    if (!dryrun)
                    {
                        Client.Server.DestroyServer(subId);
                        Console.WriteLine("Deleted server {0}", subId);
                    }
                }
                else
                {
                    Console.WriteLine("Server {0} is the same. Moving on.", subId);
                    return;
                }
            }

            if (!dryrun)
            {
                var result = Client.Server.CreateServer(
                    dcId,
                    vpsPlanId,
                    osId,
                    ISOID: isoId,
                    SCRIPTID: scriptId,
                    SNAPSHOTID: snapshotId,
                    enable_private_network: enablePrivateNetwork,
                    label: label,
                    APPID: appId,
                    userdata: userdata,
                    notify_activate: notifyActivate,
                    tag: tag
                );

                Console.WriteLine("Provisioned server with ID {0}", result.Server.SUBID);
            }

            Console.WriteLine("---");
        }

        /// <summary>
        /// Gets the ID of the given region.
        /// </summary>
        /// <param name="name">The name of the region to retrieve the ID for.</param>
        /// <returns>The ID of the given region.</returns>
        /// <exception cref="ArgumentException">If the region cannot be found.</exception>
        public int GetRegionId(string name)
        {
            var regions = Client.Region.GetRegions();

            KeyValuePair<int, Region> region;
            try
            {
                region = regions.Regions.Single(
                    r => r.Value.name == name);
            }
            catch (InvalidOperationException e)
            {
                throw new ArgumentException(
                    $"Cannot find region called {name}", nameof(name), e);
            }

            return region.Key;
        }

        /// <summary>
        /// Gets the ID of the given plan.
        /// </summary>
        /// <param name="plan">The name of the plan to retrieve the ID for.</param>
        /// <returns>The ID of the given plan.</returns>
        /// <exception cref="ArgumentException">If the plan cannot be found.</exception>
        public int GetPlanId(Configuration.Plan plan)
        {
            var plans = Client.Plan.GetPlans();

            foreach (var vultrPlan in plans.Plans.Where(
                vultrPlan =>
                    int.Parse(vultrPlan.Value.vcpu_count) == plan.Cpu
                    && int.Parse(vultrPlan.Value.ram) == plan.Memory
                    && vultrPlan.Value.plan_type == plan.Type))
                return vultrPlan.Key;

            throw new ArgumentException(
                $"Cannot find plan {plan}", nameof(plan));
        }

        /// <summary>
        /// Gets the Operating System configuration for the Server.
        /// </summary>
        /// <param name="server">The Server to extract the Operating System
        /// configuration for.</param>
        /// <returns>The Operating System configuration for the Server.</returns>
        /// <exception cref="ArgumentException">If the Server Operating System is
        /// misconfigured.</exception>
        public VultrOperatingSystem GetOs(Server server)
        {
            if (!string.IsNullOrEmpty(server.Os.App))
            {
                if (!string.IsNullOrEmpty(server.Os.Name))
                    throw new ArgumentException(
                        "OS.Name must be empty if OS.App is set", nameof(server));

                if (!string.IsNullOrEmpty(server.Os.Iso))
                    throw new ArgumentException(
                        "OS.ISO must be empty if OS.App is set", nameof(server));

                if (!string.IsNullOrEmpty(server.StartupScript))
                    throw new ArgumentException(
                        "OS.StartupScript must be empty if OS.App is set",
                        nameof(server));

                return VultrOperatingSystem.CreateApp(server.Os.App, Client);
            }

            if (!string.IsNullOrEmpty(server.Os.Iso))
            {
                if (!string.IsNullOrEmpty(server.Os.Name))
                    throw new ArgumentException(
                        "OS.Name must be empty if OS.ISO is set", nameof(server));

                if (!string.IsNullOrEmpty(server.StartupScript))
                    throw new ArgumentException(
                        "OS.StartupScript must be empty if OS.ISO is set",
                        nameof(server));

                return VultrOperatingSystem.CreateIso(server.Os.Iso, Client);
            }

            if (string.IsNullOrEmpty(server.Os.Name))
                throw new ArgumentException(
                    "OS.App, OS.ISO or OS.Name must be set", nameof(server));

            return !string.IsNullOrEmpty(server.StartupScript) ?
                VultrOperatingSystem.CreateScript(server.Os.Name, server.StartupScript, Client)
                : VultrOperatingSystem.CreateOs(server.Os.Name, Client);
        }
    }
}
