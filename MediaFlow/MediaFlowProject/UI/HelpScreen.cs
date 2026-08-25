namespace MediaFlowProject.UI;

public class HelpScreen
{
    public void Show()
    {
        Console.Clear();
        Console.WriteLine(new string('=', 60));
        Console.WriteLine("                        HELP / USAGE                        ");
        Console.WriteLine(new string('=', 60));
        Console.WriteLine();
            
        Console.WriteLine("MediaFlow is a multi-threaded media processing tool.");
        Console.WriteLine();
        Console.WriteLine("[1] Add Job      : Add a new media file to the queue.");
        Console.WriteLine("                   Requires input path, output path,");
        Console.WriteLine("                   and operation type.");
        Console.WriteLine("[2] List Jobs    : View all jobs in a simple list.");
        Console.WriteLine("[3] Live Monitor : Watch the real-time progress of jobs.");
        Console.WriteLine("                   Press any key to exit the monitor.");
        Console.WriteLine("[4] Cancel Job   : Stop a specific job by its ID.");
        Console.WriteLine("[5] Cancel All   : Stop all currently running jobs.");
        Console.WriteLine("[6] Wait For All : Block the menu until all jobs finish.");
        Console.WriteLine("[0] Exit         : Close the application safely.");
        Console.WriteLine();
        Console.WriteLine("Note: FFmpeg must be installed and accessible.");
        Console.WriteLine();
            
        Console.WriteLine("Press any key to return to the main menu...");
        Console.ReadKey(true);
    }
}