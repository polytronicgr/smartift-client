using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Lts.Sift.WinClient
{
    /// <summary>
    /// This base class can be used by any other class to implement basic property changed notification support.
    /// </summary>
    public abstract class BasePropertyChangedObject : INotifyPropertyChanged
    {
        #region Events
        /// <summary>
        /// Fired when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Event Control
        /// <summary>
        /// Fire an event for the calling property to notify of property being changed.
        /// </summary>
        /// <param name="propertyName">
        /// The property name being changed.
        /// </param>
        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}