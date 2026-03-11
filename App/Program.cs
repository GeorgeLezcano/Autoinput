using System.Diagnostics.CodeAnalysis;

namespace App;

/// <summary>
/// Application program class.
/// </summary>
[ExcludeFromCodeCoverage]
internal static class Program
{
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        using var mutex = new Mutex(initiallyOwned: true, name: @"Global\AutoInput_SingleInstance", out bool isNewInstance);
        if (!isNewInstance)
        {
            MessageBox.Show("AutoInput is already running.", "AutoInput", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        //TODO Consider using this instead of SystemAware for higher resolution. 
        // This comes with a UI adjustments to fit text and buttons.
        //Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
        Application.SetHighDpiMode(HighDpiMode.SystemAware);

        ApplicationConfiguration.Initialize();
        Application.Run(new MainForm());
    }
}
