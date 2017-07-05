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
            _ethereumManager = new EthereumManager();
        }
        #endregion

        #region Event Handlers
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