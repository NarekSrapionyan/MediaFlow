using MediaFlowProject.Models;

namespace MediaFlowProject.Processing;

public class AudioExtract : IMediaProcess
{
    public string GetFfmpegArguments(MediaJob job)
    {
        return $"-y -i \"{job.InputPath}\" -vn {job.Options} \"{job.OutputPath}\"";
    }
}