using System.Diagnostics;
using MediaFlowProject.Configuration;
using MediaFlowProject.Models;

namespace MediaFlowProject.Processes;

public class FfmpegProcessRunner
{
    public void Run(MediaJob job, string arguments, CancellationToken cancellationToken)
    {
        var parser = new FfmpegProgressParser();

        var processInfo = new ProcessStartInfo
        {
            FileName = AppSettings.FFmpegPath,
            Arguments = arguments,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using (var process = new Process { StartInfo = processInfo })
        {
            process.ErrorDataReceived += (sender, e) =>
            {
                if (e.Data != null)
                {
                    int progress = parser.ParseProgress(e.Data);
                    if (progress != -1)
                    {
                        job.UpdateProgress(progress);
                    }
                }
            };

            process.Start();
            process.BeginErrorReadLine();

            while (!process.WaitForExit(500))
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    if (!process.HasExited)
                    {
                        process.Kill(entireProcessTree: true); // ffmpeg can spawn helpers; kill them too
                    }
                    job.UpdateStatus(JobStatus.Canceled); // harmless now, status already Canceled
                    return;
                }
            }

            if (process.ExitCode == 0)
            {
                job.UpdateProgress(100);
                job.UpdateStatus(JobStatus.Completed);
            }
            else
            {
                job.UpdateStatus(JobStatus.Failed, $"FFmpeg finished with error: {process.ExitCode}");
            }
        }
    }
}