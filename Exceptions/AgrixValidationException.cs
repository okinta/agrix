using System;

namespace agrix.Exceptions
{
    /// <summary>
    /// Thrown when a configuration validation fails.
    /// </summary>
    internal class AgrixValidationException : SystemException
    {
        /// <summary>
        /// Initializes a new instance with the specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public AgrixValidationException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance with the specified error message and a reference
        /// to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current
        /// exception.</param>
        public AgrixValidationException(string message, Exception innerException) :
            base(message, innerException)
        { }
    }
}
