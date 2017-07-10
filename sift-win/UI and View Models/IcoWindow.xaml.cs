namespace Lts.Sift.WinClient
{
    /// <summary>
    /// The ICO window displays the state of SIFT during the ICO phase and offers basic functionality to display ownership statistics and allow buying of SIFT.
    /// </summary>
    public partial class IcoWindow : BaseDragableWindow
    {
        #region Constructors
        /// <summary>
        /// Create a new instance of this class.
        /// </summary>
        public IcoWindow()
        {
            // Hookup to XAML and initialise components
            InitializeComponent();
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Handle the logo details being clicked.
        /// </summary>
        /// <param name="sender">
        /// The event sender.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private void OnLogoClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start("http://SmartIFT.com");
        }
        #endregion
    }
}