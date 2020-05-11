using CommandLine;

namespace agrix
{
    /// <summary>
    /// The CLI options for the program.
    /// </summary>
    internal class Options
    {
        [Value(0, MetaName = "filename", HelpText = "The agrix config file to load")]
        public string Filename { get; set; }
    }
}
