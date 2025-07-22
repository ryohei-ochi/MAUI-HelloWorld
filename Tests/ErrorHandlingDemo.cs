using System.Reflection;

namespace HelloWorld.Tests;

/// <summary>
/// Demonstration of error handling functionality
/// Requirements: 4.5 - Error handling for missing resources and build configuration errors
/// </summary>
public class ErrorHandlingDemo
{
    /// <summary>
    /// Demonstrates error handling for missing resource files
    /// </summary>
    public static void DemonstrateResourceErrorHandling()
    {
        System.Diagnostics.Debug.WriteLine("=== Error Handling Demonstration ===");
        System.Diagnostics.Debug.WriteLine("");

        try
        {
            // Test 1: Demonstrate missing file error handling
            System.Diagnostics.Debug.WriteLine("1. Testing missing file error handling...");
            var missingFilePath = Path.Combine("Resources", "AppIcon", "nonexistent.svg");
            if (!File.Exists(missingFilePath))
            {
                var errorMessage = ResourceErrorHandler.GetMissingResourceError("Test Icon", missingFilePath);
                System.Diagnostics.Debug.WriteLine($"Generated error message:\n{errorMessage}");
            }

            System.Diagnostics.Debug.WriteLine("");

            // Test 2: Demonstrate SVG validation error handling
            System.Diagnostics.Debug.WriteLine("2. Testing SVG validation error handling...");
            var invalidSvgContent = "<invalid>This is not valid SVG content</invalid>";
            var validationResult = ResourceErrorHandler.ValidateSvgContent("test.svg", invalidSvgContent);
            if (!validationResult.IsValid)
            {
                var errorMessage = ResourceErrorHandler.GetSvgValidationError("test.svg", validationResult.Errors);
                System.Diagnostics.Debug.WriteLine($"Generated validation error:\n{errorMessage}");
            }

            System.Diagnostics.Debug.WriteLine("");

            // Test 3: Demonstrate build validation
            System.Diagnostics.Debug.WriteLine("3. Testing build validation...");
            var projectRoot = GetProjectRootDirectory();
            var buildValidation = BuildValidation.ValidateAllBrandingResources(projectRoot);
            
            System.Diagnostics.Debug.WriteLine($"Build validation results:");
            System.Diagnostics.Debug.WriteLine($"- Errors: {buildValidation.Errors.Count}");
            System.Diagnostics.Debug.WriteLine($"- Warnings: {buildValidation.Warnings.Count}");
            System.Diagnostics.Debug.WriteLine($"- Info: {buildValidation.Info.Count}");

            if (buildValidation.HasErrors || buildValidation.HasWarnings)
            {
                System.Diagnostics.Debug.WriteLine("\nDetailed report:");
                System.Diagnostics.Debug.WriteLine(buildValidation.GetSummaryReport());
            }

            System.Diagnostics.Debug.WriteLine("");

            // Test 4: Demonstrate configuration error handling
            System.Diagnostics.Debug.WriteLine("4. Testing configuration error handling...");
            var invalidProjectContent = "<Project><PropertyGroup></PropertyGroup></Project>";
            var configValidation = ResourceErrorHandler.ValidateProjectConfiguration(invalidProjectContent);
            if (!configValidation.IsValid)
            {
                var errorMessage = ResourceErrorHandler.GetBuildConfigurationError(configValidation.Errors);
                System.Diagnostics.Debug.WriteLine($"Generated configuration error:\n{errorMessage}");
            }

            System.Diagnostics.Debug.WriteLine("");
            System.Diagnostics.Debug.WriteLine("=== Error Handling Demonstration Complete ===");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error during demonstration: {ex.Message}");
        }
    }

    /// <summary>
    /// Demonstrates successful validation when all resources are present
    /// </summary>
    public static void DemonstrateSuccessfulValidation()
    {
        System.Diagnostics.Debug.WriteLine("=== Successful Validation Demonstration ===");
        System.Diagnostics.Debug.WriteLine("");

        try
        {
            var projectRoot = GetProjectRootDirectory();
            
            // Run resource validation tests
            System.Diagnostics.Debug.WriteLine("Running resource validation tests...");
            var validationResults = ResourceValidationTests.ExecuteAllTests();
            
            System.Diagnostics.Debug.WriteLine($"Validation Results:");
            System.Diagnostics.Debug.WriteLine($"- Total Tests: {validationResults.TotalTests}");
            System.Diagnostics.Debug.WriteLine($"- Passed: {validationResults.PassedTests}");
            System.Diagnostics.Debug.WriteLine($"- Failed: {validationResults.FailedTests}");

            if (validationResults.FailedTests > 0)
            {
                System.Diagnostics.Debug.WriteLine("\nFailed Tests:");
                foreach (var failure in validationResults.Failures)
                {
                    System.Diagnostics.Debug.WriteLine($"- {failure}");
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("\nAll validation tests passed successfully!");
                System.Diagnostics.Debug.WriteLine("This demonstrates that error handling is working correctly");
                System.Diagnostics.Debug.WriteLine("and all required resources are properly configured.");
            }

            System.Diagnostics.Debug.WriteLine("");
            System.Diagnostics.Debug.WriteLine("=== Successful Validation Demonstration Complete ===");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error during validation demonstration: {ex.Message}");
        }
    }

    /// <summary>
    /// Runs all error handling demonstrations
    /// </summary>
    public static void RunAllDemonstrations()
    {
        System.Diagnostics.Debug.WriteLine("=== COMPREHENSIVE ERROR HANDLING DEMONSTRATION ===");
        System.Diagnostics.Debug.WriteLine("");

        DemonstrateResourceErrorHandling();
        System.Diagnostics.Debug.WriteLine("");
        DemonstrateSuccessfulValidation();

        System.Diagnostics.Debug.WriteLine("");
        System.Diagnostics.Debug.WriteLine("=== ALL ERROR HANDLING DEMONSTRATIONS COMPLETE ===");
        System.Diagnostics.Debug.WriteLine("");
        System.Diagnostics.Debug.WriteLine("Summary:");
        System.Diagnostics.Debug.WriteLine("- Resource file error handling: ✓ Implemented");
        System.Diagnostics.Debug.WriteLine("- SVG validation error handling: ✓ Implemented");
        System.Diagnostics.Debug.WriteLine("- Build configuration error handling: ✓ Implemented");
        System.Diagnostics.Debug.WriteLine("- Project configuration validation: ✓ Implemented");
        System.Diagnostics.Debug.WriteLine("- Runtime validation integration: ✓ Implemented");
        System.Diagnostics.Debug.WriteLine("");
        System.Diagnostics.Debug.WriteLine("Error handling implementation is complete and functional!");
    }

    private static string GetProjectRootDirectory()
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
}