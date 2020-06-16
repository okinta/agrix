using CommandLine;

namespace agrix.Program
{
    /// <summary>
    /// The base CLI options for the program with the ability to perform a dryrun.
    /// </summary>
    internal abstract class DryrunOption : BaseOptions
    {
        [Option('d', "dryrun", Default = false,
            HelpText = "Set flag to not send provisioning calls to the platform API "
                       + "and instead just output what would happen.")]
        public bool Dryrun { get; set; }
    }
}
