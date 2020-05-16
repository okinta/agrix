using System.Collections.Generic;
using System;

namespace agrix.Program
{
    /// <summary>
    /// Provides an interface to read from stdin.
    /// </summary>
    internal class StdInput
    {
        /// <summary>
        /// Function to read user input.
        /// </summary>
        /// <returns>A line of user input.</returns>
        public delegate string ReadLine();

        /// <summary>
        /// Reads content from stdin.
        /// </summary>
        /// <param name="readLine">The delegate to use to read user input.</param>
        /// <returns>The content read from stdin.</returns>
        public static string Read(ReadLine readLine)
        {
            var stdin = new List<string>();
            string input;
            do
            {
                input = readLine();
                stdin.Add(input);
            }
            while (input != null);

            return string.Join(Environment.NewLine, stdin);
        }
    }
}
