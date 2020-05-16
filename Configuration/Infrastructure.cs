using System;
using System.Collections.Generic;

namespace agrix.Configuration
{
    internal class Infrastructure
    {
        private List<Type> types = new List<Type>();

        public IReadOnlyList<Type> Types
        {
            get { return types; }
        }

        private Dictionary<Type, IList<object>> Items { get; } =
            new Dictionary<Type, IList<object>>();

        public void AddItems<T>(IEnumerable<T> items)
        {
            if (!types.Contains(typeof(T)))
                types.Add(typeof(T));

            if (!Items.TryGetValue(typeof(T), out var existingItems))
                existingItems = new List<object>();

            foreach (var item in items)
                existingItems.Add(item);
        }

        public IList<T> GetItems<T>()
        {
            var items = new List<T>();

            foreach (var item in Items[typeof(T)])
                items.Add((T)item);

            return items;
        }
    }
}
