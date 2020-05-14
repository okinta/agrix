namespace agrix.Configuration
{
    /// <summary>
    /// Represents a stored script.
    /// </summary>
    internal struct Script
    {
        /// <summary>
        /// The name of the script.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The type of script.
        /// </summary>
        public string Type { get; }

        /// <summary>
        /// The contents of the script.
        /// </summary>
        public string Content { get; }
    }
}
