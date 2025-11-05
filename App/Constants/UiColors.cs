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

        /// <summary>
    /// Primary text alias used for generic labels and content.
    /// </summary>
    public static readonly Color Text = FormFore; // #F8F9FC

    /// <summary>
    /// Muted text for secondary content.
    /// </summary>
    public static readonly Color TextMuted = Color.FromArgb(182, 189, 202); // #B6BDCA

    /// <summary>
    /// Disabled or inactive text color.
    /// </summary>
    public static readonly Color TextDisabled = Color.FromArgb(122, 131, 149); // #7A8395

    /// <summary>
    /// Default button hover background.
    /// </summary>
    public static readonly Color ButtonBackHover = Color.FromArgb(64, 70, 88); // #404658

    /// <summary>
    /// Default button pressed (mouse down) background.
    /// </summary>
    public static readonly Color ButtonBackDown = Color.FromArgb(58, 64, 80); // #3A4050

    /// <summary>
    /// Default button border color.
    /// </summary>
    public static readonly Color ButtonBorder = Border; // #3A404E

    /// <summary>
    /// Default input field background (TextBox, NumericUpDown, etc.).
    /// </summary>
    public static readonly Color InputBack = Color.FromArgb(48, 52, 66); // #303442

    /// <summary>
    /// Default input text color.
    /// </summary>
    public static readonly Color InputFore = FormFore; // #F8F9FC

    /// <summary>
    /// Placeholder text color for input fields.
    /// </summary>
    public static readonly Color InputPlaceholder = Color.FromArgb(182, 189, 202); // #B6BDCA

    /// <summary>
    /// Input border highlight color when focused.
    /// </summary>
    public static readonly Color InputBorderFocus = Color.FromArgb(86, 145, 255); // #5691FF

    /// <summary>
    /// Input background color when disabled.
    /// </summary>
    public static readonly Color InputDisabledBack = Color.FromArgb(42, 46, 58); // #2A2E3A

    /// <summary>
    /// Input text color when disabled.
    /// </summary>
    public static readonly Color InputDisabledFore = Color.FromArgb(122, 131, 149); // #7A8395

    /// <summary>
    /// Active tab background color.
    /// </summary>
    public static readonly Color TabActiveBack = Color.FromArgb(46, 50, 64); // #2E3240

    /// <summary>
    /// Inactive tab background color.
    /// </summary>
    public static readonly Color TabInactiveBack = PanelBack; // #242834

    /// <summary>
    /// Active tab text color.
    /// </summary>
    public static readonly Color TabActiveFore = FormFore; // #F8F9FC

    /// <summary>
    /// Inactive tab text color.
    /// </summary>
    public static readonly Color TabInactiveFore = TextSecondary; // #D2D7E1

    /// <summary>
    /// Tab border color.
    /// </summary>
    public static readonly Color TabBorder = Border; // #3A404E

    /// <summary>
    /// DataGridView background color.
    /// </summary>
    public static readonly Color GridBack = Color.FromArgb(48, 52, 66); // #303442

    /// <summary>
    /// DataGridView header background color.
    /// </summary>
    public static readonly Color GridHeaderBack = Color.FromArgb(46, 51, 67); // #2E3343

    /// <summary>
    /// DataGridView header text color.
    /// </summary>
    public static readonly Color GridHeaderFore = Color.FromArgb(230, 234, 242); // #E6EAF2

    /// <summary>
    /// DataGridView default row background color.
    /// </summary>
    public static readonly Color GridRowBack = Color.FromArgb(42, 46, 58); // #2A2E3A

    /// <summary>
    /// DataGridView alternating row background color.
    /// </summary>
    public static readonly Color GridAltRowBack = Color.FromArgb(38, 42, 54); // #262A36

    /// <summary>
    /// DataGridView selected row background color.
    /// </summary>
    public static readonly Color GridSelectionBack = Color.FromArgb(62, 90, 128); // #3E5A80

    /// <summary>
    /// DataGridView selected row text color.
    /// </summary>
    public static readonly Color GridSelectionFore = FormFore; // #F8F9FC

    /// <summary>
    /// DataGridView grid line color.
    /// </summary>
    public static readonly Color GridLines = Border; // #3A404E

    /// <summary>
    /// Hyperlink color.
    /// </summary>
    public static readonly Color Link = Color.FromArgb(77, 163, 255); // #4DA3FF

    /// <summary>
    /// Hyperlink hover color.
    /// </summary>
    public static readonly Color LinkHover = Color.FromArgb(140, 190, 255); // #8CBEFF

}