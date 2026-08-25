using MediaFlowProject.Configuration;
using MediaFlowProject.Jobs;
using MediaFlowProject.UI;

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

        int workerCount = ReadWorkerCount();

        Console.WriteLine();
        Console.WriteLine($"Starting MediaFlow with {workerCount} worker(s)...");
        Console.WriteLine();

        var jobManager = new JobManager(workerCount);

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
            Console.Write("Number of workers [5]: ");

            string input = Console.ReadLine() ?? string.Empty;

            // Enter → use default
            if (string.IsNullOrWhiteSpace(input))
            {
                return 5;
            }

            // Not a number
            if (!int.TryParse(input, out int workerCount))
            {
                Console.WriteLine("Please enter a number from 1 to 5.");
                continue;
            }

            // Outside allowed range
            if (workerCount < 1 || workerCount > 5)
            {
                Console.WriteLine("Please enter a number from 1 to 5.");
                continue;
            }

            return workerCount;
        }
    }
}