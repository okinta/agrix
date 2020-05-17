using System;
using System.Collections.Generic;

namespace tests
{
    /// <summary>
    /// Allows input to be streamed line by line calling the ReadLine method.
    /// </summary>
    internal class LineInputter
    {
        private Queue<string> Lines { get; }

        /// <summary>
        /// Creates a new instances to stream the given content.
        /// </summary>
        /// <param name="content">The content to stream line by line.</param>
        public LineInputter(string content)
        {
            if (!string.IsNullOrEmpty(content))
                Lines = new Queue<string>(
                    content.Split(
                        new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None));

            else
                Lines = new Queue<string>();
        }

        /// <summary>
        /// Gets a new line of input.
        /// </summary>
        /// <returns>The next line of input, or null if there is no more input
        /// left.</returns>
        public string ReadLine()
        {
            Lines.TryDequeue(out var line);
            return line;
        }
    }
}
