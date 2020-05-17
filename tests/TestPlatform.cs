﻿using agrix.Configuration;
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
        public static TestPlatform LastInstance { get; private set; }

        /// <summary>
        /// Gets the list of server provision calls.
        /// </summary>
        public IReadOnlyList<Tuple<Server, bool>> Provisions => _provisions;

        private readonly List<Tuple<Server, bool>> _provisions =
            new List<Tuple<Server, bool>>();

        // ReSharper disable once UnusedParameter.Local
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
            _provisions.Add(new Tuple<Server, bool>(server, dryrun));
        }
    }
}