namespace App.Models;

/// <summary>
/// Configuration of the application.
/// Used for persistence and status tracking.
/// </summary>
public class AppConfig
{
    /// <summary>
    /// Interval in milliseconds beteen inputs.
    /// </summary>
    public int IntervalMilliseconds { get; set; }

    /// <summary>
    /// Flag to indicate if app runs until stopped or
    /// a set count is provided.
    /// </summary>
    public bool RunToStopActive { get; set; }

    /// <summary>
    /// Input Count until App stops the inputs.
    /// </summary>
    public int StopInputCount { get; set; }

    /// <summary>
    /// Start/Stop Toggle App keybind.
    /// </summary>
    public string StartStopKeybind { get; set; }  = string.Empty;

    /// <summary>
    /// Target Key to be used in the autoinput.
    /// </summary>
    public string TargetInputKey { get; set; } = string.Empty;

    /// <summary>
    /// Flag to indicate the start run time is set.
    /// </summary>
    public bool ScheduleStartEnabled { get; set; }

    /// <summary>
    /// Time of schedule start.
    /// </summary>
    public DateTime ScheduleStartTime { get; set; }

    /// <summary>
    /// Flag to indicate the stop run time is set.
    /// </summary>
    public bool ScheduleStopEnabled { get; set; }

    /// <summary>
    /// Time of schedule end.
    /// </summary>
    public DateTime ScheduleStopTime { get; set; }

    /// <summary>
    /// Folder path set for configuration files.
    /// </summary>
    public string ConfigFolderPath { get; set; } = string.Empty;
}