using MediaFlowProject.Models;

namespace MediaFlowProject.Processing;

public class MediaProcessorResolver
{
    public static IMediaProcessor Resolve(MediaOperationType operationType)
    {
        switch (operationType)
        {
            case MediaOperationType.Convert:
                return new MediaConverter();
                
            case MediaOperationType.ExtractAudio:
                return new AudioExtractor();
                
            case MediaOperationType.Compress:
                return new MediaCompressor();
                
            default:
                throw new NotSupportedException($"The operation type {operationType} is not supported.");
        }
    }
}