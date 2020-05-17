using System.Collections.Generic;
using System.Linq;
using System;

namespace agrix.Extensions
{
    /// <summary>
    /// Extends the Enumerable class.
    /// </summary>
    internal static class EnumerableExtensions
    {
        /// <summary>
        /// Returns whether or not an element in a sequence satisfies a specified
        /// condition, and throws an exception if more than one such element exists.
        /// </summary>
        /// <typeparam name="T">The type of the elements of source.</typeparam>
        /// <param name="source">The IEnumerable to check.</param>
        /// <param name="predicate">A function to test an element for a condition.</param>
        /// <returns>True if exactly one element matches the specified condition;
        /// false otherwise.</returns>
        /// <exception cref="ArgumentNullException">source or predicate are
        /// null.</exception>
        public static bool Exists<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            try
            {
                var _ = source.Single(predicate);
                return true;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }
    }
}
