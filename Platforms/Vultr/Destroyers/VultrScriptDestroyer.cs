using agrix.Configuration;
using agrix.Extensions;
using System.Collections.Generic;
using System.Linq;
using System;
using Vultr.API.Models;
using Vultr.API;

namespace agrix.Platforms.Vultr.Destroyers
{
    internal class VultrScriptDestroyer : VultrDestroyer<Script>
    {
        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="client">The VultrClient to use to destroy scripts.</param>
        public VultrScriptDestroyer(VultrClient client) : base(client)
        {
        }

        /// <summary>
        /// Destroys an existing Vultr script.
        /// </summary>
        /// <param name="script">The configuration to reference to destroy the
        /// script.</param>
        /// <param name="dryrun">Whether or not this is a dryrun. If set to true then
        /// provision commands will not be sent to the platform and instead messaging
        /// will be outputted describing what would be done.</param>
        public override void Destroy(Script script, bool dryrun = false)
        {
            Console.WriteLine("Destroying script");
            ConsoleX.WriteLine("name", script.Name);

            var existingScripts = Client.StartupScript.GetStartupScripts();

            bool Predicate(KeyValuePair<string, StartupScript> existingScript) =>
                existingScript.Value.name == script.Name
                && existingScript.Value.type == script.Type.ToString().ToLower();
            if (existingScripts.StartupScripts != null
                && existingScripts.StartupScripts.Exists(Predicate))
            {
                var (id, _) =
                    existingScripts.StartupScripts.Single(Predicate);
                Console.WriteLine("Script {0} with ID {1} exists",
                    script.Name, id);

                if (!dryrun)
                {
                    Client.StartupScript.DeleteStartupScript(id);
                    Console.WriteLine("Deleted script {0}", script.Name);
                }
            }
            else
                Console.WriteLine("Script {0} does not exist", script.Name);

            Console.WriteLine("---");
        }
    }
}
