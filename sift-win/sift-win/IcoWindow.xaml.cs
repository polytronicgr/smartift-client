using System.Windows;

namespace Lts.Sift.WinClient
{
    /// <summary>
    /// The ICO window displays the state of SIFT during the ICO phase and offers basic functionality to display ownership statistics and allow buying of SIFT.
    /// </summary>
    public partial class IcoWindow : Window
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
        /// Handle the left mouse button being pressed.
        /// </summary>
        /// <param name="sender">
        /// The event sender.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private void OnMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                DragMove();
            }
            catch
            {
                // Intentionally swallowed
            }
        }
        #endregion
    }
}