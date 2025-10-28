using System.Diagnostics.CodeAnalysis;

namespace App;

/// <summary>
/// Application program class.
/// </summary>
[ExcludeFromCodeCoverage]
internal static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new MainForm());
    }
}
