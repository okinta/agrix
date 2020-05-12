using agrix.Configuration;
using agrix.Extensions;
using OperatingSystem = agrix.Configuration.OperatingSystem;
using System.Collections.Generic;
using System.Linq;
using YamlDotNet.RepresentationModel;

namespace agrix.Platform
{
    /// <summary>
    /// Vultr configuration as code.
    /// </summary>
    internal class Vultr : IPlatform
    {
        /// <summary>
        /// Loads all the server configurations from the given YAML.
        /// </summary>
        /// <param name="config">The YAML list to load servers from.</param>
        /// <returns>The list of Server configurations loaded from the given
        /// YAML.</returns>
        public IList<Server> LoadServers(YamlSequenceNode serverItems)
        {
            var servers = new List<Server>();

            foreach (YamlMappingNode serverItem in serverItems)
            {
                var osMapping = serverItem.GetMapping("os");
                var os = new OperatingSystem(
                    app: osMapping.GetKey("app"),
                    iso: osMapping.GetKey("iso"),
                    name: osMapping.GetKey("name")
                );

                var planMapping = serverItem.GetMapping("plan");
                var plan = new Plan(
                    cpu: planMapping.GetInt("cpu"),
                    memory: planMapping.GetInt("memory"),
                    type: planMapping.GetKey("type", required: true)
                );

                servers.Add(new Server(
                    os: os,
                    plan: plan,
                    region: serverItem.GetKey("region", required: true),
                    privateNetworking: serverItem.GetBool("private-networking", false),
                    firewall: serverItem.GetKey("firewall"),
                    label: serverItem.GetKey("label"),
                    startupScript: serverItem.GetKey("startup-script"),
                    tag: serverItem.GetKey("tag"),
                    userData: serverItem.GetKey("userdata"),
                    sshKeys: serverItem.GetList("ssh-keys").ToArray()
                ));
            }

            return servers;
        }
    }
}
