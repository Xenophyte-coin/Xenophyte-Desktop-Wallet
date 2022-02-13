using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Xenophyte_Connector_All.Setting;
using Xenophyte_Connector_All.Wallet;
using Xenophyte_Wallet.Features;
using Xenophyte_Wallet.Properties;
using Xenophyte_Wallet.Utility;
using Xenophyte_Wallet.Wallet.Tcp.Option;

namespace Xenophyte_Wallet.FormPhase.MainForm
{
    public partial class RestoreWallet : Form
    {
        public RestoreWallet()
        {
            InitializeComponent();
        }

        public void GetListControl()
        {
            if (Program.WalletXenophyte.ListControlSizeRestoreWallet.Count == 0)
                for (var i = 0; i < Controls.Count; i++)
                    if (i < Controls.Count)
                        Program.WalletXenophyte.ListControlSizeRestoreWallet.Add(
                            new Tuple<Size, Point>(Controls[i].Size, Controls[i].Location));
        }

        private void buttonSearchNewWalletFile_Click(object sender, EventArgs e)
        {
            var saveFileDialogWallet = new SaveFileDialog
            {
                InitialDirectory = AppDomain.CurrentDomain.BaseDirectory,
                Filter = "Xenophyte Wallet (*.xeno) | *.xeno",
                FilterIndex = 2,
                RestoreDirectory = true
            };
            if (saveFileDialogWallet.ShowDialog() == DialogResult.OK)
                if (saveFileDialogWallet.FileName != "")
                    textBoxPathWallet.Text = saveFileDialogWallet.FileName;
        }

        private async void buttonRestoreYourWallet_ClickAsync(object sender, EventArgs e)
        {
            if (CheckPasswordValidity())
            {
                ClassParallelForm.ShowWaitingFormAsync();

                var walletPath = textBoxPathWallet.Text;
                var walletPassword = textBoxPassword.Text;
                var walletKey = textBoxPrivateKey.Text;
                walletKey = Regex.Replace(walletKey, @"\s+", string.Empty);
                textBoxPassword.Text = string.Empty;
                textBoxPathWallet.Text = string.Empty;
                textBoxPrivateKey.Text = string.Empty;
                CheckPasswordValidity();


                await Task.Factory.StartNew(async () =>
                {
                    var walletRestoreFunctions = new ClassWalletRestoreFunctions();

                    var requestRestoreQrCodeEncrypted =
                        walletRestoreFunctions.GenerateQrCodeKeyEncryptedRepresentation(walletKey, walletPassword);

                    if (Program.WalletXenophyte.ClassWalletObject != null)
                        await Program.WalletXenophyte.InitializationWalletObject();

                    if (await Program.WalletXenophyte.ClassWalletObject.InitializationWalletConnection(string.Empty,
                        walletPassword, string.Empty, ClassWalletPhase.Restore))
                    {
                        Program.WalletXenophyte.ClassWalletObject.ListenSeedNodeNetworkForWallet();

                        Program.WalletXenophyte.ClassWalletObject.WalletDataCreationPath = walletPath;
                        if (await Program.WalletXenophyte.ClassWalletObject.WalletConnect.SendPacketWallet(
                            Program.WalletXenophyte.ClassWalletObject.Certificate, string.Empty, false))
                        {
                            if (requestRestoreQrCodeEncrypted != null)
                            {
                                Program.WalletXenophyte.ClassWalletObject.WalletNewPassword = walletPassword;
                                Program.WalletXenophyte.ClassWalletObject.WalletPrivateKeyEncryptedQRCode = walletKey;

                                await Task.Delay(1000);

                                if (!await Program.WalletXenophyte.ClassWalletObject.SeedNodeConnectorWallet
                                    .SendPacketToSeedNodeAsync(
                                        ClassWalletCommand.ClassWalletSendEnumeration.AskPhase + ClassConnectorSetting.PacketContentSeperator +
                                        requestRestoreQrCodeEncrypted,
                                        Program.WalletXenophyte.ClassWalletObject.Certificate, false, true))
                                {
                                    ClassParallelForm.HideWaitingFormAsync();
#if WINDOWS
                                    ClassFormPhase.MessageBoxInterface(
                                        ClassTranslation.GetLanguageTextFromOrder(
                                            ClassTranslationEnumeration.createwalleterrorcantconnectmessagecontenttext), string.Empty,
                                        MessageBoxButtons.OK, MessageBoxIcon.Error);
#else
                            MethodInvoker invoke = () => MessageBox.Show(Program.WalletXenophyte,
                                  ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration.createwalleterrorcantconnectmessagecontenttext));
                            Program.WalletXenophyte.BeginInvoke(invoke);
#endif
                                }
                            }
                            else
                            {
                                ClassParallelForm.HideWaitingFormAsync();

#if WINDOWS
                                ClassFormPhase.MessageBoxInterface("Invalid private key inserted.", string.Empty,
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
#else
                            MethodInvoker invoke =
 () => MessageBox.Show(Program.WalletXenophyte,"Invalid private key inserted.");
                            Program.WalletXenophyte.BeginInvoke(invoke);
#endif
                            }
                        }
                        else
                        {
                            ClassParallelForm.HideWaitingFormAsync();

#if WINDOWS
                            ClassFormPhase.MessageBoxInterface(
                                ClassTranslation.GetLanguageTextFromOrder(
                                    ClassTranslationEnumeration.createwalleterrorcantconnectmessagecontenttext), string.Empty,
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
#else
                        MethodInvoker invoke = () => MessageBox.Show(Program.WalletXenophyte,
                            ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration.createwalleterrorcantconnectmessagecontenttext));
                        Program.WalletXenophyte.BeginInvoke(invoke);
#endif
                        }
                    }
                    else
                    {
                        ClassParallelForm.HideWaitingFormAsync();

#if WINDOWS
                        ClassFormPhase.MessageBoxInterface(
                            ClassTranslation.GetLanguageTextFromOrder(
                                ClassTranslationEnumeration.createwalleterrorcantconnectmessagecontenttext), string.Empty,
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
#else
                        MethodInvoker invoke = () => MessageBox.Show(Program.WalletXenophyte,
                            ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration.createwalleterrorcantconnectmessagecontenttext));
                        Program.WalletXenophyte.BeginInvoke(invoke);
#endif
                    }

                    walletRestoreFunctions.Dispose();
                }, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Current);
            }
            else
            {
#if WINDOWS
                ClassFormPhase.MessageBoxInterface(
                    ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration.createwalletlabelpasswordrequirementtext),
                    ClassTranslation.GetLanguageTextFromOrder(
                        ClassTranslationEnumeration.walletnetworkobjectcreatewalletpassworderror2titletext), MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
#else
                        await Task.Factory.StartNew(() =>
                        {
                            MethodInvoker invoke = () => MessageBox.Show(Program.WalletXenophyte,
                                ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration.createwalletlabelpasswordrequirementtext),
                                ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration.walletnetworkobjectcreatewalletpassworderror2titletext), MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Program.WalletXenophyte.BeginInvoke(invoke);
                        }).ConfigureAwait(false);

#endif
            }
        }

        private void textBoxPassword_Resize(object sender, EventArgs e)
        {
            UpdateStyles();
        }

        private void RestoreWallet_Load(object sender, EventArgs e)
        {
            UpdateStyles();
            Program.WalletXenophyte.ResizeWalletInterface();
        }

        private void textBoxPassword_TextChanged(object sender, EventArgs e)
        {
            CheckPasswordValidity();
        }

        /// <summary>
        ///     Check password validity
        /// </summary>
        private bool CheckPasswordValidity()
        {
            if (textBoxPassword.Text.Length >= ClassConnectorSetting.WalletMinPasswordLength)
            {
                if (ClassUtility.CheckPassword(textBoxPassword.Text))
                {
                    MethodInvoker invoke = () => pictureBoxPasswordStatus.BackgroundImage = Resources.valid;
                    BeginInvoke(invoke);
                    return true;
                }
                else
                {
                    MethodInvoker invoke = () => pictureBoxPasswordStatus.BackgroundImage = Resources.error;
                    BeginInvoke(invoke);
                }
            }
            else
            {
                MethodInvoker invoke = () => pictureBoxPasswordStatus.BackgroundImage = Resources.error;
                BeginInvoke(invoke);
            }

            return false;
        }
    }
}