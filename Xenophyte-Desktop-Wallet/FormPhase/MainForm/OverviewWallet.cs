﻿using System;
using System.Drawing;
using System.Windows.Forms;
using Xenophyte_Wallet.Features;

namespace Xenophyte_Wallet.FormPhase.MainForm
{
    public sealed partial class OverviewWallet : Form
    {
        public OverviewWallet()
        {
            InitializeComponent();
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            DoubleBuffered = true;
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
            if (Program.WalletXenophyte.ListControlSizeOverview.Count == 0)
                for (var i = 0; i < Controls.Count; i++)
                    if (i < Controls.Count)
                        Program.WalletXenophyte.ListControlSizeOverview.Add(
                            new Tuple<Size, Point>(Controls[i].Size, Controls[i].Location));
        }

        private void Overview_Load(object sender, EventArgs e)
        {
            UpdateStyles();
            Program.WalletXenophyte.ResizeWalletInterface();
        }

        private void Overview_Resize(object sender, EventArgs e)
        {
            UpdateStyles();
        }

        private void buttonFeeInformationAccumulated_MouseHover(object sender, EventArgs e)
        {
            var toolTipFeeAccumulatedInformation = new ToolTip();
            toolTipFeeAccumulatedInformation.SetToolTip(buttonFeeInformationAccumulated,
                ClassTranslation.GetLanguageTextFromOrder(
                    "OVERVIEW_WALLET_BUTTON_TOOLTIP_TRANSACTION_FEE_ACCUMULATED_CONTENT_TEXT"));
        }

        private void buttonFeeInformationAccumulated_Click(object sender, EventArgs e)
        {
#if WINDOWS
            ClassFormPhase.MessageBoxInterface(
                ClassTranslation.GetLanguageTextFromOrder(
                    "OVERVIEW_WALLET_BUTTON_MESSAGE_TRANSACTION_FEE_ACCUMULATED_CONTENT_TEXT"),
                ClassTranslation.GetLanguageTextFromOrder(
                    "OVERVIEW_WALLET_BUTTON_MESSAGE_TRANSACTION_FEE_ACCUMULATED_TITLE_TEXT"), MessageBoxButtons.OK,
                MessageBoxIcon.Information);
#else
            MessageBox.Show(Program.WalletXenophyte,
                ClassTranslation.GetLanguageTextFromOrder("OVERVIEW_WALLET_BUTTON_MESSAGE_TRANSACTION_FEE_ACCUMULATED_CONTENT_TEXT"),
                ClassTranslation.GetLanguageTextFromOrder("OVERVIEW_WALLET_BUTTON_MESSAGE_TRANSACTION_FEE_ACCUMULATED_TITLE_TEXT"), MessageBoxButtons.OK, MessageBoxIcon.Information);
#endif
        }
    }
}