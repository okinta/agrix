using agrix.Configuration;
using agrix.Extensions;
using System.Collections.Generic;
using System.Linq;
using System;
using Vultr.API;

namespace agrix.Platforms.Vultr.Destroyers
{
    internal class VultrServerDestroyer : VultrDestroyer<Server>
    {
        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="client">The VultrClient to use to destroy servers.</param>
        public VultrServerDestroyer(VultrClient client) : base(client)
        {
        }

        /// <summary>
        /// Destroys an existing Vultr server.
        /// </summary>
        /// <param name="server">The configuration to reference to destroy the
        /// server.</param>
        /// <param name="dryrun">Whether or not this is a dryrun. If set to true then
        /// provision commands will not be sent to the platform and instead messaging
        /// will be outputted describing what would be done.</param>
        public override void Destroy(Server server, bool dryrun = false)
        {
            var dcId = Client.Region.GetRegionId(server.Region);
            var label = server.Label;

            Console.WriteLine("Destroying server");
            ConsoleX.WriteLine("DCID", dcId);
            ConsoleX.WriteLine("label", label);

            var existingServers = Client.Server.GetServers();
            bool Predicate(KeyValuePair<
                string, global::Vultr.API.Models.Server> existingServer) =>
                    existingServer.Value.DCID == dcId.ToString()
                    && existingServer.Value.label == label;

            if (existingServers.Servers != null
                && existingServers.Servers.Exists(Predicate))
            {
                var (id, _) = existingServers.Servers.Single(Predicate);
                Console.WriteLine("Server with label {0} in DCID {1} exists",
                    label, dcId);

                var subId = int.Parse(id);
                if (!dryrun)
                {
                    Client.Server.DestroyServer(subId);
                    Console.WriteLine("Deleted server {0}", subId);
                }
            }
            else
                Console.WriteLine("Server with label {0} in DCID {1} does not exist",
                    label, dcId);

            Console.WriteLine("---");
        }
    }
}
