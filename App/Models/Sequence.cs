namespace App.Models;

/// <summary>
/// Represents a set of steps (inputs + delay)
/// in a sequencial way to perform a sequence loop.
/// </summary>
public sealed class Sequence
{
    /// <summary>
    /// Display Name of the sequence.
    /// Defaults to "New Sequence".
    /// </summary>
    public string Name { get; set; } = "New Sequence";

    /// <summary>
    /// Collection of sequence steps.
    /// Defaults to empty list.
    /// </summary>
    public List<SequenceStep> Steps = [];

}