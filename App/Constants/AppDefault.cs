namespace App.Constants;

/// <summary>
/// Default values and states for the application.
/// </summary>
public static class AppDefault
{
    /// <summary>
    /// Default interval used to reset the interval input (milliseconds).
    /// </summary>
    public const int InputInterval = 500;

    /// <summary>
    /// Default interval for run timer, in milliseconds (1000 ms = 1s).
    /// </summary>
    public const int ActiveTimerInterval = 1000;

    /// <summary>
    /// Default value for the run-until-count field.
    /// </summary>
    public const int RunCountInput = 100;

    /// <summary>
    /// Minimum time interval bounds (milliseconds).
    /// </summary>
    public const int IntervalMinimum = 100;

    /// <summary>
    /// Maximum time interval bounds (milliseconds).
    /// </summary>
    public const int IntervalMaximum = 600000;

    /// <summary>
    /// Increment between time-interval steps for input fields (milliseconds).
    /// </summary>
    public const int IntervalInputIncrement = 100;

    /// <summary>
    /// Minimum bounds for "run for count".
    /// </summary>
    public const int RunCountInputMinimum = 1;

    /// <summary>
    /// Maximum bounds for "run for count".
    /// </summary>
    public const int RunCountInputMaximum = 1000000;

    /// <summary>
    /// Initial counters and state.
    /// </summary>
    public const int InputCount = 0;
    public const int ActiveTimerSeconds = 0;
    public const bool IsRunning = false;
    public const int ForcedInputCount = 0;
    public const bool IsScheduled = false;

    /// <summary>
    /// Config placeholder default path.
    /// </summary>
    public static readonly string ConfigPathText = AppContext.BaseDirectory;

    /// <summary>
    /// Default hotkey (Start/Stop).
    /// </summary>
    public const Keys HotKey = Keys.F8;

    /// <summary>
    /// Default target key to be pressed (Left Mouse Button).
    /// </summary>
    public const Keys TargetKey = Keys.LButton;

    /// <summary>
    /// Button Labels
    /// </summary>
    public const string StartBtnLabel = "Start";
    public const string StopBtnLabel = "Stop";
    public const string ScheduleBtnLabel = "Scheduled";
    public const string ResetBtnLabel = "Reset";

    /// <summary>
    /// Defautl file filter for configuration files.
    /// </summary>
    public const string FileFormatFilter = "JSON Files (*.json)|*.json";

    /// <summary>
    /// Filename as a default configuration file.
    /// </summary>
    public const string DefaultConfigFileName = "AutoInput_Config.json";

    /// <summary>
    /// Default state for the sequence index selection.
    /// </summary>
    public const int SequenceIndex = 0;
}