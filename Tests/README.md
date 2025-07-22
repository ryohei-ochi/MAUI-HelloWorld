# App Branding Verification Tests

This directory contains comprehensive tests to verify the app branding customization functionality for the .NET MAUI HelloWorld application.

## Test Structure

### Core Test Files

- **AppIconTests.cs** - Tests for app icon display and configuration verification
- **SplashScreenTests.cs** - Tests for splash screen display and configuration verification  
- **PlatformSpecificTests.cs** - Platform-specific verification tests for Android, iOS, and Windows
- **BrandingIntegrationTests.cs** - Integration tests for overall branding functionality
- **ResourceValidationTests.cs** - Tests for resource validation and error handling
- **ErrorHandlingDemo.cs** - Demonstration of error handling scenarios
- **TestRunner.cs** - Main test runner that orchestrates all test execution
- **PlatformTestRunner.cs** - Specialized runner for platform-specific tests

### Platform-Specific Verification (Task 10)

The `PlatformSpecificTests.cs` file implements comprehensive platform-specific verification covering:

#### Android Platform Tests
- **Adaptive Icon Generation**: Verifies Android adaptive icon configuration with foreground/background separation
- **Display Validation**: Ensures adaptive icons render correctly with proper viewBox and safe zones
- **Multi-Density Support**: Validates SVG format usage for automatic density generation
- **API Level Compatibility**: Confirms minimum Android version supports modern icon features

#### iOS Platform Tests  
- **App Icon Generation**: Verifies iOS app icon configuration and multi-size generation
- **Launch Screen Generation**: Validates iOS Launch Screen configuration and device adaptation
- **Display Characteristics**: Ensures icons scale properly across different iOS device sizes
- **Version Compatibility**: Confirms minimum iOS version supports required features

#### Windows Platform Tests
- **App Icon Generation**: Verifies Windows app icon configuration (when Windows platform is enabled)
- **Splash Screen Generation**: Validates Windows splash screen configuration
- **Display Validation**: Ensures proper scaling and tile background support
- **Conditional Testing**: Gracefully handles cases where Windows platform is not configured

#### Cross-Platform Validation
- **Resource Consistency**: Verifies consistent resource configuration across all platforms
- **MacCatalyst Exclusion**: Confirms MacCatalyst is properly excluded from all configurations
- **Branding Consistency**: Validates color and resource consistency across platforms

## Integration Tests (Task 12)

The `BrandingIntegrationTests.cs` file implements comprehensive integration tests that verify the overall branding functionality works correctly across all platforms:

### Overall Branding Functionality Tests
- **Integration_OverallBranding_ShouldWorkCorrectly**: Verifies that all branding components work together correctly
  - Project configuration supports branding
  - All branding resources exist and are valid
  - Branding configuration is consistent across platforms
  - Platform-specific branding support is properly configured

### Cross-Platform Consistency Tests  
- **Integration_CrossPlatformConsistency_ShouldBeValid**: Ensures visual consistency across different devices
  - Consistent resource formats (SVG) across all platforms
  - Consistent color scheme across branding elements
  - Consistent sizing and scaling approach
  - Platform adaptations maintain brand identity

### App Icon Launcher Display Tests
- **Integration_AppIconLauncherDisplay_ShouldBeConfigured**: Verifies app icon displays correctly in device launchers
  - App icon configuration for launcher display
  - Platform-specific icon requirements are met
  - Icon resources support all required sizes
  - Adaptive icon configuration for Android launchers

### Splash Screen Display Timing Tests
- **Integration_SplashScreenDisplayTiming_ShouldBeConfigured**: Ensures splash screen displays with proper timing
  - Splash screen configuration for proper display
  - Platform-specific splash screen requirements
  - Splash screen resources support proper scaling
  - Splash screen timing configuration

### Requirements Coverage

The integration tests specifically address the following requirements:

- **Requirement 4.1**: System displays custom branding elements correctly on each target platform when built and deployed
- **Requirement 4.2**: System maintains visual consistency of branding elements across different devices
- **Requirement 4.3**: System displays custom app icon in device app launcher when installed
- **Requirement 4.4**: System displays custom splash screen with proper timing and transitions when app launches

## Test Execution

### Running Individual Test Suites

```csharp
// Run app icon tests
TestRunner.RunAppIconTests();

// Run splash screen tests  
TestRunner.RunSplashScreenTests();

// Run platform-specific tests
TestRunner.RunPlatformSpecificTests();

// Run resource validation tests
TestRunner.RunResourceValidationTests();
```

### Running Integration Tests

```csharp
// Run integration tests
TestRunner.RunBrandingIntegrationTests();
```

### Running All Tests

```csharp
// Run complete test suite
TestRunner.RunAllBrandingTests();
```

### UI Integration

The main application includes buttons to run tests directly from the UI:
- "Run App Icon Tests" - Executes app icon verification tests
- "Run Splash Screen Tests" - Executes splash screen verification tests  
- "Run Platform-Specific Tests" - Executes platform-specific verification tests
- "Run Integration Tests" - Executes integration tests for overall branding functionality
- "Run All Branding Tests" - Executes the complete test suite

## Requirements Coverage

The platform-specific tests address the following requirements:

- **Requirement 2.3**: System should properly render adaptive icons on supported platforms
- **Requirement 2.4**: System should generate appropriate icon sizes and formats for each platform
- **Requirement 3.5**: System should generate platform-appropriate splash screen resources
- **Requirement 4.1**: App should build and deploy correctly on target platforms
- **Requirement 4.2**: System should maintain visual consistency across different devices

## Test Framework

The tests use a custom lightweight testing framework with:
- `[Test]` attribute for test method marking
- `Assert` class for test assertions
- `TestResults` class for result aggregation
- Debug output for test execution logging

## Platform Configuration

The tests verify the following platform configuration:
- **Target Platforms**: Android (net8.0-android), iOS (net8.0-ios), Windows (net8.0-windows10.0.19041.0)
- **Excluded Platforms**: MacCatalyst (net8.0-maccatalyst)
- **Resource Format**: SVG for scalability across all platforms
- **Icon Configuration**: Adaptive icons with foreground/background separation
- **Splash Configuration**: Custom splash screens with background color and base size

## Error Handling

The tests include comprehensive error handling for:
- Missing resource files
- Invalid SVG content
- Incorrect project configuration
- Platform-specific configuration issues
- Build-time validation errors

All tests provide detailed error messages and debug output to facilitate troubleshooting.