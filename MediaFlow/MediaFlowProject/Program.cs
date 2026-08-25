using MediaFlowProject.Configuration;
using MediaFlowProject.Jobs;
using MediaFlowProject.UI;

namespace MediaFlowProject;

class Program
{
    static void Main(string[] args)
    {
        Console.Clear();
        Console.WriteLine("Starting MediaFlow...");
        Console.WriteLine();

        if (!FFmpegChecker.IsAvailable())
        {
            Console.WriteLine("ERROR: FFmpeg was not found.");
            Console.WriteLine();
                
            Console.WriteLine("Please install FFmpeg and make sure it is available in PATH.");
            Console.WriteLine();
            Console.WriteLine("Press Enter to exit...");

            Console.ReadLine();
            return;
        }

        Console.WriteLine("FFmpeg found.");
        Console.WriteLine();

        var jobManager = new JobManager(AppSettings.MaxWorkerCount);
        jobManager.StartWorkers();

        var menu = new ConsoleMenu(jobManager);
        menu.Show();

        Console.Clear();
        Console.WriteLine("Shutting down workers safely. Please wait...");

        jobManager.StopWorkers();

        Console.WriteLine("Goodbye!");
    }
}