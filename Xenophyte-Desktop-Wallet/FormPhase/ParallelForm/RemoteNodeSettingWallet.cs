using System;
using System.Linq;
using System.Net;
using System.Windows.Forms;
using MetroFramework;
using Xenophyte_Wallet.Features;
using Xenophyte_Wallet.Wallet.Setting;
using Xenophyte_Wallet.Wallet.Tcp;
using Program = Xenophyte_Wallet.Program;

namespace Xenophyte_Wallet.FormPhase.ParallelForm
{
    public partial class RemoteNodeSettingWallet : Form
    {
        public RemoteNodeSettingWallet()
        {
            InitializeComponent();
        }

        private async void ButtonValidSetting_Click(object sender, EventArgs e)
        {
            if (radioButtonEnableSeedNodeSync.Checked)
            {
                Program.WalletXenophyte.WalletSyncMode = ClassWalletSyncMode.WALLET_SYNC_DEFAULT;
            }
            else if (radioButtonEnablePublicRemoteNodeSync.Checked)
            {
                Program.WalletXenophyte.WalletSyncMode = ClassWalletSyncMode.WALLET_SYNC_PUBLIC_NODE;
            }
            else if (radioButtonEnableManualRemoteNodeSync.Checked)
            {
                Program.WalletXenophyte.WalletSyncMode = ClassWalletSyncMode.WALLET_SYNC_MANUAL_NODE;
                Program.WalletXenophyte.WalletSyncHostname = textBoxRemoteNodeHost.Text;
            }

            ClassWalletSetting.SaveSetting();


            if (Program.WalletXenophyte.ClassWalletObject.WalletConnect != null)
            {
                if (!Program.WalletXenophyte.ClassWalletObject.WalletClosed)
                {
                    await Program.WalletXenophyte.ClassWalletObject.DisconnectRemoteNodeTokenSync();
                    Program.WalletXenophyte.ClassWalletObject.WalletOnUseSync = false;
                }

            }

            Close();
        }

        private void RadioButtonEnableManualRemoteNodeSync_Click(object sender, EventArgs e)
        {
            radioButtonEnableSeedNodeSync.Checked = false;
            radioButtonEnablePublicRemoteNodeSync.Checked = false;
            radioButtonEnableManualRemoteNodeSync.Checked = true;
        }

        private void RadioButtonEnablePublicRemoteNodeSync_Click(object sender, EventArgs e)
        {
            radioButtonEnableManualRemoteNodeSync.Checked = false;
            radioButtonEnableSeedNodeSync.Checked = false;
            radioButtonEnablePublicRemoteNodeSync.Checked = true;
        }

        private void RadioButtonEnableSeedNodeSync_Click(object sender, EventArgs e)
        {
            radioButtonEnablePublicRemoteNodeSync.Checked = false;
            radioButtonEnableManualRemoteNodeSync.Checked = false;
            radioButtonEnableSeedNodeSync.Checked = true;
        }

        private void RemoteNodeSetting_Load(object sender, EventArgs e)
        {
            dataGridViewPeerList.AllowUserToAddRows = false;

            radioButtonEnableSeedNodeSync.Text =
                ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration.remotenodesettingmenuuseseednodenetworkonlytext);
            radioButtonEnablePublicRemoteNodeSync.Text =
                ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration.remotenodesettingmenuuseremotenodetext);
            radioButtonEnableManualRemoteNodeSync.Text =
                ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration.remotenodesettingmenuusemanualnodetext);
            labelNoticeRemoteNodeHost.Text =
                ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration.remotenodesettingmenuusemanualnodehostnametext);
            labelNoticePublicNodeInformation.Text =
                ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration.remotenodesettingmenuuseremotenodeinformationtext);
            labelNoticePrivateRemoteNode.Text =
                ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration.remotenodesettingmenuusemanualnodeinformationtext);
            textBoxRemoteNodeHost.Text = Program.WalletXenophyte.WalletSyncHostname;
            switch (Program.WalletXenophyte.WalletSyncMode)
            {
                case ClassWalletSyncMode.WALLET_SYNC_DEFAULT:
                    radioButtonEnableSeedNodeSync.Checked = true;
                    radioButtonEnablePublicRemoteNodeSync.Checked = false;
                    radioButtonEnableManualRemoteNodeSync.Checked = false;
                    break;
                case ClassWalletSyncMode.WALLET_SYNC_PUBLIC_NODE:
                    radioButtonEnableSeedNodeSync.Checked = false;
                    radioButtonEnablePublicRemoteNodeSync.Checked = true;
                    radioButtonEnableManualRemoteNodeSync.Checked = false;
                    break;
                case ClassWalletSyncMode.WALLET_SYNC_MANUAL_NODE:
                    radioButtonEnableSeedNodeSync.Checked = false;
                    radioButtonEnablePublicRemoteNodeSync.Checked = false;
                    radioButtonEnableManualRemoteNodeSync.Checked = true;
                    textBoxRemoteNodeHost.Text = Program.WalletXenophyte.WalletSyncHostname;
                    break;
            }

            if (ClassPeerList.PeerList.Count > 0)
            {
                try
                {
                    foreach (var peer in ClassPeerList.PeerList.ToArray())
                    {
                        dataGridViewPeerList.Rows.Add(peer.Value.peer_host, peer.Value.peer_status, "Remove");

                    }
                }
                catch
                {

                }
            }

            checkBoxEnablePeerTrustSystem.Checked = ClassPeerList.PeerEnableTrustSystem;
            checkBoxEnableProxyMode.Checked = Program.WalletXenophyte.WalletEnableProxyMode;
        }

        private void dataGridViewPeerList_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            var senderGrid = (DataGridView)sender;

            if (senderGrid.Columns[e.ColumnIndex] is DataGridViewButtonColumn &&
                e.RowIndex >= 0)
            {
                try
                {
                    IPAddress peer = IPAddress.Parse(senderGrid.Rows[e.RowIndex].Cells[0].Value.ToString());
                    if (ClassPeerList.PeerList.ContainsKey(peer))
                    {
                        ClassPeerList.PeerList.Remove(peer);
                    }

                    dataGridViewPeerList.Rows.RemoveAt(e.RowIndex);
                }
                catch
                {

                }
            }
        }

        private void checkBoxEnablePeerTrustSystem_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxEnablePeerTrustSystem.Checked)
            {
                ClassPeerList.PeerEnableTrustSystem = true;
            }
            else
            {
                ClassPeerList.PeerEnableTrustSystem = false;
            }
        }

        private void checkBoxEnableProxyMode_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void checkBoxEnableProxyMode_Click(object sender, EventArgs e)
        {
            if (checkBoxEnableProxyMode.Checked)
            {
#if WINDOWS
                if (MetroMessageBox.Show(Program.WalletXenophyte,
                        "This option is not mandatory to connect on the network. This is suggested to use Seed Nodes when you want to connect your wallet. Are your sure to enable this feature?",
                        "Proxy feature", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                {

                    MetroMessageBox.Show(Program.WalletXenophyte,
                        "This option is take in count, reconnect your wallet for use it.",
                        "Proxy feature", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    Program.WalletXenophyte.WalletEnableProxyMode = true;
                }
                else
                {
                    checkBoxEnableProxyMode.Checked = false;
                    Program.WalletXenophyte.WalletEnableProxyMode = false;
                }
#else
                if (MessageBox.Show(Program.WalletXenophyte,
                        "This option is not mandatory to connect on the network. This is suggested to use Seed Nodes when you want to connect your wallet. Are your sure to enable this feature?",
                        "Proxy feature", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                {

                    MessageBox.Show(Program.WalletXenophyte,
                        "This option is take in count, reconnect your wallet for use it.",
                        "Proxy feature", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    Program.WalletXenophyte.WalletEnableProxyMode = true;
                }
                else
                {
                    checkBoxEnableProxyMode.Checked = false;
                    Program.WalletXenophyte.WalletEnableProxyMode = false;
                }

#endif
                Program.WalletXenophyte.WalletEnableProxyMode = true;
            }
            else
            {
                Program.WalletXenophyte.WalletEnableProxyMode = false;
            }
        }
    }
}