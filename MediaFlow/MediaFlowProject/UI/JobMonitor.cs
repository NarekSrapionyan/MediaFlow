using MediaFlowProject.Jobs;
using MediaFlowProject.Models;

namespace MediaFlowProject.UI;

public class JobMonitor
{
    public void Run(JobManager jobManager)
    {
        Console.Clear();
            
        while (!Console.KeyAvailable)
        {
            Console.SetCursorPosition(0, 0);
                
            Console.WriteLine(new string('=', 60));
            Console.WriteLine("                         LIVE MONITOR                         ");
            Console.WriteLine(new string('=', 60));
            Console.WriteLine();

            var jobs = jobManager.GetJobs();

            foreach (var job in jobs)
            {
                string fileName = Path.GetFileName(job.InputPath);
                    
                Console.WriteLine($"#{job.Id}  {job.OperationType}".PadRight(Console.WindowWidth));
                Console.WriteLine($"   {fileName}".PadRight(Console.WindowWidth));
                    
                int barWidth = 20;
                int filled = (int)(job.Progress / 100.0 * barWidth);
                int empty = barWidth - filled;
                    
                string fillStr = new string('█', filled);
                string emptyStr = new string('-', empty);
                    
                Console.WriteLine($"   [{fillStr}{emptyStr}] {job.Progress}%      {job.Status}".PadRight(Console.WindowWidth));
                Console.WriteLine("".PadRight(Console.WindowWidth));
            }
                
            Thread.Sleep(500);
        }
            
        Console.ReadKey(true);
    }
}