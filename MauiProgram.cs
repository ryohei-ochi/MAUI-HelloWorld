using Microsoft.Extensions.Logging;

namespace HelloWorld
{
    /// <summary>
    /// Main application program class for .NET MAUI app initialization
    /// 
    /// BRANDING CUSTOMIZATION INTEGRATION:
    /// This class integrates branding resource validation into the app startup process
    /// to ensure all custom app icons and splash screens are properly configured
    /// before the application launches.
    /// 
    /// Platform Support:
    /// - Android: Validates adaptive icon resources and splash screen configuration
    /// - iOS: Validates app icon bundle and launch screen resources  
    /// - Windows: Validates app icon tiles and splash screen resources
    /// 
    /// Resource Validation:
    /// - Checks existence of appicon.svg, appiconfg.svg, and splash.svg
    /// - Validates SVG file format and structure
    /// - Verifies project configuration settings
    /// - Provides detailed error reporting for troubleshooting
    /// </summary>
    public static class MauiProgram
    {
        /// <summary>
        /// Creates and configures the MAUI application with branding validation
        /// 
        /// Branding Integration Points:
        /// 1. Pre-startup validation of all branding resources
        /// 2. Error handling for missing or invalid branding files
        /// 3. Debug output for branding configuration status
        /// 4. Graceful degradation in production environments
        /// </summary>
        /// <returns>Configured MauiApp instance</returns>
        public static MauiApp CreateMauiApp()
        {
            // BRANDING VALIDATION: Ensure all custom branding resources are valid
            // before proceeding with app initialization. This prevents runtime
            // issues with missing icons or splash screens.
            ValidateBrandingResources();

            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }

        /// <summary>
        /// Validates branding resources during app startup
        /// Requirements: 4.5 - Error handling for build process configuration errors
        /// </summary>
        private static void ValidateBrandingResources()
        {
            try
            {
                var projectRoot = GetProjectRootDirectory();
                var validationResult = BuildValidation.ValidateAllBrandingResources(projectRoot);

                if (validationResult.HasErrors)
                {
                    System.Diagnostics.Debug.WriteLine("=== BRANDING VALIDATION ERRORS DETECTED ===");
                    System.Diagnostics.Debug.WriteLine(validationResult.GetSummaryReport());
                    
                    // In a production app, you might want to show a user-friendly error message
                    // or gracefully degrade functionality instead of throwing an exception
                    throw new InvalidOperationException(
                        $"Branding resource validation failed with {validationResult.Errors.Count} errors. " +
                        "Check debug output for details.");
                }

                if (validationResult.HasWarnings)
                {
                    System.Diagnostics.Debug.WriteLine("=== BRANDING VALIDATION WARNINGS ===");
                    System.Diagnostics.Debug.WriteLine(validationResult.GetSummaryReport());
                }

                System.Diagnostics.Debug.WriteLine("Branding resource validation completed successfully.");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Branding validation failed: {ex.Message}");
                // In development, we want to know about validation issues
                // In production, you might want to handle this more gracefully
#if DEBUG
                throw;
#else
                // Log the error but don't crash the app in production
                System.Diagnostics.Debug.WriteLine($"Continuing with default branding due to validation error: {ex.Message}");
#endif
            }
        }

        /// <summary>
        /// Gets the project root directory
        /// </summary>
        private static string GetProjectRootDirectory()
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
    }
}
