using System;
using System.Windows.Forms;
using Xenophyte_Wallet.Features;
using Xenophyte_Wallet.Wallet.Setting;

namespace Xenophyte_Wallet.FormPhase.ParallelForm
{
    public partial class FirstStartWallet : Form
    {
        private bool languageSelected;

        public FirstStartWallet()
        {
            InitializeComponent();
            UpdateLangueForm();
            initializationFirstStartForm();
        }

        /// <summary>
        ///     Initilization of the first start form.
        /// </summary>
        private void initializationFirstStartForm()
        {
            foreach (var key in ClassTranslation.LanguageDatabases.Keys)
                comboBoxLanguage.Items.Add(ClassTranslation.UppercaseFirst(key));
        }

        private void UpdateLangueForm()
        {
            labelWelcomeText.Text = ClassTranslation.GetLanguageTextFromOrder("FIRST_START_MENU_LABEL_WELCOME");
        }

        private void comboBoxLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            ClassTranslation.ChangeCurrentLanguage(comboBoxLanguage.Items[comboBoxLanguage.SelectedIndex].ToString());
            languageSelected = true;
            UpdateLangueForm();
            Program.WalletXenophyte.UpdateGraphicLanguageText();
        }

        private void buttonEndSetting_Click(object sender, EventArgs e)
        {
            if (languageSelected)
            {
                ClassWalletSetting.SaveSetting();
                Close();
            }
        }
    }
}