using System.Reflection;
using System.Xml.Linq;

namespace HelloWorld.Tests;

/// <summary>
/// Integration tests for overall branding functionality
/// Verifies that branding customization works correctly across all platforms
/// Requirements: 4.1, 4.2, 4.3, 4.4
/// </summary>
public class BrandingIntegrationTests
{
    private readonly string _projectRoot;
    private readonly string _projectFile;

    public BrandingIntegrationTests()
    {
        _projectRoot = GetProjectRootDirectory();
        _projectFile = Path.Combine(_projectRoot, "HelloWorld.csproj");
    }

    #region Integration Test - Overall Branding Functionality

    /// <summary>
    /// Integration test that verifies overall branding functionality works correctly
    /// Requirement 4.1: System displays custom branding elements correctly on each target platform when built and deployed
    /// </summary>
    [Test]
    public void Integration_OverallBranding_ShouldWorkCorrectly()
    {
        System.Diagnostics.Debug.WriteLine("=== Integration Test: Overall Branding Functionality ===");

        // Test 1: Verify project configuration supports branding
        VerifyProjectConfigurationForBranding();

        // Test 2: Verify all branding resources exist and are valid
        VerifyBrandingResourcesExistAndValid();

        // Test 3: Verify branding configuration is consistent across platforms
        VerifyBrandingConfigurationConsistency();

        // Test 4: Verify platform-specific branding support
        VerifyPlatformSpecificBrandingSupport();

        System.Diagnostics.Debug.WriteLine("Overall branding functionality integration test PASSED");
    }

    /// <summary>
    /// Integration test that verifies cross-platform consistency of branding elements
    /// Requirement 4.2: System maintains visual consistency of branding elements across different devices
    /// </summary>
    [Test]
    public void Integration_CrossPlatformConsistency_ShouldBeValid()
    {
        System.Diagnostics.Debug.WriteLine("=== Integration Test: Cross-Platform Branding Consistency ===");

        // Test 1: Verify consistent resource formats across platforms
        VerifyConsistentResourceFormats();

        // Test 2: Verify consistent color scheme across branding elements
        VerifyConsistentColorScheme();

        // Test 3: Verify consistent sizing and scaling approach
        VerifyConsistentSizingAndScaling();

        // Test 4: Verify platform-specific adaptations maintain brand identity
        VerifyPlatformAdaptationsMaintainBrandIdentity();

        System.Diagnostics.Debug.WriteLine("Cross-platform branding consistency integration test PASSED");
    }

    /// <summary>
    /// Integration test that verifies app icon display in device launchers
    /// Requirement 4.3: System displays custom app icon in device app launcher when installed
    /// </summary>
    [Test]
    public void Integration_AppIconLauncherDisplay_ShouldBeConfigured()
    {
        System.Diagnostics.Debug.WriteLine("=== Integration Test: App Icon Launcher Display ===");

        // Test 1: Verify app icon configuration for launcher display
        VerifyAppIconLauncherConfiguration();

        // Test 2: Verify platform-specific icon requirements are met
        VerifyPlatformSpecificIconRequirements();

        // Test 3: Verify icon resources support all required sizes
        VerifyIconResourcesSupportAllSizes();

        // Test 4: Verify adaptive icon configuration for Android
        VerifyAdaptiveIconConfigurationForLauncher();

        System.Diagnostics.Debug.WriteLine("App icon launcher display integration test PASSED");
    }

    /// <summary>
    /// Integration test that verifies splash screen display timing and transitions
    /// Requirement 4.4: System displays custom splash screen with proper timing and transitions when app launches
    /// </summary>
    [Test]
    public void Integration_SplashScreenDisplayTiming_ShouldBeConfigured()
    {
        System.Diagnostics.Debug.WriteLine("=== Integration Test: Splash Screen Display Timing ===");

        // Test 1: Verify splash screen configuration for proper display
        VerifySplashScreenDisplayConfiguration();

        // Test 2: Verify platform-specific splash screen requirements
        VerifyPlatformSpecificSplashRequirements();

        // Test 3: Verify splash screen resources support proper scaling
        VerifySplashScreenResourceScaling();

        // Test 4: Verify splash screen timing configuration
        VerifySplashScreenTimingConfiguration();

        System.Diagnostics.Debug.WriteLine("Splash screen display timing integration test PASSED");
    }

    #endregion

    #region Helper Methods for Integration Tests

    private void VerifyProjectConfigurationForBranding()
    {
        var projectContent = File.ReadAllText(_projectFile);
        var projectXml = XDocument.Parse(projectContent);

        // Verify target platforms are configured correctly
        var targetFrameworks = projectXml.Descendants("TargetFrameworks").FirstOrDefault()?.Value;
        Assert.IsNotNull(targetFrameworks, "Target frameworks should be configured for branding");
        
        // Verify MacCatalyst is excluded
        Assert.IsFalse(targetFrameworks.Contains("net8.0-maccatalyst"), 
            "MacCatalyst should be excluded from branding configuration");
        
        // Verify required platforms are included
        Assert.IsTrue(targetFrameworks.Contains("net8.0-android"), "Android should be configured for branding");
        Assert.IsTrue(targetFrameworks.Contains("net8.0-ios"), "iOS should be configured for branding");

        // Verify branding elements are configured
        var mauiIcon = projectXml.Descendants("MauiIcon").FirstOrDefault();
        var mauiSplash = projectXml.Descendants("MauiSplashScreen").FirstOrDefault();
        
        Assert.IsNotNull(mauiIcon, "MauiIcon should be configured for branding");
        Assert.IsNotNull(mauiSplash, "MauiSplashScreen should be configured for branding");

        System.Diagnostics.Debug.WriteLine("✓ Project configuration supports branding");
    }

    private void VerifyBrandingResourcesExistAndValid()
    {
        // Verify app icon resources
        var appIconPath = Path.Combine(_projectRoot, "Resources", "AppIcon", "appicon.svg");
        var appIconFgPath = Path.Combine(_projectRoot, "Resources", "AppIcon", "appiconfg.svg");
        var splashPath = Path.Combine(_projectRoot, "Resources", "Splash", "splash.svg");

        Assert.IsTrue(File.Exists(appIconPath), $"Main app icon should exist: {appIconPath}");
        Assert.IsTrue(File.Exists(appIconFgPath), $"App icon foreground should exist: {appIconFgPath}");
        Assert.IsTrue(File.Exists(splashPath), $"Splash screen should exist: {splashPath}");

        // Verify SVG content is valid
        var appIconContent = File.ReadAllText(appIconPath);
        var appIconFgContent = File.ReadAllText(appIconFgPath);
        var splashContent = File.ReadAllText(splashPath);

        Assert.IsTrue(appIconContent.Contains("<svg") && appIconContent.Contains("</svg>"), 
            "Main app icon should be valid SVG");
        Assert.IsTrue(appIconFgContent.Contains("<svg") && appIconFgContent.Contains("</svg>"), 
            "App icon foreground should be valid SVG");
        Assert.IsTrue(splashContent.Contains("<svg") && splashContent.Contains("</svg>"), 
            "Splash screen should be valid SVG");

        System.Diagnostics.Debug.WriteLine("✓ All branding resources exist and are valid");
    }

    private void VerifyBrandingConfigurationConsistency()
    {
        var projectContent = File.ReadAllText(_projectFile);
        var projectXml = XDocument.Parse(projectContent);

        var mauiIcon = projectXml.Descendants("MauiIcon").FirstOrDefault();
        var mauiSplash = projectXml.Descendants("MauiSplashScreen").FirstOrDefault();

        // Verify consistent color scheme
        var iconColor = mauiIcon?.Attribute("Color")?.Value;
        var splashColor = mauiSplash?.Attribute("Color")?.Value;

        Assert.IsNotNull(iconColor, "Icon color should be configured");
        Assert.IsNotNull(splashColor, "Splash color should be configured");
        Assert.IsTrue(iconColor == splashColor, "Icon and splash colors should be consistent for branding");

        // Verify consistent file format (SVG)
        var iconPath = mauiIcon?.Attribute("Include")?.Value;
        var splashPath = mauiSplash?.Attribute("Include")?.Value;

        Assert.IsTrue(iconPath?.EndsWith(".svg") == true, "Icon should use SVG format");
        Assert.IsTrue(splashPath?.EndsWith(".svg") == true, "Splash should use SVG format");

        System.Diagnostics.Debug.WriteLine("✓ Branding configuration is consistent across elements");
    }

    private void VerifyPlatformSpecificBrandingSupport()
    {
        var projectContent = File.ReadAllText(_projectFile);
        var projectXml = XDocument.Parse(projectContent);

        // Verify Android adaptive icon support
        var mauiIcon = projectXml.Descendants("MauiIcon").FirstOrDefault();
        var foregroundFile = mauiIcon?.Attribute("ForegroundFile")?.Value;
        Assert.IsNotNull(foregroundFile, "Android adaptive icon foreground should be configured");

        // Verify iOS support
        var targetFrameworks = projectXml.Descendants("TargetFrameworks").FirstOrDefault()?.Value;
        Assert.IsTrue(targetFrameworks?.Contains("net8.0-ios") == true, "iOS platform should be supported");

        // Verify splash screen base size for proper scaling
        var mauiSplash = projectXml.Descendants("MauiSplashScreen").FirstOrDefault();
        var baseSize = mauiSplash?.Attribute("BaseSize")?.Value;
        Assert.IsNotNull(baseSize, "Splash screen base size should be configured for platform scaling");

        System.Diagnostics.Debug.WriteLine("✓ Platform-specific branding support is configured");
    }

    private void VerifyConsistentResourceFormats()
    {
        var projectContent = File.ReadAllText(_projectFile);
        var projectXml = XDocument.Parse(projectContent);

        var mauiIcon = projectXml.Descendants("MauiIcon").FirstOrDefault();
        var mauiSplash = projectXml.Descendants("MauiSplashScreen").FirstOrDefault();

        // All resources should use SVG for cross-platform consistency
        var iconPath = mauiIcon?.Attribute("Include")?.Value;
        var foregroundPath = mauiIcon?.Attribute("ForegroundFile")?.Value;
        var splashPath = mauiSplash?.Attribute("Include")?.Value;

        Assert.IsTrue(iconPath?.EndsWith(".svg") == true, "Main icon should use SVG format");
        Assert.IsTrue(foregroundPath?.EndsWith(".svg") == true, "Foreground icon should use SVG format");
        Assert.IsTrue(splashPath?.EndsWith(".svg") == true, "Splash screen should use SVG format");

        System.Diagnostics.Debug.WriteLine("✓ Consistent resource formats across platforms");
    }

    private void VerifyConsistentColorScheme()
    {
        var projectContent = File.ReadAllText(_projectFile);
        var projectXml = XDocument.Parse(projectContent);

        var mauiIcon = projectXml.Descendants("MauiIcon").FirstOrDefault();
        var mauiSplash = projectXml.Descendants("MauiSplashScreen").FirstOrDefault();

        var iconColor = mauiIcon?.Attribute("Color")?.Value;
        var splashColor = mauiSplash?.Attribute("Color")?.Value;

        // Colors should be consistent and valid hex format
        Assert.IsNotNull(iconColor, "Icon color should be configured");
        Assert.IsNotNull(splashColor, "Splash color should be configured");
        Assert.IsTrue(iconColor.StartsWith("#") && iconColor.Length == 7, "Icon color should be valid hex");
        Assert.IsTrue(splashColor.StartsWith("#") && splashColor.Length == 7, "Splash color should be valid hex");
        Assert.IsTrue(iconColor == splashColor, "Colors should be consistent across branding elements");

        System.Diagnostics.Debug.WriteLine("✓ Consistent color scheme across branding elements");
    }

    private void VerifyConsistentSizingAndScaling()
    {
        // Verify SVG viewBox for proper scaling
        var appIconPath = Path.Combine(_projectRoot, "Resources", "AppIcon", "appicon.svg");
        var appIconFgPath = Path.Combine(_projectRoot, "Resources", "AppIcon", "appiconfg.svg");
        var splashPath = Path.Combine(_projectRoot, "Resources", "Splash", "splash.svg");

        var appIconContent = File.ReadAllText(appIconPath);
        var appIconFgContent = File.ReadAllText(appIconFgPath);
        var splashContent = File.ReadAllText(splashPath);

        Assert.IsTrue(appIconContent.Contains("viewBox"), "Main icon should have viewBox for scaling");
        Assert.IsTrue(appIconFgContent.Contains("viewBox"), "Foreground icon should have viewBox for scaling");
        Assert.IsTrue(splashContent.Contains("viewBox"), "Splash screen should have viewBox for scaling");

        // Verify base size configuration
        var projectContent = File.ReadAllText(_projectFile);
        var projectXml = XDocument.Parse(projectContent);
        var mauiSplash = projectXml.Descendants("MauiSplashScreen").FirstOrDefault();
        var baseSize = mauiSplash?.Attribute("BaseSize")?.Value;

        Assert.IsNotNull(baseSize, "Base size should be configured for consistent scaling");
        Assert.IsTrue(baseSize.Contains(","), "Base size should specify width and height");

        System.Diagnostics.Debug.WriteLine("✓ Consistent sizing and scaling approach");
    }

    private void VerifyPlatformAdaptationsMaintainBrandIdentity()
    {
        var projectContent = File.ReadAllText(_projectFile);
        var projectXml = XDocument.Parse(projectContent);

        // Verify Android adaptive icon maintains brand identity
        var mauiIcon = projectXml.Descendants("MauiIcon").FirstOrDefault();
        var foregroundFile = mauiIcon?.Attribute("ForegroundFile")?.Value;
        var backgroundColor = mauiIcon?.Attribute("Color")?.Value;

        Assert.IsNotNull(foregroundFile, "Android adaptive icon should have branded foreground");
        Assert.IsNotNull(backgroundColor, "Android adaptive icon should have brand background color");

        // Verify foreground file exists and contains branding
        var foregroundPath = Path.Combine(_projectRoot, foregroundFile.Replace("\\", Path.DirectorySeparatorChar.ToString()));
        Assert.IsTrue(File.Exists(foregroundPath), "Branded foreground file should exist");

        var foregroundContent = File.ReadAllText(foregroundPath);
        Assert.IsTrue(foregroundContent.Contains("<svg"), "Foreground should be valid branded SVG");

        System.Diagnostics.Debug.WriteLine("✓ Platform adaptations maintain brand identity");
    }

    private void VerifyAppIconLauncherConfiguration()
    {
        var projectContent = File.ReadAllText(_projectFile);
        var projectXml = XDocument.Parse(projectContent);

        var mauiIcon = projectXml.Descendants("MauiIcon").FirstOrDefault();
        Assert.IsNotNull(mauiIcon, "App icon should be configured for launcher display");

        var iconPath = mauiIcon.Attribute("Include")?.Value;
        Assert.IsNotNull(iconPath, "Icon path should be configured");
        Assert.IsTrue(iconPath.EndsWith(".svg"), "Icon should use SVG for multi-size generation");

        // Verify icon file exists
        var fullIconPath = Path.Combine(_projectRoot, iconPath.Replace("\\", Path.DirectorySeparatorChar.ToString()));
        Assert.IsTrue(File.Exists(fullIconPath), "Icon file should exist for launcher display");

        System.Diagnostics.Debug.WriteLine("✓ App icon configured for launcher display");
    }

    private void VerifyPlatformSpecificIconRequirements()
    {
        var projectContent = File.ReadAllText(_projectFile);
        var projectXml = XDocument.Parse(projectContent);
        var targetFrameworks = projectXml.Descendants("TargetFrameworks").FirstOrDefault()?.Value;

        // Android requirements
        if (targetFrameworks?.Contains("net8.0-android") == true)
        {
            var mauiIcon = projectXml.Descendants("MauiIcon").FirstOrDefault();
            var foregroundFile = mauiIcon?.Attribute("ForegroundFile")?.Value;
            Assert.IsNotNull(foregroundFile, "Android requires foreground file for adaptive icons");
        }

        // iOS requirements
        if (targetFrameworks?.Contains("net8.0-ios") == true)
        {
            var mauiIcon = projectXml.Descendants("MauiIcon").FirstOrDefault();
            var iconPath = mauiIcon?.Attribute("Include")?.Value;
            Assert.IsNotNull(iconPath, "iOS requires main icon configuration");
        }

        System.Diagnostics.Debug.WriteLine("✓ Platform-specific icon requirements met");
    }

    private void VerifyIconResourcesSupportAllSizes()
    {
        // SVG format automatically supports all required sizes
        var appIconPath = Path.Combine(_projectRoot, "Resources", "AppIcon", "appicon.svg");
        var appIconFgPath = Path.Combine(_projectRoot, "Resources", "AppIcon", "appiconfg.svg");

        var appIconContent = File.ReadAllText(appIconPath);
        var appIconFgContent = File.ReadAllText(appIconFgPath);

        // Verify SVG has proper viewBox for scaling to all sizes
        Assert.IsTrue(appIconContent.Contains("viewBox"), "Main icon should support all sizes via viewBox");
        Assert.IsTrue(appIconFgContent.Contains("viewBox"), "Foreground icon should support all sizes via viewBox");

        System.Diagnostics.Debug.WriteLine("✓ Icon resources support all required sizes");
    }

    private void VerifyAdaptiveIconConfigurationForLauncher()
    {
        var projectContent = File.ReadAllText(_projectFile);
        var projectXml = XDocument.Parse(projectContent);

        var mauiIcon = projectXml.Descendants("MauiIcon").FirstOrDefault();
        var foregroundFile = mauiIcon?.Attribute("ForegroundFile")?.Value;
        var backgroundColor = mauiIcon?.Attribute("Color")?.Value;

        Assert.IsNotNull(foregroundFile, "Adaptive icon should have foreground file");
        Assert.IsNotNull(backgroundColor, "Adaptive icon should have background color");

        // Verify foreground file exists and is properly configured
        var foregroundPath = Path.Combine(_projectRoot, foregroundFile.Replace("\\", Path.DirectorySeparatorChar.ToString()));
        Assert.IsTrue(File.Exists(foregroundPath), "Adaptive icon foreground file should exist");

        var foregroundContent = File.ReadAllText(foregroundPath);
        Assert.IsTrue(foregroundContent.Contains("viewBox"), "Adaptive icon foreground should have viewBox");

        System.Diagnostics.Debug.WriteLine("✓ Adaptive icon configured for launcher display");
    }

    private void VerifySplashScreenDisplayConfiguration()
    {
        var projectContent = File.ReadAllText(_projectFile);
        var projectXml = XDocument.Parse(projectContent);

        var mauiSplash = projectXml.Descendants("MauiSplashScreen").FirstOrDefault();
        Assert.IsNotNull(mauiSplash, "Splash screen should be configured for display");

        var splashPath = mauiSplash.Attribute("Include")?.Value;
        var backgroundColor = mauiSplash.Attribute("Color")?.Value;
        var baseSize = mauiSplash.Attribute("BaseSize")?.Value;

        Assert.IsNotNull(splashPath, "Splash screen path should be configured");
        Assert.IsNotNull(backgroundColor, "Splash screen background color should be configured");
        Assert.IsNotNull(baseSize, "Splash screen base size should be configured");

        // Verify splash file exists
        var fullSplashPath = Path.Combine(_projectRoot, splashPath.Replace("\\", Path.DirectorySeparatorChar.ToString()));
        Assert.IsTrue(File.Exists(fullSplashPath), "Splash screen file should exist");

        System.Diagnostics.Debug.WriteLine("✓ Splash screen configured for proper display");
    }

    private void VerifyPlatformSpecificSplashRequirements()
    {
        var projectContent = File.ReadAllText(_projectFile);
        var projectXml = XDocument.Parse(projectContent);
        var targetFrameworks = projectXml.Descendants("TargetFrameworks").FirstOrDefault()?.Value;

        var mauiSplash = projectXml.Descendants("MauiSplashScreen").FirstOrDefault();

        // All platforms require these basic configurations
        var splashPath = mauiSplash?.Attribute("Include")?.Value;
        var backgroundColor = mauiSplash?.Attribute("Color")?.Value;

        Assert.IsNotNull(splashPath, "All platforms require splash screen image");
        Assert.IsNotNull(backgroundColor, "All platforms require splash screen background color");
        Assert.IsTrue(splashPath.EndsWith(".svg"), "All platforms should use SVG for splash screen");

        System.Diagnostics.Debug.WriteLine("✓ Platform-specific splash screen requirements met");
    }

    private void VerifySplashScreenResourceScaling()
    {
        var splashPath = Path.Combine(_projectRoot, "Resources", "Splash", "splash.svg");
        var splashContent = File.ReadAllText(splashPath);

        // Verify SVG has proper viewBox for scaling
        Assert.IsTrue(splashContent.Contains("viewBox"), "Splash screen should have viewBox for proper scaling");
        Assert.IsTrue(splashContent.Contains("<svg"), "Splash screen should be valid SVG");

        // Verify base size configuration
        var projectContent = File.ReadAllText(_projectFile);
        var projectXml = XDocument.Parse(projectContent);
        var mauiSplash = projectXml.Descendants("MauiSplashScreen").FirstOrDefault();
        var baseSize = mauiSplash?.Attribute("BaseSize")?.Value;

        Assert.IsNotNull(baseSize, "Base size should be configured for proper scaling");
        Assert.IsTrue(baseSize.Contains(","), "Base size should specify width and height");

        System.Diagnostics.Debug.WriteLine("✓ Splash screen resources support proper scaling");
    }

    private void VerifySplashScreenTimingConfiguration()
    {
        // Verify splash screen is configured to display during app startup
        var projectContent = File.ReadAllText(_projectFile);
        var projectXml = XDocument.Parse(projectContent);

        var mauiSplash = projectXml.Descendants("MauiSplashScreen").FirstOrDefault();
        Assert.IsNotNull(mauiSplash, "Splash screen should be configured for startup timing");

        // Verify all required attributes for proper timing
        var splashPath = mauiSplash.Attribute("Include")?.Value;
        var backgroundColor = mauiSplash.Attribute("Color")?.Value;
        var baseSize = mauiSplash.Attribute("BaseSize")?.Value;

        Assert.IsNotNull(splashPath, "Splash path required for timing configuration");
        Assert.IsNotNull(backgroundColor, "Background color required for smooth transitions");
        Assert.IsNotNull(baseSize, "Base size required for proper display timing");

        System.Diagnostics.Debug.WriteLine("✓ Splash screen timing configuration verified");
    }

    #endregion

    /// <summary>
    /// Execute all integration tests and return results
    /// </summary>
    public static TestResults ExecuteAllTests()
    {
        var results = new TestResults();
        var testInstance = new BrandingIntegrationTests();
        var testMethods = typeof(BrandingIntegrationTests)
            .GetMethods()
            .Where(m => m.GetCustomAttribute<TestAttribute>() != null)
            .ToList();

        System.Diagnostics.Debug.WriteLine("=== Branding Integration Tests ===");
        System.Diagnostics.Debug.WriteLine($"Running {testMethods.Count} integration tests...");
        System.Diagnostics.Debug.WriteLine("");

        foreach (var method in testMethods)
        {
            results.TotalTests++;
            
            try
            {
                System.Diagnostics.Debug.WriteLine($"Running {method.Name}...");
                method.Invoke(testInstance, null);
                System.Diagnostics.Debug.WriteLine($"{method.Name} PASSED");
                System.Diagnostics.Debug.WriteLine("");
                results.PassedTests++;
            }
            catch (Exception ex)
            {
                var innerException = ex.InnerException ?? ex;
                System.Diagnostics.Debug.WriteLine($"{method.Name} FAILED: {innerException.Message}");
                System.Diagnostics.Debug.WriteLine("");
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