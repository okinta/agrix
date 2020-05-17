using System;

namespace agrix.Configuration
{
    /// <summary>
    /// Represents a plan configuration.
    /// </summary>
    internal readonly struct Plan
    {
        /// <summary>
        /// The number of CPUs for the plan.
        /// </summary>
        public int Cpu { get; }

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
                throw new ArgumentOutOfRangeException(
                    nameof(cpu), cpu, "must be larger than 0");

            if (memory <= 0)
                throw new ArgumentOutOfRangeException(
                    nameof(memory), memory, "must be larger than 0");

            if (string.IsNullOrEmpty(type))
                throw new ArgumentNullException(nameof(type), "must not be empty");

            Cpu = cpu;
            Memory = memory;
            Type = type;
        }

        /// <summary>
        /// Returns the string representation of this Plan.
        /// </summary>
        /// <returns>The string representation of this Plan.</returns>
        public override string ToString()
        {
            return $"Plan: {Cpu} CPU(s), {Memory}mb memory, {Type}";
        }
    }
}
