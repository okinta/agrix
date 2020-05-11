using System;

namespace agrix.Configuration
{
    internal struct OperatingSystem
    {
        public string App { get; }
        public string ISO { get; }
        public string Name { get; }

        public OperatingSystem(string app = "", string iso = "", string name = "")
        {
            if (string.IsNullOrEmpty(app)
                && string.IsNullOrEmpty(iso)
                && string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Either app, iso or name must be set");
            }

            App = app;
            ISO = iso;
            Name = name;
        }
    }
}
