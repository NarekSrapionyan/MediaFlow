namespace MediaFlowProject.Processes;

using System.Diagnostics;

public static class FFmpegChecker
{
    public static bool IsAvailable()
    {
        try
        {
            using var process = new Process();

            process.StartInfo.FileName = "ffmpeg";
            process.StartInfo.Arguments = "-version";

            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.CreateNoWindow = true;

            process.Start();
            process.WaitForExit();

            return process.ExitCode == 0;
        }
        catch (Exception)
        {
            return false;
        }
    }
}