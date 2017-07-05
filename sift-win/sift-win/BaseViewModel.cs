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

        /// <summary>
        /// Defines the ethereum manager we use to get network and account information.
        /// </summary>
        protected readonly EthereumManager _ethereumManager;
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
        /// <param name="ethereumManager">
        /// The ethereum manager that this view model uses to obtain information.
        /// </param>
        protected BaseViewModel(EthereumManager ethereumManager)
        {
            ExitCommand = new DelegateCommand(Exit);
            IsUiEnabled = true;
            _ethereumManager = ethereumManager;
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