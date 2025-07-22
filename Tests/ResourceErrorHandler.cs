using System.Xml.Linq;

namespace HelloWorld.Tests;

/// <summary>
/// Utility class for handling resource-related errors and providing detailed error messages
/// Requirements: 4.5 - Error handling for missing resources and build configuration errors
/// </summary>
public static class ResourceErrorHandler
{
    /// <summary>
    /// Generates a detailed error message for missing resource files
    /// </summary>
    public static string GetMissingResourceError(string resourceType, string expectedPath)
    {
        return $"RESOURCE ERROR: {resourceType} file is missing.\n" +
               $"Expected location: {expectedPath}\n" +
               $"Solution: Ensure the {resourceType.ToLower()} file exists at the specified path.\n" +
               $"For SVG files, make sure they are properly formatted and contain valid XML content.\n" +
               $"Check that the file has not been accidentally deleted or moved.";
    }

    /// <summary>
    /// Generates a detailed error message for missing directories
    /// </summary>
    public static string GetMissingDirectoryError(string directoryType, string expectedPath)
    {
        return $"DIRECTORY ERROR: {directoryType} directory is missing.\n" +
               $"Expected location: {expectedPath}\n" +
               $"Solution: Create the {directoryType} directory and ensure it contains the required resource files.\n" +
               $"The directory structure should follow .NET MAUI conventions:\n" +
               $"- Resources/AppIcon/ for app icon files\n" +
               $"- Resources/Splash/ for splash screen files";
    }

    /// <summary>
    /// Validates SVG content and returns validation result
    /// </summary>
    public static SvgValidationResult ValidateSvgContent(string filePath, string content)
    {
        var result = new SvgValidationResult { IsValid = true, Errors = new List<string>() };

        try
        {
            // Check if content is not empty
            if (string.IsNullOrWhiteSpace(content))
            {
                result.IsValid = false;
                result.Errors.Add("SVG file is empty or contains only whitespace");
                return result;
            }

            // Check for basic XML structure
            if (!content.TrimStart().StartsWith("<?xml"))
            {
                result.IsValid = false;
                result.Errors.Add("SVG file should start with XML declaration (<?xml version=\"1.0\"...)");
            }

            // Check for SVG root element
            if (!content.Contains("<svg"))
            {
                result.IsValid = false;
                result.Errors.Add("SVG file must contain <svg> root element");
            }

            // Check for viewBox attribute (important for scalability)
            if (!content.Contains("viewBox"))
            {
                result.IsValid = false;
                result.Errors.Add("SVG file should contain viewBox attribute for proper scaling across different screen sizes");
            }

            // Check for width and height attributes
            if (!content.Contains("width") || !content.Contains("height"))
            {
                result.IsValid = false;
                result.Errors.Add("SVG file should contain width and height attributes");
            }

            // Try to parse as XML to check for syntax errors
            try
            {
                XDocument.Parse(content);
            }
            catch (System.Xml.XmlException xmlEx)
            {
                result.IsValid = false;
                result.Errors.Add($"SVG file contains invalid XML syntax: {xmlEx.Message}");
            }

            // Check file size (should not be too large for mobile apps)
            if (content.Length > 100000) // 100KB limit
            {
                result.IsValid = false;
                result.Errors.Add("SVG file is too large (>100KB). Consider optimizing the SVG content for mobile applications");
            }

            // Check for minimum content (should not be just a basic template)
            if (content.Length < 200)
            {
                result.IsValid = false;
                result.Errors.Add("SVG file appears to be too simple or incomplete. Ensure it contains proper branding content");
            }
        }
        catch (Exception ex)
        {
            result.IsValid = false;
            result.Errors.Add($"Unexpected error during SVG validation: {ex.Message}");
        }

        return result;
    }

    /// <summary>
    /// Generates error message for SVG validation failures
    /// </summary>
    public static string GetSvgValidationError(string filePath, List<string> errors)
    {
        var errorMessage = $"SVG VALIDATION ERROR: {Path.GetFileName(filePath)} contains validation errors.\n" +
                          $"File location: {filePath}\n" +
                          $"Validation errors:\n";

        for (int i = 0; i < errors.Count; i++)
        {
            errorMessage += $"  {i + 1}. {errors[i]}\n";
        }

        errorMessage += "\nSolution: Fix the SVG file to address the validation errors above.\n" +
                       "Ensure the SVG follows proper XML syntax and contains appropriate branding content.\n" +
                       "For app icons, include viewBox for scalability and ensure the design works at different sizes.\n" +
                       "For splash screens, ensure the design is appropriate for various screen orientations.";

        return errorMessage;
    }

    /// <summary>
    /// Validates project configuration for branding resources
    /// </summary>
    public static ProjectConfigValidationResult ValidateProjectConfiguration(string projectContent)
    {
        var result = new ProjectConfigValidationResult { IsValid = true, Errors = new List<string>() };

        try
        {
            var projectXml = XDocument.Parse(projectContent);

            // Check for MauiIcon configuration
            var mauiIconElements = projectXml.Descendants("MauiIcon").ToList();
            if (mauiIconElements.Count == 0)
            {
                result.IsValid = false;
                result.Errors.Add("MauiIcon configuration is missing from project file");
            }
            else
            {
                foreach (var iconElement in mauiIconElements)
                {
                    var includeAttr = iconElement.Attribute("Include")?.Value;
                    if (string.IsNullOrEmpty(includeAttr))
                    {
                        result.IsValid = false;
                        result.Errors.Add("MauiIcon element is missing Include attribute");
                    }
                    else if (!includeAttr.EndsWith(".svg", StringComparison.OrdinalIgnoreCase))
                    {
                        result.IsValid = false;
                        result.Errors.Add($"MauiIcon Include should reference an SVG file, found: {includeAttr}");
                    }

                    var colorAttr = iconElement.Attribute("Color")?.Value;
                    if (string.IsNullOrEmpty(colorAttr))
                    {
                        result.IsValid = false;
                        result.Errors.Add("MauiIcon element is missing Color attribute for background color");
                    }
                    else if (!colorAttr.StartsWith("#"))
                    {
                        result.IsValid = false;
                        result.Errors.Add($"MauiIcon Color should be in hex format (#RRGGBB), found: {colorAttr}");
                    }
                }
            }

            // Check for MauiSplashScreen configuration
            var splashElements = projectXml.Descendants("MauiSplashScreen").ToList();
            if (splashElements.Count == 0)
            {
                result.IsValid = false;
                result.Errors.Add("MauiSplashScreen configuration is missing from project file");
            }
            else
            {
                foreach (var splashElement in splashElements)
                {
                    var includeAttr = splashElement.Attribute("Include")?.Value;
                    if (string.IsNullOrEmpty(includeAttr))
                    {
                        result.IsValid = false;
                        result.Errors.Add("MauiSplashScreen element is missing Include attribute");
                    }
                    else if (!includeAttr.EndsWith(".svg", StringComparison.OrdinalIgnoreCase))
                    {
                        result.IsValid = false;
                        result.Errors.Add($"MauiSplashScreen Include should reference an SVG file, found: {includeAttr}");
                    }

                    var colorAttr = splashElement.Attribute("Color")?.Value;
                    if (string.IsNullOrEmpty(colorAttr))
                    {
                        result.IsValid = false;
                        result.Errors.Add("MauiSplashScreen element is missing Color attribute for background color");
                    }

                    var baseSizeAttr = splashElement.Attribute("BaseSize")?.Value;
                    if (string.IsNullOrEmpty(baseSizeAttr))
                    {
                        result.IsValid = false;
                        result.Errors.Add("MauiSplashScreen element is missing BaseSize attribute");
                    }
                    else if (!baseSizeAttr.Contains(","))
                    {
                        result.IsValid = false;
                        result.Errors.Add($"MauiSplashScreen BaseSize should contain width,height format, found: {baseSizeAttr}");
                    }
                }
            }

            // Check for target frameworks (should exclude MacCatalyst)
            var targetFrameworksElement = projectXml.Descendants("TargetFrameworks").FirstOrDefault();
            if (targetFrameworksElement != null)
            {
                var targetFrameworks = targetFrameworksElement.Value;
                if (targetFrameworks.Contains("net8.0-maccatalyst"))
                {
                    result.IsValid = false;
                    result.Errors.Add("Target frameworks should not include MacCatalyst (net8.0-maccatalyst) as per project requirements");
                }

                var expectedPlatforms = new[] { "net8.0-android", "net8.0-ios" };
                foreach (var platform in expectedPlatforms)
                {
                    if (!targetFrameworks.Contains(platform))
                    {
                        result.IsValid = false;
                        result.Errors.Add($"Target frameworks should include {platform}");
                    }
                }
            }
        }
        catch (System.Xml.XmlException xmlEx)
        {
            result.IsValid = false;
            result.Errors.Add($"Project file contains invalid XML: {xmlEx.Message}");
        }
        catch (Exception ex)
        {
            result.IsValid = false;
            result.Errors.Add($"Unexpected error during project configuration validation: {ex.Message}");
        }

        return result;
    }

    /// <summary>
    /// Generates error message for build configuration failures
    /// </summary>
    public static string GetBuildConfigurationError(List<string> errors)
    {
        var errorMessage = "BUILD CONFIGURATION ERROR: Project configuration contains errors that may cause build failures.\n" +
                          "Configuration errors:\n";

        for (int i = 0; i < errors.Count; i++)
        {
            errorMessage += $"  {i + 1}. {errors[i]}\n";
        }

        errorMessage += "\nSolution: Update the project file (.csproj) to fix the configuration errors above.\n" +
                       "Ensure that:\n" +
                       "- MauiIcon elements have Include, Color, and optionally ForegroundFile attributes\n" +
                       "- MauiSplashScreen elements have Include, Color, and BaseSize attributes\n" +
                       "- All referenced resource files exist in the specified locations\n" +
                       "- Target frameworks exclude MacCatalyst and include required platforms\n" +
                       "- Color values are in proper hex format (#RRGGBB)";

        return errorMessage;
    }

    /// <summary>
    /// Generates error message for missing configuration elements
    /// </summary>
    public static string GetMissingConfigurationError(string configType, string description)
    {
        return $"CONFIGURATION ERROR: {configType} configuration is missing.\n" +
               $"Description: {description}\n" +
               $"Solution: Add the required {configType} configuration to your project file (.csproj).\n" +
               $"Example configuration:\n" +
               GetExampleConfiguration(configType);
    }

    /// <summary>
    /// Generates error message for XML parsing failures
    /// </summary>
    public static string GetXmlParsingError(string filePath, System.Xml.XmlException xmlException)
    {
        return $"XML PARSING ERROR: Failed to parse project file.\n" +
               $"File: {filePath}\n" +
               $"Error: {xmlException.Message}\n" +
               $"Line: {xmlException.LineNumber}, Position: {xmlException.LinePosition}\n" +
               $"Solution: Fix the XML syntax error in the project file.\n" +
               $"Common issues include:\n" +
               $"- Unclosed XML tags\n" +
               $"- Invalid characters in attribute values\n" +
               $"- Missing quotation marks around attribute values\n" +
               $"- Malformed XML structure";
    }

    /// <summary>
    /// Generates detailed error message with context
    /// </summary>
    public static string GetDetailedResourceError(string operation, Exception exception)
    {
        return $"DETAILED ERROR: Failed during {operation}.\n" +
               $"Error Type: {exception.GetType().Name}\n" +
               $"Error Message: {exception.Message}\n" +
               $"Stack Trace: {exception.StackTrace}\n" +
               $"Solution: Review the error details above and fix the underlying issue.\n" +
               $"If this is a file access error, check file permissions and paths.\n" +
               $"If this is a parsing error, verify file content and format.\n" +
               $"If this persists, check that all required dependencies are installed.";
    }

    /// <summary>
    /// Provides example configuration for different types
    /// </summary>
    private static string GetExampleConfiguration(string configType)
    {
        return configType switch
        {
            "MauiIcon" => """
                <MauiIcon Include="Resources\AppIcon\appicon.svg" 
                          ForegroundFile="Resources\AppIcon\appiconfg.svg" 
                          Color="#512BD4" />
                """,
            "MauiSplashScreen" => """
                <MauiSplashScreen Include="Resources\Splash\splash.svg" 
                                  Color="#512BD4" 
                                  BaseSize="128,128" />
                """,
            _ => "Refer to .NET MAUI documentation for proper configuration syntax."
        };
    }

    /// <summary>
    /// Result class for SVG validation
    /// </summary>
    public class SvgValidationResult
    {
        public bool IsValid { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }

    /// <summary>
    /// Result class for project configuration validation
    /// </summary>
    public class ProjectConfigValidationResult
    {
        public bool IsValid { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }
}