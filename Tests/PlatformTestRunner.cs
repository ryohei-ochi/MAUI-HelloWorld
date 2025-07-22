using HelloWorld.Tests;

namespace HelloWorld.Tests;

/// <summary>
/// Simple test runner specifically for platform-specific verification tests
/// This demonstrates the platform-specific validation functionality
/// </summary>
public class PlatformTestRunner
{
    public static void RunPlatformTests()
    {
        System.Diagnostics.Debug.WriteLine("=== Platform-Specific Verification Test Runner ===");
        System.Diagnostics.Debug.WriteLine("");

        try
        {
            // Run platform-specific tests
            TestRunner.RunPlatformSpecificTests();
            
            System.Diagnostics.Debug.WriteLine("");
            System.Diagnostics.Debug.WriteLine("=== Platform-Specific Tests Completed Successfully ===");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Platform-specific tests failed: {ex.Message}");
            System.Diagnostics.Debug.WriteLine("");
            System.Diagnostics.Debug.WriteLine("Stack trace:");
            System.Diagnostics.Debug.WriteLine(ex.StackTrace);
            throw;
        }
    }
}