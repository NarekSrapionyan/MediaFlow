using MediaFlowProject.Configuration;
using MediaFlowProject.Jobs;
using MediaFlowProject.Models;

namespace MediaFlowProject.UI;

public class ConsoleMenu
    {
        private readonly JobManager _jobManager;
        private readonly JobMonitor _jobMonitor;

        public ConsoleMenu(JobManager jobManager)
        {
            _jobManager = jobManager;
            _jobMonitor = new JobMonitor();
        }

        public void Show()
        {
            bool isRunning = true;
            while (isRunning)
            {
                Console.Clear();
                var jobs = _jobManager.GetJobs();
                
                int active = jobs.Count(j => j.Status == JobStatus.Running);
                int queued = jobs.Count(j => j.Status == JobStatus.Queued);
                int done = jobs.Count(j => j.Status == JobStatus.Completed || j.Status == JobStatus.Failed || j.Status == JobStatus.Canceled);

                Console.WriteLine(new string('=', 45));
                Console.WriteLine("                  MEDIAFLOW                  ");
                Console.WriteLine(new string('=', 45));
                Console.WriteLine();
                
                Console.WriteLine(
                    $"Workers: {_jobManager.WorkerCount}"
                );
                Console.WriteLine($"Active:  {active}");
                Console.WriteLine($"Queued:  {queued}");
                Console.WriteLine($"Done:    {done}");
                Console.WriteLine();

                Console.WriteLine("[1] Add Job");
                Console.WriteLine("[2] List Jobs");
                Console.WriteLine("[3] Live Monitor");
                Console.WriteLine("[4] Cancel Job");
                Console.WriteLine("[5] Cancel All");
                Console.WriteLine("[6] Wait For All");
                Console.WriteLine("[7] Help");
                Console.WriteLine("[0] Exit");
                Console.WriteLine();
                Console.Write("Select > ");

                string choice = Console.ReadLine() ?? string.Empty;

                switch (choice)
                {
                    case "1": AddJobFlow(); break;
                    case "2": ListJobsFlow(); break;
                    case "3": _jobMonitor.Run(_jobManager); break;
                    case "4": CancelJobFlow(); break;
                    case "5": CancelAllFlow(); break;
                    case "6": WaitForAllFlow(); break;
                    case "7": new HelpScreen().Show(); break;
                    case "0": isRunning = false; break;
                    default: break;
                }
            }
        }

        private void AddJobFlow()
        {
            Console.Clear();

            string input = ReadExistingFilePath("Input file path: ");

            string output = ReadOutputFilePath("Output file path: ");

            MediaOperationType type = ReadOperationType();

            var job = new MediaJob(input, output, "", type);

            _jobManager.AddJob(job);

            Console.WriteLine("Job added successfully.");
            Console.WriteLine("\nPress any key to return...");
            Console.ReadKey(true);
        }

        private void ListJobsFlow()
        {
            while (true)
            {
                Console.Clear();               // <-- clear every frame, not just once
                Console.SetCursorPosition(0, 0);

                Console.WriteLine("=== All Jobs ===");
                Console.WriteLine();

                var jobs = _jobManager.GetJobs();

                if (!jobs.Any())
                {
                    Console.WriteLine("No jobs in the queue.");
                }
                else
                {
                    foreach (var job in jobs)
                    {
                        Console.WriteLine($"[{job.Id}] {job.OperationType,-12} | {job.Status,-10}");
                        Console.WriteLine($"    {CreateProgressBar(job.Progress)}");
                        Console.WriteLine();
                    }
                }

                Console.WriteLine("Press Enter to return...");

                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true);
                    if (key.Key == ConsoleKey.Enter)
                    {
                        break;
                    }
                }

                Thread.Sleep(200);
            }
        }

        private void CancelJobFlow()
        {
            Console.Clear();

            int id = ReadInteger("Enter Job ID to cancel: ");

            var job = _jobManager.GetJobs().FirstOrDefault(j => j.Id == id);

            if (job != null)
            {
                if (job.Status == JobStatus.Queued || job.Status == JobStatus.Running)
                {
                    job.UpdateStatus(JobStatus.Canceled);
                    Console.WriteLine($"Job [{id}] has been canceled.");
                }
                else
                {
                    Console.WriteLine($"Cannot cancel job [{id}]. Current status: {job.Status}");
                }
            }
            else
            {
                Console.WriteLine($"Job [{id}] not found.");
            }

            Console.WriteLine("\nPress any key to return...");
            Console.ReadKey(true);
        }

        private void CancelAllFlow()
        {
            Console.Clear();
            var jobs = _jobManager.GetJobs().Where(j => j.Status == JobStatus.Queued || j.Status == JobStatus.Running).ToList();
            
            foreach (var job in jobs)
            {
                job.UpdateStatus(JobStatus.Canceled);
            }
            
            Console.WriteLine($"Canceled {jobs.Count} active/queued jobs.");
            Console.WriteLine("\nPress any key to return...");
            Console.ReadKey(true);
        }

        private void WaitForAllFlow()
        {
            Console.Clear();
            Console.WriteLine("Waiting for all jobs to finish...");
            
            while (true)
            {
                var jobs = _jobManager.GetJobs();
                bool hasActive = jobs.Any(j => j.Status == JobStatus.Queued || j.Status == JobStatus.Running);
                
                if (!hasActive)
                {
                    break;
                }
                
                Thread.Sleep(1000);
            }
            
            Console.WriteLine("All jobs have been processed!");
            Console.WriteLine("\nPress any key to return...");
            Console.ReadKey(true);
        }
        
        
        // helper function for real time progress bar
        
        private string CreateProgressBar(int progress, int width = 20)
        {
            progress = Math.Clamp(progress, 0, 100);

            int filled = progress * width / 100;

            return "[" +
                   new string('█', filled) +
                   new string('░', width - filled) +
                   $"] {progress,3}%";
        }
        
        
        // helper functions for inputs
        private string ReadExistingFilePath(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string path = (Console.ReadLine() ?? string.Empty).Trim();

                if (string.IsNullOrWhiteSpace(path))
                {
                    continue;
                }

                if (File.Exists(path))
                {
                    return path;
                }

                Console.WriteLine("File not found. Please enter a valid file path.");
            }
        }

        private string ReadOutputFilePath(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string path = (Console.ReadLine() ?? string.Empty).Trim();

                if (string.IsNullOrWhiteSpace(path))
                {
                    continue;
                }

                try
                {
                    string? directory = Path.GetDirectoryName(Path.GetFullPath(path));

                    if (!string.IsNullOrEmpty(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    return path;
                }
                catch
                {
                    Console.WriteLine("Invalid output path. Please enter a valid path.");
                }
            }
        }

        private int ReadInteger(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);

                string input = (Console.ReadLine() ?? string.Empty).Trim();

                if (int.TryParse(input, out int number))
                {
                    return number;
                }

                Console.WriteLine("Please enter a number.");
            }
        }

        private MediaOperationType ReadOperationType()
        {
            while (true)
            {
                Console.WriteLine("Select operation:");
                Console.WriteLine("[1] Convert");
                Console.WriteLine("[2] Extract Audio");
                Console.WriteLine("[3] Compress");
                Console.Write("Select > ");

                string choice = (Console.ReadLine() ?? string.Empty).Trim();

                switch (choice)
                {
                    case "1":
                        return MediaOperationType.Convert;

                    case "2":
                        return MediaOperationType.ExtractAudio;

                    case "3":
                        return MediaOperationType.Compress;

                    default:
                        Console.WriteLine("Please select 1, 2, or 3.");
                        break;
                }
            }
        }
    }