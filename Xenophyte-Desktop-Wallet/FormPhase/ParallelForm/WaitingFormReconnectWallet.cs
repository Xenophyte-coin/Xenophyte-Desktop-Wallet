using System;
using System.Windows.Forms;
using Xenophyte_Wallet.Features;

namespace Xenophyte_Wallet.FormPhase.ParallelForm
{
    public partial class WaitingFormReconnect : Form
    {
        public WaitingFormReconnect()
        {
            InitializeComponent();
        }

        private void WaitingFormReconnect_Load(object sender, EventArgs e)
        {
            labelLoadingNetwork.Text = ClassTranslation.GetLanguageTextFromOrder("NETWORK_WAITING_MENU_LABEL_TEXT");
        }

        private async void ButtonClose_Click(object sender, EventArgs e)
        {
            await Program.WalletXenophyte.ClassWalletObject.FullDisconnection(true);
            Hide();
        }
    }
}