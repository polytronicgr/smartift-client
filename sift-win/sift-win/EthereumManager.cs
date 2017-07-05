using Guytp.Logging;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Lts.Sift.WinClient
{
    /// <summary>
    /// The ethereum manager is responsible for connecting to ethereum and keeping details of the network and our addresses up to date.
    /// </summary>
    public class EthereumManager : BasePropertyChangedObject, IDisposable
    {
        #region Declarations
        /// <summary>
        /// Defines the address of SIFT's contract.
        /// </summary>
        private const string ContractAddress = "0xAeE66f6DDbFD69D40A0C0e524f18fb273FF6Aac9";

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

        /// <summary>
        /// Defines the phase the SIFT contract is currently in.
        /// </summary>
        private ContractPhase _contractPhase;

        /// <summary>
        /// Defines the current block number.
        /// </summary>
        private ulong _blockNumber;

        /// <summary>
        /// Defines whether or not ethereum is currently syncing.
        /// </summary>
        private bool _isSyncing;

        /// <summary>
        /// Defines whether the last checks to the network were successful.
        /// </summary>
        private bool _lastChecksSuccessful;

        /// <summary>
        /// Defines the smart contract to use for communicating with SIFT.
        /// </summary>
        private readonly Contract _contract;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the list of accounts known about by the manager.
        /// </summary>
        public ObservableCollection<EthereumAccount> Accounts { get; private set; }

        /// <summary>
        /// Gets the current phase that the SIFT contract is in.
        /// </summary>
        public ContractPhase ContractPhase
        {
            get { return _contractPhase; }
            private set
            {
                if (_contractPhase == value)
                    return;
                _contractPhase = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the current block number.
        /// </summary>
        public ulong BlockNumber
        {
            get { return _blockNumber; }
            private set
            {
                if (_blockNumber == value)
                    return;
                _blockNumber = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets whether or not the current block is syncing.
        /// </summary>
        public bool IsSyncing
        {
            get { return _isSyncing; }
            private set
            {
                if (_isSyncing == value)
                    return;
                _isSyncing = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets whether or not the last checks to the ethereum network were successful.
        /// </summary>
        public bool LastChecksSuccessful
        {
            get { return _lastChecksSuccessful; }
            private set
            {
                if (_lastChecksSuccessful == value)
                    return;
                _lastChecksSuccessful = value;
                NotifyPropertyChanged();
            }
        }
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
            
            // Read in our contract ABI
            Assembly assembly = Assembly.GetExecutingAssembly();
            string resourceName = "Lts.Sift.WinClient.Sift.abi";
            string abi;
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                using (StreamReader reader = new StreamReader(stream))
                    abi = reader.ReadToEnd();
            if (string.IsNullOrEmpty(abi))
                throw new Exception("Error reading contract ABI");

            // Store a handle to our contract
            _contract = _web3.Eth.GetContract(abi, ContractAddress);

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
            DateTime nextContractCheckTime = DateTime.UtcNow;
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

                    // Let's get the block number and check if we're syncing to just set some basics bits and bobs up
                    BlockNumber = ulong.Parse(_web3.Eth.Blocks.GetBlockNumber.SendRequestAsync().Result.Value.ToString());
                    IsSyncing = _web3.Eth.Syncing.SendRequestAsync().Result.IsSyncing;

                    // Ask the contract our status but first check we've got the right contract
                    if (DateTime.UtcNow >= nextContractCheckTime)
                    {
                        string contractVersion = _contract.GetFunction("siftContractVersion").CallAsync<string>().Result;
                        if (contractVersion == "SIFT 201707051557")
                        {
                            bool isIcoPhase = _contract.GetFunction("icoPhase").CallAsync<bool>().Result;
                            ContractPhase = isIcoPhase ? ContractPhase.Ico : ContractPhase.Trading;
                        }
                        else
                            ContractPhase = ContractPhase.Unknown;
                        nextContractCheckTime = DateTime.UtcNow.AddSeconds(10);
                    }

                    // Signify a successful loop
                    LastChecksSuccessful = true;

                    // Wait to try again
                    DateTime nextTime = DateTime.UtcNow.AddSeconds(3);
                    while (DateTime.UtcNow < nextTime && _isAlive)
                        Thread.Sleep(100);
                }
                catch (Exception ex)
                {
                    LastChecksSuccessful = false;
                    Logger.ApplicationInstance.Error("There was an unexpected error updating Ethereum network information", ex);
                }
            }
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Enable mining for a specific transaction.
        /// </summary>
        /// <param name="transactionHash"></param>
        /// <returns></returns>
        public async Task<TransactionReceipt> MineAndGetReceiptAsync(string transactionHash)
        {
            if (!await _web3.Miner.Start.SendRequestAsync(1))
                throw new Exception("Mining start failed");
            TransactionReceipt receipt;
            while ((receipt = await _web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(transactionHash)) == null)
                Thread.Sleep(1000);
            if (!await _web3.Miner.Stop.SendRequestAsync())
                throw new Exception("Mining stop failed");
            return receipt;
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