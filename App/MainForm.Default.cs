namespace App;

/// <summary>
/// Default values and states for the application.
/// </summary>
partial class MainForm
{
    /// <summary>
    /// Default interval used to reset the interval input (milliseconds).
    /// </summary>
    private const int inputIntervalDefault = 250;

    /// <summary>
    /// Default interval for run timer, in milliseconds (1000 ms = 1s).
    /// </summary>
    private const int activeTimerIntervalDefault = 1000;

    /// <summary>
    /// Default value for the run-until-count field.
    /// </summary>
    private const int runCountInputDefault = 100;

    /// <summary>
    /// Minimum time interval bounds (milliseconds).
    /// </summary>
    private const int intervalMinimum = 100;

    /// <summary>
    /// Maximum time interval bounds (milliseconds).
    /// </summary>
    private const int intervalMaximum = 600000;

    /// <summary>
    /// Increment between time-interval steps for input fields (milliseconds).
    /// </summary>
    private const int intervalInputIncrement = 1;

    /// <summary>
    /// Minimum bounds for "run for count".
    /// </summary>
    private const int runCountInputMinimum = 1;

    /// <summary>
    /// Maximum bounds for "run for count".
    /// </summary>
    private const int runCountInputMaximum = 1000000;

    /// <summary>
    /// Maximum value for the sequence interval input (milliseconds).
    /// </summary>
    private const int sequenceIntervalInputMaximum = 600000;

    /// <summary>
    /// Initial counters and state.
    /// </summary>
    private const int inputCountDefault = 0;
    private const int activeTimerSecondsDefault = 0;
    private const bool isRunningDefault = false;
    private const int forcedInputCountDefault = 0;
    private const bool isScheduledDefault = false;

    /// <summary>
    /// Config placeholder defaults.
    /// </summary>
    private const string configPathTextDefault = "<not set>";

    /// <summary>
    /// Default hotkey (Start/Stop).
    /// </summary>
    private const Keys hotKeyDefault = Keys.F8;

    /// <summary>
    /// Default target key to be pressed (Left Mouse Button).
    /// </summary>
    private const Keys targetKeyDefault = Keys.LButton;

    /// <summary>
    /// Button Labels
    /// </summary>
    private const string startBtnLabel = "Start";
    private const string stopBtnLabel = "Stop";
    private const string scheduleBtnLabel = "Scheduled";

}