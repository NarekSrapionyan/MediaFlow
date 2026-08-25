using MediaFlowProject.Models;
namespace MediaFlowProject.Processing;

public interface IMediaProcess
{
    string GetFfmpegArguments(MediaJob job);
}