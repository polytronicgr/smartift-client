using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Lts.Sift.WinClient
{
    /// <summary>
    /// This class holds the current state of the main window when the contract is in ICO mode of operation.
    /// </summary>
    public class IcoViewModel : BaseViewModel
    {
        #region Declarations
        /// <summary>
        /// Defines the currently selected ethereum account.
        /// </summary>
        private EthereumAccount _selectedAccount;

        /// <summary>
        /// Defines whether or not the account selection pane is visible.
        /// </summary>
        private bool _isAccountSelectionVisible;

        /// <summary>
        /// Defines whether or not the purchase section is visible.
        /// </summary>
        private bool _siftPurchaseIsVisible;

        /// <summary>
        /// Defines the cost to purchase the currently selected quantity of SIFT.
        /// </summary>
        private decimal _siftCostToPurchase;

        /// <summary>
        /// Defines the maximum volume of SIFT that can be purchased with the current balance.
        /// </summary>
        private uint _siftMaximumPurchase;

        /// <summary>
        /// Defines the amount of SIFT that the user would like to purchase.
        /// </summary>
        private uint _siftAmountToPurchase;

        /// <summary>
        /// Defines whether or not the invest button is enabled.
        /// </summary>
        private bool _siftInvestIsEnabled;

        /// <summary>
        /// Defines whether the invest increase quantity button is enabled.
        /// </summary>
        private bool _siftIncreaseQuantityIsEnabled;

        /// <summary>
        /// Defines whether the invest decrease quantity button is enabled.
        /// </summary>
        private bool _siftDecreaseQuantityIsEnabled;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the list of ethereum accounts we know about.
        /// </summary>
        public ObservableCollection<EthereumAccount> Accounts { get { return _ethereumManager.Accounts; } }

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

                // Unbind from old and bind to new as appropriate for balance updates
                if (_selectedAccount != null)
                    _selectedAccount.PropertyChanged -= SelectedAccountOnPropertyChanged;
                if (value != null)
                    value.PropertyChanged += SelectedAccountOnPropertyChanged;

                // Update value and UI accordingly
                _selectedAccount = value;
                NotifyPropertyChanged();
                UpdateSiftPurchaseSettings();
            }
        }

        /// <summary>
        /// Gets whether or not the account selection pane is visible.
        /// </summary>
        public bool IsAccountSelectionVisible
        {
            get { return _isAccountSelectionVisible; }
            private set
            {
                if (_isAccountSelectionVisible == value)
                    return;
                _isAccountSelectionVisible = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets whether or not the purchase section is visible.
        /// </summary>
        public bool SiftPurchaseIsVisible
        {
            get { return _siftPurchaseIsVisible; }
            private set
            {
                if (_siftPurchaseIsVisible == value)
                    return;
                _siftPurchaseIsVisible = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the cost to purchase the currently selected quantity of SIFT.
        /// </summary>
        public decimal SiftCostToPurchase
        {
            get { return _siftCostToPurchase; }
            private set
            {
                if (_siftCostToPurchase == value)
                    return;
                _siftCostToPurchase = value;
                NotifyPropertyChanged();
            }
        }
        
        /// <summary>
        /// Gets the maximum volume of SIFT that can be purchased with the current balance.
        /// </summary>
        public uint SiftMaximumPurchase
        {
            get { return _siftMaximumPurchase; }
            private set
            {
                if (_siftMaximumPurchase == value)
                    return;
                _siftMaximumPurchase = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the amount of SIFT that the user would like to purchase.
        /// </summary>
        public uint SiftAmountToPurchase
        {
            get { return _siftAmountToPurchase; }
            set
            {
                if (_siftAmountToPurchase == value)
                    return;
                _siftAmountToPurchase = value;
                NotifyPropertyChanged();
                SiftCostToPurchase = EthereumManager.WeiPerSift * value / 1000000000000000000;
                SiftInvestIsEnabled = SiftAmountToPurchase > 0;
                SiftIncreaseQuantityIsEnabled = SiftAmountToPurchase < SiftMaximumPurchase;
                SiftDecreaseQuantityIsEnabled = SiftAmountToPurchase > 0;
            }
        }

        /// <summary>
        /// Gets whether or not the invest button is enabled.
        /// </summary>
        public bool SiftInvestIsEnabled
        {
            get { return _siftInvestIsEnabled; }
            private set
            {
                if (_siftInvestIsEnabled == value)
                    return;
                _siftInvestIsEnabled = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets whether the invest increase quantity button is enabled.
        /// </summary>
        public bool SiftIncreaseQuantityIsEnabled
        {
            get { return _siftIncreaseQuantityIsEnabled; }
            set
            {
                if (_siftIncreaseQuantityIsEnabled == value)
                    return;
                _siftIncreaseQuantityIsEnabled = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets whether the invest decrease quantity button is enabled.
        /// </summary>
        public bool SiftDecreaseQuantityIsEnabled
        {
            get { return _siftDecreaseQuantityIsEnabled; }
            set
            {
                if (_siftDecreaseQuantityIsEnabled == value)
                    return;
                _siftDecreaseQuantityIsEnabled = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the command to use to send an investment transaction to the blockchain.
        /// </summary>
        public ICommand SiftInvestCommand { get; private set; }

        /// <summary>
        /// Gets the command to increase the selected quantity of SIFT to purchase.
        /// </summary>
        public ICommand SiftIncreaseQuantityCommand { get; private set; }

        /// <summary>
        /// Gets the command to decrease the selected quantity of SIFT to purchase.
        /// </summary>
        public ICommand SiftDecreaseQuantityCommand { get; private set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Create a new instance of this class.
        /// </summary>
        /// <param name="ethereumManager">
        /// The ethereum manager that this view model uses to obtain information.
        /// </param>
        public IcoViewModel(EthereumManager ethereumManager)
            : base(ethereumManager)
        {
            if (Accounts.Count > 0)
                SelectedAccount = Accounts[0];
            _ethereumManager.Accounts.CollectionChanged += OnAccountsChanged;
            IsAccountSelectionVisible = Accounts.Count != 1;
            SiftInvestCommand = new AwaitableDelegateCommand(Invest);
            SiftIncreaseQuantityCommand = new DelegateCommand(() => { SiftAmountToPurchase++; });
            SiftDecreaseQuantityCommand = new DelegateCommand(() => { SiftAmountToPurchase--; });
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
            // If this has just been populated then selected the account
            if (Accounts.Count > 0 && e.OldItems.Count == 0 && Accounts.Count == e.NewItems.Count)
                SelectedAccount = Accounts[0];
            IsAccountSelectionVisible = Accounts.Count != 1;
        }

        /// <summary>
        /// Handle the selected account having a balance updated.
        /// </summary>
        /// <param name="sender">
        /// The event sender.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private void SelectedAccountOnPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.PropertyName) || e.PropertyName == "BalanceWei")
                UpdateSiftPurchaseSettings();
        }
        #endregion

        #region Commands
        /// <summary>
        /// Perform an investment in SIFT using the current UI settings.
        /// </summary>
        private async Task Invest()
        {
            // Disable UI
            SiftInvestIsEnabled = false;

            // Perform the purchase
            SiftPurchaseResponse response = await _ethereumManager.PurchaseSift(SelectedAccount.Address, SiftAmountToPurchase);
            if (response.WasSuccessful)
                MessageBox.Show("Congratulations!  Your SIFT transaction was successfully processed and your balance will be reflected shortly.");
            else if (response.FailureType != SiftPurchaseFailureType.UserCancelled)
                MessageBox.Show("Sorry, there was a problem processing your transaction.  Your SIFT could not be purchased at this time." + Environment.NewLine + Environment.NewLine + response.FailureReason);

            // Update the UI
            UpdateSiftPurchaseSettings();
        }
        #endregion

        /// <summary>
        /// Updates UI settings around the SIFT purchasing based on the currently selected account.
        /// </summary>
        private void UpdateSiftPurchaseSettings()
        {
            // If we haven't got an account or no balance, hide the buy section
            if (SelectedAccount == null || SelectedAccount.BalanceWei < EthereumManager.WeiPerSift)
            {
                SiftPurchaseIsVisible = false;
                return;
            }

            // Setup the various settings for this account
            SiftPurchaseIsVisible = true;
            SiftMaximumPurchase = (uint)(SelectedAccount.BalanceWei / EthereumManager.WeiPerSift);
            SiftAmountToPurchase = SiftMaximumPurchase;
            SiftInvestIsEnabled = SiftAmountToPurchase > 0;
        }

        // Ethereum Manager gets sift account balances

        // Show some kind of activity indicator whilst purchase is in progress
        // Test with invalid password
        // Ability to send double gas

        // Show ownership as % of total fund
        // Show total issuance

        // Auto-update support built in
        // Installer

        // Logging config stored
        // Command line connect URL can be passed in
    }
}