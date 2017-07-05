using System.Windows.Input;

namespace Lts.Sift.WinClient
{
    /// <summary>
    /// This class is a base view model that can be inherited by others and provides minimal basic functionality.
    /// </summary>
    public abstract class BaseViewModel : BasePropertyChangedObject
    {
        #region Declarations
        /// <summary>
        /// Defines whether or not the UI is enabled.
        /// </summary>
        private bool _isUiEnabled;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the command to exit the current application.
        /// </summary>
        public ICommand ExitCommand { get; private set; }

        /// <summary>
        /// Gets or sets whether or not the UI is enabled.
        /// </summary>
        public bool IsUiEnabled
        {
            get { return _isUiEnabled; }
            set
            {
                if (_isUiEnabled == value)
                    return;
                _isUiEnabled = value;
                NotifyPropertyChanged();
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Create a new instance of this class.
        /// </summary>
        protected BaseViewModel()
        {
            ExitCommand = new DelegateCommand(Exit);

        }
        #endregion

        #region Commands
        /// <summary>
        /// Shuts down the application.
        /// </summary>
        private void Exit()
        {
            IsUiEnabled = false;
            System.Windows.Application.Current.Shutdown();
        }
        #endregion

    }
}