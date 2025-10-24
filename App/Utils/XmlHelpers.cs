using System.Reflection;
using System.Xml.Linq;

namespace App.Utils;

/// <summary>
/// Helper class to extract information from xmls.
/// </summary>
public static class XmlHelpers
{
    private const string csprojFile = "App.csproj";
    public const string fallbackVersion = "0.0.0";


    /// <summary>
    /// Gets the app version from the project file.
    /// </summary>
    /// <returns></returns>
    public static string GetAppVersion()
    {
        var version = GetVersionFromProjectFile();
        if (version != fallbackVersion) return CleanSemanticVersion(version);

        var entry = Assembly.GetEntryAssembly() ?? typeof(LabelFormatter).Assembly;
        return CleanSemanticVersion(entry.GetName().Version?.ToString() ?? fallbackVersion);
    }

    /// <summary>
    /// Reads Version element from the .csproj file.
    /// </summary>
    private static string GetVersionFromProjectFile()
    {
        try
        {
            string projectPath = Path.Combine(AppContext.BaseDirectory, csprojFile);

            if (!File.Exists(projectPath))
            {
                var parent = Directory.GetParent(AppContext.BaseDirectory)?.Parent?.Parent;
                if (parent is not null)
                    projectPath = Path.Combine(parent.FullName, csprojFile);
            }

            if (!File.Exists(projectPath))
                return fallbackVersion;

            var xml = XDocument.Load(projectPath);
            var versionElement = xml.Root?
                .Element("PropertyGroup")?
                .Element("Version");

            return versionElement?.Value?.Trim() ?? fallbackVersion;
        }
        catch
        {
            return fallbackVersion;
        }
    }

    /// <summary>
    /// Normalize version string for UI and removes metadata.
    /// </summary>
    private static string CleanSemanticVersion(string? version)
    {
        if (string.IsNullOrWhiteSpace(version)) return fallbackVersion;

        var plus = version.IndexOf('+');
        if (plus >= 0)
            version = version[..plus];

        version = version.Trim();

        string? prerelease = null;
        var hyphen = version.IndexOf('-');
        if (hyphen >= 0)
        {
            prerelease = version[hyphen..];
            version = version[..hyphen];
        }

        var parts = version.Split('.');

        if (parts.Length == 4 && parts[3] == "0")
        {
            version = $"{parts[0]}.{parts[1]}.{parts[2]}";
        }
        else
        {
            version = string.Join('.', parts);
        }
        return prerelease is not null ? version + prerelease : version;
    }
}