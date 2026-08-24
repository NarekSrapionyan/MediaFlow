using MediaFlowProject.Models;
namespace MediaFlowProject.Processing;

public interface IMediaProcessor
{
    string GetFfmpegArguments(MediaJob job);
}