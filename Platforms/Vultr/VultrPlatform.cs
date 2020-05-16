using agrix.Configuration;
using agrix.Extensions;
using agrix.Platforms.Vultr.Provisioning;
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
            new VultrServerProvisioner(Client).Provision(server, dryrun);
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
    }
}
