using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace iostamagotchi
{
    public partial class App : Application
    {
        private static Settings m_settings = null;
        private static MainViewModel m_mainViewModel = null;
        private static StudyMenuViewModel m_studyMenuViewModel = null;
        private static StudyViewModel m_studyViewModel = null;
        private static ManageFlashCardsViewModel m_manageFlashCardsViewModel = null;
        private static StudyPlanViewModel m_studyPlanViewModel = null;
        private static MyProfileViewModel m_myProfileViewModel = null;
        private static TestViewModel m_testViewModel = null;

        public static Settings Settings
        {
            get
            {
                if (App.m_settings == null)
                {
                    App.m_settings = new Settings();
                }
                return App.m_settings;
            }
        }

        public static MainViewModel ViewModel
        {
            get
            {
                // Delay creation of the view model until necessary
                if (App.m_mainViewModel == null)
                {
                    App.m_mainViewModel = new MainViewModel();
                }

                return App.m_mainViewModel;
            }
        }

        public static StudyMenuViewModel StudyMenuViewModel
        {
            get
            {
                // Delay creation of the view model until necessary
                if (App.m_studyMenuViewModel == null)
                {
                    App.m_studyMenuViewModel = new StudyMenuViewModel();
                    App.m_studyMenuViewModel.LoadItems();
                }

                return App.m_studyMenuViewModel;
            }
        }

        public static StudyViewModel StudyViewModel
        {
            get
            {
                // Delay creation of the view model until necessary
                if (App.m_studyViewModel == null)
                {
                    App.m_studyViewModel = new StudyViewModel();
                }

                return App.m_studyViewModel;
            }
        }

        public static ManageFlashCardsViewModel ManageFlashCardsViewModel
        {
            get
            {
                // Delay creation of the view model until necessary
                if (App.m_manageFlashCardsViewModel == null)
                {
                    App.m_manageFlashCardsViewModel = new ManageFlashCardsViewModel();
                }

                return App.m_manageFlashCardsViewModel;
            }
        }

        public static StudyPlanViewModel StudyPlanViewModel
        {
            get
            {
                // Delay creation of the view model until necessary
                if (App.m_studyPlanViewModel == null)
                {
                    App.m_studyPlanViewModel = new StudyPlanViewModel();
                }

                return App.m_studyPlanViewModel;
            }
        }

        public static MyProfileViewModel MyProfileViewModel
        {
            get
            {
                // Delay creation of the view model until necessary
                if (App.m_myProfileViewModel == null)
                {
                    App.m_myProfileViewModel = new MyProfileViewModel();
                }

                return App.m_myProfileViewModel;
            }
        }

        public static TestViewModel TestViewModel
        {
            get
            {
                // Delay creation of the view model until necessary
                if (App.m_testViewModel == null)
                {
                    App.m_testViewModel = new TestViewModel();
                }

                return App.m_testViewModel;
            }
        }

        /// <summary>
        /// This property is used to transfer temporary data across frames (i.e. when picking an item in fullscreen ListPicker)
        /// </summary>
        public static Dictionary<String, object> TmpData = null;

        /// <summary>
        /// Class handling all types in this assembly
        /// </summary>
        public static AssemblyTypes CurrentTypes = null;

        /// <summary>
        /// Provides easy access to the root frame of the Phone Application.
        /// </summary>
        /// <returns>The root frame of the Phone Application.</returns>
        public PhoneApplicationFrame RootFrame { get; private set; }

        /// <summary>
        /// Provides access to the AppServiceProvider for the application.
        /// </summary>
        public AppServiceProvider Services { get; private set; }

        /// <summary>
        /// Code for detecting windows phone theme
        /// 
        /// Taken from: http://snipplr.com/view/41214/windows-phone-7-detect-current-theme/
        /// </summary>
        private static Color lightThemeBackground = Color.FromArgb(255, 0, 0, 0);
        private static Color darkThemeBackground = Color.FromArgb(255, 255, 255, 255);
        private static SolidColorBrush backgroundBrush;
        internal static string CurrentTheme
        {
            get
            {
                if (backgroundBrush == null)
                    backgroundBrush = Application.Current.Resources["PhoneBackgroundBrush"] as SolidColorBrush;

                if (backgroundBrush.Color == lightThemeBackground)
                    return "dark";
                else if (backgroundBrush.Color == darkThemeBackground)
                    return "light";

                return "dark";
            }
        }

        /// <summary>
        /// Constructor for the Application object.
        /// </summary>
        public App()
        {
            // Global handler for uncaught exceptions. 
            UnhandledException += Application_UnhandledException;

            // Standard Silverlight initialization
            InitializeComponent();

            // Phone-specific initialization
            InitializePhoneApplication();

            // Show graphics profiling information while debugging.
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // Display the current frame rate counters.
                Application.Current.Host.Settings.EnableFrameRateCounter = true;

                // Show the areas of the app that are being redrawn in each frame.
                //Application.Current.Host.Settings.EnableRedrawRegions = true;

                // Enable non-production analysis visualization mode, 
                // which shows areas of a page that are handed off to GPU with a colored overlay.
                //Application.Current.Host.Settings.EnableCacheVisualization = true;

                // Disable the application idle detection by setting the UserIdleDetectionMode property of the
                // application's PhoneApplicationService object to Disabled.
                // Caution:- Use this under debug mode only. Applications that disable user idle detection will continue to run
                // and consume battery power when the user is not using the phone.
                PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;
            }
        }

        // Code to execute when the application is launching (eg, from Start)
        // This code will not execute when the application is reactivated
        private void Application_Launching(object sender, LaunchingEventArgs e)
        {
//            IsolatedStorageExplorer.Explorer.Start("10.0.0.11");
            if (App.CurrentTypes == null)
            {
                App.CurrentTypes = new AssemblyTypes();
            }
            App.CurrentTypes.LoadTypes();

            if (App.TmpData == null)
            {
                App.TmpData = new Dictionary<string, object>();
            }

            DbSchemaUpdater.Init();
            CommonData.ResetNotificationCounter();
        }

        // Code to execute when the application is activated (brought to foreground)
        // This code will not execute when the application is first launched
        private void Application_Activated(object sender, ActivatedEventArgs e)
        {
            CommonData.ResetNotificationCounter();
            //            IsolatedStorageExplorer.Explorer.RestoreFromTombstone();
        }

        // Code to execute when the application is deactivated (sent to background)
        // This code will not execute when the application is closing
        private void Application_Deactivated(object sender, DeactivatedEventArgs e)
        {
        }

        // Code to execute when the application is closing (eg, user hit Back)
        // This code will not execute when the application is deactivated
        private void Application_Closing(object sender, ClosingEventArgs e)
        {
        }

        // Code to execute if a navigation fails
        private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // A navigation has failed; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        // Code to execute on Unhandled Exceptions
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        #region Phone application initialization

        // Avoid double-initialization
        private bool phoneApplicationInitialized = false;

        // Do not add any additional code to this method
        private void InitializePhoneApplication()
        {
            if (phoneApplicationInitialized)
                return;

            // Create the frame but don't set it as RootVisual yet; this allows the splash
            // screen to remain active until the application is ready to render.
            RootFrame = new PhoneApplicationFrame();
            RootFrame.Navigated += CompleteInitializePhoneApplication;

            // Handle navigation failures
            RootFrame.NavigationFailed += RootFrame_NavigationFailed;

            // Ensure we don't initialize again
            phoneApplicationInitialized = true;
        }

        // Do not add any additional code to this method
        private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
        {
            // Set the root visual to allow the application to render
            if (RootVisual != RootFrame)
                RootVisual = RootFrame;

            // Remove this handler since it is no longer needed
            RootFrame.Navigated -= CompleteInitializePhoneApplication;
        }

        #endregion
    }
}