﻿using System;
using System.Windows.Forms;
using Xenophyte_Wallet.Wallet;

namespace Xenophyte_Wallet.FormPhase
{
    public partial class WaitingForm : Form
    {
        public WaitingForm()
        {
            InitializeComponent();
        }

        private void WaitingForm_Load(object sender, EventArgs e)
        {
            labelLoadingNetwork.Text = ClassTranslation.GetLanguageTextFromOrder("WAITING_MENU_LABEL_TEXT");
        }

        private void ButtonClose_Click(object sender, EventArgs e)
        {
            ClassWalletObject.FullDisconnection(true);
            Hide();
        }
    }
}