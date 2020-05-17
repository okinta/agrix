using CommandLine;

namespace agrix.Program
{
    /// <summary>
    /// CLI options for validating a configuration.
    /// </summary>
    [Verb("validate", HelpText = "Validates a configuration.")]
    internal class ValidateOptions : BaseOptions { }
}
