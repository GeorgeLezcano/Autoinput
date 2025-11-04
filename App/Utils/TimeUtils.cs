namespace App.Utils;

/// <summary>
/// Utility helpers for converting between milliseconds and seconds.
/// </summary>
public static class TimeUtils
{
    /// <summary>
    /// Converts milliseconds to seconds (1 decimal precision typical for UI).
    /// </summary>
    public static decimal ToSeconds(int milliseconds)
        => milliseconds / 1000M;

    /// <summary>
    /// Converts seconds back to milliseconds (rounded to nearest int).
    /// </summary>
    public static int ToMilliseconds(decimal seconds)
        => (int)Math.Round(seconds * 1000M);

    /// <summary>
    /// Clamps a seconds value within given millisecond-based bounds.
    /// </summary>
    public static decimal ClampSeconds(decimal seconds, int minMs, int maxMs)
    {
        decimal minSec = ToSeconds(minMs);
        decimal maxSec = ToSeconds(maxMs);
        return Math.Min(Math.Max(seconds, minSec), maxSec);
    }
}
