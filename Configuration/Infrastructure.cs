using System.Collections.Generic;
using System.Linq;
using System;

namespace agrix.Configuration
{
    /// <summary>
    /// Describes infrastructure configuration.
    /// </summary>
    internal class Infrastructure
    {
        /// <summary>
        /// Gets the list of Type instances that exist within this configuration.
        /// </summary>
        public IReadOnlyList<Type> Types => _types;

        private readonly List<Type> _types = new List<Type>();
        private Dictionary<Type, IList<object>> Items { get; } =
            new Dictionary<Type, IList<object>>();

        /// <summary>
        /// Adds a collection of items to this configuration.
        /// </summary>
        /// <typeparam name="T">The type of items to add.</typeparam>
        /// <param name="items">The list of items to add.</param>
        public void AddItems<T>(IEnumerable<T> items)
        {
            var itemsArray = items as T[] ?? items.ToArray();
            if (!_types.Contains(typeof(T)) && itemsArray.Any())
                _types.Add(typeof(T));

            if (!Items.TryGetValue(typeof(T), out var existingItems))
            {
                existingItems = new List<object>();
                Items[typeof(T)] = existingItems;
            }

            foreach (var item in itemsArray)
                existingItems.Add(item);
        }

        /// <summary>
        /// Gets the items defined in this configuration.
        /// </summary>
        /// <param name="type">The type of items to retrieve.</param>
        /// <returns>The list of items in this configuration.</returns>
        public IReadOnlyList<object> GetItems(Type type)
        {
            return (IReadOnlyList<object>)Items[type];
        }
    }
}
