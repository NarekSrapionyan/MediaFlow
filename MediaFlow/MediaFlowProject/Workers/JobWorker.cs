using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq.Expressions;
using MediaFlowProject.Models;
using MediaFlowProject.Processes;
using MediaFlowProject.Processing;

namespace MediaFlowProject.Workers;

public class JobWorker
{
    private readonly BlockingCollection<MediaJob> _jobQueue;
    private readonly CancellationToken _cancellationToken;

    public JobWorker(BlockingCollection<MediaJob> jobQueue, CancellationToken cancellationToken)
    {
        _jobQueue = jobQueue;
        _cancellationToken = cancellationToken;
    }

    public void Run()
    {
        try
        {
            foreach (var job in _jobQueue.GetConsumingEnumerable(_cancellationToken))
            {
                ProcessJob(job);
            }
        }
        catch (OperationCanceledException)
        {
        }
    }
    
    private void ProcessJob(MediaJob job)
    {
        if (job.Status == JobStatus.Canceled)
        {
            return;
        }

        try
        {
            job.UpdateStatus(JobStatus.Running);

            var processor = MediaProcessorResolver.Resolve(job.OperationType);
            string arguments = processor.GetFfmpegArguments(job);
                
            var runner = new FfmpegProcessRunner();
            runner.Run(job, arguments, _cancellationToken);
        }
        catch (Exception ex)
        {
            job.UpdateStatus(JobStatus.Failed, ex.Message);
        }
    }
    
}