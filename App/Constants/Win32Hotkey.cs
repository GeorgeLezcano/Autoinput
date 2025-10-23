namespace App.Constants;

/// <summary>
/// Win32 hotkey constants used by global start/stop registration.
/// </summary>
internal static class Win32Hotkey
{
    /// <summary>
    /// Window message for a registered hotkey.
    /// </summary>
    public const int WM_HOTKEY = 0x0312;

    /// <summary>
    /// App-unique hotkey identifier (arbitrary but stable).
    /// </summary>
    public const int HOTKEY_ID = 0x0A11;

    /// <summary>
    /// No modifier keys (Shift/Ctrl/Alt/Win).
    /// </summary>
    public const uint MOD_NONE = 0x0000;
}

