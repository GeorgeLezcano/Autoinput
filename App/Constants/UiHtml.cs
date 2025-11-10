
using System.Diagnostics.CodeAnalysis;

namespace App.Constants;

/// <summary>
/// Constants for the UI html elements in the app.
/// </summary>
[ExcludeFromCodeCoverage]
public static class UiHtml
{
    /// <summary>
    /// Location for the user guide manual.
    /// </summary>
    public const string UserGuideLocation = "App.Resources.UserGuide.html";

    /// <summary>
    /// "File not found" page for embedded resource lookup.
    /// </summary>
    public static string FileNotFoundEmbedded(string resourceName, Color back, Color textSecondary)
    {
        return $@"
            <html>
            <body style='font-family:Segoe UI; background:{ColorTranslator.ToHtml(back)}; color:{ColorTranslator.ToHtml(textSecondary)}; padding:24px;'>
                <h3>User Guide</h3>
                <p>Embedded resource not found: <code>{resourceName}</code></p>
                <p>Make sure <b>Resources/UserGuide.html</b> is included as <code>&lt;EmbeddedResource&gt;</code> in the project file.</p>
            </body>
            </html>";
    }

}
