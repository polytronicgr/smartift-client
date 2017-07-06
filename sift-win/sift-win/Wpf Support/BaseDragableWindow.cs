using System.Windows;

namespace Lts.Sift.WinClient
{
    /// <summary>
    /// This window provides basic support that it can be dragged when the left mouse button is held.
    /// </summary>
    public class BaseDragableWindow : Window
    {
        #region Constructors
        /// <summary>
        /// Create a new instance of this class.
        /// </summary>
        protected BaseDragableWindow()
        {
            MouseLeftButtonDown += OnMouseLeftButtonDown;
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