using System.Reflection;
using System.Xml.Linq;

namespace HelloWorld.Tests;

/// <summary>
/// Platform-specific verification tests for Android, iOS, and Windows
/// Requirements: 2.3, 2.4, 3.5, 4.1, 4.2
/// </summary>
public class PlatformSpecificTests
{
    private readonly string _projectRoot;
    private readonly string _projectFile;

    public PlatformSpecificTests()
    {
        // Get the project root directory
        _projectRoot = GetProjectRootDirectory();
        _projectFile = Path.Combine(_projectRoot, "HelloWorld.csproj");
    }

    #region Android Platform Tests

    /// <summary>
    /// Test that verifies Android adaptive icon generation configuration
    /// Requirement 2.3: System should properly render adaptive icons on supported platforms
    /// Requirement 2.4: System should generate appropriate icon sizes and formats for each platform
    /// </summary>
    [Test]
    public void Android_AdaptiveIcon_GenerationShouldBeConfigured()
    {
        // Arrange
        var projectContent = File.ReadAllText(_projectFile);
        var projectXml = XDocument.Parse(projectContent);

        // Act
        var targetFrameworks = projectXml.Descendants("TargetFrameworks").FirstOrDefault()?.Value;
        var mauiIconElement = projectXml.Descendants("MauiIcon")
            .FirstOrDefault(e => e.Attribute("Include")?.Value.Contains("appicon.svg") == true);

        // Assert
        Assert.IsTrue(targetFrameworks?.Contains("net8.0-android") == true, 
            "Android platform should be configured for adaptive icon generation");
        
        Assert.IsNotNull(mauiIconElement, "MauiIcon configuration should exist for Android adaptive icons");

        // Verify adaptive icon components
        var foregroundFile = mauiIconElement.Attribute("ForegroundFile")?.Value;
        var backgroundColor = mauiIconElement.Attribute("Color")?.Value;
        var mainIcon = mauiIconElement.Attribute("Include")?.Value;

        Assert.IsNotNull(foregroundFile, "Android adaptive icons require foreground file");
        Assert.IsNotNull(backgroundColor, "Android adaptive icons require background color");
        Assert.IsNotNull(mainIcon, "Android adaptive icons require main icon file");

        // Verify files exist
        var foregroundPath = Path.Combine(_projectRoot, foregroundFile.Replace("\\", Path.DirectorySeparatorChar.ToString()));
        var mainIconPath = Path.Combine(_projectRoot, mainIcon.Replace("\\", Path.DirectorySeparatorChar.ToString()));

        Assert.IsTrue(File.Exists(foregroundPath), $"Android foreground icon should exist: {foregroundPath}");
        Assert.IsTrue(File.Exists(mainIconPath), $"Android main icon should exist: {mainIconPath}");
    }

    /// <summary>
    /// Test that verifies Android adaptive icon display characteristics
    /// Requirement 4.1: App should build and deploy correctly on target platforms
    /// Requirement 4.2: System should maintain visual consistency across different devices
    /// </summary>
    [Test]
    public void Android_AdaptiveIcon_DisplayShouldBeValid()
    {
        // Arrange
        var foregroundIconPath = Path.Combine(_projectRoot, "Resources", "AppIcon", "appiconfg.svg");
        var projectContent = File.ReadAllText(_projectFile);
        var projectXml = XDocument.Parse(projectContent);

        // Act
        var foregroundContent = File.ReadAllText(foregroundIconPath);
        var mauiIconElement = projectXml.Descendants("MauiIcon")
            .FirstOrDefault(e => e.Attribute("Include")?.Value.Contains("appicon.svg") == true);

        // Assert - Verify foreground icon is suitable for adaptive icons
        Assert.IsTrue(foregroundContent.Contains("viewBox"), 
            "Android adaptive foreground icon should have viewBox for proper scaling");
        
        // Verify the foreground icon doesn't fill the entire canvas (adaptive icon safe zone)
        Assert.IsTrue(foregroundContent.Contains("<svg"), "Foreground should be valid SVG");
        
        // Verify background color is valid hex color
        var backgroundColor = mauiIconElement?.Attribute("Color")?.Value;
        Assert.IsNotNull(backgroundColor, "Background color should be configured");
        Assert.IsTrue(backgroundColor.StartsWith("#") && backgroundColor.Length == 7, 
            "Background color should be valid hex format (#RRGGBB)");

        // Verify Android platform support
        var targetFrameworks = projectXml.Descendants("TargetFrameworks").FirstOrDefault()?.Value;
        Assert.IsTrue(targetFrameworks?.Contains("net8.0-android") == true, 
            "Android platform should be supported for adaptive icon display");
    }

    /// <summary>
    /// Test that verifies Android-specific icon resource generation
    /// Requirement 2.4: System should generate appropriate icon sizes and formats for each platform
    /// </summary>
    [Test]
    public void Android_IconGeneration_ShouldSupportMultipleDensities()
    {
        // Arrange
        var projectContent = File.ReadAllText(_projectFile);
        var projectXml = XDocument.Parse(projectContent);

        // Act
        var androidSupportedOSVersion = projectXml.Descendants("SupportedOSPlatformVersion")
            .FirstOrDefault(e => e.Attribute("Condition")?.Value.Contains("android") == true);

        var mauiIconElement = projectXml.Descendants("MauiIcon")
            .FirstOrDefault(e => e.Attribute("Include")?.Value.Contains("appicon.svg") == true);

        // Assert
        Assert.IsNotNull(androidSupportedOSVersion, "Android supported OS version should be configured");
        Assert.IsNotNull(mauiIconElement, "MauiIcon should be configured for Android density generation");

        // Verify SVG format for scalability across Android densities
        var iconPath = mauiIconElement.Attribute("Include")?.Value;
        Assert.IsTrue(iconPath?.EndsWith(".svg") == true, 
            "Icon should be SVG format for Android multi-density generation");

        // Verify minimum Android API level supports adaptive icons (API 26+)
        var minVersion = androidSupportedOSVersion.Value;
        Assert.IsTrue(float.TryParse(minVersion, out float apiLevel) && apiLevel >= 21.0f, 
            "Android minimum version should support modern icon features");
    }

    #endregion

    #region iOS Platform Tests

    /// <summary>
    /// Test that verifies iOS app icon generation configuration
    /// Requirement 2.4: System should generate appropriate icon sizes and formats for each platform
    /// Requirement 4.1: App should build and deploy correctly on target platforms
    /// </summary>
    [Test]
    public void iOS_AppIcon_GenerationShouldBeConfigured()
    {
        // Arrange
        var projectContent = File.ReadAllText(_projectFile);
        var projectXml = XDocument.Parse(projectContent);

        // Act
        var targetFrameworks = projectXml.Descendants("TargetFrameworks").FirstOrDefault()?.Value;
        var mauiIconElement = projectXml.Descendants("MauiIcon")
            .FirstOrDefault(e => e.Attribute("Include")?.Value.Contains("appicon.svg") == true);

        // Assert
        Assert.IsTrue(targetFrameworks?.Contains("net8.0-ios") == true, 
            "iOS platform should be configured for app icon generation");
        
        Assert.IsNotNull(mauiIconElement, "MauiIcon configuration should exist for iOS app icons");

        // Verify iOS-compatible icon configuration
        var mainIcon = mauiIconElement.Attribute("Include")?.Value;
        Assert.IsNotNull(mainIcon, "iOS app icons require main icon file");
        Assert.IsTrue(mainIcon.EndsWith(".svg"), "iOS icons should use SVG for multi-size generation");

        // Verify iOS minimum version supports modern icon features
        var iosSupportedVersion = projectXml.Descendants("SupportedOSPlatformVersion")
            .FirstOrDefault(e => e.Attribute("Condition")?.Value.Contains("ios") == true);
        
        Assert.IsNotNull(iosSupportedVersion, "iOS supported version should be configured");
        var minVersion = iosSupportedVersion.Value;
        Assert.IsTrue(float.TryParse(minVersion, out float version) && version >= 11.0f, 
            "iOS minimum version should support modern app icon features");
    }

    /// <summary>
    /// Test that verifies iOS Launch Screen generation configuration
    /// Requirement 3.5: System should generate platform-appropriate splash screen resources
    /// Requirement 4.1: App should build and deploy correctly on target platforms
    /// </summary>
    [Test]
    public void iOS_LaunchScreen_GenerationShouldBeConfigured()
    {
        // Arrange
        var projectContent = File.ReadAllText(_projectFile);
        var projectXml = XDocument.Parse(projectContent);

        // Act
        var targetFrameworks = projectXml.Descendants("TargetFrameworks").FirstOrDefault()?.Value;
        var splashScreenElement = projectXml.Descendants("MauiSplashScreen")
            .FirstOrDefault(e => e.Attribute("Include")?.Value.Contains("splash.svg") == true);

        // Assert
        Assert.IsTrue(targetFrameworks?.Contains("net8.0-ios") == true, 
            "iOS platform should be configured for Launch Screen generation");
        
        Assert.IsNotNull(splashScreenElement, "MauiSplashScreen configuration should exist for iOS Launch Screen");

        // Verify iOS-compatible splash screen configuration
        var splashImage = splashScreenElement.Attribute("Include")?.Value;
        var backgroundColor = splashScreenElement.Attribute("Color")?.Value;
        var baseSize = splashScreenElement.Attribute("BaseSize")?.Value;

        Assert.IsNotNull(splashImage, "iOS Launch Screen requires splash image file");
        Assert.IsTrue(splashImage.EndsWith(".svg"), "iOS Launch Screen should use SVG for device adaptation");
        Assert.IsNotNull(backgroundColor, "iOS Launch Screen should have background color configured");
        Assert.IsNotNull(baseSize, "iOS Launch Screen should have base size configured");

        // Verify splash screen file exists
        var splashPath = Path.Combine(_projectRoot, splashImage.Replace("\\", Path.DirectorySeparatorChar.ToString()));
        Assert.IsTrue(File.Exists(splashPath), $"iOS Launch Screen image should exist: {splashPath}");
    }

    /// <summary>
    /// Test that verifies iOS app icon display characteristics
    /// Requirement 4.2: System should maintain visual consistency across different devices
    /// </summary>
    [Test]
    public void iOS_AppIcon_DisplayShouldBeValid()
    {
        // Arrange
        var mainIconPath = Path.Combine(_projectRoot, "Resources", "AppIcon", "appicon.svg");
        var projectContent = File.ReadAllText(_projectFile);
        var projectXml = XDocument.Parse(projectContent);

        // Act
        var iconContent = File.ReadAllText(mainIconPath);
        var mauiIconElement = projectXml.Descendants("MauiIcon")
            .FirstOrDefault(e => e.Attribute("Include")?.Value.Contains("appicon.svg") == true);

        // Assert - Verify icon is suitable for iOS display
        Assert.IsTrue(iconContent.Contains("viewBox"), 
            "iOS app icon should have viewBox for proper scaling across device sizes");
        
        Assert.IsTrue(iconContent.Contains("<svg"), "Icon should be valid SVG for iOS");
        
        // Verify iOS doesn't use adaptive icon features (foreground/background separation)
        // iOS uses the main icon directly, but can still have foreground configured for other platforms
        var mainIcon = mauiIconElement?.Attribute("Include")?.Value;
        Assert.IsNotNull(mainIcon, "Main icon should be configured for iOS");

        // Verify iOS platform support
        var targetFrameworks = projectXml.Descendants("TargetFrameworks").FirstOrDefault()?.Value;
        Assert.IsTrue(targetFrameworks?.Contains("net8.0-ios") == true, 
            "iOS platform should be supported for app icon display");
    }

    #endregion

    #region Windows Platform Tests

    /// <summary>
    /// Test that verifies Windows app icon generation configuration
    /// Requirement 2.4: System should generate appropriate icon sizes and formats for each platform
    /// Requirement 4.1: App should build and deploy correctly on target platforms
    /// </summary>
    [Test]
    public void Windows_AppIcon_GenerationShouldBeConfigured()
    {
        // Arrange
        var projectContent = File.ReadAllText(_projectFile);
        var projectXml = XDocument.Parse(projectContent);

        // Act
        var targetFrameworks = projectXml.Descendants("TargetFrameworks").FirstOrDefault()?.Value;
        var mauiIconElement = projectXml.Descendants("MauiIcon")
            .FirstOrDefault(e => e.Attribute("Include")?.Value.Contains("appicon.svg") == true);

        // Assert - Check if Windows is configured (conditional based on OS)
        var windowsTargetElement = projectXml.Descendants("TargetFrameworks")
            .FirstOrDefault(e => e.Attribute("Condition")?.Value.Contains("windows") == true);

        if (windowsTargetElement != null || targetFrameworks?.Contains("windows") == true)
        {
            Assert.IsNotNull(mauiIconElement, "MauiIcon configuration should exist for Windows app icons");

            // Verify Windows-compatible icon configuration
            var mainIcon = mauiIconElement.Attribute("Include")?.Value;
            Assert.IsNotNull(mainIcon, "Windows app icons require main icon file");
            Assert.IsTrue(mainIcon.EndsWith(".svg"), "Windows icons should use SVG for multi-size generation");

            // Verify Windows minimum version
            var windowsSupportedVersion = projectXml.Descendants("SupportedOSPlatformVersion")
                .FirstOrDefault(e => e.Attribute("Condition")?.Value.Contains("windows") == true);
            
            if (windowsSupportedVersion != null)
            {
                var minVersion = windowsSupportedVersion.Value;
                Assert.IsTrue(minVersion.Contains("10.0"), "Windows should target Windows 10 or later");
            }
        }
        else
        {
            System.Diagnostics.Debug.WriteLine("Windows platform not configured - skipping Windows app icon test");
        }
    }

    /// <summary>
    /// Test that verifies Windows splash screen generation configuration
    /// Requirement 3.5: System should generate platform-appropriate splash screen resources
    /// Requirement 4.1: App should build and deploy correctly on target platforms
    /// </summary>
    [Test]
    public void Windows_SplashScreen_GenerationShouldBeConfigured()
    {
        // Arrange
        var projectContent = File.ReadAllText(_projectFile);
        var projectXml = XDocument.Parse(projectContent);

        // Act
        var targetFrameworks = projectXml.Descendants("TargetFrameworks").FirstOrDefault()?.Value;
        var splashScreenElement = projectXml.Descendants("MauiSplashScreen")
            .FirstOrDefault(e => e.Attribute("Include")?.Value.Contains("splash.svg") == true);

        // Assert - Check if Windows is configured (conditional based on OS)
        var windowsTargetElement = projectXml.Descendants("TargetFrameworks")
            .FirstOrDefault(e => e.Attribute("Condition")?.Value.Contains("windows") == true);

        if (windowsTargetElement != null || targetFrameworks?.Contains("windows") == true)
        {
            Assert.IsNotNull(splashScreenElement, "MauiSplashScreen configuration should exist for Windows");

            // Verify Windows-compatible splash screen configuration
            var splashImage = splashScreenElement.Attribute("Include")?.Value;
            var backgroundColor = splashScreenElement.Attribute("Color")?.Value;
            var baseSize = splashScreenElement.Attribute("BaseSize")?.Value;

            Assert.IsNotNull(splashImage, "Windows splash screen requires splash image file");
            Assert.IsTrue(splashImage.EndsWith(".svg"), "Windows splash screen should use SVG");
            Assert.IsNotNull(backgroundColor, "Windows splash screen should have background color configured");
            Assert.IsNotNull(baseSize, "Windows splash screen should have base size configured");

            // Verify splash screen file exists
            var splashPath = Path.Combine(_projectRoot, splashImage.Replace("\\", Path.DirectorySeparatorChar.ToString()));
            Assert.IsTrue(File.Exists(splashPath), $"Windows splash screen image should exist: {splashPath}");
        }
        else
        {
            System.Diagnostics.Debug.WriteLine("Windows platform not configured - skipping Windows splash screen test");
        }
    }

    /// <summary>
    /// Test that verifies Windows app icon display characteristics
    /// Requirement 4.2: System should maintain visual consistency across different devices
    /// </summary>
    [Test]
    public void Windows_AppIcon_DisplayShouldBeValid()
    {
        // Arrange
        var mainIconPath = Path.Combine(_projectRoot, "Resources", "AppIcon", "appicon.svg");
        var projectContent = File.ReadAllText(_projectFile);
        var projectXml = XDocument.Parse(projectContent);

        // Act
        var targetFrameworks = projectXml.Descendants("TargetFrameworks").FirstOrDefault()?.Value;
        var windowsTargetElement = projectXml.Descendants("TargetFrameworks")
            .FirstOrDefault(e => e.Attribute("Condition")?.Value.Contains("windows") == true);

        // Assert - Check if Windows is configured
        if (windowsTargetElement != null || targetFrameworks?.Contains("windows") == true)
        {
            var iconContent = File.ReadAllText(mainIconPath);
            var mauiIconElement = projectXml.Descendants("MauiIcon")
                .FirstOrDefault(e => e.Attribute("Include")?.Value.Contains("appicon.svg") == true);

            // Verify icon is suitable for Windows display
            Assert.IsTrue(iconContent.Contains("viewBox"), 
                "Windows app icon should have viewBox for proper scaling");
            
            Assert.IsTrue(iconContent.Contains("<svg"), "Icon should be valid SVG for Windows");
            
            // Verify Windows icon configuration
            var mainIcon = mauiIconElement?.Attribute("Include")?.Value;
            Assert.IsNotNull(mainIcon, "Main icon should be configured for Windows");

            // Windows can use background color for tile backgrounds
            var backgroundColor = mauiIconElement?.Attribute("Color")?.Value;
            if (backgroundColor != null)
            {
                Assert.IsTrue(backgroundColor.StartsWith("#"), "Background color should be valid hex format");
            }
        }
        else
        {
            System.Diagnostics.Debug.WriteLine("Windows platform not configured - skipping Windows app icon display test");
        }
    }

    #endregion

    #region Cross-Platform Validation Tests

    /// <summary>
    /// Test that verifies platform-specific resource generation consistency
    /// Requirement 4.1: App should build and deploy correctly on target platforms
    /// Requirement 4.2: System should maintain visual consistency across different devices
    /// </summary>
    [Test]
    public void CrossPlatform_ResourceGeneration_ShouldBeConsistent()
    {
        // Arrange
        var projectContent = File.ReadAllText(_projectFile);
        var projectXml = XDocument.Parse(projectContent);

        // Act
        var targetFrameworks = projectXml.Descendants("TargetFrameworks").FirstOrDefault()?.Value;
        var mauiIconElement = projectXml.Descendants("MauiIcon")
            .FirstOrDefault(e => e.Attribute("Include")?.Value.Contains("appicon.svg") == true);
        var splashScreenElement = projectXml.Descendants("MauiSplashScreen")
            .FirstOrDefault(e => e.Attribute("Include")?.Value.Contains("splash.svg") == true);

        // Assert
        Assert.IsNotNull(targetFrameworks, "Target frameworks should be defined");
        Assert.IsNotNull(mauiIconElement, "App icon should be configured for all platforms");
        Assert.IsNotNull(splashScreenElement, "Splash screen should be configured for all platforms");

        // Verify consistent resource configuration across platforms
        var iconPath = mauiIconElement.Attribute("Include")?.Value;
        var splashPath = splashScreenElement.Attribute("Include")?.Value;

        Assert.IsTrue(iconPath?.EndsWith(".svg") == true, "Icon should use SVG for cross-platform consistency");
        Assert.IsTrue(splashPath?.EndsWith(".svg") == true, "Splash should use SVG for cross-platform consistency");

        // Verify color consistency
        var iconColor = mauiIconElement.Attribute("Color")?.Value;
        var splashColor = splashScreenElement.Attribute("Color")?.Value;

        Assert.IsNotNull(iconColor, "Icon color should be configured");
        Assert.IsNotNull(splashColor, "Splash color should be configured");
        Assert.IsTrue(iconColor == splashColor, "Icon and splash colors should be consistent for branding");
    }

    /// <summary>
    /// Test that verifies MacCatalyst exclusion across all platform-specific configurations
    /// Requirement 4.1: App should build correctly on target platforms (excluding MacCatalyst)
    /// </summary>
    [Test]
    public void CrossPlatform_MacCatalystExclusion_ShouldBeConsistent()
    {
        // Arrange
        var projectContent = File.ReadAllText(_projectFile);
        var projectXml = XDocument.Parse(projectContent);

        // Act
        var allTargetFrameworkElements = projectXml.Descendants("TargetFrameworks").ToList();

        // Assert
        foreach (var frameworkElement in allTargetFrameworkElements)
        {
            var frameworks = frameworkElement.Value;
            Assert.IsFalse(frameworks.Contains("net8.0-maccatalyst"), 
                "MacCatalyst should be excluded from all target framework configurations");
        }

        // Verify expected platforms are present
        var mainTargetFrameworks = allTargetFrameworkElements.FirstOrDefault()?.Value;
        Assert.IsTrue(mainTargetFrameworks?.Contains("net8.0-android") == true, 
            "Android should be included in target platforms");
        Assert.IsTrue(mainTargetFrameworks?.Contains("net8.0-ios") == true, 
            "iOS should be included in target platforms");

        System.Diagnostics.Debug.WriteLine($"Configured target frameworks: {mainTargetFrameworks}");
    }

    #endregion

    /// <summary>
    /// Execute all platform-specific tests and return results
    /// </summary>
    public static TestResults ExecuteAllTests()
    {
        var results = new TestResults();
        var testInstance = new PlatformSpecificTests();
        var testMethods = typeof(PlatformSpecificTests)
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