namespace Lts.Sift.WinClient
{
    /// <summary>
    /// This class defines 
    /// </summary>
    public class SiftPurchaseResponse
    {
        #region Properties
        /// <summary>
        /// Gets whether or not the purchase was successful.
        /// </summary>
        public bool WasSuccessful { get; private set; }

        /// <summary>
        /// Gets the type of failure for the purchase
        /// </summary>
        public SiftPurchaseFailureType FailureType { get; private set; }

        /// <summary>
        /// Gets additional details describing why the purchase failed.
        /// </summary>
        public string FailureReason { get; private set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new instance of this class.
        /// </summary>
        public SiftPurchaseResponse()
        {
            WasSuccessful = true;
        }

        /// <summary>
        /// Creates a new instance of this class.
        /// </summary>
        /// <param name="failureType">
        /// The type of failure for the purchase.
        /// </param>
        /// <param name="failureReason">
        /// Additional details describing why the purchase failed.
        /// </param>
        public SiftPurchaseResponse(SiftPurchaseFailureType failureType, string failureReason)
        {
            WasSuccessful = false;
            FailureType = failureType;
            FailureReason = failureReason;
        }
        #endregion
    }
}