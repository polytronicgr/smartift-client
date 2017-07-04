using Nethereum.Web3;
using System;
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
        /// Defines the API interface to Ethereum.
        /// </summary>
        private readonly Web3 _web3;

        /// <summary>
        /// Defines the current account balance in ether.
        /// </summary>
        private decimal _etherBalance;

        /// <summary>
        /// Defines the current account balance in SIFT.
        /// </summary>
        private decimal _siftBalance;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the current account balance in ether.
        /// </summary>
        public decimal EtherBalance
        {
            get { return _etherBalance; }
            set
            {
                if (value == _etherBalance)
                    return;
                _etherBalance = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the current account balance in SIFT.
        /// </summary>
        public decimal SiftBalance
        {
            get { return _siftBalance; }
            set
            {
                if (value == _siftBalance)
                    return;
                _siftBalance = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the command to exit the current application.
        /// </summary>
        public ICommand ExitCommand { get; private set; }
        #endregion

        /// <summary>
        /// Create a new instance of this class.
        /// </summary>
        public IcoViewModel()
        {
            _web3 = new Web3("http://localhost:8000");
            string[] accounts = _web3.Eth.Accounts.SendRequestAsync().Result;
            EtherBalance = decimal.Parse(_web3.Eth.GetBalance.SendRequestAsync(accounts[0]).Result.Value.ToString()) / 1000000000000000000m;
        }

        // TODO: Threading model
        // Round based on amount displays (Eth/Wei/etc.)
        // TODO: Command line connect URL
        // TODO: Exit command
        // TODO: Must chose from multiple accounts
        // TODO: Buy SIFT
        // TODO: Show ownership as % of total fund
        // TODO: Show total issuance
        // TODO: (less important for ICO) - send / approve funds
    }
}