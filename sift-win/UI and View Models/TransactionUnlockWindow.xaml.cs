using System;
using System.Windows;
using System.Windows.Controls;

namespace Lts.Sift.WinClient
{
    /// <summary>
    /// This window allows a user to confirm a transaction and unlock their account as well as selecting how much gas they want to spend.
    /// </summary>
    public partial class TransactionUnlockWindow : BaseDragableWindow
    {
        #region Constructors
        /// <summary>
        /// Create a new instance of this class.
        /// </summary>
        public TransactionUnlockWindow()
        {
            // Hookup to XAML and initialise components
            InitializeComponent();

            // Hookup to events
            DataContextChanged += OnDataContextChanged;
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Handle the data context changing.
        /// </summary>
        /// <param name="sender">
        /// The event sender.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            TransactionUnlockViewModel oldViewModel = e.OldValue as TransactionUnlockViewModel;
            TransactionUnlockViewModel newViewModel = e.NewValue as TransactionUnlockViewModel;
            if (oldViewModel != null)
                oldViewModel.ReadyToClose -= OnViewModelReadyToClose;
            if (newViewModel != null)
                newViewModel.ReadyToClose += OnViewModelReadyToClose;
        }

        /// <summary>
        /// Handle the view model being ready to close.
        /// </summary>
        /// <param name="sender">
        /// The event sender.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private void OnViewModelReadyToClose(object sender, EventArgs e)
        {
            Dispatcher.Invoke(new Action(() => { Close(); }));
        }
        /// <summary>
        /// Handle the password being changed.
        /// </summary>
        /// <param name="sender">
        /// The event sender.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private void OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            TransactionUnlockViewModel vm = (TransactionUnlockViewModel)DataContext;
            vm.Password = ((PasswordBox)sender).SecurePassword;
        }
        #endregion
    }
}