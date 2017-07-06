using Guytp.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lts.Sift.WinClient
{
    /// <summary>
    /// This log provider is designed to hook in to the logging infrastructure and provide an application-wide way to get a series of text-based logs that can be displayed in the UI at a later date.
    /// </summary>
    public class SiftLogProvider : ILogProvider
    {
        #region Declarations
        /// <summary>
        /// Defines the levels of logging supported by this provider.
        /// </summary>
        private readonly LogLevel[] _supportedLogLevels;

        /// <summary>
        /// Defines a list of all log entries received.
        /// </summary>
        private readonly List<LogEntry> _allLogEntries = new List<LogEntry>();

        /// <summary>
        /// Defines a list of all the log strings that have been received.
        /// </summary>
        private readonly List<string> _allLogStrings = new List<string>();
        #endregion

        #region Properties
        /// <summary>
        /// Gets the application-wide logging instance to use.
        /// </summary>
        public static SiftLogProvider ApplicationInstance { get; private set; }

        /// <summary>
        /// Gets all log text since logging start.
        /// </summary>
        public string[] LogText => _allLogStrings.ToArray();

        /// <summary>
        /// Gets all log entries since logging started.
        /// </summary>
        public LogEntry[] LogEntries => _allLogEntries.ToArray();
        #endregion

        #region Constructors
        /// <summary>
        /// Create a new instance of this class.
        /// </summary>
        /// <param name="supportedLogLevels">
        /// The levels of logging supported by this provider.
        /// </param>
        public SiftLogProvider(LogLevel[] supportedLogLevels)
        {
            if (supportedLogLevels == null || supportedLogLevels.Length == 0)
                throw new ArgumentNullException(nameof(supportedLogLevels), "No supported log levels");
            _supportedLogLevels = supportedLogLevels;
            if (ApplicationInstance == null)
                ApplicationInstance = this;
        }
        #endregion

        /// <summary>
        /// Indicates a new log entry has been received and that the log provider should handle it.
        /// </summary>
        /// <param name="logEntry">
        /// The log entry to handle.
        /// </param>
        public void AddLogEntry(LogEntry logEntry)
        {
            if (!_supportedLogLevels.Contains(logEntry.Level))
                return;
            _allLogEntries.Add(logEntry);
            _allLogStrings.Add(logEntry.FormattedAsMultiline);
        }
    }
}