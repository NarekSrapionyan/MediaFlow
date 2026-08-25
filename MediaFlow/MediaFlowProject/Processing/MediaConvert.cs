using MediaFlowProject.Models;

namespace MediaFlowProject.Processing;

public class MediaConvert : IMediaProcess
{
    public string GetFfmpegArguments(MediaJob job)
    {
        return $"-y -i \"{job.InputPath}\" {job.Options} \"{job.OutputPath}\"";    
    } 
}