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

            var processType = MediaProcessorResolver.Resolve(job.OperationType);
            string arguments = processType.GetFfmpegArguments(job);

            // link the global shutdown token with this job's own cancel token
            using var linked = CancellationTokenSource.CreateLinkedTokenSource(
                _cancellationToken, job.CancellationToken);

            var runner = new FfmpegProcessRunner();
            runner.Run(job, arguments, linked.Token);
        }
        catch (Exception ex)
        {
            job.UpdateStatus(JobStatus.Failed, ex.Message);
        }
    }
    
}