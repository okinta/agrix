using CommandLine;
using System.IO;
using System;

namespace agrix
{
    /// <summary>
    /// Represents a program exit code.
    /// </summary>
    internal enum ExitCode
    {
        Success = 0,
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
        /// <param name="opt">The CLI arguments to run the program with.</param>
        private void Run(Options opt)
        {
            string input;
            if (string.IsNullOrEmpty(opt.Filename))
            {
                input = StdInput.Read();
            }
            else
            {
                try
                {
                    input = File.ReadAllText(opt.Filename);
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

            new Agrix(input).Process();
        }
    }
}
