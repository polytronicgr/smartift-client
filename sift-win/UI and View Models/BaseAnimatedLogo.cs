using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Lts.Sift.WinClient
{
    /// <summary>
    /// This class provides a way to animate a fade between frames for an animated logo effect.
    /// </summary>
    public abstract class BaseAnimatedLogo : UserControl
    {
        #region Declarations
        /// <summary>
        /// Defines our storyboard.
        /// </summary>
        private Storyboard _storyboard;

        /// <summary>
        /// Defines our main content grid.
        /// </summary>
        private Grid _grid;
        #endregion

        #region Constructors
        /// <summary>
        /// Create a new instance of this class.
        /// </summary>
        public BaseAnimatedLogo()
        {
            // Hookup to events
            IsVisibleChanged += OnVisibleChanged;

            // Create our container grid
            _grid = new Grid();
            Content = _grid;

            // Create our animation
            _storyboard = new Storyboard()
            {
                RepeatBehavior = RepeatBehavior.Forever,
                AutoReverse = true
            };
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Adds a frame to those available for aniamtion.
        /// </summary>
        /// <param name="imageSource">
        /// The source of the image for the frame.
        /// </param>
        /// <returns>
        /// A new framework element that contains this frame.
        /// </returns>
        public FrameworkElement AddFrame(string imageSource)
        {
            Image image = new Image
            {
                StretchDirection = StretchDirection.DownOnly,
                Stretch = Stretch.Uniform,
                Source = new ImageSourceConverter().ConvertFromString(imageSource) as ImageSource
        };
            _grid.Children.Add(image);
            return image;
        }

        /// <summary>
        /// Adds a fade between two frames.
        /// </summary>
        /// <param name="hideFrame">
        /// The frame to fade out.
        /// </param>
        /// <param name="showFrame">
        /// The frame to fade in.
        /// </param>
        /// <param name="duration">
        /// How long to fade over.
        /// </param>
        /// <param name="beginTime">
        /// When to start the fade.
        /// </param>
        protected void AddFade(FrameworkElement hideFrame, FrameworkElement showFrame, int duration, int beginTime)
        {
            DoubleAnimation frameVisible = new DoubleAnimation(1, new Duration(TimeSpan.FromMilliseconds(duration)))
            {
                BeginTime = TimeSpan.FromMilliseconds(beginTime)
            };
            Storyboard.SetTarget(frameVisible, showFrame);
            Storyboard.SetTargetProperty(frameVisible, new PropertyPath("Opacity"));
            DoubleAnimation frameInvisble = new DoubleAnimation(0, new Duration(TimeSpan.FromMilliseconds(duration)))
            {
                BeginTime = TimeSpan.FromMilliseconds(beginTime)
            };
            Storyboard.SetTargetProperty(frameInvisble, new PropertyPath("Opacity"));
            Storyboard.SetTarget(frameInvisble, hideFrame);
            _storyboard.Children.Add(frameVisible);
            _storyboard.Children.Add(frameInvisble);
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Handles the control having loaded.
        /// </summary>
        /// <param name="sender">
        /// The event sender.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private void OnVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (IsVisible)
                _storyboard.Begin();
            else
                _storyboard.Stop();
        }
        #endregion
    }
}
