using System.Reflection;
using System.Xml.Linq;

namespace HelloWorld.Tests;

/// <summary>
/// Tests to verify app icon display and configuration
/// Requirements: 2.1, 2.2, 2.4, 4.1, 4.3
/// </summary>
public class AppIconTests
{
    private readonly string _projectRoot;
    private readonly string _projectFile;

    public AppIconTests()
    {
        // Get the project root directory
        _projectRoot = GetProjectRootDirectory();
        _projectFile = Path.Combine(_projectRoot, "HelloWorld.csproj");
    }

    /// <summary>
    /// Test that verifies custom app icon resources exist
    /// Requirement 2.1: Custom app icon should be displayed on device home screen
    /// </summary>
    [Test]
    public void AppIcon_ResourceFiles_ShouldExist()
    {
        // Arrange
        var mainIconPath = Path.Combine(_projectRoot, "Resources", "AppIcon", "appicon.svg");
        var foregroundIconPath = Path.Combine(_projectRoot, "Resources", "AppIcon", "appiconfg.svg");

        // Act & Assert
        Assert.IsTrue(File.Exists(mainIconPath), 
            $"Main app icon file should exist at: {mainIconPath}");
        Assert.IsTrue(File.Exists(foregroundIconPath), 
            $"Foreground app icon file should exist at: {foregroundIconPath}");
    }

    /// <summary>
    /// Test that verifies app icon files have valid SVG content
    /// Requirement 2.1: Custom app icon should be properly formatted
    /// </summary>
    [Test]
    public void AppIcon_SvgFiles_ShouldHaveValidContent()
    {
        // Arrange
        var mainIconPath = Path.Combine(_projectRoot, "Resources", "AppIcon", "appicon.svg");
        var foregroundIconPath = Path.Combine(_projectRoot, "Resources", "AppIcon", "appiconfg.svg");

        // Act
        var mainIconContent = File.ReadAllText(mainIconPath);
        var foregroundIconContent = File.ReadAllText(foregroundIconPath);

        // Assert
        Assert.IsTrue(mainIconContent.Contains("<?xml"), "Main icon should be valid XML/SVG");
        Assert.IsTrue(mainIconContent.Contains("<svg"), "Main icon should contain SVG root element");
        Assert.IsTrue(mainIconContent.Contains("viewBox"), "Main icon should have viewBox for scalability");

        Assert.IsTrue(foregroundIconContent.Contains("<?xml"), "Foreground icon should be valid XML/SVG");
        Assert.IsTrue(foregroundIconContent.Contains("<svg"), "Foreground icon should contain SVG root element");
        Assert.IsTrue(foregroundIconContent.Contains("viewBox"), "Foreground icon should have viewBox for scalability");
    }

    /// <summary>
    /// Test that verifies project configuration includes custom app icon settings
    /// Requirement 2.2: App icon changes should be reflected across Android, iOS, Windows platforms
    /// </summary>
    [Test]
    public void ProjectConfiguration_ShouldIncludeCustomAppIcon()
    {
        // Arrange & Act
        var projectContent = File.ReadAllText(_projectFile);
        var projectXml = XDocument.Parse(projectContent);

        // Find MauiIcon elements
        var mauiIconElements = projectXml.Descendants("MauiIcon").ToList();

        // Assert
        Assert.IsTrue(mauiIconElements.Count > 0, "Project should contain MauiIcon configuration");

        var customIconElement = mauiIconElements.FirstOrDefault(e => 
            e.Attribute("Include")?.Value.Contains("appicon.svg") == true);

        Assert.IsNotNull(customIconElement, "Project should reference custom appicon.svg");
        
        // Verify foreground file is configured
        var foregroundFile = customIconElement.Attribute("ForegroundFile")?.Value;
        Assert.IsNotNull(foregroundFile, "Custom icon should have foreground file configured");
        Assert.IsTrue(foregroundFile.Contains("appiconfg.svg"), "Foreground file should reference appiconfg.svg");

        // Verify background color is configured
        var backgroundColor = customIconElement.Attribute("Color")?.Value;
        Assert.IsNotNull(backgroundColor, "Custom icon should have background color configured");
        Assert.IsTrue(backgroundColor.StartsWith("#"), "Background color should be in hex format");
    }

    /// <summary>
    /// Test that verifies target platforms exclude MacCatalyst
    /// Requirement 4.1: App should build and deploy correctly on target platforms
    /// </summary>
    [Test]
    public void ProjectConfiguration_ShouldExcludeMacCatalyst()
    {
        // Arrange & Act
        var projectContent = File.ReadAllText(_projectFile);
        var projectXml = XDocument.Parse(projectContent);

        var targetFrameworksElement = projectXml.Descendants("TargetFrameworks").FirstOrDefault();

        // Assert
        Assert.IsNotNull(targetFrameworksElement, "Project should have TargetFrameworks defined");
        
        var targetFrameworks = targetFrameworksElement.Value;
        Assert.IsFalse(targetFrameworks.Contains("net8.0-maccatalyst"), 
            "Target frameworks should not include MacCatalyst");
        
        // Verify expected platforms are included
        Assert.IsTrue(targetFrameworks.Contains("net8.0-android"), 
            "Target frameworks should include Android");
        Assert.IsTrue(targetFrameworks.Contains("net8.0-ios"), 
            "Target frameworks should include iOS");
    }

    /// <summary>
    /// Test that verifies adaptive icon configuration for Android
    /// Requirement 2.4: System should generate appropriate icon sizes and formats for each platform
    /// </summary>
    [Test]
    public void AppIcon_AdaptiveIconConfiguration_ShouldBeValid()
    {
        // Arrange & Act
        var projectContent = File.ReadAllText(_projectFile);
        var projectXml = XDocument.Parse(projectContent);

        var mauiIconElement = projectXml.Descendants("MauiIcon")
            .FirstOrDefault(e => e.Attribute("Include")?.Value.Contains("appicon.svg") == true);

        // Assert
        Assert.IsNotNull(mauiIconElement, "Custom app icon configuration should exist");

        // Verify adaptive icon components
        var includeAttribute = mauiIconElement.Attribute("Include")?.Value;
        var foregroundAttribute = mauiIconElement.Attribute("ForegroundFile")?.Value;
        var colorAttribute = mauiIconElement.Attribute("Color")?.Value;

        Assert.IsNotNull(includeAttribute, "Icon should have Include attribute");
        Assert.IsNotNull(foregroundAttribute, "Icon should have ForegroundFile for adaptive icons");
        Assert.IsNotNull(colorAttribute, "Icon should have Color attribute for background");

        // Verify files exist
        var mainIconPath = Path.Combine(_projectRoot, includeAttribute.Replace("\\", Path.DirectorySeparatorChar.ToString()));
        var foregroundIconPath = Path.Combine(_projectRoot, foregroundAttribute.Replace("\\", Path.DirectorySeparatorChar.ToString()));

        Assert.IsTrue(File.Exists(mainIconPath), $"Main icon file should exist: {mainIconPath}");
        Assert.IsTrue(File.Exists(foregroundIconPath), $"Foreground icon file should exist: {foregroundIconPath}");
    }

    /// <summary>
    /// Test that verifies icon resources are properly structured for cross-platform deployment
    /// Requirement 4.3: App should display custom app icon in device app launcher
    /// </summary>
    [Test]
    public void AppIcon_ResourceStructure_ShouldSupportCrossPlatform()
    {
        // Arrange
        var appIconDirectory = Path.Combine(_projectRoot, "Resources", "AppIcon");

        // Act & Assert
        Assert.IsTrue(Directory.Exists(appIconDirectory), 
            "AppIcon directory should exist in Resources folder");

        // Verify SVG format for scalability across platforms
        var svgFiles = Directory.GetFiles(appIconDirectory, "*.svg");
        Assert.IsTrue(svgFiles.Length >= 2, 
            "Should have at least 2 SVG files (main icon and foreground)");

        // Verify recommended naming convention
        var hasMainIcon = svgFiles.Any(f => Path.GetFileName(f).Equals("appicon.svg", StringComparison.OrdinalIgnoreCase));
        var hasForegroundIcon = svgFiles.Any(f => Path.GetFileName(f).Equals("appiconfg.svg", StringComparison.OrdinalIgnoreCase));

        Assert.IsTrue(hasMainIcon, "Should have main app icon file named 'appicon.svg'");
        Assert.IsTrue(hasForegroundIcon, "Should have foreground icon file named 'appiconfg.svg'");
    }

    /// <summary>
    /// Test that simulates platform-specific icon generation validation
    /// Requirement 2.4: System should generate appropriate icon sizes and formats for each platform
    /// </summary>
    [Test]
    public void AppIcon_PlatformSpecificGeneration_ShouldBeConfigured()
    {
        // Arrange
        var projectContent = File.ReadAllText(_projectFile);
        var projectXml = XDocument.Parse(projectContent);

        // Act
        var targetFrameworksElement = projectXml.Descendants("TargetFrameworks").FirstOrDefault();
        var mauiIconElement = projectXml.Descendants("MauiIcon")
            .FirstOrDefault(e => e.Attribute("Include")?.Value.Contains("appicon.svg") == true);

        // Assert
        Assert.IsNotNull(targetFrameworksElement, "Target frameworks should be defined");
        Assert.IsNotNull(mauiIconElement, "Custom app icon should be configured");

        var targetFrameworks = targetFrameworksElement.Value;
        var supportedPlatforms = new[] { "android", "ios", "windows" };

        foreach (var platform in supportedPlatforms)
        {
            Assert.IsTrue(targetFrameworks.Contains($"net8.0-{platform}"), 
                $"Should support {platform} platform for icon generation");
        }

        // Verify icon configuration supports all platforms
        var iconInclude = mauiIconElement.Attribute("Include")?.Value;
        var iconForeground = mauiIconElement.Attribute("ForegroundFile")?.Value;
        var iconColor = mauiIconElement.Attribute("Color")?.Value;

        Assert.IsNotNull(iconInclude, "Icon Include should be configured for all platforms");
        Assert.IsNotNull(iconForeground, "Icon ForegroundFile should be configured for adaptive icons");
        Assert.IsNotNull(iconColor, "Icon Color should be configured for background");
    }

    /// <summary>
    /// Execute all app icon tests and return results
    /// </summary>
    public static TestResults ExecuteAllTests()
    {
        var results = new TestResults();
        var testInstance = new AppIconTests();
        var testMethods = typeof(AppIconTests)
            .GetMethods()
            .Where(m => m.GetCustomAttribute<TestAttribute>() != null)
            .ToList();

        foreach (var method in testMethods)
        {
            results.TotalTests++;
            
            try
            {
                System.Diagnostics.Debug.WriteLine($"Running {method.Name}...");
                method.Invoke(testInstance, null);
                System.Diagnostics.Debug.WriteLine($"{method.Name} PASSED");
                results.PassedTests++;
            }
            catch (Exception ex)
            {
                var innerException = ex.InnerException ?? ex;
                System.Diagnostics.Debug.WriteLine($"{method.Name} FAILED: {innerException.Message}");
                results.FailedTests++;
                results.Failures.Add($"{method.Name}: {innerException.Message}");
            }
        }

        return results;
    }

    private string GetProjectRootDirectory()
    {
        var currentDirectory = Directory.GetCurrentDirectory();
        var directory = new DirectoryInfo(currentDirectory);

        // Walk up the directory tree to find the project root
        while (directory != null && !File.Exists(Path.Combine(directory.FullName, "HelloWorld.csproj")))
        {
            directory = directory.Parent;
        }

        if (directory == null)
        {
            throw new InvalidOperationException("Could not find project root directory");
        }

        return directory.FullName;
    }

    public class TestResults
    {
        public int TotalTests { get; set; }
        public int PassedTests { get; set; }
        public int FailedTests { get; set; }
        public List<string> Failures { get; set; } = new List<string>();
    }
}

/// <summary>
/// Simple test attribute for demonstration purposes
/// In a real project, you would use NUnit, xUnit, or MSTest
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class TestAttribute : Attribute
{
}

/// <summary>
/// Simple assertion class for demonstration purposes
/// In a real project, you would use the assertion framework from your testing library
/// </summary>
public static class Assert
{
    public static void IsTrue(bool condition, string message = "")
    {
        if (!condition)
        {
            throw new AssertionException($"Assertion failed: {message}");
        }
    }

    public static void IsFalse(bool condition, string message = "")
    {
        if (condition)
        {
            throw new AssertionException($"Assertion failed: {message}");
        }
    }

    public static void IsNotNull(object obj, string message = "")
    {
        if (obj == null)
        {
            throw new AssertionException($"Assertion failed: {message}");
        }
    }
}

public class AssertionException : Exception
{
    public AssertionException(string message) : base(message) { }
}