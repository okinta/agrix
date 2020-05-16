using agrix.Configuration.Parsers;
using agrix.Configuration;
using agrix.Extensions;
using System.Collections.Generic;
using System;
using YamlDotNet.RepresentationModel;

namespace agrix.Platforms
{
    /// <summary>
    /// Describes an interface for communicating with a platform. Intended as a base
    /// class for concrete platform implementations.
    /// </summary>
    internal abstract class Platform : IPlatform
    {
        /// <summary>
        /// Delegate used to create a Firewall instance from a YAML configuration. Can
        /// be overridden in subclasses.
        /// </summary>
        protected Parse<Firewall> ParseFirewall { get; set; } = new FirewallParser().Parse;

        /// <summary>
        /// Delegate used to create a Script instance from a YAML configuration. Can
        /// be overridden in subclasses.
        /// </summary>
        protected Parse<Script> ParseScript { get; set; } = new ScriptParser().Parse;

        /// <summary>
        /// Delegate used to create a Server instance from a YAML configuration. Can
        /// be overridden in subclasses.
        /// </summary>
        protected Parse<Server> ParseServer { get; set; } = new ServerParser().Parse;

        /// <summary>
        /// The instance used to process YAML and call the relevant Parse{T} delegate.
        /// </summary>
        protected Parser Parser { get; set; } = new Parser();

        /// <summary>
        /// Loads infrastructure configuration from the given YAML.
        /// </summary>
        /// <param name="yaml">The YAML to load configuration from.</param>
        /// <returns>The infrastructure configuration loaded from the given YAML.</returns>
        public virtual Infrastructure Load(YamlMappingNode node)
        {
            var infrastructure = new Infrastructure();

            var knownNodes = new Dictionary<string, Action<YamlNode>>
            {
                ["platform"] = item => { },
                ["servers"] = item =>
                    infrastructure.AddItems(Parser.Load("servers", item, ParseServer)),
                ["scripts"] = item =>
                    infrastructure.AddItems(Parser.Load("scripts", item, ParseScript)),
                ["firewalls"] = item =>
                    infrastructure.AddItems(Parser.Load("firewalls", item, ParseFirewall))
            };

            foreach (var item in node.Children)
            {
                if (!knownNodes.TryGetValue(item.Key.GetTag(), out var action))
                    throw new ArgumentException(string.Format(
                        "Unknown tag {0} (line {1})",
                        item.Key.GetTag(), item.Key.Start.Line));

                action(item.Value);
            }

            return infrastructure;
        }

        /// <summary>
        /// Provisions infrastructure referencing the given configuration.
        /// </summary>
        /// <param name="server">The Infrastructure configuration to provision.</param>
        /// <param name="dryrun">Whether or not this is a dryrun. If set to true then
        /// provision commands will not be sent to the platform and instead messaging
        /// will be outputted describing what would be done.</param>
        public abstract void Provision(Infrastructure server, bool dryrun = false);

        /// <summary>
        /// Tests the connection. Throws an exception if the connection is invalid.
        /// </summary>
        public abstract void TestConnection();
    }
}
