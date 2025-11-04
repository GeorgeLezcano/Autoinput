using System.Diagnostics.CodeAnalysis;

namespace App.Constants;

/// <summary>
/// Centralized UI colors used across the General tab and shell.
/// </summary>
[ExcludeFromCodeCoverage]
internal static class UiColors
{
    /// <summary>
    /// Form background (main app background).
    /// </summary>
    public static readonly Color FormBack = Color.FromArgb(24, 26, 33); // #181A21 

    /// <summary>
    /// Primary foreground text.
    /// </summary>
    public static readonly Color FormFore = Color.FromArgb(232, 234, 237); // #E8EAED 

    /// <summary>
    /// Panel or group background.
    /// </summary>
    public static readonly Color PanelBack = Color.FromArgb(33, 36, 46); // #21242E

    /// <summary>
    /// Secondary text (labels/hints).
    /// </summary>
    public static readonly Color TextSecondary = Color.FromArgb(160, 165, 180); // #A0A5B4

    /// <summary>
    /// Neutral border for controls.
    /// </summary>
    public static readonly Color Border = Color.FromArgb(52, 56, 68); // #343844 

    /// <summary>
    /// Default button background (neutral).
    /// </summary>
    public static readonly Color ButtonBackDefault = Color.FromArgb(47, 51, 65); // #2F3341

    /// <summary>
    /// Start button (running=false).
    /// </summary>
    public static readonly Color StartGreen = Color.FromArgb(0, 200, 160); // #00C8A0

    /// <summary>
    /// Stop button (running=true).
    /// </summary>
    public static readonly Color StopRed = Color.FromArgb(230, 80, 90); // #E6505A

    /// <summary>
    /// Highlight during binding mode.
    /// </summary>
    public static readonly Color BindingBack = Color.FromArgb(255, 196, 79); // #FFC44F

    /// <summary>
    /// Text color during binding mode.
    /// </summary>
    public static readonly Color BindingFore = Color.FromArgb(25, 27, 35); // #191B23

    /// <summary>
    /// Start/Stop button background while "Scheduled".
    /// </summary>
    public static readonly Color ScheduledOrange = Color.FromArgb(255, 140, 60); // #FF8C3C

    /// <summary>
    /// Hover color for "Scheduled".
    /// </summary>
    public static readonly Color ScheduledOrangeHover = Color.FromArgb(240, 130, 55); // #F08237

    /// <summary>
    /// Mouse down color for "Scheduled".
    /// </summary>
    public static readonly Color ScheduledOrangeDown = Color.FromArgb(225, 115, 50); // #E17332
}
