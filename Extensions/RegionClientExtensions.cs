using System.Collections.Generic;
using System.Linq;
using System;
using Vultr.API.Clients;
using Vultr.API.Models;

namespace agrix.Extensions
{
    /// <summary>
    /// Extends the RegionClient class.
    /// </summary>
    internal static class RegionClientExtensions
    {
        /// <summary>
        /// Gets the ID of the given region.
        /// </summary>
        /// <param name="client">The RegionClient instance to use to retrieve the region
        /// ID.</param>
        /// <param name="name">The name of the region to retrieve the ID for.</param>
        /// <returns>The ID of the given region.</returns>
        /// <exception cref="ArgumentException">If the region cannot be found.</exception>
        public static int GetRegionId(this RegionClient client, string name)
        {
            var regions = client.GetRegions();

            KeyValuePair<int, Region> region;
            try
            {
                region = regions.Regions.Single(
                    r => r.Value.name == name);
            }
            catch (InvalidOperationException e)
            {
                throw new ArgumentException(
                    $"Cannot find region called {name}", nameof(name), e);
            }

            return region.Key;
        }
    }
}
