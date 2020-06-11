using agrix.Configuration.Parsers;
using agrix.Configuration;
using agrix.Extensions;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections;
using System.Linq;
using System;
using YamlDotNet.RepresentationModel;

namespace agrix.Platforms
{
    /// <summary>
    /// Provisions infrastructure.
    /// </summary>
    /// <typeparam name="T">The type the provisioner provisions.</typeparam>
    /// <param name="item">The item to provision.</param>
    /// <param name="dryrun">Whether or not this is a dryrun. If set to true then
    /// provision commands will not be sent to the platform and instead messaging
    /// will be outputted describing what would be done.</param>
    internal delegate void Provisioner<in T>(T item, bool dryrun = false);

    /// <summary>
    /// Describes an interface for communicating with a platform. Intended as a base
    /// class for concrete platform implementations.
    /// </summary>
    internal abstract class Platform : IPlatform
    {
        /// <summary>
        /// The instance used to process YAML and call the relevant Parse{T} delegate.
        /// </summary>
        protected IParser Parser { get; set; } = new Parser();

        private Dictionary<string, Action<Infrastructure, YamlNode>> KnownParserNodes
            { get; } = new Dictionary<string, Action<Infrastructure, YamlNode>>();
        private OrderedDictionary KnownDestroyers { get; } = new OrderedDictionary();
        private OrderedDictionary KnownProvisioners { get; } = new OrderedDictionary();

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        protected Platform()
        {
            AddNullParser("platform");
            AddParser("servers", new ServerParser().Parse);
            AddParser("scripts", new ScriptParser().Parse);
            AddParser("firewalls", new FirewallParser().Parse);
        }

        /// <summary>
        /// Loads infrastructure configuration from the given YAML.
        /// </summary>
        /// <param name="node">The YAML to load configuration from.</param>
        /// <returns>The infrastructure configuration loaded from the given YAML.</returns>
        public virtual Infrastructure Load(YamlMappingNode node)
        {
            var infrastructure = new Infrastructure();

            foreach (var (tag, value) in node.Children)
            {
                if (!KnownParserNodes.TryGetValue(tag.GetTag(), out var action))
                    throw new ArgumentException(
                    $"Unknown tag {tag.GetTag()} (line {tag.Start.Line})");

                action(infrastructure, value);
            }

            return infrastructure;
        }

        /// <summary>
        /// Provisions infrastructure referencing the given configuration.
        /// </summary>
        /// <param name="infrastructure">The Infrastructure configuration to
        /// provision.</param>
        /// <param name="dryrun">Whether or not this is a dryrun. If set to true then
        /// provision commands will not be sent to the platform and instead messaging
        /// will be outputted describing what would be done.</param>
        public virtual void Provision(Infrastructure infrastructure, bool dryrun = false)
        {
            Provision(KnownProvisioners, infrastructure, dryrun);
        }

        /// <summary>
        /// Destroys infrastructure referencing the given configuration.
        /// </summary>
        /// <param name="infrastructure">The Infrastructure configuration to
        /// provision.</param>
        /// <param name="dryrun">Whether or not this is a dryrun. If set to true then
        /// provision commands will not be sent to the platform and instead messaging
        /// will be outputted describing what would be done.</param>
        public virtual void Destroy(Infrastructure infrastructure, bool dryrun)
        {
            Provision(KnownDestroyers, infrastructure, dryrun);
        }

        /// <summary>
        /// Tests the connection. Throws an exception if the connection is invalid.
        /// </summary>
        public abstract void TestConnection();

        /// <summary>
        /// Adds a parser that ignores an YAML node.
        /// </summary>
        /// <param name="name">The name of the node to ignore.</param>
        protected void AddNullParser(string name)
        {
            KnownParserNodes[name] = (infrastructure, item) => { };
        }

        /// <summary>
        /// Adds a parser to create instances from a YAML node.
        /// </summary>
        /// <typeparam name="T">The type of instances to create.</typeparam>
        /// <param name="name">The name of the node to create instances from.</param>
        /// <param name="parser">The parser to use to create instances from a YAML
        /// node.</param>
        protected void AddParser<T>(string name, Parse<T> parser)
        {
            KnownParserNodes[name] = (infrastructure, item) =>
                infrastructure.AddItems(Parser.Load(name, item, parser));
        }

        /// <summary>
        /// Adds a provisioner to create infrastructure from a configuration.
        /// </summary>
        /// <typeparam name="T">The type of configuration the provisioner
        /// provisions.</typeparam>
        /// <param name="provisioner">The provisioner to add.</param>
        protected void AddProvisioner<T>(Provisioner<T> provisioner)
        {
            Add(KnownProvisioners, provisioner);
        }

        /// <summary>
        /// Adds a destroyer to destroy infrastructure from a configuration.
        /// </summary>
        /// <typeparam name="T">The type of configuration the destroyer
        /// destroys.</typeparam>
        /// <param name="destroyer">The destroyer to add.</param>
        protected void AddDestroyer<T>(Provisioner<T> destroyer)
        {
            Add(KnownDestroyers, destroyer);
        }

        /// <summary>
        /// Provisions/destroys infrastructure referencing the given configuration.
        /// </summary>
        /// <param name="knownProvisioners">The collection of provisioners to use to
        /// setup/teardown the infrastructure.</param>
        /// <param name="infrastructure">The Infrastructure configuration to
        /// provision.</param>
        /// <param name="dryrun">Whether or not this is a dryrun. If set to true then
        /// provision commands will not be sent to the platform and instead messaging
        /// will be outputted describing what would be done.</param>
        private static void Provision(
            IEnumerable knownProvisioners, Infrastructure infrastructure, bool dryrun)
        {
            foreach (DictionaryEntry provisioners in knownProvisioners)
            {
                var type = (Type)provisioners.Key;
                var action = (Action<object, bool>)provisioners.Value;
                if (action is null) continue;
                if (!infrastructure.Types.Contains(type)) continue;

                foreach (var item in infrastructure.GetItems(type))
                    action(item, dryrun);
            }
        }

        /// <summary>
        /// Adds a provisioner/destroyer to the given dictionary.
        /// </summary>
        /// <typeparam name="T">The type of configuration the provisioner/destroyer
        /// provisions.</typeparam>
        /// <param name="provisioners">The dictionary to add the provisioner/destroyer
        /// to.</param>
        /// <param name="provisioner">The provisioner/destroyer to add.</param>
        private static void Add<T>(IDictionary provisioners, Provisioner<T> provisioner)
        {
            provisioners[typeof(T)] =
                (Action<object, bool>)((item, dryrun) =>
                    provisioner((T)item, dryrun));
        }
    }
}
