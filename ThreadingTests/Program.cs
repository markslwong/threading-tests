using System.Threading;
using System.Threading.Tasks;

namespace ThreadingTests
{
    class Program
    {
        static async void Main(string[] args)
        {
            const int AllocationsPerSecond = 50;
            const int MinutesToRun = 5;

            int seconds = 0;
            long allocated = 0;
            
            // Variable declarations outside of block scope in case there are compiler optimizations
            // that will dispose these variables when leaving block scope.
            CancellationTokenSource tokenSource;
            SemaphoreSlim semaphore;
            
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

                seconds
            }
        }
    }
}
