using System.Diagnostics.CodeAnalysis;

namespace App.Constants;

/// <summary>
/// Centralized UI colors used across the General tab and shell.
/// </summary>
[ExcludeFromCodeCoverage]
internal static class UiColors
{
    /// <summary>
    /// Form background.
    /// </summary>
    public static readonly Color FormBack = Color.FromArgb(30, 32, 46);

    /// <summary>
    /// Primary foreground text.
    /// </summary>
    public static readonly Color FormFore = Color.FromArgb(235, 238, 245);

    /// <summary>
    /// Panel/group background.
    /// </summary>
    public static readonly Color PanelBack = Color.FromArgb(40, 44, 60);

    /// <summary>
    /// Secondary text (labels/hints).
    /// </summary>
    public static readonly Color TextSecondary = Color.FromArgb(200, 204, 214);

    /// <summary>
    /// Neutral border for controls.
    /// </summary>
    public static readonly Color Border = Color.FromArgb(70, 75, 95);

    /// <summary>
    /// Default button background (neutral).
    /// </summary>
    public static readonly Color ButtonBackDefault = Color.FromArgb(60, 64, 82);

    /// <summary>
    /// Start button (running=false).
    /// </summary>
    public static readonly Color StartGreen = Color.Green;

    /// <summary>
    /// Stop button (running=true).
    /// </summary>
    public static readonly Color StopRed = Color.Red;

    /// <summary>
    /// Highlight during binding mode.
    /// </summary>
    public static readonly Color BindingBack = Color.Goldenrod;

    /// <summary>
    /// Text color during binding mode.
    /// </summary>
    public static readonly Color BindingFore = Color.Black;

    /// <summary>
    /// Start/Stop button background while "Scheduled".
    /// </summary>
    public static readonly Color ScheduledOrange = Color.FromArgb(255, 159, 67);

    /// <summary>
    /// Hover color for "Scheduled".
    /// </summary>
    public static readonly Color ScheduledOrangeHover = Color.FromArgb(230, 144, 60);

    /// <summary>
    /// Mouse down color for "Scheduled".
    /// </summary>
    public static readonly Color ScheduledOrangeDown = Color.FromArgb(210, 132, 55);
}
