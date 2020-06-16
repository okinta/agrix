using CommandLine;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        private Assembly Assembly { get; }
        private ExitCode ExitCode { get; set; } = ExitCode.Success;
        private StdInput.ReadLine ReadLine { get; }

        private Program(Assembly assembly, StdInput.ReadLine readLine)
        {
            Assembly = assembly;
            ReadLine = readLine;
        }

        /// <summary>
        /// Entrypoint for the program. Processes CLI arguments and runs the application.
        /// </summary>
        /// <param name="args">The CLI arguments provided to the program.</param>
        /// <returns>The application status code.</returns>
        public static int Main(string[] args)
        {
            return Main(null, Console.ReadLine, args);
        }

        /// <summary>
        /// Test entrypoint for the program. Processes CLI arguments and runs the
        /// application.
        /// </summary>
        /// <param name="assembly">The Assembly to use for loading platforms.</param>
        /// <param name="readLine">The function to use to read user input.</param>
        /// <param name="args">The CLI arguments provided to the program.</param>
        /// <returns>The application status code.</returns>
        public static int Main(
            Assembly assembly, StdInput.ReadLine readLine, params string[] args)
        {
            var program = new Program(assembly, readLine);
            Parser.Default.ParseArguments<
                    DestroyOptions,
                    ProvisionOptions,
                    ValidateOptions
                >(args)
                .WithParsed<ProvisionOptions>(program.Provision)
                .WithParsed<DestroyOptions>(program.Destroy)
                .WithParsed<ValidateOptions>(program.Validate)
                .WithNotParsed(program.HandleParseError);
            return (int) program.ExitCode;
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
            catch (ArgumentException e)
            {
                Console.Error.WriteLine(e.Message);
                ExitCode = ExitCode.Exception;
            }
        }

        private void Destroy(DestroyOptions options)
        {
            try
            {
                LoadAgrix(options)?.Destroy(options.Dryrun);
            }
            catch (WebException e)
            {
                Console.Error.WriteLine(e.Message);
                ExitCode = ExitCode.Exception;
            }
            catch (ArgumentException e)
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

        private void HandleParseError(IEnumerable<Error> errors)
        {
            var errorsArray = errors as Error[] ?? errors.ToArray();
            if (errorsArray.Length == 1
                && (errorsArray.First().Tag == ErrorType.HelpRequestedError
                    || errorsArray.First().Tag == ErrorType.HelpVerbRequestedError))
                return;

            ExitCode = ExitCode.InvalidArguments;
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
                input = StdInput.Read(ReadLine);

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

            return new Agrix(input, new AgrixSettings(
                options.ApiKey, options.ApiUrl, Assembly));
        }
    }
}
