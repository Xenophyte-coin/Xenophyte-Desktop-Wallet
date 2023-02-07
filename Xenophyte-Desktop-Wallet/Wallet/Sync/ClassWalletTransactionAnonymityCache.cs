﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Xenophyte_Connector_All.Remote;
using Xenophyte_Connector_All.Setting;
using Xenophyte_Connector_All.Utils;
using Xenophyte_Connector_All.Wallet;
using Xenophyte_Wallet.Features;
using Xenophyte_Wallet.Utility;
using Xenophyte_Wallet.Wallet.Sync.Object;
#if DEBUG
using Xenophyte_Wallet.Debug;
#endif

namespace Xenophyte_Wallet.Wallet.Sync
{
    public class ClassWalletTransactionAnonymityCache
    {
        private const string WalletTransactionCacheDirectory = "/Cache/";
        private const string WalletTransactionCacheFileExtension = "transaction.xenotra";
        private static bool _inClearCache;
        public static bool OnLoad;

        public static Dictionary<string, ClassWalletTransactionObject> ListTransaction = new Dictionary<string, ClassWalletTransactionObject>(); // hash, transaction object
        public static Dictionary<int, string> ListTransactionIndex = new Dictionary<int, string>(); // index, hash.

        /// <summary>
        ///     Load transaction in cache.
        /// </summary>
        /// <param name="walletAddress"></param>
        /// <returns></returns>
        public static void LoadWalletCache(string walletAddress)
        {
            if (ListTransaction != null)
                ListTransaction.Clear();
            else
                ListTransaction = new Dictionary<string, ClassWalletTransactionObject>();
            if (!string.IsNullOrEmpty(walletAddress))
            {
                OnLoad = true;
                try
                {
                    Task.Factory.StartNew(() =>
                        {
                            walletAddress += "ANONYMITY";

                            if (Directory.Exists(
                                ClassUtility.ConvertPath(AppDomain.CurrentDomain.BaseDirectory +
                                                         WalletTransactionCacheDirectory + walletAddress + "\\")))
                            {
                                if (File.Exists(ClassUtility.ConvertPath(
                                    AppDomain.CurrentDomain.BaseDirectory + WalletTransactionCacheDirectory +
                                    walletAddress + "\\" + WalletTransactionCacheFileExtension)))
                                {
                                    byte[] AesKeyIv = null;
                                    byte[] AesSalt = null;
                                    using (var password = new PasswordDeriveBytes(
                                        Program.WalletXenophyte.ClassWalletObject.WalletConnect.WalletAddress +
                                        Program.WalletXenophyte.ClassWalletObject.WalletConnect.WalletKey,
                                        ClassUtils.GetByteArrayFromString(ClassUtils.FromHex(
                                            (Program.WalletXenophyte.ClassWalletObject.WalletConnect.WalletAddress +
                                             Program.WalletXenophyte.ClassWalletObject.WalletConnect.WalletKey)
                                            .Substring(0, 8)))))
                                    {
                                        AesKeyIv = password.GetBytes(
                                            ClassConnectorSetting.MAJOR_UPDATE_1_SECURITY_CERTIFICATE_SIZE / 8);
                                        AesSalt = password.GetBytes(16);
                                    }

                                    var listTransactionEncrypted = new List<string>();

                                    using (var fs = File.Open(
                                        ClassUtility.ConvertPath(AppDomain.CurrentDomain.BaseDirectory +
                                                                 WalletTransactionCacheDirectory + walletAddress +
                                                                 "\\" +
                                                                 WalletTransactionCacheFileExtension), FileMode.Open,
                                        FileAccess.Read, FileShare.Read))
                                    using (var bs = new BufferedStream(fs))
                                    using (var sr = new StreamReader(bs))
                                    {
                                        string line;
                                        while ((line = sr.ReadLine()) != null) listTransactionEncrypted.Add(line);
                                    }

                                    if (listTransactionEncrypted.Count > 0)
                                    {
                                        var line = 0;

                                        foreach (var transactionEncrypted in listTransactionEncrypted)
                                        {
                                            line++;

                                            OnLoad = true;

                                            var walletTransactionDecrypted =
                                                ClassAlgo.GetDecryptedResult(ClassAlgoEnumeration.Rijndael,
                                                    transactionEncrypted, ClassWalletNetworkSetting.KeySize, AesKeyIv,
                                                    AesSalt);
                                            if (walletTransactionDecrypted != ClassAlgoErrorEnumeration.AlgoError)
                                            {
                                                var splitTransaction =
                                                    walletTransactionDecrypted.Split(new[] {"#"},
                                                        StringSplitOptions.None);
                                                if (!ListTransaction.ContainsKey(splitTransaction[1]))
                                                {
                                                    var splitBlockchainHeight = splitTransaction[7]
                                                        .Split(new[] {"~"}, StringSplitOptions.None);
                                                    var transactionObject = new ClassWalletTransactionObject
                                                    {
                                                        TransactionType = splitTransaction[0],
                                                        TransactionHash = splitTransaction[1],
                                                        TransactionWalletAddress = splitTransaction[2],
                                                        TransactionAmount =
                                                            decimal.Parse(
                                                                splitTransaction[3].ToString(Program.GlobalCultureInfo),
                                                                NumberStyles.Currency, Program.GlobalCultureInfo),
                                                        TransactionFee = decimal.Parse(
                                                            splitTransaction[4].ToString(Program.GlobalCultureInfo),
                                                            NumberStyles.Currency, Program.GlobalCultureInfo),
                                                        TransactionTimestampSend = long.Parse(splitTransaction[5]),
                                                        TransactionTimestampRecv = long.Parse(splitTransaction[6]),
                                                        TransactionBlockchainHeight =
                                                            splitBlockchainHeight[0].Replace("{", "")
                                                    };
                                                    ListTransaction.Add(splitTransaction[1], transactionObject);
                                                    ListTransactionIndex.Add(ListTransactionIndex.Count, splitTransaction[1]);
                                                    Program.WalletXenophyte.UpdateLabelSyncInformation(
                                                        "On load transaction database - total transactions loaded and decrypted: " +
                                                        (ClassWalletTransactionCache.ListTransaction.Count +
                                                         ListTransaction.Count));
                                                }
#if DEBUG
                                            else
                                            {
                                                Log.WriteLine("Duplicate anonymous transaction: " + walletTransactionDecrypted);
                                            }
#endif
                                            }
#if DEBUG
                                        else
                                        {
                                            Log.WriteLine("Wrong anonymous transaction at line: " + line);
                                        }
#endif
                                        }
                                    }

                                    Program.WalletXenophyte.ClassWalletObject.TotalTransactionInSyncAnonymity =
                                        ListTransaction.Count;
                                    listTransactionEncrypted.Clear();
                                    AesKeyIv = null;
                                    AesSalt = null;
                                }
                                else
                                {
                                    File.Create(ClassUtility.ConvertPath(
                                        AppDomain.CurrentDomain.BaseDirectory + WalletTransactionCacheDirectory +
                                        walletAddress + "\\" + WalletTransactionCacheFileExtension)).Close();
                                }
                            }
                            else
                            {
                                if (Directory.Exists(ClassUtility.ConvertPath(
                                        AppDomain.CurrentDomain.BaseDirectory + WalletTransactionCacheDirectory)) ==
                                    false)
                                    Directory.CreateDirectory(ClassUtility.ConvertPath(
                                        AppDomain.CurrentDomain.BaseDirectory + WalletTransactionCacheDirectory));

                                Directory.CreateDirectory(ClassUtility.ConvertPath(
                                    AppDomain.CurrentDomain.BaseDirectory + WalletTransactionCacheDirectory +
                                    walletAddress));
                            }


                            Program.WalletXenophyte.ClassWalletObject.TotalTransactionInSyncAnonymity = ListTransaction.Count;
                            OnLoad = false;
                        }, Program.WalletXenophyte.WalletCancellationToken.Token, TaskCreationOptions.LongRunning, TaskScheduler.Current).ConfigureAwait(false);
                }
                catch
                {
                    Program.WalletXenophyte.ClassWalletObject.TotalTransactionInSyncAnonymity = 0;
                    ListTransaction.Clear();
                    ListTransactionIndex.Clear();
                    OnLoad = false;
                }
            }
        }

        /// <summary>
        ///     Save each transaction into cache
        /// </summary>
        /// <param name="walletAddress"></param>
        /// <param name="transaction"></param>
        public static async Task SaveWalletCacheAsync(string walletAddress, string transaction)
        {
            if (!string.IsNullOrEmpty(walletAddress))
            {
                walletAddress += "ANONYMITY";
                if (ListTransaction.Count > 0 && _inClearCache == false)
                {
                    if (Directory.Exists(
                            ClassUtility.ConvertPath(AppDomain.CurrentDomain.BaseDirectory +
                                                     WalletTransactionCacheDirectory)) ==
                        false)
                        Directory.CreateDirectory(
                            ClassUtility.ConvertPath(AppDomain.CurrentDomain.BaseDirectory +
                                                     WalletTransactionCacheDirectory));

                    if (Directory.Exists(ClassUtility.ConvertPath(AppDomain.CurrentDomain.BaseDirectory +
                                                                  WalletTransactionCacheDirectory +
                                                                  walletAddress)) == false)
                        Directory.CreateDirectory(ClassUtility.ConvertPath(
                            AppDomain.CurrentDomain.BaseDirectory + WalletTransactionCacheDirectory +
                            walletAddress));


                    if (!File.Exists(ClassUtility.ConvertPath(AppDomain.CurrentDomain.BaseDirectory +
                                                              WalletTransactionCacheDirectory +
                                                              walletAddress +
                                                              "\\" + WalletTransactionCacheFileExtension)))
                        File.Create(ClassUtility.ConvertPath(AppDomain.CurrentDomain.BaseDirectory +
                                                             WalletTransactionCacheDirectory + walletAddress + "\\" +
                                                             WalletTransactionCacheFileExtension)).Close();
                    try
                    {
                        using (var transactionFile = new StreamWriter(ClassUtility.ConvertPath(
                            AppDomain.CurrentDomain.BaseDirectory + WalletTransactionCacheDirectory +
                            walletAddress +
                            "\\" + WalletTransactionCacheFileExtension), true))
                        {
                            await transactionFile.WriteAsync(transaction + "\n").ConfigureAwait(false);
                        }
                    }
                    catch
                    {
                        // ignored
                    }
                }
            }
        }

        /// <summary>
        ///     Clear each transaction into cache.
        /// </summary>
        /// <param name="walletAddress"></param>
        public static bool RemoveWalletCache(string walletAddress)
        {
            _inClearCache = true;
            if (!string.IsNullOrEmpty(walletAddress))
            {
                walletAddress += "ANONYMITY";
                if (Directory.Exists(
                    ClassUtility.ConvertPath(AppDomain.CurrentDomain.BaseDirectory + WalletTransactionCacheDirectory +
                                             walletAddress + "\\")))
                {
                    if (File.Exists(ClassUtility.ConvertPath(AppDomain.CurrentDomain.BaseDirectory +
                                                             WalletTransactionCacheDirectory + walletAddress + "\\" +
                                                             WalletTransactionCacheFileExtension)))
                        File.Delete(ClassUtility.ConvertPath(AppDomain.CurrentDomain.BaseDirectory +
                                                             WalletTransactionCacheDirectory + walletAddress + "\\" +
                                                             WalletTransactionCacheFileExtension));

                    Directory.Delete(
                        ClassUtility.ConvertPath(AppDomain.CurrentDomain.BaseDirectory +
                                                 WalletTransactionCacheDirectory + walletAddress + "\\"), true);
                    Directory.CreateDirectory(ClassUtility.ConvertPath(AppDomain.CurrentDomain.BaseDirectory +
                                                                       WalletTransactionCacheDirectory +
                                                                       walletAddress));
                }
                ListTransactionIndex.Clear();
                ListTransaction.Clear();
            }

            _inClearCache = false;
            return true;
        }

        /// <summary>
        ///     Clear wallet transaction cache.
        /// </summary>
        public static void ClearWalletCache()
        {
            try
            {
                ListTransactionIndex.Clear();
            }
            catch
            {

            }
            try
            {
                ListTransaction.Clear();
            }
            catch
            {
            }
        }

        /// <summary>
        ///     Add transaction to the list.
        /// </summary>
        /// <param name="transaction"></param>
        public static async Task AddWalletTransactionAsync(string transaction, IPAddress node)
        {
#if DEBUG
                Log.WriteLine("Wallet transaction history received: " + transaction
                                       .Replace(
                                           ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration
                                               .WalletTransactionPerId, ""));
#endif

            bool errorSyncTransaction = false;

            if (!OnLoad)
            {
                var splitTransaction = transaction
                    .Replace(
                        ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration.WalletAnonymityTransactionPerId,
                        string.Empty).Split(new[] {"#"}, StringSplitOptions.None);



                var hashTransaction = splitTransaction[4]; // Transaction Hash.
                if (!ListTransaction.ContainsKey(hashTransaction))
                {
                    var type = splitTransaction[0];
                    var timestamp = splitTransaction[3]; // Timestamp Send CEST.
                    var realFeeAmountSend = splitTransaction[7]; // Real fee and amount crypted for sender.
                    var realFeeAmountRecv = splitTransaction[8]; // Real fee and amount crypted for sender.

                    var amountAndFeeDecrypted = ClassAlgoErrorEnumeration.AlgoError;
                    if (type == "SEND")
                        amountAndFeeDecrypted = ClassAlgo.GetDecryptedResultManual(ClassAlgoEnumeration.Rijndael,
                            realFeeAmountSend,
                            Program.WalletXenophyte.ClassWalletObject.WalletConnect.WalletAddress +
                            Program.WalletXenophyte.ClassWalletObject.WalletConnect.WalletKey,
                            ClassWalletNetworkSetting.KeySize); // AES
                    else if (type == "RECV")
                        amountAndFeeDecrypted = ClassAlgo.GetDecryptedResultManual(ClassAlgoEnumeration.Rijndael,
                            realFeeAmountRecv,
                            Program.WalletXenophyte.ClassWalletObject.WalletConnect.WalletAddress +
                            Program.WalletXenophyte.ClassWalletObject.WalletConnect.WalletKey,
                            ClassWalletNetworkSetting.KeySize); // AES

                    if (amountAndFeeDecrypted != ClassAlgoErrorEnumeration.AlgoError)
                    {
#if DEBUG
                        Log.WriteLine("Wallet anonymity transaction transaction history received decrypted: " + amountAndFeeDecrypted);
#endif
                        var splitDecryptedAmountAndFee =
                            amountAndFeeDecrypted.Split(new[] {"-"}, StringSplitOptions.None);
                        var amountDecrypted = splitDecryptedAmountAndFee[0].Replace(".", ",");
                        var feeDecrypted = splitDecryptedAmountAndFee[1].Replace(".", ",");
                        var walletDstOrSrc = splitDecryptedAmountAndFee[2];


                        var timestampRecv = splitTransaction[5]; // Timestamp Recv CEST.
                        var blockchainHeight = splitTransaction[6]; // Blockchain height.

                        var finalTransaction = type + "#" + hashTransaction + "#" + walletDstOrSrc + "#" +
                                               amountDecrypted + "#" + feeDecrypted + "#" + timestamp + "#" +
                                               timestampRecv + "#" + blockchainHeight;

                        var finalTransactionEncrypted = ClassAlgo.GetEncryptedResultManual(
                            ClassAlgoEnumeration.Rijndael, finalTransaction,
                            Program.WalletXenophyte.ClassWalletObject.WalletConnect.WalletAddress +
                            Program.WalletXenophyte.ClassWalletObject.WalletConnect.WalletKey,
                            ClassWalletNetworkSetting.KeySize); // AES

                        if (finalTransactionEncrypted == ClassAlgoErrorEnumeration.AlgoError) // Ban bad remote node.
                        {
                            errorSyncTransaction = true;
                        }
                        else
                        {
                            var splitBlockchainHeight = blockchainHeight.Split(new[] {"~"}, StringSplitOptions.None);

                            var transactionObject = new ClassWalletTransactionObject
                            {
                                TransactionType = type,
                                TransactionHash = hashTransaction,
                                TransactionWalletAddress = walletDstOrSrc,
                                TransactionAmount = decimal.Parse(amountDecrypted.ToString(Program.GlobalCultureInfo),
                                    NumberStyles.Currency, Program.GlobalCultureInfo),
                                TransactionFee = decimal.Parse(feeDecrypted.ToString(Program.GlobalCultureInfo),
                                    NumberStyles.Currency, Program.GlobalCultureInfo),
                                TransactionTimestampSend = long.Parse(timestamp),
                                TransactionTimestampRecv = long.Parse(timestampRecv),
                                TransactionBlockchainHeight = splitBlockchainHeight[0].Replace("{", "")
                            };


                            ListTransaction.Add(hashTransaction, transactionObject);
                            ListTransactionIndex.Add(ListTransactionIndex.Count, hashTransaction);

                            await SaveWalletCacheAsync(
                                Program.WalletXenophyte.ClassWalletObject.WalletConnect.WalletAddress,
                                finalTransactionEncrypted);

#if DEBUG
                            Log.WriteLine("Total transactions downloaded: " +
                                               ListTransaction.Count + "/" +
                                               Program.WalletXenophyte.ClassWalletObject.TotalTransactionInSync + ".");
#endif
                        }
                    }
                    else
                    {
#if DEBUG
                    Log.WriteLine("Can't decrypt transaction: " + transaction + " result: " +
                                  amountAndFeeDecrypted);
#endif
                        errorSyncTransaction = true;
                    }
                }
                else
                {
#if DEBUG
                    Log.WriteLine("Wallet anonymous transaction hash: " + hashTransaction + " already exist on database.");
#endif
                    errorSyncTransaction = true;
                }
            }
#if DEBUG
            else
            {
                Log.WriteLine("Can't save transaction, the database is currently on loading.");
            }
#endif

            if (errorSyncTransaction)
            {
                if (!ClassConnectorSetting.SeedNodeIp.ContainsKey(node))
                {
                    if (!Program.WalletXenophyte.ClassWalletObject.ListRemoteNodeBanned.ContainsKey(node))
                    {
                        Program.WalletXenophyte.ClassWalletObject.ListRemoteNodeBanned.Add(node,
                            ClassUtils.DateUnixTimeNowSecond());
                        ClassPeerList.IncrementPeerDisconnect(node);
                    }
                    else
                    {
                        Program.WalletXenophyte.ClassWalletObject.ListRemoteNodeBanned[node] = ClassUtils.DateUnixTimeNowSecond();
                        ClassPeerList.IncrementPeerDisconnect(node);
                    }
                }

                await Program.WalletXenophyte.ClassWalletObject.DisconnectRemoteNodeTokenSync();
                Program.WalletXenophyte.ClassWalletObject.WalletOnUseSync = false;
            }

            Program.WalletXenophyte.ClassWalletObject.InReceiveTransactionAnonymity = false;
        }
    }
}