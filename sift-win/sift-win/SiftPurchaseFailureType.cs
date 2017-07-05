namespace Lts.Sift.WinClient
{
    /// <summary>
    /// Defines the different reasons that a SIFT purchase can fail.
    /// </summary>
    public enum SiftPurchaseFailureType
    {
        /// <summary>
        /// Indicates the purchase failed for an unknown reason.
        /// </summary>
        Unknown,

        /// <summary>
        /// Indicates the selected account did not have the gas to complete the purchase.
        /// </summary>
        InsufficientGas,

        /// <summary>
        /// Indicates the selected account did not have the funds for purchase.
        /// </summary>
        InsufficientFunds,

        /// <summary>
        /// Indicates a problem connecting to the ethereum RPC interface.
        /// </summary>
        RpcError,

        /// <summary>
        /// Indicates the supplied account address is not one known by our wallet.
        /// </summary>
        UnknownAccount,

        /// <summary>
        /// Indicates the user chose to cancel the transaction.
        /// </summary>
        UserCancelled,

        /// <summary>
        /// Indicates a problem unlocking the account.
        /// </summary>
        UnlockError,

        /// <summary>
        /// Indicates the personal RPC hasn't been enabled in geth.
        /// </summary>
        MissingRpcPersonal,

        /// <summary>
        /// Indicates no receipt was retrieved from the network as mining couldn't be automated.
        /// </summary>
        NoReceipt
    }
}