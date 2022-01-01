using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Xenophyte_Connector_All.Setting;
using Xenophyte_Connector_All.Wallet;
using Xenophyte_Wallet.Features;
using Xenophyte_Wallet.Properties;
using Xenophyte_Wallet.Utility;

namespace Xenophyte_Wallet.FormPhase.MainForm
{
    public partial class CreateWallet : Form
    {
        public bool InCreation;

        public CreateWallet()
        {
            InitializeComponent();
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        }

        protected override CreateParams CreateParams
        {
            get
            {
                var CP = base.CreateParams;
                CP.ExStyle = CP.ExStyle | 0x02000000; // WS_EX_COMPOSITED
                return CP;
            }
        }

        public void GetListControl()
        {
            if (Program.WalletXenophyte.ListControlSizeCreateWallet.Count == 0)
                for (var i = 0; i < Controls.Count; i++)
                    if (i < Controls.Count)
                        Program.WalletXenophyte.ListControlSizeCreateWallet.Add(
                            new Tuple<Size, Point>(Controls[i].Size, Controls[i].Location));
        }

        private void SaveWalletFilePath()
        {
            var saveFileDialogWallet = new SaveFileDialog
            {
                InitialDirectory = AppDomain.CurrentDomain.BaseDirectory,
                Filter = @"Wallet File (*.xeno) | *.xeno",
                FilterIndex = 2,
                RestoreDirectory = true
            };
            if (saveFileDialogWallet.ShowDialog() == DialogResult.OK)
                if (saveFileDialogWallet.FileName != "")
                    textBoxPathWallet.Text = saveFileDialogWallet.FileName;
        }

        private void ButtonCreateYourWallet_Click(object sender, EventArgs e)
        {
            CreateWalletAsync();
        }

        private void ButtonSearchNewWalletFile_Click(object sender, EventArgs e)
        {
            SaveWalletFilePath();
        }

        private void CreateWallet_Load(object sender, EventArgs e)
        {
            UpdateStyles();
            Program.WalletXenophyte.ResizeWalletInterface();
        }

        private void CreateWallet_Resize(object sender, EventArgs e)
        {
            UpdateStyles();
        }

        private async void CreateWalletAsync()
        {
            if (InCreation)
            {
                await Program.WalletXenophyte.ClassWalletObject.FullDisconnection(true);
                InCreation = false;
            }

            if (CheckPasswordValidity())
            {
                if (textBoxPathWallet.Text != "")
                    if (textBoxSelectWalletPassword.Text != "")
                    {
                        if (Program.WalletXenophyte.ClassWalletObject != null)
                            await Program.WalletXenophyte.InitializationWalletObject();
                        if (Program.WalletXenophyte.ClassWalletObject != null && await Program.WalletXenophyte.ClassWalletObject.InitializationWalletConnection("",
                                textBoxSelectWalletPassword.Text, "",
                                ClassWalletPhase.Create))
                        {
                            Program.WalletXenophyte.ClassWalletObject.WalletNewPassword =
                                textBoxSelectWalletPassword.Text;
                            Program.WalletXenophyte.ClassWalletObject.ListenSeedNodeNetworkForWallet();

                            InCreation = true;
                            Program.WalletXenophyte.ClassWalletObject.WalletDataCreationPath = textBoxPathWallet.Text;

                            await Task.Factory.StartNew(async delegate
                            {
                                if (await Program.WalletXenophyte.ClassWalletObject.WalletConnect.SendPacketWallet(
                                    Program.WalletXenophyte.ClassWalletObject.Certificate, string.Empty, false))
                                {
                                    await Task.Delay(100);
                                    if (!await Program.WalletXenophyte.ClassWalletObject
                                        .SendPacketWalletToSeedNodeNetwork(
                                            ClassWalletCommand.ClassWalletSendEnumeration.CreatePhase + ClassConnectorSetting.PacketContentSeperator +
                                            textBoxSelectWalletPassword.Text))
                                    {
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

                                    void MethodInvoker()
                                    {
                                        textBoxSelectWalletPassword.Text = "";
                                    }

                                    BeginInvoke((MethodInvoker) MethodInvoker);
                                    CheckPasswordValidity();
                                }
                                else
                                {
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
                            }, CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Current);
                        }
                    }
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

        private void textBoxSelectWalletPassword_KeyDownAsync(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) CreateWalletAsync();
        }

        /// <summary>
        ///     Check password validity
        /// </summary>
        private bool CheckPasswordValidity()
        {
            if (textBoxSelectWalletPassword.Text.Length >= ClassConnectorSetting.WalletMinPasswordLength)
            {
                if (ClassUtility.CheckPassword(textBoxSelectWalletPassword.Text))
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

        private void textBoxSelectWalletPassword_TextChanged(object sender, EventArgs e)
        {
            CheckPasswordValidity();
        }
    }
}