using CommandLine;

namespace agrix.Program
{
    /// <summary>
    /// CLI options for provisioning infrastructure.
    /// </summary>
    [Verb("provision", HelpText = "Provisions infrastructure.")]
    internal class ProvisionOptions : BaseOptions
    {
    }
}
