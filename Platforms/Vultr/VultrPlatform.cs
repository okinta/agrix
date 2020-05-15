using agrix.Configuration;
using agrix.Extensions;
using Server = agrix.Configuration.Server;
using System.Collections.Generic;
using System.Linq;
using System;
using Vultr.API.Models;
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
            var userdata = server.UserData;
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
        /// Provisions a Script using the given configuration.
        /// </summary>
        /// <param name="script">The Script configuration to provision.</param>
        /// <param name="dryrun">Whether or not this is a dryrun. If set to true then
        /// provision commands will not be sent to the platform and instead messaging
        /// will be outputted describing what would be done.</param>
        public void Provision(Script script, bool dryrun = false)
        {
            var type = script.Type switch
            {
                Configuration.ScriptType.Boot => global::Vultr.API.Models.ScriptType.boot,
                Configuration.ScriptType.PXE => global::Vultr.API.Models.ScriptType.pxe,
                _ => throw new ArgumentException(
                    string.Format("Unknown script type {0}", script.Type), "script"),
            };

            Console.WriteLine("Creating script");
            ConsoleX.WriteLine("name", script.Name);
            ConsoleX.WriteLine("type", type);
            ConsoleX.WriteLine("script", script.Content);

            var existingScripts = Client.StartupScript.GetStartupScripts();

            bool predicate(KeyValuePair<string, StartupScript> existingScript) =>
                existingScript.Value.name == script.Name;
            if (existingScripts.StartupScripts != null
                && existingScripts.StartupScripts.Exists(predicate))
            {
                var existingScript = existingScripts.StartupScripts.Single(predicate);
                Console.WriteLine("Script {0} with ID {1} already exists",
                    script.Name, existingScript.Key);

                if (existingScript.Value.type != script.Type.ToString().ToLower())
                {
                    Console.WriteLine("Script {0} type is different", script.Name);
                    Console.WriteLine("Deleting script {0}", script.Name);

                    if (!dryrun)
                    {
                        Client.StartupScript.DeleteStartupScript(existingScript.Key);
                        Console.WriteLine("Deleted script {0}", script.Name);
                    }

                    Console.WriteLine("Creating new script called {0}", script.Name);
                    if (!dryrun)
                    {
                        if (!dryrun)
                        {
                            var result = Client.StartupScript.CreateStartupScript(
                                script.Name, script.Content, type);
                            Console.WriteLine("Created script with ID {0}",
                                result.StartupScript.SCRIPTID);
                        }
                    }
                }
                else if (existingScript.Value.script != script.Content)
                {
                    Console.WriteLine("Script {0} content is different", script.Name);
                    Console.WriteLine("Updating script {0} content", script.Name);

                    if (!dryrun)
                    {
                        Client.StartupScript.UpdateStartupScript(
                            existingScript.Key, script: script.Content);
                        Console.WriteLine("Updated script {0} content", script.Name);
                    }
                }
                else
                {
                    Console.WriteLine("Script {0} is the same. Moving on.", script.Name);
                }
            }
            else
            {
                if (!dryrun)
                {
                    var result = Client.StartupScript.CreateStartupScript(
                    script.Name, script.Content, type);
                    Console.WriteLine(
                        "Created script with ID {0}", result.StartupScript.SCRIPTID);
                }
            }
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
        public VultrOS GetOS(Server server)
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

                return VultrOS.CreateApp(server.OS.App, Client);
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

                return VultrOS.CreateISO(server.OS.ISO, Client);
            }

            if (string.IsNullOrEmpty(server.OS.Name))
            {
                throw new ArgumentException(
                    "OS.App, OS.ISO or OS.Name must be set", "server");
            }

            if (!string.IsNullOrEmpty(server.StartupScript))
            {
                return VultrOS.CreateScript(server.OS.Name, server.StartupScript, Client);
            }
            else
            {
                return VultrOS.CreateOS(server.OS.Name, Client);
            }
        }
    }
}
