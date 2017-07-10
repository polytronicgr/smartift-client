using System.Windows;

namespace Lts.Sift.WinClient
{
    /// <summary>
    /// The ethereum animated error logo shows a red variant of ethereum logo that slightly fades in and out.
    /// </summary>
    public class EthereumAnimatedErrorLogo : BaseAnimatedLogo
    {
        /// <summary>
        /// Create a new instance of this class.
        /// </summary>
        public EthereumAnimatedErrorLogo()
        {
            // Add our frames
            FrameworkElement[] frames = new FrameworkElement[2];
            frames[0] = AddFrame("pack://application:,,,/sift-win;component/Images/sift-win-logo-eth-animation-error-01.png");
            frames[1] = AddFrame("pack://application:,,,/sift-win;component/Images/sift-win-logo-eth-animation-error-02.png");

            // Now setup the animation
            const int interval = 800;
            AddFade(frames[0], frames[1], interval, 0);
            AddFade(frames[1], frames[0], interval, interval);
        }
    }
}