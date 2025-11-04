namespace App.Models;

/// <summary>
/// Represents a step within a sequence of keys/mouse inputs.
/// </summary>
public sealed class SequenceStep
{
    /// <summary>
    /// The key to be executed.
    /// </summary>
    public Keys Key { get; set; }

    /// <summary>
    /// Delay in milliseconds after the key is pressed.
    /// </summary>
    public int DelayMS { get; set; }
}