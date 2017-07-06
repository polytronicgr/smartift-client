using Guytp.Logging;
using System;
using System.Windows;

namespace Lts.Sift.WinClient
{
    /// <summary>
    /// This class provides the main entry point to the application.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// This method provides the main entry point to the application.
        /// </summary>
        [STAThread]
        public static void Main(string[] argv)
        {
            try
            {
                SiftLogProvider logProvider = new SiftLogProvider(new LogLevel[] { LogLevel.Debug, LogLevel.Error, LogLevel.Info, LogLevel.Warning });
                Logger.ApplicationInstance.AddLogProvider(logProvider);
            }
            catch
            {
                // Intentionall swallowed as we don't have logging yet
            }
            try
            {
                SiftApp app = new SiftApp();
                app.DispatcherUnhandledException += OnDispatcherUnhandledException;
                AppDomain.CurrentDomain.UnhandledException += OnAppDomainUnhandledException;
                app.Run();
            }
            catch (Exception ex)
            {
                HandleFatalException(ex);
            }
        }

        /// <summary>
        /// Handle an unexpected exception in a background thread.
        /// </summary>
        /// <param name="sender">
        /// The event sender.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private static void OnAppDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            HandleFatalException(e.ExceptionObject as Exception);
        }

        /// <summary>
        /// Handle an unexpected exception in the UI thread.
        /// </summary>
        /// <param name="sender">
        /// The event sender.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private static void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            HandleFatalException(e.Exception);
        }

        /// <summary>
        /// Handle a fatal exception by calling our logger and then informing the user if possible.
        /// </summary>
        /// <param name="ex">
        /// The exception that triggered this.
        /// </param>
        private static void HandleFatalException(Exception ex)
        {
            try
            {
                Logger.ApplicationInstance.Error("Fatal error within Sift.", ex);
            }
            catch
            {
                // Intentionally swallowed
            }
            try
            {
                MessageBox.Show("There was a fatal error within SIFT, please contact support with this message." + Environment.NewLine + ex);
            }
            catch
            {
                // Intentionally swallowed
            }
        }
    }
}