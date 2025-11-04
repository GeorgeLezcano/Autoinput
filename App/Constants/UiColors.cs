using System.Diagnostics.CodeAnalysis;

namespace App.Constants;

/// <summary>
/// Centralized UI colors used across the General tab and shell (Graphite Pro+ theme).
/// </summary>
[ExcludeFromCodeCoverage]
internal static class UiColors
{
    /// <summary>
    /// Form background (global).
    /// </summary>
    public static readonly Color FormBack = Color.FromArgb(20, 22, 28); // #14161C 

    /// <summary>
    /// Primary foreground text.
    /// </summary>
    public static readonly Color FormFore = Color.FromArgb(248, 249, 252); // #F8F9FC 

    /// <summary>
    /// Panel/group background.
    /// </summary>
    public static readonly Color PanelBack = Color.FromArgb(36, 40, 52); // #242834

    /// <summary>
    /// Secondary text (labels/hints).
    /// </summary>
    public static readonly Color TextSecondary = Color.FromArgb(210, 215, 225); // #D2D7E1 

    /// <summary>
    /// Neutral border for controls (a bit more visible).
    /// </summary>
    public static readonly Color Border = Color.FromArgb(58, 64, 78); // #3A404E 

    /// <summary>
    /// Default button background.
    /// </summary>
    public static readonly Color ButtonBackDefault = Color.FromArgb(54, 60, 74); // #363C4A 

    /// <summary>
    /// Start button (running=false).
    /// </summary>
    public static readonly Color StartGreen = Color.FromArgb(55, 205, 130); // #37CD82 

    /// <summary>
    /// Stop button (running=true).
    /// </summary>
    public static readonly Color StopRed = Color.FromArgb(235, 87, 87); // #EB5757 

    /// <summary>
    /// Highlight during binding mode.
    /// </summary>
    public static readonly Color BindingBack = Color.FromArgb(250, 200, 65); // #FAC841 

    /// <summary>
    /// Text color during binding mode.
    /// </summary>
    public static readonly Color BindingFore = Color.FromArgb(18, 20, 26); // #12141A

    /// <summary>
    /// Start/Stop button background while "Scheduled".
    /// </summary>
    public static readonly Color ScheduledOrange = Color.FromArgb(245, 155, 68); // #F59B44 

    /// <summary>
    /// Hover color for "Scheduled".
    /// </summary>
    public static readonly Color ScheduledOrangeHover = Color.FromArgb(232, 140, 60); // #E88C3C

    /// <summary>
    /// Mouse down color for "Scheduled".
    /// </summary>
    public static readonly Color ScheduledOrangeDown = Color.FromArgb(210, 125, 52); // #D27D34
}