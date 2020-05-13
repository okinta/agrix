using CommandLine;
using System.IO;
using System.Runtime.CompilerServices;
using System;

[assembly: InternalsVisibleTo("tests")]

namespace agrix
{
    /// <summary>
    /// Represents a program exit code.
    /// </summary>
    internal enum ExitCode
    {
        Success = 0,
        BadConfig = 1,
        InvalidArguments = 2
    }

    /// <summary>
    /// Entrypoint for the program.
    /// </summary>
    internal class Program
    {
        private ExitCode ExitCode { get; set; } = ExitCode.Success;

        /// <summary>
        /// Entrypoint for the program. Processes CLI arguments and runs the application.
        /// </summary>
        /// <param name="args">The CLI arguments provided to the program.</param>
        public static int Main(string[] args)
        {
            var program = new Program();
            Parser.Default.ParseArguments<Options>(args).WithParsed(program.Run);
            return (int)program.ExitCode;
        }

        /// <summary>
        /// Runs the application with the given CLI arguments.
        /// </summary>
        /// <param name="options">The CLI arguments to run the program with.</param>
        private void Run(Options options)
        {
            string input;
            if (string.IsNullOrEmpty(options.Filename))
            {
                input = StdInput.Read();
            }
            else
            {
                try
                {
                    input = File.ReadAllText(options.Filename);
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine(e.Message);
                    ExitCode = ExitCode.InvalidArguments;
                    return;
                }
            }

            if (string.IsNullOrEmpty(input))
            {
                Console.Error.WriteLine("config is empty");
                ExitCode = ExitCode.InvalidArguments;
                return;
            }

            if (string.IsNullOrEmpty(options.ApiKey))
            {
                options.ApiKey = Environment.GetEnvironmentVariable(
                    Constants.EnvPlatformApiKey);
            }

            if (string.IsNullOrEmpty(options.ApiKey))
            {
                Console.Error.WriteLine(
                    "No platform API key provided. Either set {0} command line argument or the {1} environment variable",
                    Constants.ApiKeyArgument, Constants.EnvPlatformApiKey);
                ExitCode = ExitCode.InvalidArguments;
                return;
            }

            var agrix = new Agrix(input, options.ApiKey);

            try
            {
                agrix.Validate();
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
                ExitCode = ExitCode.BadConfig;
                return;
            }

            if (!options.Validate) agrix.Process();
        }
    }
}
