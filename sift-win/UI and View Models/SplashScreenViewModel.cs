using Guytp.Logging;
using System;
using System.Reflection;
using System.Threading;
using System.Windows;

namespace Lts.Sift.WinClient
{
    /// <summary>
    /// This view model supports the splash screen and is used to allow the application to transition to the correct mode of operation (pre or post ICO).
    /// </summary>
    public class SplashScreenViewModel : BaseViewModel, IDisposable
    {
        #region Declarations
        /// <summary>
        /// Defines the message to show when we are connecting to ethereum.
        /// </summary>
        private const string MessageConnecting = "We are attempting to connect to the Ethereum Network.\r\nThis may take a few seconds...";

        /// <summary>
        /// Defines the message to show when it's not possible to connect to ethereum's RPC interface.
        /// </summary>
        private const string MessageDelayedStart = "There seems to be a problem connecting to Ethereum, is your wallet running?";

        /// <summary>
        /// Defines the message to show when there are no addresses in a person's wallet.
        /// </summary>
        private const string MessageNoAddresses = "You appear to have no addresses in your Ethereum wallet - we will continue to as soon as you add some in your wallet application.";

        /// <summary>
        /// Defines the message to show when the network is marked as syncing.
        /// </summary>
        private const string MessageSyncing = "The Ethereum network is syncing with your wallet, please wait...";

        /// <summary>
        /// Defines the message to show when a synced ethereum client cannot find SIFT.
        /// </summary>
        private const string MessageUnknownContractState = "The SIFT smart contract cannot be found, are you on the correct ethereum network?";

        /// <summary>
        /// Defines the background thread we use to check the status of the ethereum network.
        /// </summary>
        private Thread _thread;

        /// <summary>
        /// Defines the text we display on the splash screen.
        /// </summary>
        private string _statusText;

        /// <summary>
        /// Defines the header we display on the splash screen.
        /// </summary>
        private string _statusHeader;

        /// <summary>
        /// Defines whether or not the splash screen thread should be running.
        /// </summary>
        private bool _isAlive;

        /// <summary>
        /// Defines whether the exit button is visible.
        /// </summary>
        private bool _isErrorState;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the header we display on the splash screen.
        /// </summary>
        public string StatusHeader
        {
            get { return _statusHeader; }
            private set
            {
                if (_statusHeader == value)
                    return;
                _statusHeader = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the text we display on the splash screen.
        /// </summary>
        public string StatusText
        {
            get { return _statusText; }
            private set
            {
                if (_statusText == value)
                    return;
                _statusText = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets whether the window is showing an error state is visible.
        /// </summary>
        public bool IsErrorState
        {
            get { return _isErrorState; }
            set
            {
                if (_isErrorState == value)
                    return;
                _isErrorState = value;
                NotifyPropertyChanged();
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Create a new instance of this class.
        /// </summary>
        /// <param name="ethereumManager">
        /// The ethereum manager that this view model uses to obtain information.
        /// </param>
        public SplashScreenViewModel(EthereumManager ethereumManager)
            : base(ethereumManager)
        {
            // Setup defaults
            StatusText = MessageConnecting;

            // Start our thread that waits for ethereum network contact to have been established
            _thread = new Thread(ThreadEntry) { IsBackground = true, Name = "Startup Ethereum Monitoring" };
            _isAlive = true;
            _thread.Start();
        }
        #endregion

        /// <summary>
        /// This method is the main entry point for the ethereum checks and it is responsible for changing message content.
        /// </summary>
        private void ThreadEntry()
        {
            // Always check for updates first
            StatusText = "Checking for updates to sift-win...";
            StatusHeader = "Please Wait...";
            try
            {
                // Login to update API
                Logger.ApplicationInstance.Debug("Checking for updated version of SIFT");
                AuthenticationClient authClient = new AuthenticationClient();
                AuthenticateUserResponse authResponse = authClient.AuthenticateJwtAsync(new AuthenticateUserRequest
                {
                    Username = "sift-win-updatecheck",
                    Password = "exXzzgY3AEANtg8V"
                }).Result;
                if (authResponse == null)
                    throw new Exception("Null response when checking authentication details");

                // Call to get latest summary information
                ProductClient client = new ProductClient();
                ProductSummaryResponse response = client.ProductSummaryGetAsync("D7682386-897C-4798-84B3-911EDEB8BD44", "BEARER " + authResponse?.JsonWebToken).Result;
                if (response == null)
                    throw new Exception("Null response when checking product details");

                // Determine any changes to state an update UI accordingly
                Logger.ApplicationInstance.Debug("Latest version is " + response.LatestVersion + " from " + response.LatestDownloadUrl);
                if (!string.IsNullOrEmpty(response.LatestVersion))
                {
                    Logger.ApplicationInstance.Info("A new version of SIFT - version " + response.LatestVersion + " is available from " + response.LatestDownloadUrl);
                    Version version = Version.Parse(response.LatestVersion);
                    if (!System.Diagnostics.Debugger.IsAttached)
                        if (Assembly.GetEntryAssembly().GetName().Version < version && MessageBox.Show("A new version of SIFT is available (" + version + ").  Would you like to download it now?", "Smart Investment Fund Token (SIFT)", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                            System.Diagnostics.Process.Start(string.IsNullOrWhiteSpace(response.LatestDownloadUrl) ? "http://smartift.com/sift-win/latest" : response.LatestDownloadUrl);
                }
            }
            catch (Exception ex)
            {
                Logger.ApplicationInstance.Error("Failed to check for latest version", ex);
            }
            finally
            {
            }

            DateTime started = DateTime.UtcNow;
            StatusText = MessageConnecting;
            while (_isAlive)
            {
                // Break out of loop if we know what we're doing
                if (_ethereumManager.ContractPhase != ContractPhase.Unknown && !_ethereumManager.IsSyncing && _ethereumManager.LastChecksSuccessful && _ethereumManager.BlockNumber > 0 && _ethereumManager.Accounts.Count > 0)
                    break;

                // If we've just gone over 5 seconds display a "this may be broken" message and enable exit button
                if (DateTime.UtcNow > started.AddSeconds(5) || _ethereumManager.LastChecksSuccessful)
                {
                    StatusHeader = "Error!";
                    if (!_ethereumManager.LastChecksSuccessful || _ethereumManager.BlockNumber < 1)
                        StatusText = MessageDelayedStart;
                    else if (_ethereumManager.Accounts.Count < 1)
                        StatusText = MessageNoAddresses;
                    else if (_ethereumManager.IsSyncing)
                        StatusText = MessageSyncing;
                    else
                        StatusText = MessageUnknownContractState;
                    IsErrorState = true;
                }

                // Wait to retry
                Thread.Sleep(100);
            }

            // If we're still alive show appropriate window
            if (_isAlive)
            {
                // Show correct window and close the splash screen
                Action act = () =>
                {
                    Window mainWindow = Application.Current.MainWindow;
                    Window newWindow;
                    if (_ethereumManager.ContractPhase == ContractPhase.Ico)
                        newWindow = new IcoWindow { DataContext = new IcoViewModel(_ethereumManager) };
                    else
                        newWindow = new PostIcoWindow { DataContext = new PostIcoViewModel(_ethereumManager) };
                    newWindow.Show();
                    Application.Current.MainWindow = newWindow;
                    mainWindow.Close();
                };
                Application.Current.Dispatcher.BeginInvoke(act);
            }
        }

        #region IDisposable Implementation
        /// <summary>
        /// Free up our resources.
        /// </summary>
        public void Dispose()
        {
            _isAlive = false;
            _thread?.Join();
            _thread = null;
        }
        #endregion
    }
}