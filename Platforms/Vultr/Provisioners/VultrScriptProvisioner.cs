﻿using agrix.Configuration;
using agrix.Extensions;
using ScriptType = agrix.Configuration.ScriptType;
using System.Collections.Generic;
using System.Linq;
using System;
using Vultr.API.Models;
using Vultr.API;

namespace agrix.Platforms.Vultr.Provisioners
{
    /// <summary>
    /// Provisions Vultr startup scripts.
    /// </summary>
    internal class VultrScriptProvisioner : VultrProvisioner<Script>
    {
        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="client">The VultrClient to use to provision servers.</param>
        public VultrScriptProvisioner(VultrClient client) : base(client) { }

        /// <summary>
        /// Provisions a new Vultr startup script.
        /// </summary>
        /// <param name="script">The configuration to reference to provision the
        /// script.</param>
        /// <param name="dryrun">Whether or not this is a dryrun. If set to true then
        /// provision commands will not be sent to the platform and instead messaging
        /// will be outputted describing what would be done.</param>
        public override void Provision(Script script, bool dryrun = false)
        {
            var type = script.Type switch
            {
                ScriptType.Boot => global::Vultr.API.Models.ScriptType.boot,
                ScriptType.PXE => global::Vultr.API.Models.ScriptType.pxe,
                _ => throw new ArgumentException(
                    $"Unknown script type {script.Type}", nameof(script)),
            };

            Console.WriteLine("Creating script");
            ConsoleX.WriteLine("name", script.Name);
            ConsoleX.WriteLine("type", type);
            ConsoleX.WriteLine("script", script.Content);

            var existingScripts = Client.StartupScript.GetStartupScripts();

            bool Predicate(KeyValuePair<string, StartupScript> existingScript) =>
                existingScript.Value.name == script.Name;
            if (existingScripts.StartupScripts != null
                && existingScripts.StartupScripts.Exists(Predicate))
            {
                var (id, existingScript) =
                    existingScripts.StartupScripts.Single(Predicate);
                Console.WriteLine("Script {0} with ID {1} already exists",
                    script.Name, id);

                if (existingScript.type != script.Type.ToString().ToLower())
                {
                    Console.WriteLine("Script {0} type is different", script.Name);
                    Console.WriteLine("Deleting script {0}", script.Name);

                    if (!dryrun)
                    {
                        Client.StartupScript.DeleteStartupScript(id);
                        Console.WriteLine("Deleted script {0}", script.Name);
                    }

                    Console.WriteLine("Creating new script called {0}", script.Name);
                    if (!dryrun)
                    {
                        var result = Client.StartupScript.CreateStartupScript(
                            script.Name, script.Content, type);
                        Console.WriteLine("Created script with ID {0}",
                            result.StartupScript.SCRIPTID);
                    }
                }
                else if (existingScript.script != script.Content)
                {
                    Console.WriteLine("Script {0} content is different", script.Name);
                    Console.WriteLine("Updating script {0} content", script.Name);

                    if (!dryrun)
                    {
                        Client.StartupScript.UpdateStartupScript(
                            id, script: script.Content);
                        Console.WriteLine("Updated script {0} content", script.Name);
                    }
                }
                else
                    Console.WriteLine("Script {0} is the same. Moving on.", script.Name);
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

            Console.WriteLine("---");
        }
    }
}
