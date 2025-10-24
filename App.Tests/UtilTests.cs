using System.Diagnostics.CodeAnalysis;
using App.Utils;

namespace App.Tests;

/// <summary>
/// Unit tests for utility classes under App.Utils.
/// </summary>
[ExcludeFromCodeCoverage]
public class UtilTests : IDisposable
{
    private readonly string _baseDir = AppContext.BaseDirectory;
    private readonly string _csprojPath;

    public UtilTests()
    {
        _csprojPath = Path.Combine(_baseDir, "App.csproj");
        CleanupProjectFile();
    }

    public void Dispose()
    {
        CleanupProjectFile();
    }

    private void CleanupProjectFile()
    {
        try
        {
            if (File.Exists(_csprojPath))
                File.Delete(_csprojPath);
        }
        catch
        {
        }
    }

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
        File.WriteAllText(_csprojPath, xml);
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
        string actual = LabelFormatter.SetInputCountLabel(count);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void SetIntervalHint_Should_Correctly_Display_The_Range()
    {
        int min = 50;
        int max = 1200;
        string expected = $"Range: {min} – {max} ms \n1 second = 1000 milliseconds";
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
}
