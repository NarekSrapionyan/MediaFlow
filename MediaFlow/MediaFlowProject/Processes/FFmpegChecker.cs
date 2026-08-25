namespace MediaFlowProject.Processes;

using System.Diagnostics;

public static class FFmpegChecker
{
    public static string ResolvedPath { get; private set; } = "ffmpeg";

    public static bool IsAvailable()
    {
        if (Check("ffmpeg"))
        {
            ResolvedPath = "ffmpeg";
            return true;
        }

        string localPath = Path.Combine(
            AppContext.BaseDirectory,
            "ffmpeg",
            OperatingSystem.IsWindows() ? "ffmpeg.exe" : "ffmpeg");

        if (Check(localPath))
        {
            ResolvedPath = localPath;
            return true;
        }

        return false;
    }
    
    private static bool Check(string fileName)
    {
        try
        {
            using var process = new Process();

            process.StartInfo.FileName = fileName;
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