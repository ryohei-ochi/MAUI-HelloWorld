namespace HelloWorld
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();
        }

        private void OnCounterClicked(object sender, EventArgs e)
        {
            count++;

            if (count == 1)
                CounterBtn.Text = $"Clicked {count} time";
            else
                CounterBtn.Text = $"Clicked {count} times";

            SemanticScreenReader.Announce(CounterBtn.Text);
        }

        private async void OnTestClicked(object sender, EventArgs e)
        {
            try
            {
                TestBtn.Text = "Running Tests...";
                TestBtn.IsEnabled = false;

                // Run the app icon tests
                HelloWorld.Tests.TestRunner.RunAppIconTests();

                await DisplayAlert("Test Results", "App Icon tests completed successfully! Check debug output for details.", "OK");
                TestBtn.Text = "Tests Passed ✓";
            }
            catch (Exception ex)
            {
                await DisplayAlert("Test Error", $"Tests failed: {ex.Message}", "OK");
                TestBtn.Text = "Tests Failed ✗";
            }
            finally
            {
                TestBtn.IsEnabled = true;
            }
        }

        private async void OnSplashTestClicked(object sender, EventArgs e)
        {
            try
            {
                SplashTestBtn.Text = "Running Tests...";
                SplashTestBtn.IsEnabled = false;

                // Run the splash screen tests
                HelloWorld.Tests.TestRunner.RunSplashScreenTests();

                await DisplayAlert("Test Results", "Splash Screen tests completed successfully! Check debug output for details.", "OK");
                SplashTestBtn.Text = "Tests Passed ✓";
            }
            catch (Exception ex)
            {
                await DisplayAlert("Test Error", $"Tests failed: {ex.Message}", "OK");
                SplashTestBtn.Text = "Tests Failed ✗";
            }
            finally
            {
                SplashTestBtn.IsEnabled = true;
            }
        }

        private async void OnPlatformTestClicked(object sender, EventArgs e)
        {
            try
            {
                PlatformTestBtn.Text = "Running Platform Tests...";
                PlatformTestBtn.IsEnabled = false;

                // Run the platform-specific tests
                HelloWorld.Tests.TestRunner.RunPlatformSpecificTests();

                await DisplayAlert("Test Results", "Platform-specific tests completed successfully! Check debug output for details.", "OK");
                PlatformTestBtn.Text = "Platform Tests Passed ✓";
            }
            catch (Exception ex)
            {
                await DisplayAlert("Test Error", $"Tests failed: {ex.Message}", "OK");
                PlatformTestBtn.Text = "Platform Tests Failed ✗";
            }
            finally
            {
                PlatformTestBtn.IsEnabled = true;
            }
        }

        private async void OnIntegrationTestClicked(object sender, EventArgs e)
        {
            try
            {
                IntegrationTestBtn.Text = "Running Integration Tests...";
                IntegrationTestBtn.IsEnabled = false;

                // Run the integration tests
                HelloWorld.Tests.TestRunner.RunBrandingIntegrationTests();

                await DisplayAlert("Test Results", "Integration tests completed successfully! Check debug output for details.", "OK");
                IntegrationTestBtn.Text = "Integration Tests Passed ✓";
            }
            catch (Exception ex)
            {
                await DisplayAlert("Test Error", $"Tests failed: {ex.Message}", "OK");
                IntegrationTestBtn.Text = "Integration Tests Failed ✗";
            }
            finally
            {
                IntegrationTestBtn.IsEnabled = true;
            }
        }

        private async void OnAllTestsClicked(object sender, EventArgs e)
        {
            try
            {
                AllTestsBtn.Text = "Running All Tests...";
                AllTestsBtn.IsEnabled = false;

                // Run all branding tests
                HelloWorld.Tests.TestRunner.RunAllBrandingTests();

                await DisplayAlert("Test Results", "All branding tests completed successfully! Check debug output for details.", "OK");
                AllTestsBtn.Text = "All Tests Passed ✓";
            }
            catch (Exception ex)
            {
                await DisplayAlert("Test Error", $"Tests failed: {ex.Message}", "OK");
                AllTestsBtn.Text = "Tests Failed ✗";
            }
            finally
            {
                AllTestsBtn.IsEnabled = true;
            }
        }
    }

}
