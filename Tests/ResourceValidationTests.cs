using System.Reflection;
using System.Xml.Linq;

namespace HelloWorld.Tests;

/// <summary>
/// Tests for error handling and resource validation
/// Requirements: 4.5 - Error handling for missing resources and build configuration errors
/// </summary>
public class ResourceValidationTests
{
    private readonly string _projectRoot;
    private readonly string _projectFile;

    public ResourceValidationTests()
    {
        _projectRoot = GetProjectRootDirectory();
        _projectFile = Path.Combine(_projectRoot, "HelloWorld.csproj");
    }

    /// <summary>
    /// Test that verifies error handling when app icon resource files are missing
    /// Requirement 4.5: System should provide clear error messages when branding resources are missing
    /// </summary>
    [Test]
    public void ResourceValidation_MissingAppIconFiles_ShouldProvideErrorMessage()
    {
        // Arrange
        var mainIconPath = Path.Combine(_projectRoot, "Resources", "AppIcon", "appicon.svg");
        var foregroundIconPath = Path.Combine(_projectRoot, "Resources", "AppIcon", "appiconfg.svg");

        // Act & Assert - Check if files exist and provide meaningful error messages
        try
        {
            if (!File.Exists(mainIconPath))
            {
                var errorMessage = ResourceErrorHandler.GetMissingResourceError("App Icon", mainIconPath);
                System.Diagnostics.Debug.WriteLine($"ERROR: {errorMessage}");
                Assert.IsTrue(false, errorMessage);
            }

            if (!File.Exists(foregroundIconPath))
            {
                var errorMessage = ResourceErrorHandler.GetMissingResourceError("App Icon Foreground", foregroundIconPath);
                System.Diagnostics.Debug.WriteLine($"ERROR: {errorMessage}");
                Assert.IsTrue(false, errorMessage);
            }

            // If we reach here, files exist
            Assert.IsTrue(true, "App icon resource files are present");
        }
        catch (Exception ex)
        {
            var detailedError = ResourceErrorHandler.GetDetailedResourceError("App Icon validation", ex);
            System.Diagnostics.Debug.WriteLine($"DETAILED ERROR: {detailedError}");
            throw new Exception(detailedError, ex);
        }
    }

    /// <summary>
    /// Test that verifies error handling when splash screen resource file is missing
    /// Requirement 4.5: System should provide clear error messages when branding resources are missing
    /// </summary>
    [Test]
    public void ResourceValidation_MissingSplashScreenFile_ShouldProvideErrorMessage()
    {
        // Arrange
        var splashScreenPath = Path.Combine(_projectRoot, "Resources", "Splash", "splash.svg");

        // Act & Assert
        try
        {
            if (!File.Exists(splashScreenPath))
            {
                var errorMessage = ResourceErrorHandler.GetMissingResourceError("Splash Screen", splashScreenPath);
                System.Diagnostics.Debug.WriteLine($"ERROR: {errorMessage}");
                Assert.IsTrue(false, errorMessage);
            }

            // If we reach here, file exists
            Assert.IsTrue(true, "Splash screen resource file is present");
        }
        catch (Exception ex)
        {
            var detailedError = ResourceErrorHandler.GetDetailedResourceError("Splash Screen validation", ex);
            System.Diagnostics.Debug.WriteLine($"DETAILED ERROR: {detailedError}");
            throw new Exception(detailedError, ex);
        }
    }

    /// <summary>
    /// Test that verifies error handling for invalid SVG content
    /// Requirement 4.5: System should provide clear error messages for invalid resource content
    /// </summary>
    [Test]
    public void ResourceValidation_InvalidSvgContent_ShouldProvideErrorMessage()
    {
        // Arrange
        var resourceFiles = new[]
        {
            Path.Combine(_projectRoot, "Resources", "AppIcon", "appicon.svg"),
            Path.Combine(_projectRoot, "Resources", "AppIcon", "appiconfg.svg"),
            Path.Combine(_projectRoot, "Resources", "Splash", "splash.svg")
        };

        // Act & Assert
        foreach (var filePath in resourceFiles)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    var content = File.ReadAllText(filePath);
                    var validationResult = ResourceErrorHandler.ValidateSvgContent(filePath, content);
                    
                    if (!validationResult.IsValid)
                    {
                        var errorMessage = ResourceErrorHandler.GetSvgValidationError(filePath, validationResult.Errors);
                        System.Diagnostics.Debug.WriteLine($"SVG VALIDATION ERROR: {errorMessage}");
                        Assert.IsTrue(false, errorMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                var detailedError = ResourceErrorHandler.GetDetailedResourceError($"SVG validation for {Path.GetFileName(filePath)}", ex);
                System.Diagnostics.Debug.WriteLine($"DETAILED ERROR: {detailedError}");
                throw new Exception(detailedError, ex);
            }
        }

        Assert.IsTrue(true, "All SVG files have valid content");
    }

    /// <summary>
    /// Test that verifies error handling for build configuration errors
    /// Requirement 4.5: System should provide clear error messages for build configuration issues
    /// </summary>
    [Test]
    public void ResourceValidation_BuildConfigurationErrors_ShouldProvideErrorMessage()
    {
        try
        {
            // Arrange & Act
            var projectContent = File.ReadAllText(_projectFile);
            var configValidation = ResourceErrorHandler.ValidateProjectConfiguration(projectContent);

            // Assert
            if (!configValidation.IsValid)
            {
                var errorMessage = ResourceErrorHandler.GetBuildConfigurationError(configValidation.Errors);
                System.Diagnostics.Debug.WriteLine($"BUILD CONFIGURATION ERROR: {errorMessage}");
                Assert.IsTrue(false, errorMessage);
            }

            Assert.IsTrue(true, "Project configuration is valid");
        }
        catch (Exception ex)
        {
            var detailedError = ResourceErrorHandler.GetDetailedResourceError("Build configuration validation", ex);
            System.Diagnostics.Debug.WriteLine($"DETAILED ERROR: {detailedError}");
            throw new Exception(detailedError, ex);
        }
    }

    /// <summary>
    /// Test that verifies error handling for missing project configuration elements
    /// Requirement 4.5: System should provide clear error messages for missing configuration
    /// </summary>
    [Test]
    public void ResourceValidation_MissingProjectConfiguration_ShouldProvideErrorMessage()
    {
        try
        {
            // Arrange & Act
            var projectContent = File.ReadAllText(_projectFile);
            var projectXml = XDocument.Parse(projectContent);

            // Check for required MauiIcon configuration
            var mauiIconElements = projectXml.Descendants("MauiIcon").ToList();
            if (mauiIconElements.Count == 0)
            {
                var errorMessage = ResourceErrorHandler.GetMissingConfigurationError("MauiIcon", "App icon configuration is missing from project file");
                System.Diagnostics.Debug.WriteLine($"CONFIGURATION ERROR: {errorMessage}");
                Assert.IsTrue(false, errorMessage);
            }

            // Check for required MauiSplashScreen configuration
            var splashScreenElements = projectXml.Descendants("MauiSplashScreen").ToList();
            if (splashScreenElements.Count == 0)
            {
                var errorMessage = ResourceErrorHandler.GetMissingConfigurationError("MauiSplashScreen", "Splash screen configuration is missing from project file");
                System.Diagnostics.Debug.WriteLine($"CONFIGURATION ERROR: {errorMessage}");
                Assert.IsTrue(false, errorMessage);
            }

            Assert.IsTrue(true, "All required project configurations are present");
        }
        catch (System.Xml.XmlException xmlEx)
        {
            var errorMessage = ResourceErrorHandler.GetXmlParsingError(_projectFile, xmlEx);
            System.Diagnostics.Debug.WriteLine($"XML PARSING ERROR: {errorMessage}");
            throw new Exception(errorMessage, xmlEx);
        }
        catch (Exception ex)
        {
            var detailedError = ResourceErrorHandler.GetDetailedResourceError("Project configuration validation", ex);
            System.Diagnostics.Debug.WriteLine($"DETAILED ERROR: {detailedError}");
            throw new Exception(detailedError, ex);
        }
    }

    /// <summary>
    /// Test that verifies error handling for resource directory structure issues
    /// Requirement 4.5: System should provide clear error messages for resource organization issues
    /// </summary>
    [Test]
    public void ResourceValidation_ResourceDirectoryStructure_ShouldProvideErrorMessage()
    {
        try
        {
            // Arrange
            var resourcesPath = Path.Combine(_projectRoot, "Resources");
            var appIconPath = Path.Combine(resourcesPath, "AppIcon");
            var splashPath = Path.Combine(resourcesPath, "Splash");

            // Act & Assert
            if (!Directory.Exists(resourcesPath))
            {
                var errorMessage = ResourceErrorHandler.GetMissingDirectoryError("Resources", resourcesPath);
                System.Diagnostics.Debug.WriteLine($"DIRECTORY ERROR: {errorMessage}");
                Assert.IsTrue(false, errorMessage);
            }

            if (!Directory.Exists(appIconPath))
            {
                var errorMessage = ResourceErrorHandler.GetMissingDirectoryError("AppIcon", appIconPath);
                System.Diagnostics.Debug.WriteLine($"DIRECTORY ERROR: {errorMessage}");
                Assert.IsTrue(false, errorMessage);
            }

            if (!Directory.Exists(splashPath))
            {
                var errorMessage = ResourceErrorHandler.GetMissingDirectoryError("Splash", splashPath);
                System.Diagnostics.Debug.WriteLine($"DIRECTORY ERROR: {errorMessage}");
                Assert.IsTrue(false, errorMessage);
            }

            Assert.IsTrue(true, "Resource directory structure is correct");
        }
        catch (Exception ex)
        {
            var detailedError = ResourceErrorHandler.GetDetailedResourceError("Resource directory validation", ex);
            System.Diagnostics.Debug.WriteLine($"DETAILED ERROR: {detailedError}");
            throw new Exception(detailedError, ex);
        }
    }

    /// <summary>
    /// Execute all resource validation tests and return results
    /// </summary>
    public static TestResults ExecuteAllTests()
    {
        var results = new TestResults();
        var testInstance = new ResourceValidationTests();
        var testMethods = typeof(ResourceValidationTests)
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