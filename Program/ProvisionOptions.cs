﻿using CommandLine;

namespace agrix.Program
{
    /// <summary>
    /// CLI options for provisioning infrastructure.
    /// </summary>
    [Verb("provision", HelpText = "Provisions infrastructure.")]
    internal class ProvisionOptions : BaseOptions
    {
        [Option('d', "dryrun", Default = false,
            HelpText = "Set flag to not send provisioning calls to the platform API and instead just output what would happen.")]
        public bool Dryrun { get; set; }
    }
}