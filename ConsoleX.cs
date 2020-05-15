using System;

namespace agrix
{
    /// <summary>
    /// Describes methods to output to stdout.
    /// </summary>
    internal static class ConsoleX
    {
        /// <summary>
        /// Writes a variable to stdout if <paramref name="value"/> is not null. Text is
        /// outputted in the form of "[name]: [value]".
        /// </summary>
        /// <param name="name">The name of the variable to output.</param>
        /// <param name="value">The value of the variable to ouput.</param>
        public static void WriteLine(string name, object value)
        {
            if (value != null)
                Console.WriteLine("{0}: {1}", name, value);
        }

        /// <summary>
        /// Writes a variable to stdout if <paramref name="value"/> is not null. Text is
        /// outputted in the form of "[name]: [value]".
        /// </summary>
        /// <param name="name">The name of the variable to output.</param>
        /// <param name="value">The value of the variable to ouput.</param>
        public static void WriteLine(string name, string value)
        {
            if (!string.IsNullOrEmpty(value))
                Console.WriteLine("{0}: {1}", name, value);
        }

        /// <summary>
        /// Writes a variable to stdout if <paramref name="value"/> is not null. Text is
        /// outputted in the form of "[name]: [value]".
        /// </summary>
        /// <param name="name">The name of the variable to output.</param>
        /// <param name="value">The value of the variable to ouput.</param>
        public static void WriteLine(string name, bool? value)
        {
            if (value != null)
            {
                var outputValue = value == true ? "yes" : "no";
                Console.WriteLine("{0}: {1}", name, outputValue);
            }
        }
    }
}
