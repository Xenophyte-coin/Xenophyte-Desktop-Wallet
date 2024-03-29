﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xenophyte_Wallet.Utility;
using Xenophyte_Wallet.Wallet.Sync.Object;
#if DEBUG
using Xenophyte_Wallet.Debug;
#endif

namespace Xenophyte_Wallet.Wallet.Sync
{
    public class ClassBlockCache
    {
        private const string WalletBlockCacheDirectory = "/Blockchain/";
        private const string WalletBlockCacheFileExtension = ".xenoblock";
        public static Dictionary<string, ClassBlockObject> ListBlock;
        public static Dictionary<int, string> ListBlockIndex;

        public static bool OnLoad;

        /// <summary>
        ///     Load block in cache.
        /// </summary>
        /// <returns></returns>
        public static void LoadBlockchainCache()
        {
            if (ListBlock != null)
            {
                ListBlock.Clear();
                ListBlockIndex.Clear();
            }
            else
            {
                ListBlock = new Dictionary<string, ClassBlockObject>();
                ListBlockIndex = new Dictionary<int, string>();
            }
            OnLoad = true;
            try
            {
                Task.Factory.StartNew(() =>
                    {
                        if (Directory.Exists(
                            ClassUtility.ConvertPath(AppDomain.CurrentDomain.BaseDirectory + WalletBlockCacheDirectory +
                                                     "\\")))
                        {
                            if (
                                File.Exists(ClassUtility.ConvertPath(AppDomain.CurrentDomain.BaseDirectory +
                                                                     WalletBlockCacheDirectory +
                                                                     "/blockchain" + WalletBlockCacheFileExtension)))
                            {
                                var counter = 0;
                                using (var fs =
                                    File.Open(
                                        ClassUtility.ConvertPath(AppDomain.CurrentDomain.BaseDirectory +
                                                                 WalletBlockCacheDirectory + "/blockchain" +
                                                                 WalletBlockCacheFileExtension), FileMode.Open,
                                        FileAccess.Read, FileShare.ReadWrite))
                                using (var bs = new BufferedStream(fs))
                                using (var sr = new StreamReader(bs))
                                {
                                    string line;
                                    while ((line = sr.ReadLine()) != null)
                                    {
                                        var blockLine = line.Split(new[] {"#"}, StringSplitOptions.None);
                                        if (!ListBlock.ContainsKey(blockLine[1]))
                                        {
                                            var blockObject = new ClassBlockObject
                                            {
                                                BlockHeight = blockLine[0],
                                                BlockHash = blockLine[1],
                                                BlockTransactionHash = blockLine[2],
                                                BlockTimestampCreate = blockLine[3],
                                                BlockTimestampFound = blockLine[4],
                                                BlockDifficulty = blockLine[5],
                                                BlockReward = blockLine[6]
                                            };

                                            ListBlock.Add(blockLine[1], blockObject);
                                            ListBlockIndex.Add(ListBlockIndex.Count, blockLine[1]);
                                            counter++;
                                        }
#if DEBUG
                            else
                            {
                                Log.WriteLine("Duplicate block line: " + line);
                            }
#endif
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (Directory.Exists(
                                    ClassUtility.ConvertPath(AppDomain.CurrentDomain.BaseDirectory +
                                                             WalletBlockCacheDirectory)) ==
                                false)
                                Directory.CreateDirectory(
                                    ClassUtility.ConvertPath(AppDomain.CurrentDomain.BaseDirectory +
                                                             WalletBlockCacheDirectory));

                            Directory.CreateDirectory(
                                ClassUtility.ConvertPath(AppDomain.CurrentDomain.BaseDirectory +
                                                         WalletBlockCacheDirectory));
                        }

                        OnLoad = false;
                        Program.WalletXenophyte.ClassWalletObject.TotalBlockInSync = ListBlock.Count;
                    }, Program.WalletXenophyte.WalletCancellationToken.Token,
                    TaskCreationOptions.DenyChildAttach, TaskScheduler.Current).ConfigureAwait(false);
            }
            catch
            {
                Program.WalletXenophyte.ClassWalletObject.TotalBlockInSync = 0;
                ListBlock.Clear();
                ListBlockIndex.Clear();
                OnLoad = false;
            }
        }

        /// <summary>
        ///     Save each block into cache
        /// </summary>
        /// <param name="block"></param>
        public static async Task SaveWalletBlockCache(string block)
        {
            if (Directory.Exists(
                    ClassUtility.ConvertPath(AppDomain.CurrentDomain.BaseDirectory + WalletBlockCacheDirectory)) ==
                false)
                Directory.CreateDirectory(
                    ClassUtility.ConvertPath(AppDomain.CurrentDomain.BaseDirectory + WalletBlockCacheDirectory));
            if (File.Exists(ClassUtility.ConvertPath(AppDomain.CurrentDomain.BaseDirectory + WalletBlockCacheDirectory +
                                                     "/blockchain" + WalletBlockCacheFileExtension)) == false)
                try
                {
                    File.Create(ClassUtility.ConvertPath(AppDomain.CurrentDomain.BaseDirectory +
                                                         WalletBlockCacheDirectory +
                                                         "/blockchain" + WalletBlockCacheFileExtension)).Close();

                    using (var transactionFile = new StreamWriter(ClassUtility.ConvertPath(
                        AppDomain.CurrentDomain.BaseDirectory + WalletBlockCacheDirectory +
                        "/blockchain" + WalletBlockCacheFileExtension), true))
                    {
                        await transactionFile.WriteAsync(block + "\n").ConfigureAwait(false);
                    }
                }
                catch
                {
                    // ignored
                }
            else
                try
                {
                    using (var transactionFile = new StreamWriter(ClassUtility.ConvertPath(
                        AppDomain.CurrentDomain.BaseDirectory + WalletBlockCacheDirectory +
                        "/blockchain" + WalletBlockCacheFileExtension), true))
                    {
                        await transactionFile.WriteAsync(block + "\n").ConfigureAwait(false);
                    }
                }
                catch
                {
                    //
                }
        }


        /// <summary>
        ///     Clear each block into cache.
        /// </summary>
        public static bool RemoveWalletBlockCache()
        {
            try
            {
                if (Directory.Exists(
                    ClassUtility.ConvertPath(AppDomain.CurrentDomain.BaseDirectory + WalletBlockCacheDirectory + "\\")))
                {
                    File.Delete(ClassUtility.ConvertPath(AppDomain.CurrentDomain.BaseDirectory +
                                                         WalletBlockCacheDirectory +
                                                         "/blockchain." + WalletBlockCacheFileExtension));
                    Directory.Delete(
                        ClassUtility.ConvertPath(AppDomain.CurrentDomain.BaseDirectory + WalletBlockCacheDirectory +
                                                 "\\"), true);
                    Directory.CreateDirectory(
                        ClassUtility.ConvertPath(AppDomain.CurrentDomain.BaseDirectory + WalletBlockCacheDirectory));
                }
            }
            catch
            {
                //
            }
            try
            {
                ListBlockIndex.Clear();
            }
            catch
            {

            }
            try
            {
                ListBlock.Clear();
            }
            catch
            {
            }

            return true;
        }
    }
}