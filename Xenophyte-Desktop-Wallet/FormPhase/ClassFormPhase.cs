#if WINDOWS
using MetroFramework;
#endif
using System.Windows.Forms;
using System;
using Xenophyte_Connector_All.Setting;
using Xenophyte_Wallet.Features;

namespace Xenophyte_Wallet.FormPhase
{
    public class ClassFormPhaseEnumeration
    {
        public const string Main = "MAIN";
        public const string OpenWallet = "OPEN";
        public const string CreateWallet = "CREATE";
        public const string Overview = "OVERVIEW";
        public const string SendTransaction = "SEND";
        public const string TransactionHistory = "TRANSACTION";
        public const string BlockExplorer = "BLOCK";
        public const string RestoreWallet = "RESTORE";
        public const string ContactWallet = "CONTACT";
    }

    public class ClassFormPhase
    {
        public static string FormPhase;

        /// <summary>
        ///     Initialize the public static object of the interface for share the object with every class.
        /// </summary>
        /// <param name="wallet"></param>
        public static void InitializeMainInterface(WalletXenophyte wallet)
        {
            Program.WalletXenophyte.SwitchForm(ClassFormPhaseEnumeration.Main);
        }

        /// <summary>
        ///     Change form phase.
        /// </summary>
        /// <param name="phase"></param>
        public static void SwitchFormPhase(string phase)
        {
            Program.WalletXenophyte.SwitchForm(phase);
        }

        /// <summary>
        ///     Show wallet menu.
        /// </summary>
        public static void ShowWalletMenu()
        {
            Program.WalletXenophyte.BeginInvoke((MethodInvoker) delegate
            {
                Program.WalletXenophyte.panelControlWallet.Visible = true;
            });
        }

        /// <summary>
        ///     Show amount and wallet address.
        /// </summary>
        /// <param name="walletAddress"></param>
        /// <param name="walletAmount"></param>
        public static void ShowWalletInformationInMenu(string walletAddress, string walletAmount)
        {
            Program.WalletXenophyte.BeginInvoke((MethodInvoker) delegate
            {
                Program.WalletXenophyte.labelNoticeWalletAddress.Text =
                    ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration.panelwalletaddresstext) + " " + walletAddress;
            });


            var showPendingAmount = false;
            if (Program.WalletXenophyte.ClassWalletObject.WalletAmountInPending != null)
                if (!string.IsNullOrEmpty(Program.WalletXenophyte.ClassWalletObject.WalletAmountInPending))
                    showPendingAmount = true;
            if (!showPendingAmount)
                Program.WalletXenophyte.BeginInvoke((MethodInvoker) delegate
                {
                    Program.WalletXenophyte.labelNoticeWalletBalance.Text =
                        ClassTranslation.GetLanguageTextFromOrder("PANEL_WALLET_BALANCE_TEXT") + " " +
                        walletAmount + " " + ClassConnectorSetting.CoinNameMin;
                });
            else
                Program.WalletXenophyte.BeginInvoke((MethodInvoker) delegate
                {
                    Program.WalletXenophyte.labelNoticeWalletBalance.Text =
                        ClassTranslation.GetLanguageTextFromOrder("PANEL_WALLET_BALANCE_TEXT") + " " +
                        Program.WalletXenophyte.ClassWalletObject.WalletConnect.WalletAmount + " " +
                        ClassConnectorSetting.CoinNameMin + " | " +
                        ClassTranslation.GetLanguageTextFromOrder("PANEL_WALLET_PENDING_BALANCE_TEXT") + " " +
                        Program.WalletXenophyte.ClassWalletObject.WalletAmountInPending + " " +
                        ClassConnectorSetting.CoinNameMin;
                });
        }

        /// <summary>
        ///     Hide wallet menu.
        /// </summary>
        public static void HideWalletMenu()
        {
            try
            {
                void Invoke()
                {
                    Program.WalletXenophyte.panelControlWallet.Visible = false;
                }

                Program.WalletXenophyte.BeginInvoke((MethodInvoker) Invoke);
                SwitchFormPhase(ClassFormPhaseEnumeration.Main);
                ClassParallelForm.HidePinFormAsync();
                ClassParallelForm.HideWaitingFormAsync();
                ClassParallelForm.HideWaitingCreateWalletFormAsync();
            }
            catch
            {
            }
        }

#if WINDOWS

        /// <summary>
        ///     Show a message in front of the main interface.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="title"></param>
        /// <param name="button"></param>
        /// <param name="icon"></param>
        public static DialogResult MessageBoxInterface(string text, string title, MessageBoxButtons button,
            MessageBoxIcon icon)
        {
            return (DialogResult) Program.WalletXenophyte.Invoke((Func<DialogResult>) (() =>
                MetroMessageBox.Show(Program.WalletXenophyte, text, title, button, icon)));
        }

#endif
    }
}