using System.Runtime.InteropServices;

namespace App.Utils;

/// <summary>
/// Minimal SendInput wrapper for keyboard and mouse.
/// </summary>
public static class NativeInput
{
    private const string User32 = "user32.dll";

    [StructLayout(LayoutKind.Sequential)]
    private struct INPUT
    {
        public uint type; // 1 = Keyboard, 0 = Mouse
        public InputUnion U;
    }

    [StructLayout(LayoutKind.Explicit)]
    private struct InputUnion
    {
        [FieldOffset(0)] public MOUSEINPUT mi;
        [FieldOffset(0)] public KEYBDINPUT ki;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct MOUSEINPUT
    {
        public int dx, dy;
        public uint mouseData, dwFlags, time;
        public IntPtr dwExtraInfo;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct KEYBDINPUT
    {
        public ushort wVk, wScan;
        public uint dwFlags, time;
        public IntPtr dwExtraInfo;
    }

    [DllImport(User32, SetLastError = true)]
    private static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

    [DllImport(User32)]
    private static extern uint MapVirtualKey(uint uCode, uint uMapType);

    [DllImport(User32)]
    private static extern IntPtr GetMessageExtraInfo();

    private const uint INPUT_MOUSE = 0;
    private const uint INPUT_KEYBOARD = 1;
    private const uint KEYEVENTF_EXTENDEDKEY = 0x0001;
    private const uint KEYEVENTF_KEYUP = 0x0002;
    private const uint KEYEVENTF_SCANCODE = 0x0008;

    private const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
    private const uint MOUSEEVENTF_LEFTUP = 0x0004;
    private const uint MOUSEEVENTF_RIGHTDOWN = 0x0008;
    private const uint MOUSEEVENTF_RIGHTUP = 0x0010;
    private const uint MOUSEEVENTF_MIDDLEDOWN = 0x0020;
    private const uint MOUSEEVENTF_MIDDLEUP = 0x0040;

    public static bool IsMouseKey(Keys key) =>
        key == Keys.LButton || key == Keys.RButton || key == Keys.MButton;

    /// <summary>
    /// Sends a full key press (down + up) for the given key.
    /// </summary>
    public static void SendKeyPress(Keys key)
    {
        if (key == Keys.None) return;

        if (TryBuildKeyboardInput(key, keyUp: false, out var down) &&
            TryBuildKeyboardInput(key, keyUp: true, out var up))
        {
            var inputs = new[] { down, up };
            _ = SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(INPUT)));
            return;
        }

        ushort vk = (ushort)((uint)key & 0xFFFF);
        var extra = GetMessageExtraInfo();

        var vkInputs = new INPUT[2];
        vkInputs[0] = new INPUT
        {
            type = INPUT_KEYBOARD,
            U = new InputUnion { ki = new KEYBDINPUT { wVk = vk, dwExtraInfo = extra } }
        };
        vkInputs[1] = new INPUT
        {
            type = INPUT_KEYBOARD,
            U = new InputUnion { ki = new KEYBDINPUT { wVk = vk, dwFlags = KEYEVENTF_KEYUP, dwExtraInfo = extra } }
        };

        _ = SendInput((uint)vkInputs.Length, vkInputs, Marshal.SizeOf(typeof(INPUT)));
    }

    /// <summary>
    /// Sends a full mouse click (down + up) for the given mouse button.
    /// </summary>
    public static void ClickMouseButton(Keys mouseKey)
    {
        if (mouseKey == Keys.None) return;

        uint down, up;
        if (mouseKey == Keys.LButton) { down = MOUSEEVENTF_LEFTDOWN; up = MOUSEEVENTF_LEFTUP; }
        else if (mouseKey == Keys.RButton) { down = MOUSEEVENTF_RIGHTDOWN; up = MOUSEEVENTF_RIGHTUP; }
        else if (mouseKey == Keys.MButton) { down = MOUSEEVENTF_MIDDLEDOWN; up = MOUSEEVENTF_MIDDLEUP; }
        else return;

        var extra = GetMessageExtraInfo();

        var inputs = new INPUT[2];
        inputs[0] = new INPUT { type = INPUT_MOUSE, U = new InputUnion { mi = new MOUSEINPUT { dwFlags = down, dwExtraInfo = extra } } };
        inputs[1] = new INPUT { type = INPUT_MOUSE, U = new InputUnion { mi = new MOUSEINPUT { dwFlags = up, dwExtraInfo = extra } } };
        _ = SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(INPUT)));
    }

    /// <summary>
    /// Sends a key down (press and hold) for the given key.
    /// </summary>
    public static void KeyDown(Keys key)
    {
        if (key == Keys.None) return;

        if (TryBuildKeyboardInput(key, keyUp: false, out var input))
        {
            _ = SendInput(1, [input], Marshal.SizeOf(typeof(INPUT)));
            return;
        }

        ushort vk = (ushort)((uint)key & 0xFFFF);
        var extra = GetMessageExtraInfo();

        var vkInput = new INPUT
        {
            type = INPUT_KEYBOARD,
            U = new InputUnion { ki = new KEYBDINPUT { wVk = vk, dwFlags = 0, dwExtraInfo = extra } }
        };
        _ = SendInput(1, [vkInput], Marshal.SizeOf(typeof(INPUT)));
    }

    /// <summary>
    /// Sends a key up (release) for the given key.
    /// </summary>
    public static void KeyUp(Keys key)
    {
        if (key == Keys.None) return;

        if (TryBuildKeyboardInput(key, keyUp: true, out var input))
        {
            _ = SendInput(1, [input], Marshal.SizeOf(typeof(INPUT)));
            return;
        }

        ushort vk = (ushort)((uint)key & 0xFFFF);
        var extra = GetMessageExtraInfo();

        var vkInput = new INPUT
        {
            type = INPUT_KEYBOARD,
            U = new InputUnion { ki = new KEYBDINPUT { wVk = vk, dwFlags = KEYEVENTF_KEYUP, dwExtraInfo = extra } }
        };
        _ = SendInput(1, [vkInput], Marshal.SizeOf(typeof(INPUT)));
    }

    /// <summary>
    /// Sends a mouse button down (press and hold) for the given mouse button.
    /// </summary>
    public static void MouseDown(Keys mouseKey)
    {
        uint down =
            mouseKey == Keys.LButton ? MOUSEEVENTF_LEFTDOWN :
            mouseKey == Keys.RButton ? MOUSEEVENTF_RIGHTDOWN :
            mouseKey == Keys.MButton ? MOUSEEVENTF_MIDDLEDOWN :
            0;

        if (down == 0) return;

        var extra = GetMessageExtraInfo();

        var inputs = new INPUT[1];
        inputs[0] = new INPUT
        {
            type = INPUT_MOUSE,
            U = new InputUnion { mi = new MOUSEINPUT { dwFlags = down, dwExtraInfo = extra } }
        };

        _ = SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(INPUT)));
    }

    /// <summary>
    /// Sends a mouse button up (release) for the given mouse button.
    /// </summary>
    public static void MouseUp(Keys mouseKey)
    {
        uint up =
            mouseKey == Keys.LButton ? MOUSEEVENTF_LEFTUP :
            mouseKey == Keys.RButton ? MOUSEEVENTF_RIGHTUP :
            mouseKey == Keys.MButton ? MOUSEEVENTF_MIDDLEUP :
            0;

        if (up == 0) return;

        var extra = GetMessageExtraInfo();

        var inputs = new INPUT[1];
        inputs[0] = new INPUT
        {
            type = INPUT_MOUSE,
            U = new InputUnion { mi = new MOUSEINPUT { dwFlags = up, dwExtraInfo = extra } }
        };

        _ = SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(INPUT)));
    }

    /// <summary>
    /// Attempts to build the keyboard input. Uses scan codes; returns false to fall back to VK.
    /// </summary>
    private static bool TryBuildKeyboardInput(Keys key, bool keyUp, out INPUT input)
    {
        input = default;

        uint vk = (uint)key & 0xFFFF;

        ushort scan = (ushort)MapVirtualKey(vk, 0);
        if (scan == 0)
            scan = (ushort)MapVirtualKey(vk, 3);

        if (scan == 0)
            return false;

        uint flags = KEYEVENTF_SCANCODE;
        if (keyUp) flags |= KEYEVENTF_KEYUP;
        if (IsExtendedKey((Keys)vk)) flags |= KEYEVENTF_EXTENDEDKEY;

        input = new INPUT
        {
            type = INPUT_KEYBOARD,
            U = new InputUnion
            {
                ki = new KEYBDINPUT
                {
                    wVk = 0,
                    wScan = scan,
                    dwFlags = flags,
                    time = 0,
                    dwExtraInfo = GetMessageExtraInfo()
                }
            }
        };

        return true;
    }

    /// <summary>
    /// Extended keys need KEYEVENTF_EXTENDEDKEY when using scan codes.
    /// </summary>
    private static bool IsExtendedKey(Keys key) => key switch
    {
        Keys.Insert or Keys.Delete or Keys.Home or Keys.End or Keys.PageUp or Keys.PageDown => true,
        Keys.Up or Keys.Down or Keys.Left or Keys.Right => true,
        Keys.RControlKey or Keys.RMenu => true, // right ctrl/alt
        Keys.NumLock or Keys.Cancel or Keys.PrintScreen or Keys.Divide => true,
        _ => false
    };
}