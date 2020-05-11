using System;

namespace agrix.Configuration
{
    internal struct Plan
    {
        public int CPU { get; }
        public int Memory { get; }
        public string Type { get; }

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
