﻿using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using Xenophyte_Wallet.Debug;
using Xenophyte_Wallet.Features;
using Xenophyte_Wallet.Utility;

#if DEBUG
using Xenophyte_Wallet.Debug;
#endif

namespace Xenophyte_Wallet
{
    internal static class Program
    {
        public static CultureInfo GlobalCultureInfo = new CultureInfo("fr-FR"); // Set the global culture info, I don't suggest to change this, this one is used by the blockchain and by the whole network.

        public static WalletXenophyte WalletXenophyte;

        /// <summary>
        ///     Point d'entrée principal de l'application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            CultureInfo.DefaultThreadCurrentCulture = GlobalCultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = GlobalCultureInfo;
            Thread.CurrentThread.Name = Path.GetFileName(Environment.GetCommandLineArgs()[0]);
            ServicePointManager.DefaultConnectionLimit = 65535;


            ThreadPool.SetMinThreads(65535, 100);
            ThreadPool.SetMaxThreads(65535, 100);


#if DEBUG
            Log.InitializeLog(); // Initialization of log system.
            Log.AutoWriteLog(); // Start the automatic write of log lines.
#endif
            AppDomain.CurrentDomain.UnhandledException += Application_ThreadException;

            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

#if WINDOWS
            ClassMemory.CleanMemory();
#endif

            ClassTranslation.InitializationLanguage(); // Initialization of language system.
            ClassContact.InitializationContactList(); // Initialization of contact system.
            ClassPeerList.LoadPeerList();
#if WINDOWS
            Application.EnableVisualStyles();
#endif
            Application.SetCompatibleTextRenderingDefault(false);
            WalletXenophyte = new WalletXenophyte();
            Application.Run(WalletXenophyte); // Start the main interface.
        }

        private static void Application_ThreadException(object sender, UnhandledExceptionEventArgs e)
        {
            var filePath = ClassUtility.ConvertPath(AppDomain.CurrentDomain.BaseDirectory + "\\error_wallet.txt");
            var exception = (Exception)e.ExceptionObject;
            using (var writer = new StreamWriter(filePath, true))
            {
                writer.WriteLine("Message :" + exception.Message + "<br/>" + Environment.NewLine +
                                 "StackTrace :" +
                                 exception.StackTrace +
                                 "" + Environment.NewLine + "Date :" + DateTime.Now);
                writer.WriteLine(Environment.NewLine +
                                 "-----------------------------------------------------------------------------" +
                                 Environment.NewLine);
            }

            MessageBox.Show(
                @"An error has been detected, send the file error_wallet.txt to the Team for fix the issue.");
            Trace.TraceError(exception.StackTrace);

            Environment.Exit(1);
        }
    }
}