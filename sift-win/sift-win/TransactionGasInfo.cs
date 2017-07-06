namespace Lts.Sift.WinClient
{
    /// <summary>
    /// This object details information about the cost of sending a transaction.
    /// </summary>
    public class TransactionGasInfo
    {
        #region Properties
        /// <summary>
        /// Gets the cost of gas, per unit, at the current market price.
        /// </summary>
        public decimal GasPrice { get; private set; }

        /// <summary>
        /// Gets the cost of gas for this transaction.
        /// </summary>
        public decimal GasCost { get; private set; }

        /// <summary>
        /// Gets how many gas this transaction uses.
        /// </summary>
        public decimal Gas { get; private set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Create a new instance of this class.
        /// </summary>
        /// <param name="gasPrice">
        /// The cost of gas, per unit, at the current market price.
        /// </param>
        /// <param name="gasCost">
        /// The cost of gas for this transaction.
        /// </param>
        /// <param name="gas">
        /// How many gas this transaction uses.
        /// </param>
        public TransactionGasInfo(decimal gasPrice, decimal gasCost, decimal gas)
        {
            GasPrice = gasPrice;
            GasCost = gasCost;
            Gas = gas;
        }
        #endregion
    }
}