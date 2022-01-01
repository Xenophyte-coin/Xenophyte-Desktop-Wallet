using System;
using System.Windows.Forms;
using Xenophyte_Connector_All.Setting;
using Xenophyte_Connector_All.Wallet;
using Xenophyte_Wallet.Features;

namespace Xenophyte_Wallet.FormPhase.ParallelForm
{
    public partial class ChangeWalletPasswordWallet : Form
    {
        public ChangeWalletPasswordWallet()
        {
            InitializeComponent();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            Program.WalletXenophyte.ClassWalletObject.WalletNewPassword = textBoxWalletNewPassword.Text;
            Program.WalletXenophyte.ClassWalletObject.WalletConnect.SendPacketWallet(
                ClassWalletCommand.ClassWalletSendEnumeration.ChangePassword + ClassConnectorSetting.PacketContentSeperator + textBoxWalletOldPassword.Text +
                ClassConnectorSetting.PacketContentSeperator + textBoxPinCode.Text + ClassConnectorSetting.PacketContentSeperator + textBoxWalletNewPassword.Text + "",
                Program.WalletXenophyte.ClassWalletObject.Certificate,
                true);
            textBoxPinCode.Text = "";
            textBoxWalletNewPassword.Text = "";
            textBoxWalletOldPassword.Text = "";
            Close();
        }

        private void ChangeWalletPassword_Load(object sender, EventArgs e)
        {
            labelChangePasswordWallet.Text =
                ClassTranslation.GetLanguageTextFromOrder("CHANGE_PASSWORD_WALLET_LABEL_OLD_PASSWORD_TEXT");
            labelChangePasswordNewPassword.Text =
                ClassTranslation.GetLanguageTextFromOrder("CHANGE_PASSWORD_WALLET_LABEL_NEW_PASSWORD_TEXT");
            labelChangePasswordPinCode.Text =
                ClassTranslation.GetLanguageTextFromOrder("CHANGE_PASSWORD_WALLET_LABEL_PIN_CODE_TEXT");
        }
    }
}