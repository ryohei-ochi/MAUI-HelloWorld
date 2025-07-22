using System.Reflection;
using System.Xml.Linq;

namespace HelloWorld.Tests;

/// <summary>
/// Tests to verify splash screen display and configuration
/// Requirements: 3.1, 3.2, 3.3, 3.4, 4.1, 4.4
/// </summary>
public class SplashScreenTests
{
    private readonly string _projectRoot;
    private readonly string _projectFile;

    public SplashScreenTests()
    {
        // Get the project root directory
        _projectRoot = GetProjectRootDirectory();
        _projectFile = Path.Combine(_projectRoot, "HelloWorld.csproj");
    }

    /// <summary>
    /// Test that verifies custom splash screen resource exists
    /// Requirement 3.2: Custom splash screen image should be used instead of default
    /// </summary>
    [Test]
    public void SplashScreen_ResourceFile_ShouldExist()
    {
        // Arrange
        var splashScreenPath = Path.Combine(_projectRoot, "Resources", "Splash", "splash.svg");

        // Act & Assert
        Assert.IsTrue(File.Exists(splashScreenPath), 
            $"Splash screen file should exist at: {splashScreenPath}");
    }

    /// <summary>
    /// Test that verifies splash screen file has valid SVG content
    /// Requirement 3.2: Custom splash screen should be properly formatted
    /// </summary>
    [Test]
    public void SplashScreen_SvgFile_ShouldHaveValidContent()
    {
        // Arrange
        var splashScreenPath = Path.Combine(_projectRoot, "Resources", "Splash", "splash.svg");

        // Act
        var splashScreenContent = File.ReadAllText(splashScreenPath);

        // Assert
        Assert.IsTrue(splashScreenContent.Contains("<?xml"), "Splash screen should be valid XML/SVG");
        Assert.IsTrue(splashScreenContent.Contains("<svg"), "Splash screen should contain SVG root element");
        Assert.IsTrue(splashScreenContent.Contains("viewBox"), "Splash screen should have viewBox for scalability");
        Assert.IsTrue(splashScreenContent.Contains("width") && splashScreenContent.Contains("height"), 
            "Splash screen should have width and height attributes");
    }

    /// <summary>
    /// Test that verifies project configuration includes custom splash screen settings
    /// Requirement 3.1: Custom splash screen should be displayed before main UI loads
    /// Requirement 3.3: Custom background color should be applied
    /// </summary>
    [Test]
    public void ProjectConfiguration_ShouldIncludeCustomSplashScreen()
    {
        // Arrange & Act
        var projectContent = File.ReadAllText(_projectFile);
        var projectXml = XDocument.Parse(projectContent);

        // Find MauiSplashScreen elements
        var splashScreenElements = projectXml.Descendants("MauiSplashScreen").ToList();

        // Assert
        Assert.IsTrue(splashScreenElements.Count > 0, "Project should contain MauiSplashScreen configuration");

        var customSplashElement = splashScreenElements.FirstOrDefault(e => 
            e.Attribute("Include")?.Value.Contains("splash.svg") == true);

        Assert.IsNotNull(customSplashElement, "Project should reference custom splash.svg");
        
        // Verify background color is configured (Requirement 3.3)
        var backgroundColor = customSplashElement.Attribute("Color")?.Value;
        Assert.IsNotNull(backgroundColor, "Custom splash screen should have background color configured");
        Assert.IsTrue(backgroundColor.StartsWith("#"), "Background color should be in hex format");

        // Verify base size is configured (Requirement 3.4)
        var baseSize = customSplashElement.Attribute("BaseSize")?.Value;
        Assert.IsNotNull(baseSize, "Custom splash screen should have base size configured");
        Assert.IsTrue(baseSize.Contains(","), "Base size should contain width and height separated by comma");
    }

    /// <summary>
    /// Test that verifies splash screen maintains proper aspect ratio and sizing
    /// Requirement 3.4: Splash screen should maintain proper aspect ratio and size across different screen sizes
    /// </summary>
    [Test]
    public void SplashScreen_AspectRatioAndSizing_ShouldBeValid()
    {
        // Arrange
        var projectContent = File.ReadAllText(_projectFile);
        var projectXml = XDocument.Parse(projectContent);
        var splashScreenPath = Path.Combine(_projectRoot, "Resources", "Splash", "splash.svg");

        // Act
        var splashScreenElement = projectXml.Descendants("MauiSplashScreen")
            .FirstOrDefault(e => e.Attribute("Include")?.Value.Contains("splash.svg") == true);
        var splashScreenContent = File.ReadAllText(splashScreenPath);

        // Assert
        Assert.IsNotNull(splashScreenElement, "Custom splash screen configuration should exist");

        // Verify BaseSize configuration
        var baseSize = splashScreenElement.Attribute("BaseSize")?.Value;
        Assert.IsNotNull(baseSize, "Splash screen should have BaseSize configured");
        
        var sizeParts = baseSize.Split(',');
        Assert.IsTrue(sizeParts.Length == 2, "BaseSize should have width and height");
        
        Assert.IsTrue(int.TryParse(sizeParts[0].Trim(), out int width), "Width should be a valid integer");
        Assert.IsTrue(int.TryParse(sizeParts[1].Trim(), out int height), "Height should be a valid integer");
        Assert.IsTrue(width > 0 && height > 0, "Width and height should be positive values");

        // Verify SVG viewBox for scalability
        Assert.IsTrue(splashScreenContent.Contains("viewBox"), "SVG should have viewBox for proper scaling");
    }

    /// <summary>
    /// Test that verifies splash screen supports cross-platform deployment
    /// Requirement 3.5: System should generate platform-appropriate splash screen resources
    /// Requirement 4.1: App should build and deploy correctly on target platforms
    /// </summary>
    [Test]
    public void SplashScreen_CrossPlatformSupport_ShouldBeConfigured()
    {
        // Arrange
        var projectContent = File.ReadAllText(_projectFile);
        var projectXml = XDocument.Parse(projectContent);

        // Act
        var targetFrameworksElement = projectXml.Descendants("TargetFrameworks").FirstOrDefault();
        var splashScreenElement = projectXml.Descendants("MauiSplashScreen")
            .FirstOrDefault(e => e.Attribute("Include")?.Value.Contains("splash.svg") == true);

        // Assert
        Assert.IsNotNull(targetFrameworksElement, "Target frameworks should be defined");
        Assert.IsNotNull(splashScreenElement, "Custom splash screen should be configured");

        var targetFrameworks = targetFrameworksElement.Value;
        var supportedPlatforms = new[] { "android", "ios" };

        foreach (var platform in supportedPlatforms)
        {
            Assert.IsTrue(targetFrameworks.Contains($"net8.0-{platform}"), 
                $"Should support {platform} platform for splash screen generation");
        }

        // Verify Windows support if available
        if (targetFrameworks.Contains("windows"))
        {
            Assert.IsTrue(targetFrameworks.Contains("net8.0-windows"), 
                "Should support Windows platform for splash screen generation");
        }

        // Verify splash screen configuration supports all platforms
        var splashInclude = splashScreenElement.Attribute("Include")?.Value;
        var splashColor = splashScreenElement.Attribute("Color")?.Value;
        var splashBaseSize = splashScreenElement.Attribute("BaseSize")?.Value;

        Assert.IsNotNull(splashInclude, "Splash screen Include should be configured for all platforms");
        Assert.IsNotNull(splashColor, "Splash screen Color should be configured for all platforms");
        Assert.IsNotNull(splashBaseSize, "Splash screen BaseSize should be configured for all platforms");
    }

    /// <summary>
    /// Test that verifies splash screen resource structure is properly organized
    /// Requirement 4.4: Splash screen should be displayed at appropriate timing with proper transition
    /// </summary>
    [Test]
    public void SplashScreen_ResourceStructure_ShouldSupportProperDisplay()
    {
        // Arrange
        var splashDirectory = Path.Combine(_projectRoot, "Resources", "Splash");

        // Act & Assert
        Assert.IsTrue(Directory.Exists(splashDirectory), 
            "Splash directory should exist in Resources folder");

        // Verify SVG format for scalability across platforms
        var svgFiles = Directory.GetFiles(splashDirectory, "*.svg");
        Assert.IsTrue(svgFiles.Length >= 1, 
            "Should have at least 1 SVG file for splash screen");

        // Verify recommended naming convention
        var hasSplashFile = svgFiles.Any(f => Path.GetFileName(f).Equals("splash.svg", StringComparison.OrdinalIgnoreCase));
        Assert.IsTrue(hasSplashFile, "Should have splash screen file named 'splash.svg'");

        // Verify file is not empty
        var splashScreenPath = Path.Combine(splashDirectory, "splash.svg");
        var fileInfo = new FileInfo(splashScreenPath);
        Assert.IsTrue(fileInfo.Length > 0, "Splash screen file should not be empty");
    }

    /// <summary>
    /// Test that verifies splash screen content includes branding elements
    /// Requirement 3.2: Custom splash screen should display branded content
    /// </summary>
    [Test]
    public void SplashScreen_BrandingContent_ShouldBePresent()
    {
        // Arrange
        var splashScreenPath = Path.Combine(_projectRoot, "Resources", "Splash", "splash.svg");

        // Act
        var splashScreenContent = File.ReadAllText(splashScreenPath);

        // Assert - Check for branding elements that should be present
        Assert.IsTrue(splashScreenContent.Contains("HelloWorld") || 
                     splashScreenContent.Contains("gradient") || 
                     splashScreenContent.Contains("rect") || 
                     splashScreenContent.Contains("circle") ||
                     splashScreenContent.Contains("text"), 
            "Splash screen should contain branding elements (text, shapes, gradients, etc.)");

        // Verify it's not just a basic template
        Assert.IsTrue(splashScreenContent.Length > 500, 
            "Splash screen should have substantial content, not just a basic template");
    }

    /// <summary>
    /// Test that verifies splash screen configuration excludes MacCatalyst
    /// Requirement 4.1: App should build correctly on target platforms (excluding MacCatalyst)
    /// </summary>
    [Test]
    public void SplashScreen_PlatformConfiguration_ShouldExcludeMacCatalyst()
    {
        // Arrange & Act
        var projectContent = File.ReadAllText(_projectFile);
        var projectXml = XDocument.Parse(projectContent);

        var targetFrameworksElement = projectXml.Descendants("TargetFrameworks").FirstOrDefault();

        // Assert
        Assert.IsNotNull(targetFrameworksElement, "Project should have TargetFrameworks defined");
        
        var targetFrameworks = targetFrameworksElement.Value;
        Assert.IsFalse(targetFrameworks.Contains("net8.0-maccatalyst"), 
            "Target frameworks should not include MacCatalyst for splash screen deployment");
        
        // Verify expected platforms are included
        Assert.IsTrue(targetFrameworks.Contains("net8.0-android"), 
            "Target frameworks should include Android for splash screen");
        Assert.IsTrue(targetFrameworks.Contains("net8.0-ios"), 
            "Target frameworks should include iOS for splash screen");
    }

    /// <summary>
    /// Execute all splash screen tests and return results
    /// </summary>
    public static TestResults ExecuteAllTests()
    {
        var results = new TestResults();
        var testInstance = new SplashScreenTests();
        var testMethods = typeof(SplashScreenTests)
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