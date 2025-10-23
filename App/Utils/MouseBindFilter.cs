namespace App.Utils;

/// <summary>
/// IMessageFilter used only while binding target mouse clicks.
/// </summary>
public sealed class MouseBindFilter(Action<MouseButtons> onClick) : IMessageFilter
{
    private readonly Action<MouseButtons> _onClick = onClick;
    private const int WM_LBUTTONDOWN = 0x0201;
    private const int WM_RBUTTONDOWN = 0x0204;
    private const int WM_MBUTTONDOWN = 0x0207;

    public bool PreFilterMessage(ref Message m)
    {
        switch (m.Msg)
        {
            case WM_LBUTTONDOWN: _onClick(MouseButtons.Left); return true;
            case WM_RBUTTONDOWN: _onClick(MouseButtons.Right); return true;
            case WM_MBUTTONDOWN: _onClick(MouseButtons.Middle); return true;
            default: return false;
        }
    }
}