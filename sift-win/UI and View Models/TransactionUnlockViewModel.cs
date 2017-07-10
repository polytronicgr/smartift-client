using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Lts.Sift.WinClient
{
    /// <summary>
    /// This view model is used to unlock an account and confirm a transaction.
    /// </summary>
    public class TransactionUnlockViewModel : BasePropertyChangedObject
    {
        #region Declarations
        /// <summary>
        /// Defines the gas multiplier that the user has selected.
        /// </summary>
        private byte _selectedGasMultiplier;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the base gas price for this transaction.
        /// </summary>
        public decimal BaseGasCost { get; private set; }

        /// <summary>
        /// Gets how much gas is needed for this transaction.
        /// </summary>
        public decimal Gas { get; private set; }

        /// <summary>
        /// Gets the maximum gas price multiplier that the user has funds for.
        /// </summary>
        public byte MaximumGasMultiplier { get; private set; }

        /// <summary>
        /// Gets the gas multiplier that the user has selected.
        /// </summary>
        public byte SelectedGasMultiplier
        {
            get { return _selectedGasMultiplier; }
            set
            {
                if (_selectedGasMultiplier == value)
                    return;
                _selectedGasMultiplier = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged("GasCost");
                NotifyPropertyChanged("GasCostEther");
                NotifyPropertyChanged("TotalCost");
                NotifyPropertyChanged("TotalCostUnit");
            }
        }

        /// <summary>
        /// Gets the calculated gas cost based on the selected multiplier.
        /// </summary>
        public decimal GasCost => SelectedGasMultiplier * BaseGasCost * Gas;

        /// <summary>
        /// Gets the gas cost in rounded ether.
        /// </summary>
        public decimal GasCostEther => Math.Round(GasCost / 1000000000000000000m, 8);

        /// <summary>
        /// Gets the total cost for this transaction including gas and the send amount.
        /// </summary>
        public decimal TotalCost => new EthereumAmount(GasCost + WeiAmount).FriendlyAmount;

        /// <summary>
        /// Gets the unit of display for the total cost.
        /// </summary>
        public string TotalCostUnit => new EthereumAmount(GasCost + WeiAmount).FriendlyUnit;

        /// <summary>
        /// Gets who the transaction is being sent from.
        /// </summary>
        public string From { get; private set; }

        /// <summary>
        /// Gets the shortened version of the from address.
        /// </summary>
        public string FromShortened
        {
            get
            {
                return From == null || From.Length < 15 ? From : From.Substring(0, 7) + "..." + From.Substring(From.Length - 5, 5);
            }
        }

        /// <summary>
        /// Gets who the tranasction is being sent to.
        /// </summary>
        public string To { get; private set; }

        /// <summary>
        /// Gets the shortened version of the to address.
        /// </summary>
        public string ToShortened
        {
            get
            {
                return To == null || To.Length < 15 ? To : To.Substring(0, 7) + "..." + To.Substring(To.Length - 5, 5);
            }
        }

        /// <summary>
        /// Gets the amount to be sent with the transaction in whatever unit and rounding is described by DisplayUnit.
        /// </summary>
        public decimal DisplayAmount { get; private set; }

        /// <summary>
        /// Gets the unit (i.e. wei, ether, etc.) that DisplayAmount describes.
        /// </summary>
        public string DisplayUnit { get; private set; }

        /// <summary>
        /// Gets the amount of wei that makes up this transaction.
        /// </summary>
        public decimal WeiAmount { get; private set; }

        /// <summary>
        /// Gets or sets the password to unlock the account with.
        /// </summary>
        public SecureString Password { get; set; }

        /// <summary>
        /// Gets whether or not the user cancelled this transaction.
        /// </summary>
        public bool WasCancelled { get; set; }

        /// <summary>
        /// Gets the command to send the transaction.
        /// </summary>
        public ICommand SendCommand { get; private set; }

        /// <summary>
        /// Gets the command to cancel the transaction.
        /// </summary>
        public ICommand CancelCommand { get; private set; }
        #endregion

        #region Events
        /// <summary>
        /// Indicates the window can be closed.
        /// </summary>
        public event EventHandler ReadyToClose;
        #endregion

        #region Constructors
        /// <summary>
        /// Create a new instance of this class.
        /// </summary>
        /// <param name="baseGasCost">
        /// The base gas price for this transaction.
        /// </param>
        /// <param name="maximumGasMultiplier">
        /// The maximum gas price multiplier that the user has funds for.
        /// </param>
        /// <param name="gas">
        /// How much gas is needed for this transaction.
        /// </param>
        /// <param name="from">
        /// Who the transaction is being sent from.
        /// </param>
        /// <param name="to">
        /// Who the tranasction is being sent to.
        /// </param>
        /// <param name="weiAmount">
        /// The amount of wei that makes up this transaction.
        /// </param>
        public TransactionUnlockViewModel(decimal baseGasCost, byte maximumGasMultiplier, decimal gas, string from, string to, decimal weiAmount)
        {
            BaseGasCost = baseGasCost;
            MaximumGasMultiplier = maximumGasMultiplier;
            Gas = gas;
            From = from;
            To = to;
            EthereumAmount amount = new EthereumAmount(weiAmount);
            DisplayAmount = amount.FriendlyAmount;
            DisplayUnit = amount.FriendlyUnit;
            WeiAmount = weiAmount;
            SelectedGasMultiplier = (byte)(maximumGasMultiplier * 0.25);
            CancelCommand = new DelegateCommand(Cancel);
            SendCommand = new DelegateCommand(Send);
        }
        #endregion

        #region Commands
        /// <summary>
        /// Cancel the transaction.
        /// </summary>
        private void Cancel()
        {
            Password = null;
            WasCancelled = true;
            ReadyToClose?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// Confirm that we want to send the transaction.
        /// </summary>
        private void Send()
        {
            WasCancelled = false;
            ReadyToClose?.Invoke(this, new EventArgs());
        }
        #endregion
    }
}