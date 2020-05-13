using System.Collections.Generic;
using System;

namespace agrix.Exceptions
{
    /// <summary>
    /// The exception that is thrown when the key specified for accessing an element
    /// in a collection does not match any key in the collection. Adds ability to
    /// retrieve the specific key which was not found.
    /// </summary>
    /// <typeparam name="T">The type of key.</typeparam>
    internal class KnownKeyNotFoundException<T> : KeyNotFoundException
    {
        /// <summary>
        /// The key which was not found.
        /// </summary>
        public T Key { get; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="key">The key which was not found.</param>
        public KnownKeyNotFoundException(T key) : base()
        {
            Key = key;
        }

        /// <summary>
        /// Instantiates a new instance with the specified error message.
        /// </summary>
        /// <param name="key">The key which was not found.</param>
        /// <param name="message">The message that describes the error.</param>
        public KnownKeyNotFoundException(T key, string message) : base(message)
        {
            Key = key;
        }

        /// <summary>
        /// Instantiates a new instance with the specified error message and a reference
        /// to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="key">The key which was not found.</param>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException"></param>
        public KnownKeyNotFoundException(
            T key, string message, Exception innerException) :
            base(message, innerException)
        {
            Key = key;
        }

        /// <summary>
        /// Converts a KeyNotFoundException to a KnownKeyNotFoundException instance.
        /// </summary>
        /// <param name="key">The key which was not found.</param>
        /// <param name="e">The KeyNotFoundException instance to convert.</param>
        public KnownKeyNotFoundException(T key, KeyNotFoundException e) :
            base(e.Message, e)
        {
            Key = key;
        }
    }
}
