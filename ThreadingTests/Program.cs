using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Profiler.Windows.Api;


namespace ThreadingTests
{
    class Program
    {
        static void Main(string[] args)
        {
            Task task = TestAllocations();
            task.Wait();
        }

        private static async Task TestAllocations()
        {
            const int AllocationsPerSecond = 1000;
            const int MinutesToRun = 10;

            long allocated = 0;

            // Variable declarations outside of block scope in case there are compiler optimizations
            // that will dispose these variables when leaving block scope.
            CancellationTokenSource tokenSource;
            SemaphoreSlim semaphore;

            DateTime endTime = DateTime.Now.AddMinutes(MinutesToRun);

            if (MemoryProfiler.IsActive)
            {
                if (MemoryProfiler.CanControlAllocations)
                    MemoryProfiler.EnableAllocations();

                MemoryProfiler.Dump();
            }

            while (true)
            {
                tokenSource = new CancellationTokenSource();
                semaphore = new SemaphoreSlim(1);

                await semaphore.WaitAsync(tokenSource.Token);
                semaphore.Release();

                // Delay before creating next sempahore.
                await Task.Delay(1000 / AllocationsPerSecond, tokenSource.Token);

                // Do not Dispose CancellationTokenSource or Semaphore.  Let this leak and see how many
                // sempaphores are required to make this crash.

                allocated++;

                if (endTime < DateTime.Now)
                    break;
            }

            if (MemoryProfiler.IsActive)
                MemoryProfiler.Dump();

            Console.WriteLine("Allocated {0} number of Semaphores and CancellationTokenSources", allocated);
            Console.WriteLine("Excuted for {0} minutes", MinutesToRun);
            Console.ReadLine();
        }
    }
}
