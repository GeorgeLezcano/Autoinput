using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;
using App.Utils;

namespace App.Tests;

/// <summary>
/// Unit tests for utility classes under App.Utils.
/// </summary>
[ExcludeFromCodeCoverage]
public class UtilTests : IDisposable
{
    private readonly string _baseDir = AppContext.BaseDirectory;
    private readonly string csprojPath;

    /// <summary>
    /// Primaty Constructor.
    /// </summary>
    public UtilTests()
    {
        csprojPath = Path.Combine(_baseDir, "App.csproj");
        CleanupProjectFile();
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        CleanupProjectFile();
    }

    [Fact]
    public void SetTimeLabel_Should_Convert_Time_Correctly_Hours()
    {
        int seconds = 3856; // 1h 4m 16s
        string expected = "Active Time: 01:04:16";
        string actual = LabelFormatter.SetTimeLabel(seconds);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void SetInputCountLabel_Should_Display_The_Correct_Count()
    {
        int count = 230;
        string expected = $"Input Count: {count}";
        string actual = LabelFormatter.SetInputCountLabel(count, false);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void SetIntervalHint_Should_Correctly_Display_The_Range()
    {
        int min = 500;
        int max = 1200;
        decimal minSec = TimeUtils.ToSeconds(min);
        decimal maxSec = TimeUtils.ToSeconds(max);
        string expected = $"Range: {minSec:0.0} – {maxSec:0.0} Seconds";
        string actual = LabelFormatter.SetIntervalHint(min, max);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void SetVersionLabel_Should_Display_App_Version()
    {
        WriteCsprojWithVersion("1.2.3.0+meta");
        string expected = "App Version: 1.2.3";

        string actual = LabelFormatter.SetVersionLabel();

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void SetAppShellText_Should_Display_Correct_Version_Format()
    {
        WriteCsprojWithVersion("4.5.6-beta+sha123");
        string expected = "AutoInput_v4.5.6-beta";
        string actual = LabelFormatter.SetAppShellText();
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void GetAppVersion_Should_Read_From_Project_File_And_Clean()
    {
        WriteCsprojWithVersion("7.8.9.0+build.1");
        string version = XmlHelpers.GetAppVersion();
        Assert.Equal("7.8.9", version);
    }

    [Fact]
    public void GetAppVersion_Should_Preserve_Prerelease()
    {
        WriteCsprojWithVersion("3.1.4-rc.2+meta");
        string version = XmlHelpers.GetAppVersion();
        Assert.Equal("3.1.4-rc.2", version);
    }

    [Fact]
    public void GetAppVersion_Should_Fallback_When_No_Project_File()
    {
        CleanupProjectFile();
        string version = XmlHelpers.GetAppVersion();

        Assert.False(string.IsNullOrWhiteSpace(version));
        Assert.Matches(@"^\d+\.\d+\.\d+(-[0-9A-Za-z\.-]+)?$", version);
    }

    [Theory]
    [InlineData("1.0.0+abc", "1.0.0")]
    [InlineData("1.0.0.0", "1.0.0")]
    [InlineData("1.0.0-beta", "1.0.0-beta")]
    [InlineData("  2.0.0-rc.1+meta  ", "2.0.0-rc.1")]
    public void GetAppVersion_Should_Clean_Various_Formats(string raw, string expected)
    {
        WriteCsprojWithVersion(raw);
        string actual = XmlHelpers.GetAppVersion();
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void PreFilterMessage_Should_Return_False_When_Not_Mouse_Input()
    {
        bool clicked = false;
        var filter = new MouseBindFilter(_ => clicked = true);

        var message = new Message
        {
            Msg = 0x0100,
            WParam = IntPtr.Zero,
            LParam = IntPtr.Zero
        };

        bool result = filter.PreFilterMessage(ref message);

        Assert.False(result);
        Assert.False(clicked);
    }

    [Theory]
    [InlineData(0x0201, MouseButtons.Left)]
    [InlineData(0x0204, MouseButtons.Right)]
    [InlineData(0x0207, MouseButtons.Middle)]
    public void PreFilterMessage_Should_Invoke_OnClick_For_MouseButtons(int msg, MouseButtons expected)
    {
        MouseButtons? clicked = null;
        var filter = new MouseBindFilter(b => clicked = b);

        var message = new Message { Msg = msg };
        bool result = filter.PreFilterMessage(ref message);

        Assert.True(result);
        Assert.Equal(expected, clicked);
    }

    [Fact]
    public void Time_To_Milliseconds_Converts_The_Value()
    {
        decimal seconds = 0.3M;
        int expectedMS = 300;
        int actualMS = TimeUtils.ToMilliseconds(seconds);
        Assert.Equal(expectedMS, actualMS);
    }

    [Fact]
    public void Time_To_Seconds_Converts_The_Value()
    {
        int ms = 800;
        decimal expectedSeconds = 0.8M;
        decimal actualSeconds = TimeUtils.ToSeconds(ms);
        Assert.Equal(expectedSeconds, actualSeconds);
    }

    [Fact]
    public void ClampSeconds_Should_Return_Value_When_Within_Bounds()
    {
        decimal seconds = 1.5M;
        int minMs = 1000;
        int maxMs = 3000;
        decimal result = TimeUtils.ClampSeconds(seconds, minMs, maxMs);
        Assert.Equal(1.5M, result);
    }

    [Fact]
    public void ClampSeconds_Should_Clamp_To_Min_When_Below_Bounds()
    {
        decimal seconds = 0.5M;
        int minMs = 1000;
        int maxMs = 3000;
        decimal result = TimeUtils.ClampSeconds(seconds, minMs, maxMs);
        Assert.Equal(1.0M, result);
    }

    [Fact]
    public void ClampSeconds_Should_Clamp_To_Max_When_Above_Bounds()
    {
        decimal seconds = 5.0M;
        int minMs = 1000;
        int maxMs = 3000;
        decimal result = TimeUtils.ClampSeconds(seconds, minMs, maxMs);
        Assert.Equal(3.0M, result);
    }

    [Fact]
    public void IsMouseKey_Should_Return_True_When_MouseKey()
    {
        Keys key = Keys.LButton;
        bool result = NativeInput.IsMouseKey(key);
        Assert.True(result);
    }

    /********************
    ***Private helpers***
    *********************/

    /// <summary>
    /// Deletes the created csproj file if exists.
    /// </summary>
    private void CleanupProjectFile()
    {
        try
        {
            if (File.Exists(csprojPath))
                File.Delete(csprojPath);
        }
        catch { }
    }

    /// <summary>
    /// Returns a csproj mock file with the specified version.
    /// </summary>
    /// <param name="version">The version to be tested.</param>
    private void WriteCsprojWithVersion(string version)
    {
        var xml = $$"""
        <Project Sdk="Microsoft.NET.Sdk">
          <PropertyGroup>
            <TargetFramework>net8.0</TargetFramework>
            <Version>{{version}}</Version>
          </PropertyGroup>
        </Project>
        """;
        File.WriteAllText(csprojPath, xml);
    }
}
