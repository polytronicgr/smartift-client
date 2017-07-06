namespace Lts.Sift.WinClient
{
    /// <summary>
    /// Defines the different phase of operation that the SIFT smart contract can be in.
    /// </summary>
    public enum ContractPhase
    {
        /// <summary>
        /// Indicates the coin is in an unknown state.
        /// </summary>
        Unknown,

        /// <summary>
        /// Indicates the coin is in the ICO phase.
        /// </summary>
        Ico,

        /// <summary>
        /// Indicates the coin is now in the trading phase.
        /// </summary>
        Trading
    }
}