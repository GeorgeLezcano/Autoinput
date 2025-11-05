using App.Constants;

namespace App;

/// <summary>
/// Simple in-app manual placeholder window.
/// </summary>
public sealed class UserGuideForm : Form
{
    public UserGuideForm()
    {
        Text = "User Guide";
        Icon = null;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ClientSize = new Size(600, 300);
        BackColor = UiColors.FormBack;
        ForeColor = UiColors.FormFore;
        StartPosition = FormStartPosition.CenterParent;

        var placeholder = new Label
        {
            AutoSize = false,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter,
            Font = new Font("Segoe UI", 12F, FontStyle.Italic),
            ForeColor = UiColors.TextSecondary,
            Text =
                " User Guide Placeholder"
        };

        Controls.Add(placeholder);
    }
}
