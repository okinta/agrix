using agrix.Configuration;
using agrix.Platforms;
using System.Collections.Generic;
using System;

namespace tests
{
    /// <summary>
    /// A non-functional platform used for testing purposes.
    /// </summary>
    [Platform("test")]
    internal class TestPlatform : Platform
    {
        /// <summary>
        /// Gets the last TestPlatform instance that was instantiated.
        /// </summary>
        public static TestPlatform LastInstance { get; private set; } = null;

        /// <summary>
        /// Gets the list of server provision calls.
        /// </summary>
        public IReadOnlyList<Tuple<Server, bool>> Provisions { get { return provisions; } }

        private readonly List<Tuple<Server, bool>> provisions =
            new List<Tuple<Server, bool>>();

        public TestPlatform(string _)
        {
            AddProvisioner<Server>(ProvisionServer);
            LastInstance = this;
        }

        public override void TestConnection()
        {
        }

        private void ProvisionServer(Server server, bool dryrun = false)
        {
            provisions.Add(new Tuple<Server, bool>(server, dryrun));
        }
    }
}
