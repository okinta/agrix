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
        /// Reads content from stdin.
        /// </summary>
        /// <returns>The content read from stdin.</returns>
        public static string Read()
        {
            var stdin = new List<string>();
            string input;
            do
            {
                input = Console.ReadLine();
                stdin.Add(input);
            }
            while (input != null);

            return string.Join(Environment.NewLine, stdin);
        }
    }
}
