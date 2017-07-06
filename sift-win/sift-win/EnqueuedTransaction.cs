using Nethereum.RPC.Eth.DTOs;
using System;

namespace Lts.Sift.WinClient
{
    /// <summary>
    /// This class wraps up a transaction hash and is enqueued pending mining.  Once it has been mined, successfully or otherwise, it emits an event.
    /// </summary>
    public class EnqueuedTransaction : BasePropertyChangedObject
    {
        #region Properties
        /// <summary>
        /// Gets whether or not this transaction was successfully mined.
        /// </summary>
        public bool WasSuccessful { get; private set; }

        /// <summary>
        /// Gets the reason a transaction failed if WasSuccessful is set to false.
        /// </summary>
        public string ErrorDetails { get; private set; }

        /// <summary>
        /// Gets the hash of the transaction that is enqueued.
        /// </summary>
        public string TransactionHash { get; private set; }

        /// <summary>
        /// Gets the receipt for a transaction when WasSuccessful is true.
        /// </summary>
        public TransactionReceipt Receipt { get; private set; }

        /// <summary>
        /// Gets whether or not this transaction has finished processing.
        /// </summary>
        public bool Completed { get; private set; }

        /// <summary>
        /// Gets the date that this tranasction was enqueued.
        /// </summary>
        public DateTime Enqueued { get; private set; }

        /// <summary>
        /// Gets how old this transaction is.
        /// </summary>
        public TimeSpan Age => DateTime.UtcNow.Subtract(Enqueued);
        #endregion

        #region Constructors
        /// <summary>
        /// Create a new instance of this class.
        /// </summary>
        /// <param name="transactionHash">
        /// The hash of the transaction that is enqueued.
        /// </param>
        public EnqueuedTransaction(string transactionHash)
        {
            Enqueued = DateTime.UtcNow;
            Completed = false;
            TransactionHash = transactionHash;
        }
        #endregion

        /// <summary>
        /// Marks the transaction as successfully mined.
        /// </summary>
        /// <param name="receipt">
        /// The receipt confirming the transaction.
        /// </param>
        public void MarkSuccess(TransactionReceipt receipt)
        {
            WasSuccessful = true;
            Completed = true;
            Receipt = receipt;
            NotifyPropertyChanged("WasSuccessful");
            NotifyPropertyChanged("Completed");
            NotifyPropertyChanged("Receipt");
        }

        /// <summary>
        /// Marks the transaction as having failed to be mined.
        /// </summary>
        /// <param name="errorDetails">
        /// The reason that the transaction failed.
        /// </param>
        public void MarkFailed(string errorDetails)
        {
            WasSuccessful = false;
            ErrorDetails = errorDetails;
            Completed = true;
            NotifyPropertyChanged("WasSuccessful");
            NotifyPropertyChanged("Completed");
            NotifyPropertyChanged("ErrorDetails");
        }
    }
}