namespace Lts.Sift.WinClient
{
    /// <summary>
    /// This view model provides support for the main container window that allows users to manage their SIFT holdings.
    /// </summary>
    public class PostIcoViewModel : BaseViewModel
    {
        #region Constructors
        /// <summary>
        /// Create a new instance of this class.
        /// </summary>
        /// <param name="ethereumManager">
        /// The ethereum manager that this view model uses to obtain information.
        /// </param>
        public PostIcoViewModel(EthereumManager ethereumManager)
            : base(ethereumManager)
        {
        }
        #endregion
    }
}