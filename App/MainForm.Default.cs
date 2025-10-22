namespace App;

/// <summary>
/// Default values and states for the application.
/// </summary>
public partial class MainForm
{
    /// <summary>
    /// Default interval used to reset the interval input (milliseconds).
    /// </summary>
    private const int intervalDefault = 1000;

    /// <summary>
    /// Default value for the run until count field.
    /// </summary>
    private const int runCountInputDefault = 100;

    /// <summary>
    /// Minimum Time Interval (milliseconds).
    /// </summary>
    private const int intervalMinimum = 100;

    /// <summary>
    /// Maximum Time Interval (milliseconds).
    /// </summary>
    private const int intervalMaximum = 10000;

    /// <summary>
    /// Increment between time intervals for input fields (milliseconds).
    /// </summary>
    private const int intervalInputIncrement = 100;

    /// <summary>
    /// Minimum count allowed when run until count field is set.
    /// </summary>
    private const int runCountInputMinimum = 1;

    /// <summary>
    /// Maximum count allowed when run until count field is set.
    /// </summary>
    private const int runCountInputMaximum = 1000000;

    /// <summary>
    /// Maximum value for the sequence input field.
    /// </summary>
    private const int sequenceIntervalInputMaximum = 600000;

    /// <summary>
    /// Default value for input count initialization and reset.
    /// </summary>
    private const int inputCountDefault = 0;

    /// <summary>
    /// Default value for active time initialization and reset.
    /// </summary>
    private const int activeTimerSecondsDefault = 0;

    /// <summary>
    /// Default value for application running state.
    /// </summary>
    private const bool isRunningDefault = false;

    /// <summary>
    /// Default value for force input count initialization and reset.
    /// </summary>
    private const int forcedInputCountDefault = 0;

}