using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Lts.Sift.WinClient
{
    /// <summary>
    /// This class holds the current state of the main window when the contract is in ICO mode of operation.
    /// </summary>
    public class IcoViewModel : BasePropertyChangedObject
    {
        #region Declarations
        /// <summary>
        /// Defines the ethereum manager we use to get network and account information.
        /// </summary>
        private readonly EthereumManager _ethereumManager;

        /// <summary>
        /// Defines the currently selected ethereum account.
        /// </summary>
        private EthereumAccount _selectedAccount;

        /// <summary>
        /// Defines whether or not the UI is enabled.
        /// </summary>
        private bool _isUiEnabled;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the command to exit the current application.
        /// </summary>
        public ICommand ExitCommand { get; private set; }

        /// <summary>
        /// Gets the list of ethereum accounts we know about.
        /// </summary>
        public ObservableCollection<EthereumAccount> Accounts {  get { return _ethereumManager.Accounts; } }

        /// <summary>
        /// Gets or sets the currently selected account.
        /// </summary>
        public EthereumAccount SelectedAccount
        {
            get { return _selectedAccount; }
            set
            {
                if (_selectedAccount == value)
                    return;
                _selectedAccount = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets whether or not the UI is enabled.
        /// </summary>
        public bool IsUiEnabled
        {
            get { return _isUiEnabled; }
            set
            {
                if (_isUiEnabled == value)
                    return;
                _isUiEnabled = value;
                NotifyPropertyChanged();
            }
        }
        #endregion

        /// <summary>
        /// Create a new instance of this class.
        /// </summary>
        /// <param name="ethereumManager">
        /// The ethereum manager that this view model uses to obtain information.
        /// </param>
        public IcoViewModel(EthereumManager ethereumManager)
        {
            IsUiEnabled = true;
            _ethereumManager = ethereumManager;
            if (Accounts.Count > 0)
                SelectedAccount = Accounts[0];
            else
                _ethereumManager.Accounts.CollectionChanged += OnAccountsChanged;
            ExitCommand = new DelegateCommand(Exit);
        }

        #region Commands
        /// <summary>
        /// Shuts down the application.
        /// </summary>
        private void Exit()
        {
            IsUiEnabled = false;
            System.Windows.Application.Current.Shutdown();
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Handle the accounts list loading and select the first account.
        /// </summary>
        /// <param name="sender">
        /// The event sender.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private void OnAccountsChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (Accounts.Count > 0)
            {
                SelectedAccount = Accounts[0];
                _ethereumManager.Accounts.CollectionChanged += OnAccountsChanged;
            }
        }
        #endregion

        // Splash Screen -> Check if ICO, Chose Next Screen -> Requires Ethereum Accounts before Continuing
        // Placeholder for non ICO mode

        // Logging config stored
        // Command line connect URL can be passed in

        // Ethereum Manager gets sift account balances
        // TODO: Buy SIFT
        // TODO: Show ownership as % of total fund
        // TODO: Show total issuance

        // TODO: Auto-update support built in
        // TODO: (less important for ICO) - send / approve funds
    }
}