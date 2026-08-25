namespace MediaFlowProject.Models;

public class MediaJob
{
    private readonly object _lock = new object();
    private readonly CancellationTokenSource _cts = new CancellationTokenSource();

    private static int _idCounter = 0;

    private JobStatus _status;
    private int _progress;
    private string _errorMessage;

    public int Id { get; }
    public string InputPath { get; }
    public string OutputPath { get; }
    public string Options { get; }
    public MediaOperationType OperationType { get; }

    public CancellationToken CancellationToken => _cts.Token;

    public MediaJob(string inputPath, string outputPath, string options, MediaOperationType operationType)
    {
        Id = Interlocked.Increment(ref _idCounter);
        InputPath = inputPath;
        OutputPath = outputPath;
        Options = options ?? string.Empty;
        OperationType = operationType;

        _status = JobStatus.Queued;
        _progress = 0;
        _errorMessage = string.Empty;
    }

    public JobStatus Status { get { lock (_lock) return _status; } }
    public int Progress { get { lock (_lock) return _progress; } }
    public string ErrorMessage { get { lock (_lock) return _errorMessage; } }

    public void UpdateProgress(int newProgress)
    {
        lock (_lock) { _progress = Math.Clamp(newProgress, 0, 100); }
    }

    public void UpdateStatus(JobStatus newStatus, string errorMessage = "")
    {
        lock (_lock)
        {
            _status = newStatus;
            if (!string.IsNullOrEmpty(errorMessage)) _errorMessage = errorMessage;
        }
    }

    // NEW: this is the only correct way to cancel a job.
    public void Cancel()
    {
        lock (_lock)
        {
            if (_status == JobStatus.Completed || _status == JobStatus.Failed || _status == JobStatus.Canceled)
                return;
            _status = JobStatus.Canceled;
        }
        _cts.Cancel();
    }
}