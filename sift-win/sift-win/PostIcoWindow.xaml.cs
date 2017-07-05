using System.Windows;

namespace Lts.Sift.WinClient
{
    /// <summary>
    /// This window is the main window for managing a user's interaction with SIFT after the ICO has completed.
    /// </summary>
    public partial class PostIcoWindow : BaseDragableWindow
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
    }
}