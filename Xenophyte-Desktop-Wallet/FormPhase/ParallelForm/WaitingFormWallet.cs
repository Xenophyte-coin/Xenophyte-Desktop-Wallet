using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using Xenophyte_Wallet.Features;

namespace Xenophyte_Wallet.FormPhase.ParallelForm
{
    public partial class WaitingForm : Form
    {
        public WaitingForm()
        {
            InitializeComponent();
        }

        private void WaitingForm_Load(object sender, EventArgs e)
        {
            labelLoadingNetwork.Text = ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration.waitingmenulabeltext);
        }

        private async void ButtonClose_Click(object sender, EventArgs e)
        {
            await Program.WalletXenophyte.ClassWalletObject.FullDisconnection(true);
            Hide();
        }

        private void labelLoadingNetwork_Click(object sender, EventArgs e)
        {
        }
    }
}