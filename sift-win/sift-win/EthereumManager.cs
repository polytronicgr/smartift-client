using Guytp.Logging;
using Nethereum.Web3;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;

namespace Lts.Sift.WinClient
{
    /// <summary>
    /// The ethereum manager is responsible for connecting to ethereum and keeping details of the network and our addresses up to date.
    /// </summary>
    public class EthereumManager : BasePropertyChangedObject, IDisposable
    {
        #region Declarations
        /// <summary>
        /// Defines whether or not the thread is alive that checks in the background.
        /// </summary>
        private bool _isAlive;

        /// <summary>
        /// Defines the thread that frequently checks the status from Ethereum in the background.
        /// </summary>
        private Thread _thread;

        /// <summary>
        /// Defines the API client to access the ethereum network.
        /// </summary>
        private readonly Web3 _web3;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the list of accounts known about by the manager.
        /// </summary>
        public ObservableCollection<EthereumAccount> Accounts { get; private set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Create a new instance of this class.
        /// </summary>
        public EthereumManager(string url = "http://localhost:8545/")
        {
            // Create basic objects we'll need
            _web3 = new Web3(url);
            Accounts = new ObservableCollection<EthereumAccount>();

            // Start our thread
            _isAlive = true;
            _thread = new Thread(ThreadEntry) { IsBackground = true, Name = "Ethereum Manager" };
            _thread.Start();
        }
        #endregion

        #region Threads
        /// <summary>
        /// This method provides the main entry point for the background thread that keeps ethereum wallet information synchronised.
        /// </summary>
        private void ThreadEntry()
        {
            while (_isAlive)
            {
                try
                {
                    // Get a list of all accounts currently know about by our wallet
                    string[] addresses = _web3.Eth.Accounts.SendRequestAsync().Result;

                    // First remove any existing accounts that do not exist in our new list
                    if (addresses != null)
                    {
                        EthereumAccount[] removedAccounts = Accounts.Where(ac => !addresses.Contains(ac.Address)).ToArray();
                        foreach (EthereumAccount removedAccount in removedAccounts)
                            Accounts.Remove(removedAccount);

                        // Process each of these accounts
                        foreach (string address in addresses)
                        {
                            // Get the current balance for this account
                            decimal balance = decimal.Parse(_web3.Eth.GetBalance.SendRequestAsync(address).Result.Value.ToString());

                            // See if we have an existing account - if not we'll need to create a new one
                            EthereumAccount existingAccount = Accounts.FirstOrDefault(ac => ac.Address == address);
                            if (existingAccount == null)
                                Accounts.Add(new EthereumAccount(address, balance));
                            else if (existingAccount.BalanceWei != balance)
                                existingAccount.BalanceWei = balance;
                        }
                    }

                    // Wait to try again
                    DateTime nextTime = DateTime.UtcNow.AddSeconds(3);
                    while (DateTime.UtcNow < nextTime && _isAlive)
                        Thread.Sleep(100);
                }
                catch (Exception ex)
                {
                    Logger.ApplicationInstance.Error("There was an unexpected error updating Ethereum network information", ex);
                }
            }
        }
        #endregion

        #region IDisposable Implementation
        /// <summary>
        /// Free up our resources.
        /// </summary>
        public void Dispose()
        {
            _isAlive = false;
            if (_thread != null)
            {
                _thread.Join();
                _thread = null;
            }
        }
        #endregion
    }
}