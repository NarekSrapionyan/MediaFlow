using System.Text.RegularExpressions;

namespace MediaFlowProject.Processes;

public class FfmpegProgressParser
{
    private TimeSpan _totalDuration =  TimeSpan.Zero;

    public int ParseProgress(string consoleLine)
    {
        if (string.IsNullOrWhiteSpace(consoleLine))
        {
            return -1;
        }

        if (_totalDuration == TimeSpan.Zero)
        {
            var durationMatch = Regex.Match(consoleLine, @"Duration: (\d{2}:\d{2}:\d{2}\.\d{2,3})");
            if (durationMatch.Success)
            {
                TimeSpan.TryParse(durationMatch.Groups[1].Value, out _totalDuration);
            }
        }

        if (_totalDuration != TimeSpan.Zero)
        {
            var timeMatch = Regex.Match(consoleLine, @"time=(\d{2}:\d{2}:\d{2}\.\d{2,3})");
            if (timeMatch.Success)
            {
                if (TimeSpan.TryParse(timeMatch.Groups[1].Value, out TimeSpan currentTime))
                {
                    int progress = (int)((currentTime.TotalSeconds / _totalDuration.TotalSeconds) * 100);
                    return progress;
                }
            }
        }
        return -1;
    }
}
