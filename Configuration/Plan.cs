using System;

namespace agrix.Configuration
{
    /// <summary>
    /// Represents a plan configuration.
    /// </summary>
    internal struct Plan
    {
        /// <summary>
        /// The number of CPUs for the plan.
        /// </summary>
        public int CPU { get; }

        /// <summary>
        /// The amount of memory (MB) for the plan.
        /// </summary>
        public int Memory { get; }

        /// <summary>
        /// The type of plan.
        /// </summary>
        public string Type { get; }

        /// <summary>
        /// Creates a new plan configuration.
        /// </summary>
        /// <param name="cpu">The number of CPUs for the plan.</param>
        /// <param name="memory">The amount of memory (MB) for the plan.</param>
        /// <param name="type">The type of plan.</param>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="cpu"/> is 0
        /// or negative.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="memory"/> is
        /// 0 or negative.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="type"/> is
        /// empty.</exception>
        public Plan(int cpu, int memory, string type)
        {
            if (cpu <= 0)
            {
                throw new ArgumentOutOfRangeException("cpu", cpu, "must be larger than 0");
            }

            if (memory <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    "memory", memory, "must be larger than 0");
            }

            if (string.IsNullOrEmpty(type))
            {
                throw new ArgumentNullException("type", "must not be empty");
            }

            CPU = cpu;
            Memory = memory;
            Type = type;
        }
    }
}
