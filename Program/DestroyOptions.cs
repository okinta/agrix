using CommandLine;

namespace agrix.Program
{
    /// <summary>
    /// CLI options for destroying infrastructure.
    /// </summary>
    [Verb("destroy", HelpText = "Destroys infrastructure.")]
    internal class DestroyOptions : DryrunOption
    {
    }
}
