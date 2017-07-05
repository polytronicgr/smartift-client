using System;

namespace Lts.Sift.WinClient
{
    /// <summary>
    /// An Ethereum Account represents summary information about a single account in the ethereum blockchain.
    /// </summary>
    public class EthereumAccount : BasePropertyChangedObject
    {
        #region Declarations
        /// <summary>
        /// Defines the raw balance in wei.
        /// </summary>
        private decimal _balanceWei;

        /// <summary>
        /// Defines the balance in SIFT that this user account.
        /// </summary>
        private uint _siftBalance;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the address for this account.
        /// </summary>
        public string Address { get; private set; }

        /// <summary>
        /// Gets or sets the current sift balance for this account.
        /// </summary>
        public uint SiftBalance
        {
            get
            {
                return _siftBalance;
            }
            set
            {
                if (_siftBalance == value)
                    return;
                _siftBalance = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the raw balance in wei for this account.
        /// </summary>
        public decimal BalanceWei
        {
            get { return _balanceWei; }
            set
            {
                // Store this value
                if (_balanceWei == value)
                    return;
                _balanceWei = value;

                // Now we need to determine best units - let's start with dividing it by 18 - if it's <= 1 then we display in ether
                decimal ether = _balanceWei / 1000000000000000000;
                decimal finney = _balanceWei / 1000000000000000;
                decimal szabo = _balanceWei / 1000000000000;
                decimal gwei = _balanceWei / 1000000000;
                decimal mwei = _balanceWei / 1000000;
                decimal kwei = _balanceWei / 1000;
                if (ether >= 1)
                {
                    Balance = Math.Round(ether, 2);
                    BalanceUnit = "ether";
                }
                else if (finney >= 1)
                {
                    Balance = Math.Round(finney, 2);
                    BalanceUnit = "finney";
                }
                else if (szabo >= 1)
                {
                    Balance = Math.Round(szabo, 2);
                    BalanceUnit = "szabo";
                }
                else if (gwei >= 1)
                {
                    Balance = Math.Round(gwei, 2);
                    BalanceUnit = "Gwei";
                }
                else if (mwei >= 1)
                {
                    Balance = Math.Round(mwei, 2);
                    BalanceUnit = "Mwei";
                }
                else if (kwei >= 1)
                {
                    Balance = Math.Round(kwei, 2);
                    BalanceUnit = "Kwei";
                }
                else
                {
                    Balance = value;
                    BalanceUnit = "wei";
                }

                // Notify all the properties
                NotifyPropertyChanged();
                NotifyPropertyChanged("Balance");
                NotifyPropertyChanged("BalanceUnit");
            }
        }

        /// <summary>
        /// Gets the best display balance (i.e. divide and round appropriately for ether vs wei).
        /// </summary>
        public decimal Balance { get; private set; }

        /// <summary>
        /// Gets the best display unit for the balance.
        /// </summary>
        public string BalanceUnit { get; private set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Create a new instance of this class.
        /// </summary>
        /// <param name="address">
        /// The address of the account.
        /// </param>
        /// <param name="balanceWei">
        /// The balance in wei for the account.
        /// </param>
        public EthereumAccount(string address, decimal balanceWei)
        {
            Address = address;
            BalanceWei = balanceWei;
        }
        #endregion
    }
}