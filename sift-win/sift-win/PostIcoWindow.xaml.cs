using System.Windows;

namespace Lts.Sift.WinClient
{
    /// <summary>
    /// This window is the main window for managing a user's interaction with SIFT after the ICO has completed.
    /// </summary>
    public partial class PostIcoWindow : Window
    {
        #region Constructors
        /// <summary>
        /// Create a new instance of this class.
        /// </summary>
        public PostIcoWindow()
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