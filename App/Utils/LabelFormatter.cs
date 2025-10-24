namespace App.Utils;

internal static class LabelFormatter
{
    /// <summary>
    /// Build the time elapsed label text.
    /// </summary>
    /// <param name="seconds">The total time in seconds.</param>
    public static string SetTimeLabel(int seconds)
    {
        int hours = seconds / 3600;
        int minutes = seconds % 3600 / 60;
        int secs = seconds % 60;

        return $"Active Time: {hours:D2}:{minutes:D2}:{secs:D2}";
    }

    /// <summary>
    /// Builds the input count label text.
    /// </summary>
    /// <param name="count">The key press count.</param>
    public static string SetInputCountLabel(int count)
    {
        return $"Input Count: {count}";
    }

    /// <summary>
    /// Builds the label for the time interval hint.
    /// </summary>
    /// <param name="min">Minimun interval value</param>
    /// <param name="max">Maximum interval value</param>
    public static string SetIntervalHint(int min, int max)
    {
        return $"Range: {min} – {max} ms \n1 second = 1000 milliseconds";
    }

    /// <summary>
    /// Sets the version label in the information tab.
    /// </summary>
    public static string SetVersionLabel()
    {
        return $"App Version: {XmlHelpers.GetAppVersion()}";
    }

    /// <summary>
    /// Sets the shell label.
    /// </summary>
    public static string SetAppShellText()
    {
        return $"AutoInput_v{XmlHelpers.GetAppVersion()}";
    }

}
