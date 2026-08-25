using MediaFlowProject.Models;

namespace MediaFlowProject.Processing;

public class MediaProcessorResolver
{
    public static IMediaProcess Resolve(MediaOperationType operationType)
    {
        switch (operationType)
        {
            case MediaOperationType.Convert:
                return new MediaConvert();
                
            case MediaOperationType.ExtractAudio:
                return new AudioExtract();
                
            case MediaOperationType.Compress:
                return new MediaCompress();
                
            default:
                throw new NotSupportedException($"The operation type {operationType} is not supported.");
        }
    }
}