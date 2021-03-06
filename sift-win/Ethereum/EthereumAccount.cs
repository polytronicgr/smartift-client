﻿using System;

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
        private ulong _siftBalance;

        /// <summary>
        /// Defines the shareholding percentage (0-100) for this account.
        /// </summary>
        private decimal _shareholdingPercentage;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the address for this account.
        /// </summary>
        public string Address { get; private set; }

        /// <summary>
        /// Gets or sets the current sift balance for this account.
        /// </summary>
        public ulong SiftBalance
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
        /// Gets the percentage shareholding that this user has.
        /// </summary>
        public decimal ShareholdingPercentage
        {
            get { return _shareholdingPercentage; }
            set
            {
                if (_shareholdingPercentage == value)
                    return;
                _shareholdingPercentage = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged("DisplayShareholdingPercentage");
            }
        }

        /// <summary>
        /// Gets the display text indicating the shareholding as a percentage of the total fund.
        /// </summary>
        public string DisplayShareholdingPercentage
        {
            get
            {
                if (ShareholdingPercentage <= 0)
                    return "0%";
                else if (ShareholdingPercentage <= 0.01m)
                    return "<0.01%";
                else if (ShareholdingPercentage < 1)
                    return Math.Round(ShareholdingPercentage, 2) + "%";
                else
                    return Math.Round(ShareholdingPercentage, 1) + "%";
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
                if (value == _balanceWei)
                    return;

                // Convert to friendly variant and store it
                EthereumAmount amount = new EthereumAmount(value);
                _balanceWei = value;
                Balance = amount.FriendlyAmount;
                BalanceUnit = amount.FriendlyUnit;

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
        /// <param name="siftBalance">
        /// The SIFT balance that this account has.
        /// </param>
        public EthereumAccount(string address, decimal balanceWei, ulong siftBalance)
        {
            Address = address;
            BalanceWei = balanceWei;
            SiftBalance = siftBalance;
        }
        #endregion
    }
}