using System.Collections.Concurrent;
using MediaFlowProject.Configuration;
using MediaFlowProject.Models;
using MediaFlowProject.Workers;

namespace MediaFlowProject.Jobs;

public class JobManager
{
    private readonly BlockingCollection<MediaJob> _jobQueue;
    private readonly List<MediaJob> _alljobs;
    private readonly object _listLock = new object();

    private readonly int _workerCount;
    public int WorkerCount => _workerCount;
    private readonly List<Thread> _workers;

    private readonly CancellationTokenSource _tokenSource;
    
    public JobManager(int workerCount)
    {
        _workerCount = workerCount;

        _jobQueue = new BlockingCollection<MediaJob>();
        _alljobs = new List<MediaJob>();

        _workers = new List<Thread>();
        _tokenSource = new CancellationTokenSource();
    }

    public void AddJob(MediaJob job)
    {
        lock (_listLock)
        {
            _alljobs.Add(job);
        }

        if (!_jobQueue.IsAddingCompleted)
        {
            _jobQueue.Add(job);
        }
    } 
    public List<MediaJob> GetJobs()
    {
        lock (_listLock)
        {
            return _alljobs.ToList(); 
        }
    }

    public void StartWorkers()
    {
        for (int i = 0; i < _workerCount; i++)
        {
            var thread = new Thread(WorkerLoop)
            {
                Name = $"WorkerThread-{i + 1}",
                IsBackground = true
            };
            _workers.Add(thread);
            thread.Start();
        }
    }

    public void StopWorkers()
    {
        _tokenSource.Cancel();
        _jobQueue.CompleteAdding();
        foreach (var worker in _workers)
        {
            if (worker.IsAlive)
            {
                worker.Join();
            }
        }
    }

    private void WorkerLoop()
    {
        var worker = new JobWorker(_jobQueue, _tokenSource.Token);
        worker.Run();
    }
    
    
}
    
    
