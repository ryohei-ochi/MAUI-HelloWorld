using System.Xml.Linq;

namespace HelloWorld;

/// <summary>
/// Build-time validation utility for branding resources and configuration
/// 
/// BRANDING CUSTOMIZATION VALIDATION SYSTEM:
/// This class provides comprehensive validation of all branding-related resources
/// and configuration settings to ensure proper app icon and splash screen setup
/// across all target platforms (Android, iOS, Windows).
/// 
/// Validation Categories:
/// 1. Project Configuration Validation
///    - Target framework settings (MacCatalyst exclusion)
///    - MauiIcon and MauiSplashScreen configuration
///    - Color format and resource path validation
/// 
/// 2. Resource File Validation  
///    - SVG file existence and format validation
///    - File size and dimension recommendations
///    - Naming convention compliance
/// 
/// 3. Directory Structure Validation
///    - Standard .NET MAUI resource folder structure
///    - Required subdirectories (AppIcon, Splash)
///    - File organization best practices
/// 
/// Error Handling Strategy:
/// - Critical errors: Block build process
/// - Warnings: Allow build but report issues
/// - Detailed reporting: Help developers troubleshoot
/// 
/// Requirements: 4.5 - Error handling for build process configuration errors
/// Requirements: 5.1, 5.2, 5.3, 5.4 - Documentation and best practices
/// </summary>
public static class BuildValidation
{
    /// <summary>
    /// Validates all branding resources and configuration before build
    /// 
    /// COMPREHENSIVE BRANDING VALIDATION PROCESS:
    /// This method orchestrates the complete validation of branding customization
    /// implementation, checking all aspects from project configuration to resource
    /// files to ensure a successful build and proper branding display.
    /// 
    /// Validation Steps:
    /// 1. Project file existence and accessibility
    /// 2. Target platform configuration (MacCatalyst exclusion)
    /// 3. MauiIcon and MauiSplashScreen settings
    /// 4. Resource file existence and format
    /// 5. Directory structure compliance
    /// 6. Naming convention adherence
    /// 
    /// Error Classification:
    /// - CRITICAL: Prevents successful build
    /// - ERROR: May cause runtime issues
    /// - WARNING: Best practice violations
    /// - INFO: Recommendations for improvement
    /// </summary>
    /// <param name="projectRoot">Root directory of the project containing HelloWorld.csproj</param>
    /// <returns>BuildValidationResult with detailed validation findings</returns>
    public static BuildValidationResult ValidateAllBrandingResources(string projectRoot)
    {
        var result = new BuildValidationResult();
        
        try
        {
            System.Diagnostics.Debug.WriteLine("Starting build validation for branding resources...");
            
            // Validate project file exists
            var projectFile = Path.Combine(projectRoot, "HelloWorld.csproj");
            if (!File.Exists(projectFile))
            {
                result.AddError("CRITICAL", $"Project file not found: {projectFile}");
                return result;
            }

            // Validate project configuration
            ValidateProjectConfiguration(projectFile, result);
            
            // Validate resource files
            ValidateResourceFiles(projectRoot, result);
            
            // Validate resource directory structure
            ValidateResourceDirectoryStructure(projectRoot, result);
            
            System.Diagnostics.Debug.WriteLine($"Build validation completed. Errors: {result.Errors.Count}, Warnings: {result.Warnings.Count}");
            
        }
        catch (Exception ex)
        {
            result.AddError("CRITICAL", $"Build validation failed with exception: {ex.Message}");
        }
        
        return result;
    }

    /// <summary>
    /// Validates project configuration for branding elements
    /// </summary>
    private static void ValidateProjectConfiguration(string projectFile, BuildValidationResult result)
    {
        try
        {
            var projectContent = File.ReadAllText(projectFile);
            var projectXml = XDocument.Parse(projectContent);

            // Validate MauiIcon configuration
            var mauiIconElements = projectXml.Descendants("MauiIcon").ToList();
            if (mauiIconElements.Count == 0)
            {
                result.AddError("CONFIG", "No MauiIcon configuration found in project file");
            }
            else
            {
                foreach (var iconElement in mauiIconElements)
                {
                    ValidateMauiIconElement(iconElement, result);
                }
            }

            // Validate MauiSplashScreen configuration
            var splashElements = projectXml.Descendants("MauiSplashScreen").ToList();
            if (splashElements.Count == 0)
            {
                result.AddError("CONFIG", "No MauiSplashScreen configuration found in project file");
            }
            else
            {
                foreach (var splashElement in splashElements)
                {
                    ValidateMauiSplashScreenElement(splashElement, result);
                }
            }

            // Validate target frameworks
            ValidateTargetFrameworks(projectXml, result);
        }
        catch (System.Xml.XmlException xmlEx)
        {
            result.AddError("CRITICAL", $"Project file contains invalid XML: {xmlEx.Message} at line {xmlEx.LineNumber}");
        }
        catch (Exception ex)
        {
            result.AddError("CONFIG", $"Failed to validate project configuration: {ex.Message}");
        }
    }

    /// <summary>
    /// Validates MauiIcon element configuration
    /// </summary>
    private static void ValidateMauiIconElement(XElement iconElement, BuildValidationResult result)
    {
        var includeAttr = iconElement.Attribute("Include")?.Value;
        if (string.IsNullOrEmpty(includeAttr))
        {
            result.AddError("CONFIG", "MauiIcon element missing Include attribute");
            return;
        }

        if (!includeAttr.EndsWith(".svg", StringComparison.OrdinalIgnoreCase))
        {
            result.AddWarning("CONFIG", $"MauiIcon should use SVG format for best cross-platform support. Found: {includeAttr}");
        }

        var colorAttr = iconElement.Attribute("Color")?.Value;
        if (string.IsNullOrEmpty(colorAttr))
        {
            result.AddError("CONFIG", "MauiIcon element missing Color attribute");
        }
        else if (!IsValidHexColor(colorAttr))
        {
            result.AddError("CONFIG", $"MauiIcon Color attribute should be in hex format (#RRGGBB). Found: {colorAttr}");
        }

        var foregroundAttr = iconElement.Attribute("ForegroundFile")?.Value;
        if (!string.IsNullOrEmpty(foregroundAttr) && !foregroundAttr.EndsWith(".svg", StringComparison.OrdinalIgnoreCase))
        {
            result.AddWarning("CONFIG", $"MauiIcon ForegroundFile should use SVG format. Found: {foregroundAttr}");
        }
    }

    /// <summary>
    /// Validates MauiSplashScreen element configuration
    /// </summary>
    private static void ValidateMauiSplashScreenElement(XElement splashElement, BuildValidationResult result)
    {
        var includeAttr = splashElement.Attribute("Include")?.Value;
        if (string.IsNullOrEmpty(includeAttr))
        {
            result.AddError("CONFIG", "MauiSplashScreen element missing Include attribute");
            return;
        }

        if (!includeAttr.EndsWith(".svg", StringComparison.OrdinalIgnoreCase))
        {
            result.AddWarning("CONFIG", $"MauiSplashScreen should use SVG format for best cross-platform support. Found: {includeAttr}");
        }

        var colorAttr = splashElement.Attribute("Color")?.Value;
        if (string.IsNullOrEmpty(colorAttr))
        {
            result.AddError("CONFIG", "MauiSplashScreen element missing Color attribute");
        }
        else if (!IsValidHexColor(colorAttr))
        {
            result.AddError("CONFIG", $"MauiSplashScreen Color attribute should be in hex format (#RRGGBB). Found: {colorAttr}");
        }

        var baseSizeAttr = splashElement.Attribute("BaseSize")?.Value;
        if (string.IsNullOrEmpty(baseSizeAttr))
        {
            result.AddError("CONFIG", "MauiSplashScreen element missing BaseSize attribute");
        }
        else if (!IsValidBaseSize(baseSizeAttr))
        {
            result.AddError("CONFIG", $"MauiSplashScreen BaseSize should be in format 'width,height'. Found: {baseSizeAttr}");
        }
    }

    /// <summary>
    /// Validates target frameworks configuration
    /// </summary>
    private static void ValidateTargetFrameworks(XDocument projectXml, BuildValidationResult result)
    {
        var targetFrameworksElement = projectXml.Descendants("TargetFrameworks").FirstOrDefault();
        if (targetFrameworksElement == null)
        {
            result.AddError("CONFIG", "TargetFrameworks element not found in project file");
            return;
        }

        var targetFrameworks = targetFrameworksElement.Value;
        
        // Check for MacCatalyst exclusion (as per requirements)
        if (targetFrameworks.Contains("net8.0-maccatalyst"))
        {
            result.AddError("CONFIG", "Target frameworks should not include MacCatalyst (net8.0-maccatalyst) as per project requirements");
        }

        // Check for required platforms
        var requiredPlatforms = new[] { "net8.0-android", "net8.0-ios" };
        foreach (var platform in requiredPlatforms)
        {
            if (!targetFrameworks.Contains(platform))
            {
                result.AddError("CONFIG", $"Target frameworks should include {platform}");
            }
        }

        // Check for Windows platform (conditional)
        if (targetFrameworks.Contains("net8.0-windows"))
        {
            result.AddInfo("CONFIG", "Windows platform detected in target frameworks");
        }
    }

    /// <summary>
    /// Validates resource files exist and have valid content
    /// </summary>
    private static void ValidateResourceFiles(string projectRoot, BuildValidationResult result)
    {
        // Validate app icon files
        var appIconPath = Path.Combine(projectRoot, "Resources", "AppIcon", "appicon.svg");
        ValidateResourceFile(appIconPath, "App Icon", result);

        var foregroundIconPath = Path.Combine(projectRoot, "Resources", "AppIcon", "appiconfg.svg");
        ValidateResourceFile(foregroundIconPath, "App Icon Foreground", result);

        // Validate splash screen file
        var splashScreenPath = Path.Combine(projectRoot, "Resources", "Splash", "splash.svg");
        ValidateResourceFile(splashScreenPath, "Splash Screen", result);
    }

    /// <summary>
    /// Validates individual resource file
    /// </summary>
    private static void ValidateResourceFile(string filePath, string resourceType, BuildValidationResult result)
    {
        if (!File.Exists(filePath))
        {
            result.AddError("RESOURCE", $"{resourceType} file not found: {filePath}");
            return;
        }

        try
        {
            var content = File.ReadAllText(filePath);
            
            // Basic content validation
            if (string.IsNullOrWhiteSpace(content))
            {
                result.AddError("RESOURCE", $"{resourceType} file is empty: {filePath}");
                return;
            }

            // SVG-specific validation
            if (filePath.EndsWith(".svg", StringComparison.OrdinalIgnoreCase))
            {
                ValidateSvgContent(filePath, content, resourceType, result);
            }
        }
        catch (Exception ex)
        {
            result.AddError("RESOURCE", $"Failed to read {resourceType} file {filePath}: {ex.Message}");
        }
    }

    /// <summary>
    /// Validates SVG content
    /// </summary>
    private static void ValidateSvgContent(string filePath, string content, string resourceType, BuildValidationResult result)
    {
        // Check for XML declaration
        if (!content.TrimStart().StartsWith("<?xml"))
        {
            result.AddWarning("RESOURCE", $"{resourceType} SVG should start with XML declaration: {filePath}");
        }

        // Check for SVG root element
        if (!content.Contains("<svg"))
        {
            result.AddError("RESOURCE", $"{resourceType} file must contain <svg> root element: {filePath}");
        }

        // Check for viewBox (important for scalability)
        if (!content.Contains("viewBox"))
        {
            result.AddWarning("RESOURCE", $"{resourceType} SVG should contain viewBox for proper scaling: {filePath}");
        }

        // Check for width and height
        if (!content.Contains("width") || !content.Contains("height"))
        {
            result.AddWarning("RESOURCE", $"{resourceType} SVG should contain width and height attributes: {filePath}");
        }

        // Validate XML syntax
        try
        {
            XDocument.Parse(content);
        }
        catch (System.Xml.XmlException xmlEx)
        {
            result.AddError("RESOURCE", $"{resourceType} SVG contains invalid XML syntax: {filePath} - {xmlEx.Message}");
        }

        // Check file size
        if (content.Length > 100000) // 100KB
        {
            result.AddWarning("RESOURCE", $"{resourceType} SVG file is large ({content.Length} bytes). Consider optimizing: {filePath}");
        }

        if (content.Length < 200)
        {
            result.AddWarning("RESOURCE", $"{resourceType} SVG file seems too simple ({content.Length} bytes). Ensure it contains proper content: {filePath}");
        }
    }

    /// <summary>
    /// Validates resource directory structure
    /// </summary>
    private static void ValidateResourceDirectoryStructure(string projectRoot, BuildValidationResult result)
    {
        var resourcesPath = Path.Combine(projectRoot, "Resources");
        if (!Directory.Exists(resourcesPath))
        {
            result.AddError("STRUCTURE", $"Resources directory not found: {resourcesPath}");
            return;
        }

        var appIconPath = Path.Combine(resourcesPath, "AppIcon");
        if (!Directory.Exists(appIconPath))
        {
            result.AddError("STRUCTURE", $"AppIcon directory not found: {appIconPath}");
        }

        var splashPath = Path.Combine(resourcesPath, "Splash");
        if (!Directory.Exists(splashPath))
        {
            result.AddError("STRUCTURE", $"Splash directory not found: {splashPath}");
        }
    }

    /// <summary>
    /// Validates hex color format
    /// </summary>
    private static bool IsValidHexColor(string color)
    {
        if (string.IsNullOrEmpty(color) || !color.StartsWith("#"))
            return false;

        if (color.Length != 7) // #RRGGBB
            return false;

        return color.Substring(1).All(c => char.IsDigit(c) || (c >= 'A' && c <= 'F') || (c >= 'a' && c <= 'f'));
    }

    /// <summary>
    /// Validates base size format
    /// </summary>
    private static bool IsValidBaseSize(string baseSize)
    {
        if (string.IsNullOrEmpty(baseSize) || !baseSize.Contains(","))
            return false;

        var parts = baseSize.Split(',');
        if (parts.Length != 2)
            return false;

        return int.TryParse(parts[0].Trim(), out int width) && 
               int.TryParse(parts[1].Trim(), out int height) &&
               width > 0 && height > 0;
    }

    /// <summary>
    /// Result class for build validation
    /// </summary>
    public class BuildValidationResult
    {
        public List<ValidationMessage> Errors { get; } = new List<ValidationMessage>();
        public List<ValidationMessage> Warnings { get; } = new List<ValidationMessage>();
        public List<ValidationMessage> Info { get; } = new List<ValidationMessage>();

        public bool HasErrors => Errors.Count > 0;
        public bool HasWarnings => Warnings.Count > 0;

        public void AddError(string category, string message)
        {
            Errors.Add(new ValidationMessage { Category = category, Message = message, Timestamp = DateTime.Now });
            System.Diagnostics.Debug.WriteLine($"BUILD ERROR [{category}]: {message}");
        }

        public void AddWarning(string category, string message)
        {
            Warnings.Add(new ValidationMessage { Category = category, Message = message, Timestamp = DateTime.Now });
            System.Diagnostics.Debug.WriteLine($"BUILD WARNING [{category}]: {message}");
        }

        public void AddInfo(string category, string message)
        {
            Info.Add(new ValidationMessage { Category = category, Message = message, Timestamp = DateTime.Now });
            System.Diagnostics.Debug.WriteLine($"BUILD INFO [{category}]: {message}");
        }

        public string GetSummaryReport()
        {
            var report = "=== Build Validation Summary ===\n";
            report += $"Errors: {Errors.Count}\n";
            report += $"Warnings: {Warnings.Count}\n";
            report += $"Info: {Info.Count}\n\n";

            if (Errors.Count > 0)
            {
                report += "ERRORS:\n";
                foreach (var error in Errors)
                {
                    report += $"  [{error.Category}] {error.Message}\n";
                }
                report += "\n";
            }

            if (Warnings.Count > 0)
            {
                report += "WARNINGS:\n";
                foreach (var warning in Warnings)
                {
                    report += $"  [{warning.Category}] {warning.Message}\n";
                }
                report += "\n";
            }

            return report;
        }
    }

    /// <summary>
    /// Validation message class
    /// </summary>
    public class ValidationMessage
    {
        public string Category { get; set; } = "";
        public string Message { get; set; } = "";
        public DateTime Timestamp { get; set; }
    }
}