using MediaFlowProject.Configuration;
using MediaFlowProject.Jobs;
using MediaFlowProject.UI;

namespace MediaFlowProject;

class Program
{
    static void Main(string[] args)
    {
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