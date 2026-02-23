using System.Diagnostics.CodeAnalysis;

namespace App.Models;

/// <summary>
/// Configuration of the application.
/// Used for persistence and status tracking.
/// </summary>
[ExcludeFromCodeCoverage]
public class AppConfig
{
    /// <summary>
    /// Interval in milliseconds between inputs.
    /// </summary>
    public int IntervalMilliseconds { get; set; }

    /// <summary>
    /// Flag to indicate if app runs until stopped.
    /// </summary>
    public bool RunUntilStopActive { get; set; }

    /// <summary>
    /// Flag to indicate if app runs until set count. 
    /// </summary>
    public bool RunUntilSetCountActive { get; set; }

    /// <summary>
    /// Input Count until App stops the inputs.
    /// </summary>
    public int StopInputCount { get; set; }

    /// <summary>
    /// Start/Stop Toggle App keybind.
    /// </summary>
    public string StartStopKeybind { get; set; } = string.Empty;

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
    /// Time of scheduled end.
    /// </summary>
    public DateTime ScheduleStopTime { get; set; }

    /// <summary>
    /// Default folder path for configuration files.
    /// </summary>
    public string ConfigFolderPath { get; set; } = string.Empty;

    /// <summary>
    /// Currently seleted sequence from the list.
    /// Defaults to zero as a fallback.
    /// </summary>
    public int SelectedSequenceIndex { get; set; } = 0;

    /// <summary>
    /// Collection of user defined input sequences.
    /// </summary>
    public List<Sequence> Sequences { get; set; } = [];

    /// <summary>
    /// Indicates wether the app is running the sequence or
    /// the single target key.
    /// </summary>
    public bool SequenceModeActive { get; set; }

    /// <summary>
    /// Flag that indicates if the hold key is enabled.
    /// </summary>
    public bool HoldTargetEnabled { get; set; }
}