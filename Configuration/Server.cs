using System;

namespace agrix.Configuration
{
    internal class Server
    {
        public bool PrivateNetworking { get; } = false;
        public OperatingSystem OS { get; }
        public Plan Plan { get; }
        public string Firewall { get; } = "";
        public string Label { get; } = "";
        public string Region { get; }
        public string StartupScript { get; } = "";
        public string Tag { get; } = "";
        public string UserData { get; } = "";
        public string[] SSHKeys { get; }

        public Server(OperatingSystem os, Plan plan, string region,
            bool privateNetworking = false, string firewall = "", string label = "",
            string startupScript = "", string tag = "", string userData = "",
            string[] sshKeys = null)
        {
            if (string.IsNullOrEmpty(region))
            {
                throw new ArgumentNullException("region", "must not be empty");
            }

            Firewall = firewall;
            Label = label;
            OS = os;
            Plan = plan;
            PrivateNetworking = privateNetworking;
            Region = region;
            SSHKeys = sshKeys;
            StartupScript = startupScript;
            Tag = tag;
            UserData = userData;
        }
    }
}
