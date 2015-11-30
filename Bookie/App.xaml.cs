using System;
using System.Diagnostics;
using System.Linq;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Storage;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Bookie.Common;
using Bookie.Data;
using Bookie.Views;
using MetroLog;
using MetroLog.Targets;
using Microsoft.ApplicationInsights;
using Microsoft.Data.Entity;

namespace Bookie
{
    /// <summary>
    ///     Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public sealed partial class App : Application
    {
        /// <summary>
        ///     Initializes the singleton application object.  This is the first line of authored code
        ///     executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
         public static ILogger Log;

        public App()
        {
            var configuration = new LoggingConfiguration();
            configuration.AddTarget(LogLevel.Trace, LogLevel.Fatal, new FileStreamingTarget());
            configuration.IsEnabled = true;

            LogManagerFactory.DefaultConfiguration = configuration;
            Log = LogManagerFactory.DefaultLogManager.GetLogger<App>();

            UnhandledException += App_UnhandledException;
            GlobalCrashHandler.Configure();

            BookieSettings.LoadSettings();



            //var localSettings = ApplicationData.Current.LocalSettings;
            //var theme = localSettings.Values["Theme"];
            //if (theme == null)
            //{
            //    Current.RequestedTheme = ApplicationTheme.Dark;
            //}
            //else if (theme.ToString() == "Dark")
            //{
            //    Current.RequestedTheme = ApplicationTheme.Dark;
            //}
            //else if (theme.ToString() == "Light")
            //{
            //    Current.RequestedTheme = ApplicationTheme.Light;
            //}


            WindowsAppInitializer.InitializeAsync(
                WindowsCollectors.Metadata |
                WindowsCollectors.Session);
            InitializeComponent();
            Suspending += OnSuspending;
            using (var db = new Context())
            {
             db.Database.Migrate();
            }
            var covers = Globals.GetCoversFolder();








        }

        private void App_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var t = Log.IsFatalEnabled;
            var l = Log;
            try
            {
                Log.Fatal("Unexpected error: " + e.Exception);
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        ///     Invoked when the application is launched normally by the end user.  Other entry points
        ///     will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            // Load Theme
            var loadedResources = App.Current.Resources.MergedDictionaries.ToList();
            foreach (var resource in loadedResources)
            {
                if (resource.Source.ToString().Contains("Theme"))
                {
                    App.Current.Resources.MergedDictionaries.Remove(resource);
                }
            }
            App.Current.Resources.MergedDictionaries.Add(BookieSettings.Theme.Resource);




            var titleBar = ApplicationView.GetForCurrentView().TitleBar;
            titleBar.BackgroundColor = Colors.Black;
            titleBar.ButtonBackgroundColor = Colors.Black;
            titleBar.ButtonForegroundColor = Colors.White;
            titleBar.ForegroundColor = Colors.White;
#if DEBUG
            if (Debugger.IsAttached)
            {
               // DebugSettings.EnableFrameRateCounter = true;
            }
#endif

            var rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }
                //  Display an extended splash screen if app was not previously running.
                if (e.PreviousExecutionState != ApplicationExecutionState.Running)
                {
                    var loadState = e.PreviousExecutionState == ApplicationExecutionState.Terminated;
                    var extendedSplash = new ExtendedSplash(e.SplashScreen, loadState);
                    rootFrame.Content = extendedSplash;
                    Window.Current.Content = rootFrame;
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                rootFrame.Navigate(typeof (Shell), e.Arguments);
            }
            // Ensure the current window is active
            Window.Current.Activate();
        }

        /// <summary>
        ///     Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        private void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        ///     Invoked when application execution is being suspended.  Application state is saved
        ///     without knowing whether the application will be terminated or resumed with the contents
        ///     of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }
    }
}