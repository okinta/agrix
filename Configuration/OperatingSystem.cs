using System;

namespace agrix.Configuration
{
    /// <summary>
    /// Represents an operating system configuration.
    /// </summary>
    internal readonly struct OperatingSystem
    {
        /// <summary>
        /// The application name.
        /// </summary>
        public string App { get; }

        /// <summary>
        /// The operating system's ISO name.
        /// </summary>

        public string Iso { get; }

        /// <summary>
        /// The name of the operating system.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Creates a new operating system configuration.
        /// </summary>
        /// <param name="app">The application name.</param>
        /// <param name="iso">The ISO name.</param>
        /// <param name="name">The name of the operating system.</param>
        /// <exception cref="ArgumentException">If <paramref name="app"/>,
        /// <paramref name="iso"/> and <paramref name="name"/> are all empty. At least
        /// one parameter must be set.</exception>
        public OperatingSystem(string app = "", string iso = "", string name = "")
        {
            if (string.IsNullOrEmpty(app)
                && string.IsNullOrEmpty(iso)
                && string.IsNullOrEmpty(name))
                throw new ArgumentException("Either app, iso or name must be set");

            App = app;
            Iso = iso;
            Name = name;
        }
    }
}
