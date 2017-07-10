using System;
using System.Windows.Input;

namespace Lts.Sift.WinClient
{
    /// <summary>
    /// This view model supports the SiftDialog - an on-screen information popup.
    /// </summary>
    public class SiftDialogViewModel : BaseViewModel
    {
        #region Declarations
        /// <summary>
        /// Defines the title to show on-screen.
        /// </summary>
        private string _title;

        /// <summary>
        /// Defines the main message to display on-screen.
        /// </summary>
        private string _message;

        /// <summary>
        /// Defines whether or not the return button is visible.
        /// </summary>
        private bool _isReturnButtonVisible;

        /// <summary>
        /// Defines whether or not the yes button is visible.
        /// </summary>
        private bool _isYesButtonVisible;

        /// <summary>
        /// Defines whether or not the no button is visible.
        /// </summary>
        private bool _isNoButtonVisible;

        /// <summary>
        /// Defines whether or not the log button is visible.
        /// </summary>
        private bool _isLogButtonVisible;

        /// <summary>
        /// Defines whether or not the sift logo is visible.
        /// </summary>
        private bool _isSiftLogoVisible;

        /// <summary>
        /// Defines whether or not the animated ethereum logo is visible.
        /// </summary>
        private bool _isEthereumAnimatedLogoVisible;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the title to show on-screen.
        /// </summary>
        public string Title
        {
            get { return _title; }
            set
            {
                if (_title == value)
                    return;
                _title = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the main message to display on-screen.
        /// </summary>
        public string Message
        {
            get { return _message; }
            set
            {
                if (_message == value)
                    return;
                _message = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets whether or not the return button is visible.
        /// </summary>
        public bool IsReturnButtonVisible
        {
            get { return _isReturnButtonVisible; }
            set
            {
                if (_isReturnButtonVisible == value)
                    return;
                _isReturnButtonVisible = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets whether or not the yes button is visible.
        /// </summary>
        public bool IsYesButtonVisible
        {
            get { return _isYesButtonVisible; }
            set
            {
                if (_isYesButtonVisible == value)
                    return;
                _isYesButtonVisible = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets whether or not the no button is visible.
        /// </summary>
        public bool IsNoButtonVisible
        {
            get { return _isNoButtonVisible; }
            set
            {
                if (_isNoButtonVisible == value)
                    return;
                _isNoButtonVisible = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets whether or not the log button is visible.
        /// </summary>
        public bool IsLogButtonVisible
        {
            get { return _isLogButtonVisible; }
            set
            {
                if (_isLogButtonVisible == value)
                    return;
                _isLogButtonVisible = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets whether or not the sift logo is visible.
        /// </summary>
        public bool IsSiftLogoVisible
        {
            get { return _isSiftLogoVisible; }
            set
            {
                if (_isSiftLogoVisible == value)
                    return;
                _isSiftLogoVisible = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets whether or not the animated ethereum logo is visible.
        /// </summary>
        public bool IsEthereumAnimatedLogoVisible
        {
            get { return _isEthereumAnimatedLogoVisible; }
            set
            {
                if (_isEthereumAnimatedLogoVisible == value)
                    return;
                _isEthereumAnimatedLogoVisible = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the command to execute when the return button is pressed.
        /// </summary>
        public ICommand ReturnCommand { get; private set; }

        /// <summary>
        /// Gets the command to execute when the yes button is pressed.
        /// </summary>
        public ICommand YesCommand { get; private set; }

        /// <summary>
        /// Gets the command to execute when the no button is pressed.
        /// </summary>
        public ICommand NoCommand { get; private set; }

        /// <summary>
        /// Gets whether or not the return button was pressed.
        /// </summary>
        public bool WasReturnPressed { get; private set; }

        /// <summary>
        /// Gets whether or not the yes button was pressed.
        /// </summary>
        public bool WasYesPressed { get; private set; }

        /// <summary>
        /// Gets whether or not the no button was pressed.
        /// </summary>
        public bool WasNoPressed { get; private set; }
        #endregion

        #region Events
        /// <summary>
        /// Fired when the a button is pressed and the window should be closed.
        /// </summary>
        public event EventHandler CloseRequested;
        #endregion

        #region Constructors
        /// <summary>
        /// Create a new instance of this class.
        /// </summary>
        public SiftDialogViewModel()
            : base(null)
        {
            ReturnCommand = new DelegateCommand(() =>
            {
                WasReturnPressed = true;
                CloseRequested?.Invoke(this, new EventArgs());
            });
            YesCommand = new DelegateCommand(() =>
            {
                WasYesPressed = true;
                CloseRequested?.Invoke(this, new EventArgs());
            });
            NoCommand = new DelegateCommand(() =>
            {
                WasNoPressed = true;
                CloseRequested?.Invoke(this, new EventArgs());
            });
        }
        #endregion
    }
}