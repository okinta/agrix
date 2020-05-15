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
    internal struct VultrOS
    {
        public int OSID { get; }
        public int? APPID { get; }
        public int? ISOID { get; }
        public int? SCRIPTID { get; }
        public int? SNAPSHOTID { get; }

        private const int AppOSID = 186;
        private const int ISOOSID = 159;
        private const int SnapshotOSID = 164;

        private VultrOS(int osid, int? appid = null, int? isoid = null,
            int? scriptid = null, int? snapshotid = null)
        {
            OSID = osid;
            APPID = appid;
            ISOID = isoid;
            SCRIPTID = scriptid;
            SNAPSHOTID = snapshotid;
        }

        /// <summary>
        /// Creates a new application configuration.
        /// </summary>
        /// <param name="name">The name of the application.</param>
        /// <param name="client">The VultrClient instance to communicate with
        /// Vultr.</param>
        /// <returns>The configured OS instance.</returns>
        public static VultrOS CreateApp(string name, VultrClient client)
        {
            var apps = client.Application.GetApplications();

            KeyValuePair<string, Application> app;
            try
            {
                app = apps.Applications.Single(app => app.Value.deploy_name == name);
            }
            catch (InvalidOperationException e)
            {
                throw new ArgumentException(
                    string.Format("Cannot find app called {0}", name), "name", e);
            }

            return new VultrOS(AppOSID, appid: int.Parse(app.Key));
        }

        /// <summary>
        /// Creates a new ISO configuration.
        /// </summary>
        /// <param name="name">The name of the ISO.</param>
        /// <param name="client">The VultrClient instance to communicate with
        /// Vultr.</param>
        /// <returns>The configured OS instance.</returns>
        public static VultrOS CreateISO(string name, VultrClient client)
        {
            var isos = client.ISOImage.GetISOImages();

            KeyValuePair<string, ISOImage> iso;
            try
            {
                iso = isos.ISOImages.Single(iso => iso.Value.filename == name);
            }
            catch (InvalidOperationException e)
            {
                throw new ArgumentException(
                    string.Format("Cannot find ISO called {0}", name), "name", e);
            }

            return new VultrOS(ISOOSID, isoid: int.Parse(iso.Key));
        }

        /// <summary>
        /// Creates a new Operating System configuration.
        /// </summary>
        /// <param name="name">The name of the Operating System.</param>
        /// <param name="client">The VultrClient instance to communicate with
        /// Vultr.</param>
        /// <returns>The configured OS instance.</returns>
        public static VultrOS CreateOS(string name, VultrClient client)
        {
            return new VultrOS(FindOperatingSystem(name, client));
        }

        /// <summary>
        /// Creates a new script configuration.
        /// </summary>
        /// <param name="name">The name of the Operating System.</param>
        /// <param name="scriptName">The name of the script.</param>
        /// <param name="client">The VultrClient instance to communicate with
        /// Vultr.</param>
        /// <returns>The configured OS instance.</returns>
        public static VultrOS CreateScript(string name, string scriptName, VultrClient client)
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
                    string.Format("Cannot find script called {0}", scriptName),
                    "scriptName", e);
            }

            return new VultrOS(
                FindOperatingSystem(name, client), scriptid: int.Parse(script.Key));
        }

        /// <summary>
        /// Creates a new snapshot configuration.
        /// </summary>
        /// <param name="id">The ID of the snapshot.</param>
        /// <param name="client">The VultrClient instance to communicate with
        /// Vultr.</param>
        /// <returns>The configured OS instance.</returns>
        public static VultrOS CreateSnapshot(int id, VultrClient client)
        {
            var snapshots = client.Snapshot.GetSnapshots();

            if (!snapshots.Snapshots.ContainsKey(id.ToString()))
            {
                throw new ArgumentException(
                    string.Format("Cannot find snapshot with ID {0}", id), "id");
            }

            return new VultrOS(SnapshotOSID, snapshotid: id);
        }

        private static int FindOperatingSystem(string name, VultrClient client)
        {
            var systems = client.OperatingSystem.GetOperatingSystems();

            KeyValuePair<int, OperatingSystem> system;
            try
            {
                system = systems.OperatingSystems.Single(
                    system => system.Value.name == name);
            }
            catch (InvalidOperationException e)
            {
                throw new ArgumentException(
                    string.Format("Cannot find Operating System called {0}", name),
                    "name", e);
            }

            return system.Key;
        }
    }
}
