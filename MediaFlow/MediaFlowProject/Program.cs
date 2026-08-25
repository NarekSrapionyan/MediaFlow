using MediaFlowProject.Configuration;
using MediaFlowProject.Jobs;
using MediaFlowProject.UI;
using MediaFlowProject.Processes;

namespace MediaFlowProject;

class Program
{
    static void Main(string[] args)
    {
        Console.Clear();

        Console.WriteLine("=============================================");
        Console.WriteLine("              MEDIAFLOW STARTUP              ");
        Console.WriteLine("=============================================");
        Console.WriteLine();

        if (!FFmpegChecker.IsAvailable())
        {
            Console.WriteLine("FFmpeg is not installed or is not available.");
            Console.WriteLine("Please install FFmpeg and make sure it is in PATH.");
            return;
        }

        int workerCount = ReadWorkerCount();

        Console.WriteLine();
        Console.WriteLine($"Starting MediaFlow with {workerCount} worker(s)...");
        Console.WriteLine();

        var jobManager = new JobManager(workerCount);
        bool shuttingDown = false;
        object shutdownLock = new object();

        Console.CancelKeyPress += (sender, e) =>
        {
            // Don't let Ctrl+C immediately terminate the application.
            e.Cancel = true;

            lock (shutdownLock)
            {
                if (shuttingDown) return; // ignore repeated Ctrl+C
                shuttingDown = true;
            }

            Console.WriteLine("\nCtrl+C detected.");
            Console.WriteLine("Stopping all workers and FFmpeg processes...");
            
            var stopTask = Task.Run(() => jobManager.StopWorkers());

            if (!stopTask.Wait(TimeSpan.FromSeconds(5)))
            {
                Console.WriteLine("Workers did not stop in time. Forcing exit.");
            }
            else
            {
                Console.WriteLine("Goodbye!");
            }

            Environment.Exit(0);
        };

        jobManager.StartWorkers();

        var menu = new ConsoleMenu(jobManager);
        menu.Show();

        Console.Clear();
        Console.WriteLine("Shutting down workers safely. Please wait...");

        jobManager.StopWorkers();

        Console.WriteLine("Goodbye!");
    }

    private static int ReadWorkerCount()
    {
        while (true)
        {
            Console.Write($"Number of workers [{AppSettings.MinWorkerCount} - {AppSettings.MaxWorkerCount}]: ");

            string input = Console.ReadLine() ?? string.Empty;

            // Enter -> use default
            if (string.IsNullOrWhiteSpace(input))
            {
                return AppSettings.MaxWorkerCount;
            }

            // Not a number
            if (!int.TryParse(input, out int workerCount))
            {
                Console.WriteLine($"Please enter a number from {AppSettings.MinWorkerCount} to {AppSettings.MaxWorkerCount}.");
                continue;
            }

            // Outside allowed range
            if (workerCount < AppSettings.MinWorkerCount ||
                workerCount > AppSettings.MaxWorkerCount)
            {
                Console.WriteLine(
                    $"Please enter a number from {AppSettings.MinWorkerCount} to {AppSettings.MaxWorkerCount}."
                );

                continue;
            }

            return workerCount;
        }
    }
}