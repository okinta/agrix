using System;

namespace agrix.Configuration
{
    /// <summary>
    /// Represents a server configuration.
    /// </summary>
    internal readonly struct Server
    {
        /// <summary>
        /// Whether or not private networking is enabled.
        /// </summary>
        public bool PrivateNetworking { get; }

        /// <summary>
        /// The operating system to run on the server.
        /// </summary>
        public OperatingSystem Os { get; }

        /// <summary>
        /// The plan for the server.
        /// </summary>
        public Plan Plan { get; }

        /// <summary>
        /// The firewall group for the server.
        /// </summary>
        public string Firewall { get; }

        /// <summary>
        /// The label assigned to the server.
        /// </summary>
        public string Label { get; }

        /// <summary>
        /// The region the server resides in.
        /// </summary>
        public string Region { get; }

        /// <summary>
        /// The script for the server to load on first boot.
        /// </summary>
        public string StartupScript { get; }

        /// <summary>
        /// The tag assigned to the server.
        /// </summary>
        public string Tag { get; }

        /// <summary>
        /// User data associated with the server.
        /// </summary>
        public string UserData { get; }

        /// <summary>
        /// SSH keys assigned to the server.
        /// </summary>
        public string[] SshKeys { get; }

        /// <summary>
        /// Creates a new server configuration.
        /// </summary>
        /// <param name="os">The server's operating system.</param>
        /// <param name="plan">The server's plan.</param>
        /// <param name="region">The server's region.</param>
        /// <param name="privateNetworking">Whether or not to enable private
        /// networking. Defaults to false.</param>
        /// <param name="firewall">(Optional) The firewall group to assign the server
        /// to.</param>
        /// <param name="label">(Optional) The label to assign the server.</param>
        /// <param name="startupScript">(Optional) The script to load upon the server's
        /// first boot.</param>
        /// <param name="tag">(Optional) The tag to assign to the server.</param>
        /// <param name="userData">(Optional) User data associated with the server.</param>
        /// <param name="sshKeys">(Optional) The list of SSH keys to assign to the
        /// server.</param>
        public Server(OperatingSystem os, Plan plan, string region,
            bool privateNetworking = false, string firewall = "", string label = "",
            string startupScript = "", string tag = "", string userData = "",
            string[] sshKeys = null)
        {
            if (string.IsNullOrEmpty(region))
                throw new ArgumentNullException(
                    nameof(region), "must not be empty");

            Firewall = firewall;
            Label = label;
            Os = os;
            Plan = plan;
            PrivateNetworking = privateNetworking;
            Region = region;
            SshKeys = sshKeys ?? new string[0];
            StartupScript = startupScript;
            Tag = tag;
            UserData = userData;
        }
    }
}
