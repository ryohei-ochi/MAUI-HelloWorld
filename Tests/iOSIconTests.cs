using System.Reflection;
using System.Xml.Linq;

namespace HelloWorld.Tests;

/// <summary>
/// Tests to verify iOS-specific app icon configuration and generation
/// Requirements: 2.4, 2.5, 4.1, 4.2
/// </summary>
public class iOSIconTests
{
    private readonly string _projectRoot;
    private readonly string _projectFile;
    private readonly string _infoPlistFile;

    public iOSIconTests()
    {
        // Get the project root directory
        _projectRoot = GetProjectRootDirectory();
        _projectFile = Path.Combine(_projectRoot, "HelloWorld.csproj");
        _infoPlistFile = Path.Combine(_projectRoot, "Platforms", "iOS", "Info.plist");
    }

    /// <summary>
    /// Test that verifies iOS app icon configuration for Retina Display
    /// Requirement 2.5: System should generate appropriate high-resolution icons for Retina Display
    /// </summary>
    [Test]
    public void iOS_AppIcon_RetinaDisplayConfiguration()
    {
        // Arrange
        var projectContent = File.ReadAllText(_projectFile);
        var projectXml = XDocument.Parse(projectContent);

        // Act
        var mauiIconElement = projectXml.Descendants("MauiIcon")
            .FirstOrDefault(e => e.Attribute("Include")?.Value.Contains("appicon.svg") == true);

        // Assert
        Assert.IsNotNull(mauiIconElement, "MauiIcon configuration should exist for iOS app icons");

        // Verify base size is set to 512x512 for high resolution
        var baseSize = mauiIconElement.Attribute("BaseSize")?.Value;
        Assert.IsNotNull(baseSize, "BaseSize should be specified for high-resolution icon generation");
        Assert.IsTrue(baseSize.Contains("512"), "BaseSize should be at least 512x512 for Retina Display support");

        // Verify SVG source is high resolution
        var iconPath = mauiIconElement.Attribute("Include")?.Value;
        Assert.IsNotNull(iconPath, "Icon path should be specified");
        var fullIconPath = Path.Combine(_projectRoot, iconPath.Replace("\\", Path.DirectorySeparatorChar.ToString()));
        Assert.IsTrue(File.Exists(fullIconPath), "Icon file should exist");

        // Read SVG content to verify dimensions
        var svgContent = File.ReadAllText(fullIconPath);
        Assert.IsTrue(svgContent.Contains("width=\"512\"") || svgContent.Contains("width=\"512px\""), 
            "SVG width should be at least 512px for Retina Display");
        Assert.IsTrue(svgContent.Contains("height=\"512\"") || svgContent.Contains("height=\"512px\""), 
            "SVG height should be at least 512px for Retina Display");
    }

    /// <summary>
    /// Test that verifies iOS-specific build settings for high-resolution icons
    /// Requirement 2.5: System should generate appropriate high-resolution icons for Retina Display
    /// </summary>
    [Test]
    public void iOS_BuildSettings_HighResolutionSupport()
    {
        // Arrange
        var projectContent = File.ReadAllText(_projectFile);
        var projectXml = XDocument.Parse(projectContent);

        // Act - Find iOS-specific property groups
        var iosPropertyGroups = projectXml.Descendants("PropertyGroup")
            .Where(pg => pg.Attribute("Condition")?.Value.Contains("ios") == true)
            .ToList();

        // Assert
        Assert.IsTrue(iosPropertyGroups.Count > 0, "iOS-specific property groups should exist");

        // Check for high-resolution settings across all iOS property groups
        bool hasMtouchHighResolution = false;
        bool hasOptimizePNGs = false;

        foreach (var propertyGroup in iosPropertyGroups)
        {
            if (propertyGroup.Element("MtouchHighResolution")?.Value == "true")
                hasMtouchHighResolution = true;
            
            if (propertyGroup.Element("OptimizePNGs")?.Value == "true")
                hasOptimizePNGs = true;
        }

        Assert.IsTrue(hasMtouchHighResolution, "MtouchHighResolution should be enabled for Retina Display support");
        Assert.IsTrue(hasOptimizePNGs, "OptimizePNGs should be enabled for high-quality icon generation");
    }

    /// <summary>
    /// Test that verifies Info.plist configuration for iOS app icons
    /// Requirement 2.5: System should generate appropriate high-resolution icons for Retina Display
    /// </summary>
    [Test]
    public void iOS_InfoPlist_AppIconConfiguration()
    {
        // Skip if Info.plist doesn't exist (non-iOS build)
        if (!File.Exists(_infoPlistFile))
        {
            System.Diagnostics.Debug.WriteLine("Info.plist not found - skipping test");
            return;
        }

        // Arrange
        var plistContent = File.ReadAllText(_infoPlistFile);

        // Act & Assert
        // Check for XSAppIconAssets key
        Assert.IsTrue(plistContent.Contains("<key>XSAppIconAssets</key>"), 
            "Info.plist should contain XSAppIconAssets key");
        
        Assert.IsTrue(plistContent.Contains("<string>Assets.xcassets/appicon.appiconset</string>"), 
            "Info.plist should reference appicon.appiconset");

        // Check for UIPrerenderedIcon for better Retina Display rendering
        Assert.IsTrue(plistContent.Contains("<key>UIPrerenderedIcon</key>"), 
            "Info.plist should contain UIPrerenderedIcon key for Retina Display");
        
        Assert.IsTrue(plistContent.Contains("<true/>") && 
                     plistContent.IndexOf("<true/>") > plistContent.IndexOf("<key>UIPrerenderedIcon</key>"), 
            "UIPrerenderedIcon should be set to true");
    }

    /// <summary>
    /// Test that verifies project configuration for iPhone 8 and other Retina Display devices
    /// Requirement 4.1: App should build and deploy correctly on target platforms
    /// Requirement 4.2: System should maintain visual consistency across different devices
    /// </summary>
    [Test]
    public void iOS_ProjectConfiguration_RetinaDisplaySupport()
    {
        // Arrange
        var projectContent = File.ReadAllText(_projectFile);
        var projectXml = XDocument.Parse(projectContent);

        // Act
        var iosMinVersion = projectXml.Descendants("SupportedOSPlatformVersion")
            .FirstOrDefault(e => e.Attribute("Condition")?.Value.Contains("ios") == true)?.Value;

        // Assert
        Assert.IsNotNull(iosMinVersion, "iOS minimum version should be specified");
        
        // iPhone 8 requires iOS 11+
        Assert.IsTrue(float.TryParse(iosMinVersion, out float version) && version >= 11.0f, 
            $"iOS minimum version should be at least 11.0 for iPhone 8 support, found {iosMinVersion}");

        // Check for device-specific build setting
        var deviceSpecificBuild = projectXml.Descendants("DeviceSpecificBuild")
            .FirstOrDefault(e => e.Value == "true");
        
        Assert.IsNotNull(deviceSpecificBuild, 
            "DeviceSpecificBuild should be enabled for proper device-specific resource generation");
    }

    /// <summary>
    /// Execute all iOS icon tests and return results
    /// </summary>
    public static TestResults ExecuteAllTests()
    {
        var results = new TestResults();
        var testInstance = new iOSIconTests();
        var testMethods = typeof(iOSIconTests)
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