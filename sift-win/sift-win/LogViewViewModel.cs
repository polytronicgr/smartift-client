using System.Text;

namespace Lts.Sift.WinClient
{
    /// <summary>
    /// This class displays the log window with a static current state of the application log.
    /// </summary>
    public class LogViewViewModel
    {
        #region Properties
        /// <summary>
        /// Gets the text to display on-screen.
        /// </summary>
        public string LogText { get; private set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Create a new instance of this class.
        /// </summary>
        public LogViewViewModel()
        {
            string[] textLines = SiftLogProvider.ApplicationInstance?.LogText;
            StringBuilder sb = new StringBuilder();
            if (textLines != null)
                for (int i = 0; i < textLines.Length; i++)
                {
                    if (i == 0)
                        sb.AppendLine();
                    sb.AppendLine(textLines[i]);
                }
            LogText = sb.ToString();
        }
        #endregion
    }
}