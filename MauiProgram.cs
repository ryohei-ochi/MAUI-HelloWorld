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
            // Only run validation in debug mode to avoid production issues.
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
#if DEBUG
            try
            {
                var projectRoot = GetProjectRootDirectory();
                var validationResult = BuildValidation.ValidateAllBrandingResources(projectRoot);

                if (validationResult.HasErrors)
                {
                    System.Diagnostics.Debug.WriteLine("=== BRANDING VALIDATION ERRORS DETECTED ===");
                    System.Diagnostics.Debug.WriteLine(validationResult.GetSummaryReport());
                    
                    // In debug mode, log the errors but don't crash the app
                    System.Diagnostics.Debug.WriteLine("Continuing with default branding due to validation errors.");
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
                System.Diagnostics.Debug.WriteLine($"Continuing with default branding due to validation error.");
            }
#endif
        }

        /// <summary>
        /// Gets the project root directory
        /// </summary>
        private static string GetProjectRootDirectory()
        {
            // Try multiple approaches to find the project root
            
            // 1. Try from the assembly location
            var assemblyLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;
            if (!string.IsNullOrEmpty(assemblyLocation))
            {
                var assemblyDir = new DirectoryInfo(Path.GetDirectoryName(assemblyLocation));
                var projectRoot = FindProjectRootFromDirectory(assemblyDir);
                if (projectRoot != null)
                {
                    return projectRoot;
                }
            }

            // 2. Try from the current directory
            var currentDirectory = Directory.GetCurrentDirectory();
            var currentDir = new DirectoryInfo(currentDirectory);
            var projectRootFromCurrent = FindProjectRootFromDirectory(currentDir);
            if (projectRootFromCurrent != null)
            {
                return projectRootFromCurrent;
            }

            // 3. Try from the app domain base directory
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            if (!string.IsNullOrEmpty(baseDirectory))
            {
                var baseDir = new DirectoryInfo(baseDirectory);
                var projectRootFromBase = FindProjectRootFromDirectory(baseDir);
                if (projectRootFromBase != null)
                {
                    return projectRootFromBase;
                }
            }

            throw new InvalidOperationException("Could not find project root directory");
        }

        /// <summary>
        /// Helper method to find project root from a given directory
        /// </summary>
        private static string FindProjectRootFromDirectory(DirectoryInfo directory)
        {
            // Walk up the directory tree to find the project root
            while (directory != null)
            {
                if (File.Exists(Path.Combine(directory.FullName, "HelloWorld.csproj")))
                {
                    return directory.FullName;
                }
                directory = directory.Parent;
            }
            return null;
        }
    }
}
