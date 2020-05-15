using System;
using Vultr.API.Models;

namespace agrix.Platforms.Vultr
{
    /// <summary>
    /// Extends the Vultr.API.Models.Server class.
    /// </summary>
    internal static class ServerExtensions
    {
        /// <summary>
        /// Returns whether or not the two given servers are equivalent.
        /// </summary>
        /// <param name="server">The Server to compare <paramref name="other"/>
        /// with.</param>
        /// <param name="other">The Server to compare <paramref name="server"/>
        /// with.</param>
        /// <returns>True if the Server instances are equivalent; false
        /// otherwise.</returns>
        public static bool IsEquivalent(this Server server, Server other)
        {
            if (server is null)
                throw new ArgumentNullException("server", "server must not be null");

            if (other is null)
                throw new ArgumentNullException("other", "other must not be null");

            foreach (var property in server.GetType().GetProperties())
            {
                var serverValue = property.GetValue(server);
                if (serverValue is null) continue;
                if (serverValue.ToString() == "0") continue;

                var otherValue = property.GetValue(other);
                if (serverValue == otherValue) continue;
                if (serverValue.Equals(otherValue)) continue;

                if (serverValue.GetType() == typeof(double)
                    && otherValue?.GetType() == typeof(double)
                    && Math.Abs((double)serverValue - (double)otherValue) < 0.1)
                        continue;

                Console.WriteLine("Servers do not match. {0} is different.",
                    property.Name);
                return false;
            }

            return true;
        }
    }
}
