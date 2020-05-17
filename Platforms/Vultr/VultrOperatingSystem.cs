using OperatingSystem = Vultr.API.Models.OperatingSystem;
using System.Collections.Generic;
using System.Linq;
using System;
using Vultr.API.Models;
using Vultr.API;

namespace agrix.Platforms.Vultr
{
    /// <summary>
    /// Represents a Vultr Operating System configuration.
    /// </summary>
    internal readonly struct VultrOperatingSystem
    {
        public int OsId { get; }
        public int? Appid { get; }
        public int? IsoId { get; }
        public int? ScriptId { get; }
        public string SnapshotId { get; }

        private const int AppOsId = 186;
        private const int IsoOsId = 159;
        private const int SnapshotOsId = 164;

        private VultrOperatingSystem(int osId, int? appId = null, int? isoId = null,
            int? scriptId = null, string snapshotId = null)
        {
            OsId = osId;
            Appid = appId;
            IsoId = isoId;
            ScriptId = scriptId;
            SnapshotId = snapshotId;
        }

        /// <summary>
        /// Creates a new application configuration.
        /// </summary>
        /// <param name="name">The name of the application.</param>
        /// <param name="client">The VultrClient instance to communicate with
        /// Vultr.</param>
        /// <returns>The configured OS instance.</returns>
        public static VultrOperatingSystem CreateApp(string name, VultrClient client)
        {
            var apps = client.Application.GetApplications();

            KeyValuePair<string, Application> app;
            try
            {
                app = apps.Applications.Single(
                    a => a.Value.deploy_name == name);
            }
            catch (InvalidOperationException e)
            {
                throw new ArgumentException(
                    $"Cannot find app called {name}", nameof(name), e);
            }

            return new VultrOperatingSystem(AppOsId, int.Parse(app.Key));
        }

        /// <summary>
        /// Creates a new ISO configuration.
        /// </summary>
        /// <param name="name">The name of the ISO.</param>
        /// <param name="client">The VultrClient instance to communicate with
        /// Vultr.</param>
        /// <returns>The configured OS instance.</returns>
        public static VultrOperatingSystem CreateIso(string name, VultrClient client)
        {
            var images = client.ISOImage.GetISOImages();

            KeyValuePair<string, ISOImage> iso;
            try
            {
                iso = images.ISOImages.Single(
                    i => i.Value.filename == name);
            }
            catch (InvalidOperationException e)
            {
                throw new ArgumentException(
                    $"Cannot find ISO called {name}", nameof(name), e);
            }

            return new VultrOperatingSystem(IsoOsId, isoId: int.Parse(iso.Key));
        }

        /// <summary>
        /// Creates a new Operating System configuration.
        /// </summary>
        /// <param name="name">The name of the Operating System.</param>
        /// <param name="client">The VultrClient instance to communicate with
        /// Vultr.</param>
        /// <returns>The configured OS instance.</returns>
        public static VultrOperatingSystem CreateOs(string name, VultrClient client)
        {
            return new VultrOperatingSystem(FindOperatingSystem(name, client));
        }

        /// <summary>
        /// Creates a new script configuration.
        /// </summary>
        /// <param name="name">The name of the Operating System.</param>
        /// <param name="scriptName">The name of the script.</param>
        /// <param name="client">The VultrClient instance to communicate with
        /// Vultr.</param>
        /// <returns>The configured OS instance.</returns>
        public static VultrOperatingSystem CreateScript(
            string name, string scriptName, VultrClient client)
        {
            var scripts = client.StartupScript.GetStartupScripts();

            KeyValuePair<string, StartupScript> script;
            try
            {
                script = scripts.StartupScripts.Single(
                    system => system.Value.name == scriptName);
            }
            catch (InvalidOperationException e)
            {
                throw new ArgumentException(
                    $"Cannot find script called {scriptName}",
                    nameof(scriptName), e);
            }

            return new VultrOperatingSystem(
                FindOperatingSystem(name, client), scriptId: int.Parse(script.Key));
        }

        /// <summary>
        /// Creates a new snapshot configuration.
        /// </summary>
        /// <param name="id">The ID of the snapshot.</param>
        /// <param name="client">The VultrClient instance to communicate with
        /// Vultr.</param>
        /// <returns>The configured OS instance.</returns>
        public static VultrOperatingSystem CreateSnapshot(string id, VultrClient client)
        {
            var snapshots = client.Snapshot.GetSnapshots();

            if (!snapshots.Snapshots.ContainsKey(id))
            {
                throw new ArgumentException(
                    $"Cannot find snapshot with ID {id}", nameof(id));
            }

            return new VultrOperatingSystem(SnapshotOsId, snapshotId: id);
        }

        private static int FindOperatingSystem(string name, VultrClient client)
        {
            var systems = client.OperatingSystem.GetOperatingSystems();

            KeyValuePair<int, OperatingSystem> system;
            try
            {
                system = systems.OperatingSystems.Single(
                    s => s.Value.name == name);
            }
            catch (InvalidOperationException e)
            {
                throw new ArgumentException(
                    $"Cannot find Operating System called {name}",
                    nameof(name), e);
            }

            return system.Key;
        }
    }
}
