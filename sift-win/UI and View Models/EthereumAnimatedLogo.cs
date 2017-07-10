using System.Windows;

namespace Lts.Sift.WinClient
{
    /// <summary>
    /// The ethereum animated logo shows an ethereum logo that slightly fades in and out.
    /// </summary>
    public class EthereumAnimatedLogo : BaseAnimatedLogo
    {
        /// <summary>
        /// Create a new instance of this class.
        /// </summary>
        public EthereumAnimatedLogo()
        {
            // Add our frames
            FrameworkElement[] frames = new FrameworkElement[4];
            frames[0] = AddFrame("pack://application:,,,/sift-win;component/Images/sift-win-logo-eth-animation-01.png");
            frames[1] = AddFrame("pack://application:,,,/sift-win;component/Images/sift-win-logo-eth-animation-02.png");
            frames[2] = AddFrame("pack://application:,,,/sift-win;component/Images/sift-win-logo-eth-animation-03.png");
            frames[3] = AddFrame("pack://application:,,,/sift-win;component/Images/sift-win-logo-eth-animation-04.png");

            // Now setup the animation
            const int interval = 400;
            AddFade(frames[0], frames[1], interval, 0);
            AddFade(frames[1], frames[2], interval, interval);
            AddFade(frames[2], frames[3], interval, interval * 2);
        }
    }
}