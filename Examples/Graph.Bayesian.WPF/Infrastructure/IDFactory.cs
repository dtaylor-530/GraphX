using System;

namespace Graph.Bayesian.WPF.Infrastructure
{
    class IDFactory
    {
        static Random random { get; } = new();

        public static long Get => random.Next(0, int.MaxValue);
    }
}
