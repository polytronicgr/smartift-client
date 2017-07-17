using Guytp.Logging;
using System;
using System.Configuration;
using System.Windows;

namespace Lts.Sift.WinClient
{
    /// <summary>
    /// The SiftApp provides the plumbing between .Net and WPF startup and allows us to hook up to our first window.
    /// </summary>
    public class SiftApp : Application
    {
        #region Declarations
        /// <summary>
        /// Defines our ethereum manager thread that the UI needs.
        /// </summary>
        private EthereumManager _ethereumManager;
        #endregion

        #region Constructors
        /// <summary>
        /// Create a new instance of htis class.
        /// </summary>
        public SiftApp()
        {
            // Create our ethereum manager thread that the UI will need
            string url = null;
            try
            {
                string value = ConfigurationManager.AppSettings["Lts.Sift.WinClient.EthereumRpcUrl"];
            }
            catch (Exception ex)
            {
                Logger.ApplicationInstance.Error("Error parsing value for ethereum URL, will use default", ex);
            }
            _ethereumManager = new EthereumManager(string.IsNullOrEmpty(url) ? "http://localhost:8545/" : url);

            // Hookup to events
            Exit += OnExit;
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Handle the application shutting down.
        /// </summary>
        /// <param name="sender">
        /// The event sender.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private void OnExit(object sender, ExitEventArgs e)
        {
            // Free up all our view models
            foreach (Window window in Windows)
            {
                IDisposable disposable = window.DataContext as IDisposable;
                if (disposable == null)
                    continue;
                try
                {
                    disposable.Dispose();
                }
                catch (Exception ex)
                {
                    Logger.ApplicationInstance.Error("Failed to free up view model " + disposable.GetType().Name + " for window " + window.GetType().Name, ex);
                }
            }

            // Free up our ethereum manager
            try
            {
                _ethereumManager?.Dispose();
            }
            catch (Exception ex)
            {
                Logger.ApplicationInstance.Error("Failed to free up ethereum manager", ex);
            }
            _ethereumManager = null;
        }

        /// <summary>
        /// Handle the application starting up and show our first window.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected override void OnStartup(StartupEventArgs e)
        {
            new SplashScreenWindow()
            {
                DataContext = new SplashScreenViewModel(_ethereumManager)
            }.Show();
        }

        /// <summary>
        /// Handle the application terminating.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected override void OnExit(ExitEventArgs e)
        {
            // Free any resources we are using
            _ethereumManager?.Dispose();
            _ethereumManager = null;

            // Call to base
            base.OnExit(e);
        }
        #endregion
    }
}