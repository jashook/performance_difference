using System;
using System.Diagnostics;

namespace perf_difference
{
    public ref struct TimeHelper
    {
        private Action<double> Callback;
        private Stopwatch Timer;

        public TimeHelper(Stopwatch timer, Action<double> callback)
        {
            Timer = timer;
            Callback = callback;
        }

        ////////////////////////////////////////////////////////////////////////
        // Memnber Methods
        ////////////////////////////////////////////////////////////////////////

        public void Dispose()
        {
            Timer.Stop();

            double elapsedMilliseconds = Timer.Elapsed.TotalMilliseconds;
            Callback(elapsedMilliseconds);
        }

        public double GetTimeElapsedInMilliseconds()
        {
            return Timer.ElapsedMilliseconds;
        }
    }

    class Program
    {
        private static int HighTierMethodHelper(uint index)
        {
            // Force the method to not be inlined.
            if (index % 1000000 == 0)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }

        private static void HighTierMethod(uint iterations)
        {
            for (uint index = 0; index < iterations; ++index)
            {
                HighTierMethodHelper(index);
            }
        }

        private static bool LowTierMethod(uint index, uint iterations)
        {
            if (index == iterations)
            {
                return true;
            }

            return LowTierMethod(index + 1, iterations);
        }

        static void Main(string[] args)
        {
            uint iterations = 32000;

            using (TimeHelper highTierTimer = new TimeHelper(Stopwatch.StartNew(), (elapsedTimeMs) => {
                Console.WriteLine($"[HighTierMethod]: Elapsed Time: {elapsedTimeMs}");
            }))
            {
                HighTierMethod(iterations);
            }

            using (TimeHelper lowTierTimer = new TimeHelper(Stopwatch.StartNew(), (elapsedTimeMs) => {
                Console.WriteLine($"[LowTierMethod]: Elapsed Time: {elapsedTimeMs}");
            }))
            {
                LowTierMethod(0, iterations);
            }
        }
    }
}
