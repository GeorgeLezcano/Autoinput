using System.Net;
using System.Reflection;
using App.Constants;
using App.Utils;

namespace App;

/// <summary>
/// Simple in-app manual window that loads embedded Resources/UserGuide.html.
/// RootNamespace ("App") + folder + filename to load files.
/// </summary>
public sealed class UserGuideForm : Form
{
    private readonly WebBrowser _viewer = new();

    /// <summary>
    /// Default constructor to initialize the form.
    /// </summary>
    public UserGuideForm()
    {
        Text = "User Guide";
        Icon = null;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ClientSize = new Size(710, 820);
        BackColor = UiColors.FormBack;
        ForeColor = UiColors.FormFore;
        StartPosition = FormStartPosition.CenterParent;

        _viewer.Dock = DockStyle.Fill;
        _viewer.AllowWebBrowserDrop = false;
        _viewer.IsWebBrowserContextMenuEnabled = false;
        _viewer.WebBrowserShortcutsEnabled = true;

        Controls.Add(_viewer);

        Load += (_, __) => LoadFromEmbedded(UiHtml.UserGuideLocation);
    }

    /// <summary>
    /// Tries to load the embeded file name provided.
    /// </summary>
    private void LoadFromEmbedded(string resourceName)
    {
        using Stream? stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
        if (stream is null)
        {
            _viewer.DocumentText = UiHtml.FileNotFoundEmbedded(resourceName, UiColors.FormBack, UiColors.TextSecondary);
            return;
        }

        using var reader = new StreamReader(stream);
        string html = reader.ReadToEnd();

        string version = WebUtility.HtmlEncode(XmlHelpers.GetAppVersion());

        html = html
            .Replace("{{APP_VERSION}}", version ?? "1.0.0")
            .Replace("{{YEAR}}", DateTime.UtcNow.Year.ToString());

        _viewer.DocumentText = html;
    }
}
