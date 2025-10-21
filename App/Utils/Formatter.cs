namespace App.Utils;

public static class Formatter
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
}