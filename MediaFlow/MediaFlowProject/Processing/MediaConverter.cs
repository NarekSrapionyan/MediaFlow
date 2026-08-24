using MediaFlowProject.Models;

namespace MediaFlowProject.Processing;

public class MediaConverter : IMediaProcessor
{
    public string GetFfmpegArguments(MediaJob job)
    {
        return $"-y -i \"{job.InputPath}\" {job.Options} \"{job.OutputPath}\"";    
    } 
}