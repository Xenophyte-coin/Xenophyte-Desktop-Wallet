﻿using System;
using System.Drawing;
using System.Windows.Forms;

namespace Xenophyte_Wallet.FormPhase.MainForm
{
    public partial class MainWallet : Form
    {
        public MainWallet()
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


        private void ButtonMainOpenMenuWallet_Click(object sender, EventArgs e)
        {
            ClassFormPhase.SwitchFormPhase(ClassFormPhaseEnumeration.OpenWallet);
        }

        private void ButtonMainCreateWallet_Click(object sender, EventArgs e)
        {
            ClassFormPhase.SwitchFormPhase(ClassFormPhaseEnumeration.CreateWallet);
        }

        public void GetListControl()
        {
            if (Program.WalletXenophyte.ListControlSizeMain.Count == 0)
                for (var i = 0; i < Controls.Count; i++)
                    if (i < Controls.Count)
                        Program.WalletXenophyte.ListControlSizeMain.Add(
                            new Tuple<Size, Point>(Controls[i].Size, Controls[i].Location));
        }

        private void Main_Load(object sender, EventArgs e)
        {
            UpdateStyles();
            Program.WalletXenophyte.ResizeWalletInterface();
        }


        private void Main_Resize_1(object sender, EventArgs e)
        {
            UpdateStyles();
        }
    }
}