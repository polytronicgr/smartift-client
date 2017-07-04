using System.Windows;

namespace Lts.Sift.WinClient
{
    /// <summary>
    /// The ICO window displays the state of SIFT during the ICO phase and offers basic functionality to display ownership statistics and allow buying of SIFT.
    /// </summary>
    public partial class IcoWindow : Window
    {
        public IcoWindow()
        {
            // Hookup to XAML and initialise components
            InitializeComponent();

            // Create our main view model
            DataContext = new IcoViewModel();
        }
    }
}