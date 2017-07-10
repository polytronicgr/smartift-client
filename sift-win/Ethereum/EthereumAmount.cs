using System;

namespace Lts.Sift.WinClient
{
    /// <summary>
    /// This class wraps the base unit of Wei to help display more user-friendly units and rounding.
    /// </summary>
    public class EthereumAmount
    {
        #region Properties
        /// <summary>
        /// Gets the best display FriendlyAmount (i.e. divide and round appropriately for ether vs wei).
        /// </summary>
        public decimal FriendlyAmount { get; private set; }

        /// <summary>
        /// Gets the best display unit for the FriendlyAmount.
        /// </summary>
        public string FriendlyUnit { get; private set; }

        /// <summary>
        /// Gets the raw amount of wei this amount describes.
        /// </summary>
        public decimal Wei { get; private set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Create a new instance of this class.
        /// </summary>
        /// <param name="wei">
        /// The wei to create a friendly amount from.
        /// </param>
        public EthereumAmount(decimal wei)
        {
            // Store this value
            Wei = wei;

            // Now we need to determine best units - let's start with dividing it by 18 - if it's <= 1 then we display in ether
            decimal ether = Wei / 1000000000000000000;
            decimal finney = Wei / 1000000000000000;
            decimal szabo = Wei / 1000000000000;
            decimal gwei = Wei / 1000000000;
            decimal mwei = Wei / 1000000;
            decimal kwei = Wei / 1000;
            if (ether >= 1)
            {
                FriendlyAmount = Math.Round(ether, 2);
                FriendlyUnit = "ETH";
            }
            else if (finney >= 1)
            {
                FriendlyAmount = Math.Round(finney, 2);
                FriendlyUnit = "finney";
            }
            else if (szabo >= 1)
            {
                FriendlyAmount = Math.Round(szabo, 2);
                FriendlyUnit = "szabo";
            }
            else if (gwei >= 1)
            {
                FriendlyAmount = Math.Round(gwei, 2);
                FriendlyUnit = "Gwei";
            }
            else if (mwei >= 1)
            {
                FriendlyAmount = Math.Round(mwei, 2);
                FriendlyUnit = "Mwei";
            }
            else if (kwei >= 1)
            {
                FriendlyAmount = Math.Round(kwei, 2);
                FriendlyUnit = "Kwei";
            }
            else
            {
                FriendlyAmount = Wei;
                FriendlyUnit = "wei";
            }
        }
        #endregion
    }
}