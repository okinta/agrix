using CommandLine;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System;

[assembly: InternalsVisibleTo("tests")]

namespace agrix.Program
{
    /// <summary>
    /// Represents a program exit code.
    /// </summary>
    internal enum ExitCode
    {
        Success = 0,
        BadConfig = 1,
        InvalidArguments = 2,
        Exception = 3
    }

    /// <summary>
    /// Entrypoint for the program.
    /// </summary>
    internal class Program
    {
        private ExitCode ExitCode { get; set; } = ExitCode.Success;

        private Assembly Assembly { get; }

        private Program(Assembly assembly) { Assembly = assembly; }

        /// <summary>
        /// Entrypoint for the program. Processes CLI arguments and runs the application.
        /// </summary>
        /// <param name="args">The CLI arguments provided to the program.</param>
        /// <returns>The application status code.</returns>
        public static int Main(string[] args) { return Main(null, args); }

        /// <summary>
        /// Test entrypoint for the program. Processes CLI arguments and runs the
        /// application.
        /// </summary>
        /// <param name="assembly">The Assembly to use for loading platforms.</param>
        /// <param name="args">The CLI arguments provided to the program.</param>
        /// <returns>The application status code.</returns>
        public static int Main(Assembly assembly, string[] args)
        {
            var program = new Program(assembly);
            Parser.Default.ParseArguments<
                ProvisionOptions,
                ValidateOptions
            >(args)
                .WithParsed<ProvisionOptions>(program.Provision)
                .WithParsed<ValidateOptions>(program.Validate);
            return (int)program.ExitCode;
        }

        private void Provision(ProvisionOptions options)
        {
            try
            {
                LoadAgrix(options)?.Process(options.Dryrun);
            }
            catch (WebException e)
            {
                Console.Error.WriteLine(e.Message);
                ExitCode = ExitCode.Exception;
            }
        }

        private void Validate(ValidateOptions options)
        {
            var agrix = LoadAgrix(options);
            if (agrix is null) return;

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

            Console.WriteLine("Configuration is valid");
        }

        /// <summary>
        /// Loads an Agrix instance with the user specified configuration. Configuration
        /// is pulled from either CLI options or stdin.
        /// </summary>
        /// <param name="options">The CLI options.</param>
        /// <returns>An Agrix with the loaded user configuration, or null if there was
        /// an error with loading configuration.</returns>
        private Agrix LoadAgrix(BaseOptions options)
        {
            string input;
            if (string.IsNullOrEmpty(options.Filename))
                input = StdInput.Read();

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
                    return null;
                }
            }

            if (string.IsNullOrEmpty(input))
            {
                Console.Error.WriteLine("config is empty");
                ExitCode = ExitCode.InvalidArguments;
                return null;
            }

            if (string.IsNullOrEmpty(options.ApiKey))
                options.ApiKey = Environment.GetEnvironmentVariable(
                    Constants.EnvPlatformApiKey);

            if (string.IsNullOrEmpty(options.ApiKey))
            {
                Console.Error.WriteLine(
                    "No platform API key provided. Either set {0} command line argument or the {1} environment variable",
                    Constants.ApiKeyArgument, Constants.EnvPlatformApiKey);
                ExitCode = ExitCode.InvalidArguments;
                return null;
            }

            return new Agrix(input, options.ApiKey, Assembly);
        }
    }
}
