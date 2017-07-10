using Guytp.Logging;
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

        /// <summary>
        /// Defines the transaction currently being mined after a purchase, if one is set.
        /// </summary>
        private EnqueuedTransaction _transactionToMine;

        /// <summary>
        /// Defines the dialog that is shown when a transaction is being confirmed.
        /// </summary>
        private SiftDialog _miningTransactionDialog;

        /// <summary>
        /// Defines the view model for the transaction confirmation popup.
        /// </summary>
        private SiftDialogViewModel _miningTransactionDialogViewModel;
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
                IsInvestmentBalanceSufficient = value > 1;
                NotifyPropertyChanged();
                NotifyPropertyChanged("IsInvestmentBalanceSufficient");
            }
        }

        /// <summary>
        /// Gets whether the balance in the investment account is enough to buy SIFT.
        /// </summary>
        public bool IsInvestmentBalanceSufficient { get; private set; }

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
        /// Gets how many SIFT have been issued in total.
        /// </summary>
        public ulong TotalSiftIssued
        {
            get { return _ethereumManager.TotalSupply; }
        }

        /// <summary>
        /// Gets the transaction currently being mined after a purchase, if one is set.
        /// </summary>
        public EnqueuedTransaction TransactionToMine
        {
            get { return _transactionToMine; }
            private set
            {
                if (_transactionToMine == value)
                    return;
                _transactionToMine = value;
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
            _ethereumManager.PropertyChanged += OnEthereumManagerPropertyChanged;
            IsAccountSelectionVisible = Accounts.Count != 1;
            SiftInvestCommand = new AwaitableDelegateCommand(Invest);
            SiftIncreaseQuantityCommand = new DelegateCommand(() => { SiftAmountToPurchase++; });
            SiftDecreaseQuantityCommand = new DelegateCommand(() => { SiftAmountToPurchase--; });
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Handle the transaction we're mining having it's properties updated.
        /// </summary>
        /// <param name="sender">
        /// The event sender.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private void OnTransactionToMinePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            // Only process changes indicating completion
            EnqueuedTransaction transaction = sender as EnqueuedTransaction;
            if (transaction == null || TransactionToMine == null)
                return;
            if (!transaction.Completed)
                return;

            // Clear out the transaction to mine
            TransactionToMine = null;
            SiftDialogViewModel viewModel = _miningTransactionDialogViewModel;

            // Display a message accordingly
            viewModel.IsReturnButtonVisible = true;
            if (transaction.WasSuccessful)
            {
                Logger.ApplicationInstance.Info("Purchase of SIFT added to block " + transaction.Receipt.BlockNumber.Value.ToString() + " for " + transaction.Receipt.TransactionHash + " at index " + transaction.Receipt.TransactionIndex.Value.ToString());
                viewModel.Title = "SIFT Purchase Successful";
                viewModel.Message = "Congratulations!  Your transaction for the purchase of SIFT has gone onto the Ethereum network.  Your new balance will be updated shortly and will be reflected in your Ethereum wallet.";
                viewModel.IsSiftLogoVisible = true;
                viewModel.IsEthereumAnimatedLogoVisible = false;
            }
            else
            {
                Logger.ApplicationInstance.Error("TransactionToMine failed with message.  " + transaction.ErrorDetails);
                viewModel.Title = "Problem Purchasing SIFT";
                viewModel.Message = "There was a problem processing your transaction for SIFT.  " + transaction.ErrorDetails;
            }
        }


        /// <summary>
        /// Handle a property changing in the ethereum manager.
        /// </summary>
        /// <param name="sender">
        /// The event sender.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private void OnEthereumManagerPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.PropertyName) || e.PropertyName == "TotalSiftIssued")
                NotifyPropertyChanged("TotalSiftIssued");
        }

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
            if (!response.WasSuccessful)
                SiftDialog.ShowDialog("SIFT Investment Problem", "Sorry, there was a problem processing your transaction.  Your SIFT could not be purchased at this time." + Environment.NewLine + Environment.NewLine + response.FailureReason);
            else
            {
                TransactionToMine = _ethereumManager.EnqueueTransactionPendingReceipt(response.TransactionHash);
                if (TransactionToMine == null)
                    SiftDialog.ShowDialog("SIFT Delayed Investment", "Your transaction to buy SIFT was successfully sent with hash " + response.TransactionHash + ", but we could not validate the transaction.  Your balance should update shortly, but if not please retry the transaction after checking your Ethereum wallet.");
                else
                {
                    // Hookup to wait to hear the status, or process it immediately if we have it
                    Action act = () =>
                    {
                        TransactionToMine.PropertyChanged += OnTransactionToMinePropertyChanged;
                        _miningTransactionDialogViewModel = new SiftDialogViewModel
                        {
                            IsEthereumAnimatedLogoVisible = true,
                            IsLogButtonVisible = true,
                            Title = "Confirming SIFT Investment",
                            Message = "Your transaction to invest in SIFT has been sent to the Ethereum network.  Depending on the current network congestion it may take anywhere between a few seconds and minutes for the transaction to confirm.  If the transaction does not confirm within a few minutes you can check your Ethereum wallet for more information." + Environment.NewLine + Environment.NewLine + "Please wait..."
                        };
                        _miningTransactionDialog = new SiftDialog
                        {
                            DataContext = _miningTransactionDialogViewModel,
                            Owner = Application.Current.MainWindow
                        };
                        _miningTransactionDialogViewModel.CloseRequested += (s, e) =>
                        {
                            _miningTransactionDialog.Close();
                            _miningTransactionDialog = null;
                            _miningTransactionDialogViewModel = null;
                        };
                        _miningTransactionDialog.ShowDialog();
                    };
                    if (Application.Current.Dispatcher.CheckAccess())
                        act();
                    else
                        Application.Current.Dispatcher.Invoke(act);
                }
            }

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

            // Determine maximum purchase volume - do the raw calculation then factor in gas to see if we need to take away one sift
            uint maximumPurchaseVolume = (uint)(SelectedAccount.BalanceWei / EthereumManager.WeiPerSift);
            decimal spendAmount = maximumPurchaseVolume * EthereumManager.WeiPerSift;
            TransactionGasInfo gasInfo = _ethereumManager.DefaultGasInfo;
            decimal totalSpend = spendAmount + gasInfo.GasCost;
            if (totalSpend > SelectedAccount.BalanceWei)
            {
                if (maximumPurchaseVolume == 1)
                {
                    SiftPurchaseIsVisible = false;
                    return;
                }
                maximumPurchaseVolume--;
            }

            // Setup the various settings for this account
            SiftPurchaseIsVisible = true;
            SiftMaximumPurchase = maximumPurchaseVolume;
            SiftAmountToPurchase = SiftMaximumPurchase;
            SiftInvestIsEnabled = SiftAmountToPurchase > 0;
        }
    }
}