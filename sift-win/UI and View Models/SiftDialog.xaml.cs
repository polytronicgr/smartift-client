using System;
using System.Windows;

namespace Lts.Sift.WinClient
{
    /// <summary>
    /// A SiftDialog is a stylised message dialog that aims to serve the same purpose as the MessageBox but with additional functionality.
    /// </summary>
    public partial class SiftDialog : BaseDragableWindow
    {
        #region Constructors
        /// <summary>
        /// Create a new instance of this class.
        /// </summary>
        public SiftDialog()
        {
            // Hookup to XAML and initialise components.
            InitializeComponent();
        }
        #endregion

        /// <summary>
        /// Shows a SIFT dialog with the specified parameters as a dialog.
        /// </summary>
        /// <param name="title">
        /// The title of the message.
        /// </param>
        /// <param name="message">
        /// The message to show.
        /// </param>
        /// <param name="isYesNo">
        /// If this is a yes/no dialog (true) or a single button dialog (false).
        /// </param>
        /// <param name="isEthereum">
        /// If this dialog is related to the ethereum network (true) and as such the ethereum logo should be displayed, otherwise false.
        /// </param>
        /// <returns>
        /// Null if return was pressed, false if No was pressed, true if Yes was pressed.
        /// </returns>
        public static bool? ShowDialog(string title, string message, bool isYesNo = false, bool isEthereum = false)
        {
            Func<bool?> uiCallback = () =>
            {
                SiftDialogViewModel viewModel = new SiftDialogViewModel
                {
                    Title = title,
                    Message = message,
                    IsEthereumAnimatedLogoVisible = isEthereum,
                    IsSiftLogoVisible = !isEthereum,
                    IsLogButtonVisible = true,
                    IsNoButtonVisible = isYesNo,
                    IsReturnButtonVisible = !isYesNo,
                    IsYesButtonVisible = isYesNo
                };
                SiftDialog dialog = new SiftDialog
                {
                    DataContext = viewModel,
                    Owner = Application.Current.MainWindow
                };
                viewModel.CloseRequested += (s, e) => dialog.Close();
                dialog.ShowDialog();
                return !isYesNo ? null : (bool?)viewModel.WasYesPressed;
            };
            if (Application.Current.Dispatcher.CheckAccess())
                return uiCallback();
            else
                return Application.Current.Dispatcher.Invoke(uiCallback);
        }

        /// <summary>
        /// Shows a SIFT dialog without any buttons in non-modal mode.
        /// </summary>
        /// <param name="title">
        /// The title of the message.
        /// </param>
        /// <param name="message">
        /// The message to show.
        /// </param>
        /// <param name="isEthereum">
        /// If this dialog is related to the ethereum network (true) and as such the ethereum logo should be displayed, otherwise false.
        /// </param>
        /// <returns>
        /// The dialog that was shown.
        /// </returns>
        public static SiftDialog ShowButtonless(string title, string message, bool isEthereum = false)
        {
            Func<SiftDialog> uiCallback = () =>
            {
                SiftDialogViewModel viewModel = new SiftDialogViewModel
                {
                    Title = title,
                    Message = message,
                    IsEthereumAnimatedLogoVisible = isEthereum,
                    IsSiftLogoVisible = !isEthereum,
                    IsLogButtonVisible = true,
                    IsNoButtonVisible = false,
                    IsReturnButtonVisible = false,
                    IsYesButtonVisible = false
                };
                SiftDialog dialog = new SiftDialog
                {
                    DataContext = viewModel,
                    Owner = Application.Current.MainWindow
                };
                viewModel.CloseRequested += (s, e) => dialog.Close();
                dialog.Show();
                return dialog;
            };
            if (Application.Current.Dispatcher.CheckAccess())
                return uiCallback();
            else
                return Application.Current.Dispatcher.Invoke(uiCallback);
        }
    }
}