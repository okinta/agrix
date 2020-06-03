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
            const bool notifyActivate = false;
            var appId = os.Appid;
            var dcId = GetRegionId(server.Region);
            var enablePrivateNetwork = server.PrivateNetworking;
            var firewallId = GetFirewallId(server.Firewall);
            var isoId = os.IsoId?.ToString();
            var label = server.Label;
            var osId = os.OsId;
            var scriptId = os.ScriptId;
            var snapshotId = os.SnapshotId;
            var sshKeys = GetSshKeys(server.SshKeys);
            var tag = server.Tag;
            var userdata = server.UserData.Base64Encode();
            var vpsPlanId = GetPlanId(server.Plan);

            Console.WriteLine("Provisioning server");
            ConsoleX.WriteLine("APPID", appId);
            ConsoleX.WriteLine("DCID", dcId);
            ConsoleX.WriteLine("enable_private_network", enablePrivateNetwork);
            ConsoleX.WriteLine("FIREWALLGROUPID", firewallId);
            ConsoleX.WriteLine("ISOID", isoId);
            ConsoleX.WriteLine("label", label);
            ConsoleX.WriteLine("notify_activate", notifyActivate);
            ConsoleX.WriteLine("OSID", osId);
            ConsoleX.WriteLine("SCRIPTID", scriptId);
            ConsoleX.WriteLine("SNAPSHOTID", snapshotId);
            ConsoleX.WriteLine("SSHKEYID", sshKeys);
            ConsoleX.WriteLine("tag", tag);
            ConsoleX.WriteLine("userdata", server.UserData);
            ConsoleX.WriteLine("VPSPLANID", vpsPlanId);

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

                var vultrServer = new global::Vultr.API.Models.Server
                {
                    OSID = osId.ToString(),
                    tag = tag,
                    label = label,
                    APPID = appId?.ToString(),
                    DCID = dcId.ToString(),
                    FIREWALLGROUPID = firewallId,
                    VPSPLANID = vpsPlanId.ToString()
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
                    APPID: appId,
                    enable_private_network: enablePrivateNetwork,
                    FIREWALLGROUPID: firewallId,
                    ISOID: isoId,
                    label: label,
                    notify_activate: notifyActivate,
                    SCRIPTID: scriptId,
                    SNAPSHOTID: snapshotId,
                    SSHKEYID: sshKeys,
                    tag: tag,
                    userdata: userdata
                );

                Console.WriteLine("Provisioned server with ID {0}", result.Server.SUBID);
            }

            Console.WriteLine("---");
        }

        /// <summary>
        /// Gets the ID of the given firewall.
        /// </summary>
        /// <param name="firewall">The name of the firewall to retrieve the ID for.</param>
        /// <returns>The ID of the given firewall.</returns>
        /// <exception cref="ArgumentException">If the given firewall cannot be
        /// found.</exception>
        private string GetFirewallId(string firewall)
        {
            if (string.IsNullOrEmpty(firewall)) return "0";

            var groups = Client.Firewall.GetFirewallGroups();

            try
            {
                return groups.FirewallGroups.Single(
                    g =>
                        g.Value.description == firewall).Value.FIREWALLGROUPID;
            }
            catch (InvalidOperationException e)
            {
                throw new ArgumentException(
                    $"Cannot find firewall group called {firewall}",
                    nameof(firewall), e);
            }
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

        /// <summary>
        /// Gets the SSH key IDs for the given SSH key names.
        /// </summary>
        /// <param name="serverSshKeys">The collection of SSH key names to retrieve IDs
        /// for.</param>
        /// <returns>The comma-separated list of SSH key IDs.</returns>
        private string GetSshKeys(IEnumerable<string> serverSshKeys)
        {
            if (serverSshKeys is null) return "";

            var keys = new List<string>();
            var availableKeys = Client.SSHKey.GetSSHKeys().SSHKeys;
            foreach (var sshKey in serverSshKeys)
            {
                try
                {
                    keys.Add(availableKeys.Single(
                            k
                                => k.Value.name == sshKey)
                        .Value.SSHKEYID);
                }
                catch (InvalidOperationException e)
                {
                    throw new ArgumentException(
                        $"Cannot find SSH key named {sshKey}", nameof(sshKey), e);
                }
            }

            return string.Join(',', keys);
        }
    }
}
