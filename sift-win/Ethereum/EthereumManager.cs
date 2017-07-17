using Guytp.Logging;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.StandardTokenEIP20;
using Nethereum.Web3;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

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
        public const string SiftContractAddress = "0x8a187d5285d316bcbc9adafc08b51d70a0d8e000";

        /// <summary>
        /// Defines the version of the SIFT contract we are expecting.
        /// </summary>
        public const decimal SiftContractVersion = 500201707171440m;

        /// <summary>
        /// Defines the address of the ICO's contract.
        /// </summary>
        public const string IcoContractAddress = "0xf8Fc0cc97d01A47E0Ba66B167B120A8A0DeAb949";

        /// <summary>
        /// Defines the version of the ICO contract we are expecting.
        /// </summary>
        public const decimal IcoContractVersion = 300201707171440m;

        /// <summary>
        /// Defines whether or not the thread is alive that checks in the background.
        /// </summary>
        private bool _isAlive;

        /// <summary>
        /// Defines the thread that frequently checks the status from Ethereum in the background.
        /// </summary>
        private Thread _networkCheckThread;

        /// <summary>
        /// Defines the thread that mines queued transactions.
        /// </summary>
        private Thread _transactionConfirmationThread;

        /// <summary>
        /// Defines the API client to access the ethereum network.
        /// </summary>
        private readonly Web3 _web3;

        /// <summary>
        /// Defines the phase the SIFT contract is currently in.
        /// </summary>
        private ContractPhase _contractPhase;

        /// <summary>
        /// Defines whether or not the ICO has been abandoned.
        /// </summary>
        private bool _isIcoAbandoned;

        /// <summary>
        /// Defines the earliest that people can invest in the ICO.
        /// </summary>
        private DateTime _icoStartDate;

        /// <summary>
        /// Defines the latest that people can invest in the ICO.
        /// </summary>
        private DateTime _icoEndDate;

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
        /// Defines the total supply of SIFT within the contract.
        /// </summary>
        private ulong _totalSupply;

        /// <summary>
        /// Defines the smart contract to use for communicating with SIFT.
        /// </summary>
        private readonly Contract _siftContract;

        /// <summary>
        /// Defines the smart contract to use for communicating with the ICO.
        /// </summary>
        private readonly Contract _icoContract;

        /// <summary>
        /// Defines the interface to the ERC20 part of the SIFT contract
        /// </summary>
        private readonly StandardTokenService _tokenService;

        /// <summary>
        /// Defines an object containing default gas info for sending some gas from an account to the contract with some wei to use as a rule-of-thumb.
        /// </summary>
        private TransactionGasInfo _defaultGasInfo;

        /// <summary>
        /// Defines a locker object that allows access to our mining queue.
        /// </summary>
        private readonly object _miningQueueLocker = new object();

        /// <summary>
        /// Defines a list of transactions we need to wait to be mined.
        /// </summary>
        private readonly List<EnqueuedTransaction> _miningQueue = new List<EnqueuedTransaction>();
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
        /// Gets whether or not the ICO has been abandoned.
        /// </summary>
        public bool IsIcoAbandoned
        {
            get { return _isIcoAbandoned; }
            private set
            {
                if (_isIcoAbandoned == value)
                    return;
                _isIcoAbandoned = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the earliest that people can invest in the ICO.
        /// </summary>
        public DateTime IcoStartDate
        {
            get { return _icoStartDate; }
            private set
            {
                if (_icoStartDate == value)
                    return;
                _icoStartDate = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the latest that people can invest in the ICO.
        /// </summary>
        public DateTime IcoEndDate
        {
            get { return _icoEndDate; }
            private set
            {
                if (_icoEndDate == value)
                    return;
                _icoEndDate = value;
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

        /// <summary>
        /// Gets the total supply of SIFT within the contract.
        /// </summary>
        public ulong TotalSupply
        {
            get { return _totalSupply; }
            set
            {
                if (_totalSupply == value)
                    return;
                _totalSupply = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets how many wei translate to a single sift.
        /// </summary>
        public static decimal WeiPerSift { get { return 1 * 10000000000000000; } }

        /// <summary>
        /// Gets or sets an object defining the gas to send some ether to the contract.
        /// </summary>
        public TransactionGasInfo DefaultGasInfo
        {
            get { return _defaultGasInfo; }
            private set
            {
                if (_defaultGasInfo == value)
                    return;
                _defaultGasInfo = value;
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
            // Log this start
            Logger.ApplicationInstance.Info("Starting EthereumManager against " + url);

            // Create basic objects we'll need
            _web3 = new Web3(url);
            Accounts = new ObservableCollection<EthereumAccount>();

            // Store a handle to our contracts
            _icoContract = _web3.Eth.GetContract(GetAbi("IcoPhaseManagement"), IcoContractAddress);
            _siftContract = _web3.Eth.GetContract(GetAbi("SmartInvestmentFundToken"), SiftContractAddress);
            _tokenService = new StandardTokenService(_web3, _siftContract.Address);

            // Start our thread
            _isAlive = true;
            _networkCheckThread = new Thread(NetworkCheckThreadEntry) { IsBackground = true, Name = "Ethereum Manager" };
            _networkCheckThread.Start();
            _transactionConfirmationThread = new Thread(TransactionConfirmationThreadEntry) { IsBackground = true, Name = "Transaction Confirmation" };
            _transactionConfirmationThread.Start();
        }
        #endregion

        #region Threads
        /// <summary>
        /// This method provides the main entry point for the background thread that keeps ethereum wallet information synchronised.
        /// </summary>
        private void NetworkCheckThreadEntry()
        {
            Logger.ApplicationInstance.Info("Network status thread started");
            bool connectedYet = false;
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

                            // And check the SIFT balance
                            ulong siftBalance = _tokenService.GetBalanceOfAsync<ulong>(address).Result;

                            // See if we have an existing account - if not we'll need to create a new one
                            EthereumAccount existingAccount = Accounts.FirstOrDefault(ac => ac.Address == address);
                            if (existingAccount == null)
                            {
                                Accounts.Add(new EthereumAccount(address, balance, siftBalance));
                                Logger.ApplicationInstance.Info("Found new account " + address + " with " + balance + " ETH / " + siftBalance + " SIFT");
                            }
                            else
                            {
                                if (existingAccount.BalanceWei != balance)
                                {
                                    existingAccount.BalanceWei = balance;
                                    Logger.ApplicationInstance.Info("Account " + address + " now has " + balance + " ETH");
                                }
                                if (existingAccount.SiftBalance != siftBalance)
                                {
                                    existingAccount.SiftBalance = siftBalance;
                                    Logger.ApplicationInstance.Info("Account " + address + " now has " + siftBalance + " SIFT");
                                }
                            }
                        }
                    }

                    // Let's get the block number and check if we're syncing to just set some basics bits and bobs up
                    BlockNumber = ulong.Parse(_web3.Eth.Blocks.GetBlockNumber.SendRequestAsync().Result.Value.ToString());
                    IsSyncing = _web3.Eth.Syncing.SendRequestAsync().Result.IsSyncing;

                    // Ask the contract our status but first check we've got the right contract
                    if (DateTime.UtcNow >= nextContractCheckTime)
                    {
                        // First check the ICO contract
                        Logger.ApplicationInstance.Debug("Checking ICO contract version");
                        decimal icoContractVersion = decimal.Parse(_icoContract.GetFunction("contractVersion").CallAsync<ulong>().Result.ToString());
                        if (icoContractVersion == IcoContractVersion)
                        {
                            // Get the phase of the ICO
                            bool isIcoPhase = _icoContract.GetFunction("icoPhase").CallAsync<bool>().Result;
                            ContractPhase = isIcoPhase ? ContractPhase.Ico : ContractPhase.Trading;

                            // Check if the ICO is abandoned
                            IsIcoAbandoned = _icoContract.GetFunction("icoAbandoned").CallAsync<bool>().Result;

                            // Check the ICO dates
                            IcoStartDate = DateFromTimestamp(_icoContract.GetFunction("icoStartTime").CallAsync<ulong>().Result);
                            IcoEndDate = DateFromTimestamp(_icoContract.GetFunction("icoEndTime").CallAsync<ulong>().Result);

                            // Whilst we're here we also want to check the gas cost to send
                            DefaultGasInfo = CalculateGasCostForEtherSend("0xAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA", IcoContractAddress, 1000000000000000000m).Result;
                            Logger.ApplicationInstance.Error("Default gas calculating for sending as " + DefaultGasInfo.Gas + " at " + DefaultGasInfo.GasPrice + " = " + DefaultGasInfo.GasCost);
                        }
                        else
                        {
                            ContractPhase = ContractPhase.Unknown;
                            Logger.ApplicationInstance.Error("ICO contract mismatch - expected " + IcoContractVersion + " but got " + icoContractVersion + " at " + IcoContractAddress);
                        }

                        // Now check SIFT contract
                        Logger.ApplicationInstance.Debug("Checking SIFT contract version");
                        decimal siftContractVersion = decimal.Parse(_siftContract.GetFunction("contractVersion").CallAsync<ulong>().Result.ToString());
                        if (siftContractVersion != SiftContractVersion)
                        {
                            ContractPhase = ContractPhase.Unknown;
                            Logger.ApplicationInstance.Error("SIFT contract mismatch - expected " + SiftContractVersion + " but got " + siftContractVersion + " at " + SiftContractAddress);
                        }

                        // Now check the SIFT contract
                        nextContractCheckTime = DateTime.UtcNow.AddSeconds(10);
                    }

                    // If valid contract check total supply
                    if (ContractPhase != ContractPhase.Unknown)
                    {
                        Logger.ApplicationInstance.Debug("Checking total supply and shareholding");
                        TotalSupply = _tokenService.GetTotalSupplyAsync<ulong>().Result;
                        foreach (EthereumAccount account in Accounts)
                            account.ShareholdingPercentage = TotalSupply == 0 ? 0 : (decimal)account.SiftBalance / TotalSupply * 100;
                    }

                    // Signify a successful loop
                    LastChecksSuccessful = true;
                    if (!connectedYet)
                    {
                        connectedYet = true;
                        Logger.ApplicationInstance.Info("Completed first successful network check, we should be good to go");
                    }
                }
                catch (Exception ex)
                {
                    LastChecksSuccessful = false;
                    Logger.ApplicationInstance.Error("There was an unexpected error updating Ethereum network information", ex);
                }

                // Wait to try again
                DateTime nextTime = DateTime.UtcNow.AddSeconds(3);
                while (DateTime.UtcNow < nextTime && _isAlive)
                    Thread.Sleep(100);
            }
        }

        /// <summary>
        /// This thread is responsible for confirming transactions that are in a queue.
        /// </summary>
        private void TransactionConfirmationThreadEntry()
        {
            bool minerStarted = false;
            while (_isAlive)
            {
                // No point doing this if we aren't successfully connected to the wallet
                if (!LastChecksSuccessful)
                {
                    Thread.Sleep(100);
                    continue;
                }

                double waitTime = 3;
                try
                {
                    // Get a copy of the queue
                    EnqueuedTransaction[] queuedTransactions;
                    lock (_miningQueueLocker)
                    {
                        queuedTransactions = _miningQueue.ToArray();
                        _miningQueue.Clear();
                    }

                    // If we have queued transactions let's try to give them all a process now
                    decimal hashRate;
                    if (queuedTransactions.Length > 0)
                    {
                        // If we are not mining, try to start mining
                        hashRate = decimal.Parse(new Nethereum.RPC.Eth.Mining.EthHashrate(_web3.Client).SendRequestAsync().Result.Value.ToString());
                        if (hashRate < 0)
                        {
                            Logger.ApplicationInstance.Debug("Starting miner to mine these transactions");
                            try
                            {
                                int cores = Environment.ProcessorCount / 2;
                                if (!_web3.Miner.Start.SendRequestAsync(cores).Result)
                                    Logger.ApplicationInstance.Error("Mining start failed");
                                else
                                {
                                    Logger.ApplicationInstance.Info("Started to mine for tranasctions using " + cores + " threads");
                                    minerStarted = true;
                                }
                            }
                            catch (Exception ex)
                            {
                                Logger.ApplicationInstance.Error("Failed to start mining", ex);
                            }
                        }

                        // Now let's mine everything
                        foreach (EnqueuedTransaction transaction in queuedTransactions)
                        {
                            try
                            {
                                TransactionReceipt receipt = _web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(transaction.TransactionHash).Result;
                                if (receipt != null)
                                    transaction.MarkSuccess(receipt);
                                else if (transaction.Age.TotalMinutes > 10)
                                    transaction.MarkFailed("Transaction did not mine within ten minutes, please check your full Ethereum wallet for more information");
                            }
                            catch (Exception ex)
                            {
                                // This could be RPC error or anything else, so don't actually fail it here
                                Logger.ApplicationInstance.Error("Failed to mine transaction " + transaction.TransactionHash, ex);
                            }
                        }
                    }

                    // Remove any complete transactions from our list
                    EnqueuedTransaction[] remainingTransactions = queuedTransactions.Where(t => !t.Completed).ToArray();
                    bool anyTransactionsInQueue;
                    lock (_miningQueueLocker)
                    {
                        if (remainingTransactions.Length > 0)
                            _miningQueue.AddRange(remainingTransactions);
                        anyTransactionsInQueue = _miningQueue.Count > 0;
                    }

                    // If no transactions left and miner is running and we started it, we can terminate the miner now
                    if (!anyTransactionsInQueue && minerStarted)
                    {
                        minerStarted = false;
                        hashRate = decimal.Parse(new Nethereum.RPC.Eth.Mining.EthHashrate(_web3.Client).SendRequestAsync().Result.Value.ToString());
                        Logger.ApplicationInstance.Debug("No transactions to mine and we started miner, so attempting to stop it now");
                        if (hashRate > 0)
                        {
                            try
                            {
                                if (!_web3.Miner.Stop.SendRequestAsync().Result)
                                    Logger.ApplicationInstance.Error("Stopping mining failed");
                                else
                                    Logger.ApplicationInstance.Info("Miner stopped successfully");
                            }
                            catch (Exception ex)
                            {
                                Logger.ApplicationInstance.Error("Failed to stop mining", ex);
                            }
                        }
                    }

                    // Update the queue - set wait time accordingly
                    waitTime = anyTransactionsInQueue ? 0.1 : 1;
                }
                catch (Exception ex)
                {
                    Logger.ApplicationInstance.Error("Unexpected error attempting to mine queue", ex);
                    waitTime = 3;
                }

                // Wait to try again
                DateTime nextTime = DateTime.UtcNow.AddSeconds(waitTime);
                while (DateTime.UtcNow < nextTime && _isAlive)
                    Thread.Sleep(100);
            }
        }
        #endregion

        /// <summary>
        /// Calculate details about how much gas it will cost for a transaction.
        /// </summary>
        /// <param name="from">
        /// Where the data is being sent from.
        /// </param>
        /// <param name="to">
        /// To whom the data is being sent.
        /// </param>
        /// <param name="wei">
        /// The amount of wei to send.
        /// </param>
        /// <returns>
        /// An object describing the costs involved in this transaction.
        /// </returns>
        public async Task<TransactionGasInfo> CalculateGasCostForEtherSend(string from, string to, decimal wei)
        {
            // See if we have enough gas, if not we'll fail it
            HexBigInteger hexValue = new HexBigInteger(new System.Numerics.BigInteger(wei));
            CallInput input = new CallInput { From = from, To = IcoContractAddress, Value = hexValue };
            HexBigInteger rawGas = await _web3.Eth.Transactions.EstimateGas.SendRequestAsync(input);
            decimal gas = decimal.Parse(rawGas.Value.ToString());
            HexBigInteger rawGasPrice = await _web3.Eth.GasPrice.SendRequestAsync();
            decimal gasPrice = decimal.Parse(rawGas.Value.ToString());
            decimal gasCost = gas * gasPrice;
            return new TransactionGasInfo(gasPrice, gasCost, gas);
        }

        /// <summary>
        /// Purchase SIFT from one of our accounts.
        /// </summary>
        /// <param name="address">
        /// The account to use for purchase.
        /// </param>
        /// <param name="quantity">
        /// The quantity to purchase.
        /// </param>
        /// <returns>
        /// Details of the purchase result indicating whether a success or failure happened.
        /// </returns>
        public async Task<SiftPurchaseResponse> PurchaseSift(string address, uint quantity)
        {
            try
            {
                // Ensure we have the balance
                decimal purchaseCostWei = WeiPerSift * quantity;
                EthereumAccount account = Accounts.FirstOrDefault(acc => acc.Address == address);
                if (account == null)
                    return new SiftPurchaseResponse(SiftPurchaseFailureType.UnknownAccount, "The specified account address is not known.");
                if (account.BalanceWei < purchaseCostWei)
                    return new SiftPurchaseResponse(SiftPurchaseFailureType.InsufficientFunds, "The specified account does not have sufficient funds.");

                // Determine the gas cost for this
                TransactionGasInfo gasInfo = await CalculateGasCostForEtherSend(address, IcoContractAddress, purchaseCostWei);
                decimal gasPrice = gasInfo.GasPrice;
                decimal gasCost = gasInfo.GasCost;
                decimal gas = gasInfo.Gas;
                decimal remainingGasMoney = account.BalanceWei - purchaseCostWei;
                if (remainingGasMoney < gasCost)
                    return new SiftPurchaseResponse(SiftPurchaseFailureType.InsufficientGas, "You do not have enough gas for this transaction.");
                byte gasMultiple = 25;
                decimal maximumGasCost;
                while ((maximumGasCost = gasCost * gasMultiple) * gasPrice > remainingGasMoney)
                    gasMultiple--;

                // Prompt to unlock account
                Logger.ApplicationInstance.Debug("Asking user confirmation for sending " + purchaseCostWei + " from " + address + " to " + IcoContractAddress);
                Func<TransactionUnlockViewModel> fnc = new Func<TransactionUnlockViewModel>(() =>
                    {
                        TransactionUnlockViewModel vm = new TransactionUnlockViewModel(gasCost, gasMultiple, gas, address, IcoContractAddress, purchaseCostWei);
                        TransactionUnlockWindow window = new TransactionUnlockWindow
                        {
                            DataContext = vm
                        };
                        window.ShowDialog();
                        return vm;
                    });
                TransactionUnlockViewModel viewModel = Application.Current.Dispatcher.Invoke(fnc);
                if (viewModel.WasCancelled)
                    return new SiftPurchaseResponse(SiftPurchaseFailureType.UserCancelled, "User cancelled transaction on confirmation screen.");

                // Unlock the account
                SiftDialog dialog = SiftDialog.ShowButtonless("Sending Transaction", "Please wait whilst the transaction to buy SIFT is sent to the Ethereum network, this should only take a few seconds...", true);
                Logger.ApplicationInstance.Debug("Attempting to unlock " + address);
                if (!await _web3.Personal.UnlockAccount.SendRequestAsync(address, viewModel.Password == null ? string.Empty : viewModel.Password.ToString(), 120))
                    return new SiftPurchaseResponse(SiftPurchaseFailureType.UnlockError, "Unable to unlock account with the supplied password.");

                // Send and mine the transaction
                Logger.ApplicationInstance.Debug("Account unlocked, sending " + purchaseCostWei + " from " + address + " to " + IcoContractAddress);
                TransactionInput transactionInput = new TransactionInput
                {
                    From = address,
                    To = IcoContractAddress,
                    Value = new HexBigInteger(new BigInteger(purchaseCostWei)),
                    GasPrice = new HexBigInteger(new BigInteger(viewModel.SelectedGasMultiplier * gasCost)),
                    Gas = new HexBigInteger(new BigInteger(viewModel.Gas))
                };
                Logger.ApplicationInstance.Info("Sending transaction for " + purchaseCostWei + " to " + IcoContractAddress + " from " + address);
                string transactionHash = await _web3.Eth.Transactions.SendTransaction.SendRequestAsync(transactionInput);
                Action act = dialog.Close;
                if (dialog.Dispatcher.CheckAccess())
                    act();
                else
                    dialog.Dispatcher.Invoke(act);

                // Return the response of the transaction hash
                return new SiftPurchaseResponse(transactionHash);
            }
            catch (Exception ex)
            {
                Logger.ApplicationInstance.Error("Error purchasing SIFT", ex);
                if (ex.Message.Contains("personal_unlockAccount method not implemented"))
                    return new SiftPurchaseResponse(SiftPurchaseFailureType.MissingRpcPersonal, "Your geth installation doesn't have the personal RPC enabled, please enable it to continue.");
                else if (ex.Message.Contains("could not decrypt key with given passphrase"))
                    return new SiftPurchaseResponse(SiftPurchaseFailureType.PasswordInvalid, "The password you supplied was incorrect");
                return new SiftPurchaseResponse(SiftPurchaseFailureType.Unknown, ex.ToString());
            }
        }

        /// <summary>
        /// Enqueue a transaction that has been sent into the mining queue to get a receipt for it.
        /// </summary>
        /// <param name="transactionHash">
        /// The hash of the transaction.
        /// </param>
        /// <returns>
        /// An object to track the state of the mining.
        /// </returns>
        public EnqueuedTransaction EnqueueTransactionPendingReceipt(string transactionHash)
        {
            EnqueuedTransaction transaction = new EnqueuedTransaction(transactionHash);
            lock (_miningQueueLocker)
                _miningQueue.Add(transaction);
            return transaction;
        }

        /// <summary>
        /// Gets the ABI data for the specified contract.
        /// </summary>
        /// <param name="contractName">
        /// The name of the contract to get the ABI for.
        /// </param>
        /// <returns>
        /// The contents of the ABI for the contract, otherwise an exception is thrown.
        /// </returns>
        private string GetAbi(string contractName)
        {
            string abi = null;
            try
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                string resourceName = "Lts.Sift.WinClient.ContractAbis." + contractName + ".abi";
                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                using (StreamReader reader = new StreamReader(stream))
                    abi = reader.ReadToEnd();
            }
            catch (Exception ex)
            {
                Logger.ApplicationInstance.Error("Error reading ABI", ex);
            }
            if (string.IsNullOrEmpty(abi))
                throw new Exception("Error reading contract ABI");
            Logger.ApplicationInstance.Debug("ABI loaded successfully for contract: " + contractName);
            return abi;
        }

        /// <summary>
        /// Gets a date time from a unix epoch timestamp.
        /// </summary>
        /// <param name="timestamp">
        /// The timestamp to get the DateTime object from.
        /// </param>
        /// <returns>
        /// A new date time object.
        /// </returns>
        private static DateTime DateFromTimestamp(ulong timestamp)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(timestamp);
        }

        #region IDisposable Implementation
        /// <summary>
        /// Free up our resources.
        /// </summary>
        public void Dispose()
        {
            _isAlive = false;
            if (_networkCheckThread != null)
            {
                _networkCheckThread?.Join();
                _networkCheckThread = null;
            }
            if (_transactionConfirmationThread != null)
            {
                _transactionConfirmationThread?.Join();
                _transactionConfirmationThread = null;
            }
        }
        #endregion
    }
}