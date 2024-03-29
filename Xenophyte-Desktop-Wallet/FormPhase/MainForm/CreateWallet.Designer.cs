﻿using System.Windows.Forms;

namespace Xenophyte_Wallet.FormPhase.MainForm
{
    partial class CreateWallet
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
#if WINDOWS
            this.textBoxPathWallet = new MetroFramework.Controls.MetroTextBox();
            this.textBoxSelectWalletPassword = new MetroFramework.Controls.MetroTextBox();
#else
            this.textBoxPathWallet = new System.Windows.Forms.TextBox();
            this.textBoxSelectWalletPassword = new System.Windows.Forms.TextBox();
#endif
            this.buttonSearchNewWalletFile = new MetroFramework.Controls.MetroButton();
            this.buttonCreateYourWallet = new MetroFramework.Controls.MetroButton();
            this.labelCreateSelectSavingPathWallet = new System.Windows.Forms.Label();
            this.labelCreateSelectWalletPassword = new System.Windows.Forms.Label();
            this.labelCreateYourWallet = new System.Windows.Forms.Label();
            this.labelCreateNoticePasswordRequirement = new System.Windows.Forms.Label();
            this.pictureBoxPasswordStatus = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPasswordStatus)).BeginInit();
            this.SuspendLayout();
            // 
            // textBoxPathWallet
            // 
            // 
            // 
            // 
#if WINDOWS
            this.textBoxPathWallet.CustomButton.Image = null;
            this.textBoxPathWallet.CustomButton.Location = new System.Drawing.Point(225, 2);
            this.textBoxPathWallet.CustomButton.Name = "";
            this.textBoxPathWallet.CustomButton.Size = new System.Drawing.Size(15, 15);
            this.textBoxPathWallet.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.textBoxPathWallet.CustomButton.TabIndex = 1;
            this.textBoxPathWallet.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.textBoxPathWallet.CustomButton.UseSelectable = true;
            this.textBoxPathWallet.CustomButton.Visible = false;
#endif
            this.textBoxPathWallet.Lines = new string[0];
            this.textBoxPathWallet.Location = new System.Drawing.Point(261, 255);
            this.textBoxPathWallet.MaxLength = 32767;
            this.textBoxPathWallet.Name = "textBoxPathWallet";
            this.textBoxPathWallet.PasswordChar = '\0';
            this.textBoxPathWallet.ReadOnly = true;
            this.textBoxPathWallet.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.textBoxPathWallet.SelectedText = "";
            this.textBoxPathWallet.SelectionLength = 0;
            this.textBoxPathWallet.SelectionStart = 0;
            this.textBoxPathWallet.ShortcutsEnabled = true;
            this.textBoxPathWallet.Size = new System.Drawing.Size(243, 20);
            this.textBoxPathWallet.TabIndex = 22;
#if WINDOWS
            this.textBoxPathWallet.UseSelectable = true;
            this.textBoxPathWallet.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.textBoxPathWallet.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
#endif
            // 
            // textBoxSelectWalletPassword
            // 
            // 
            // 
            // 
#if WINDOWS
            this.textBoxSelectWalletPassword.CustomButton.Image = null;
            this.textBoxSelectWalletPassword.CustomButton.Location = new System.Drawing.Point(225, 2);
            this.textBoxSelectWalletPassword.CustomButton.Name = "";
            this.textBoxSelectWalletPassword.CustomButton.Size = new System.Drawing.Size(15, 15);
            this.textBoxSelectWalletPassword.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.textBoxSelectWalletPassword.CustomButton.TabIndex = 1;
            this.textBoxSelectWalletPassword.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.textBoxSelectWalletPassword.CustomButton.UseSelectable = true;
            this.textBoxSelectWalletPassword.CustomButton.Visible = false;
#endif
            this.textBoxSelectWalletPassword.Lines = new string[0];
            this.textBoxSelectWalletPassword.Location = new System.Drawing.Point(261, 318);
            this.textBoxSelectWalletPassword.MaxLength = 32767;
            this.textBoxSelectWalletPassword.Name = "textBoxSelectWalletPassword";
            this.textBoxSelectWalletPassword.PasswordChar = '*';
            this.textBoxSelectWalletPassword.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.textBoxSelectWalletPassword.SelectedText = "";
            this.textBoxSelectWalletPassword.SelectionLength = 0;
            this.textBoxSelectWalletPassword.SelectionStart = 0;
            this.textBoxSelectWalletPassword.ShortcutsEnabled = true;
            this.textBoxSelectWalletPassword.Size = new System.Drawing.Size(243, 20);
            this.textBoxSelectWalletPassword.TabIndex = 16;
#if WINDOWS
            this.textBoxSelectWalletPassword.UseSelectable = true;
            this.textBoxSelectWalletPassword.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.textBoxSelectWalletPassword.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
#endif
            this.textBoxSelectWalletPassword.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxSelectWalletPassword_KeyDownAsync);
            this.textBoxSelectWalletPassword.TextChanged += new System.EventHandler(this.textBoxSelectWalletPassword_TextChanged);
            // 
            // buttonSearchNewWalletFile
            // 
            this.buttonSearchNewWalletFile.Location = new System.Drawing.Point(510, 253);
            this.buttonSearchNewWalletFile.Name = "buttonSearchNewWalletFile";
            this.buttonSearchNewWalletFile.Size = new System.Drawing.Size(27, 22);
            this.buttonSearchNewWalletFile.TabIndex = 20;
            this.buttonSearchNewWalletFile.Text = "...";
#if WINDOWS
            this.buttonSearchNewWalletFile.UseSelectable = true;
#endif
            this.buttonSearchNewWalletFile.Click += new System.EventHandler(this.ButtonSearchNewWalletFile_Click);
            // 
            // buttonCreateYourWallet
            // 
            this.buttonCreateYourWallet.Location = new System.Drawing.Point(313, 358);
            this.buttonCreateYourWallet.Name = "buttonCreateYourWallet";
            this.buttonCreateYourWallet.Size = new System.Drawing.Size(133, 49);
            this.buttonCreateYourWallet.TabIndex = 18;
            this.buttonCreateYourWallet.Text = "Create my Wallet";
#if WINDOWS
            this.buttonCreateYourWallet.UseSelectable = true;
#endif
            this.buttonCreateYourWallet.Click += new System.EventHandler(this.ButtonCreateYourWallet_Click);
            // 
            // labelCreateSelectSavingPathWallet
            // 
            this.labelCreateSelectSavingPathWallet.AutoSize = true;
            this.labelCreateSelectSavingPathWallet.Location = new System.Drawing.Point(261, 237);
            this.labelCreateSelectSavingPathWallet.Name = "labelCreateSelectSavingPathWallet";
            this.labelCreateSelectSavingPathWallet.Size = new System.Drawing.Size(216, 13);
            this.labelCreateSelectSavingPathWallet.TabIndex = 21;
            this.labelCreateSelectSavingPathWallet.Text = "Select the path directory for your new wallet:";
            // 
            // labelCreateSelectWalletPassword
            // 
            this.labelCreateSelectWalletPassword.AutoSize = true;
            this.labelCreateSelectWalletPassword.Location = new System.Drawing.Point(261, 300);
            this.labelCreateSelectWalletPassword.Name = "labelCreateSelectWalletPassword";
            this.labelCreateSelectWalletPassword.Size = new System.Drawing.Size(136, 13);
            this.labelCreateSelectWalletPassword.TabIndex = 17;
            this.labelCreateSelectWalletPassword.Text = "Write your wallet password:";
            // 
            // labelCreateYourWallet
            // 
            this.labelCreateYourWallet.AutoSize = true;
            this.labelCreateYourWallet.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelCreateYourWallet.Location = new System.Drawing.Point(333, 20);
            this.labelCreateYourWallet.Name = "labelCreateYourWallet";
            this.labelCreateYourWallet.Size = new System.Drawing.Size(113, 13);
            this.labelCreateYourWallet.TabIndex = 15;
            this.labelCreateYourWallet.Text = "Create your wallet:";
            // 
            // labelCreateNoticePasswordRequirement
            // 
            this.labelCreateNoticePasswordRequirement.AutoSize = true;
            this.labelCreateNoticePasswordRequirement.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelCreateNoticePasswordRequirement.Location = new System.Drawing.Point(171, 174);
            this.labelCreateNoticePasswordRequirement.Name = "labelCreateNoticePasswordRequirement";
            this.labelCreateNoticePasswordRequirement.Size = new System.Drawing.Size(441, 13);
            this.labelCreateNoticePasswordRequirement.TabIndex = 23;
            this.labelCreateNoticePasswordRequirement.Text = "Password must be a least 8 characters and contain letters, numbers, and special c" +
    "haracters.";
            // 
            // pictureBoxPasswordStatus
            // 
            this.pictureBoxPasswordStatus.BackgroundImage = global::Xenophyte_Wallet.Properties.Resources.error;
            this.pictureBoxPasswordStatus.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBoxPasswordStatus.Location = new System.Drawing.Point(509, 315);
            this.pictureBoxPasswordStatus.Name = "pictureBoxPasswordStatus";
            this.pictureBoxPasswordStatus.Size = new System.Drawing.Size(25, 25);
            this.pictureBoxPasswordStatus.TabIndex = 24;
            this.pictureBoxPasswordStatus.TabStop = false;
            // 
            // CreateWallet
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(784, 496);
            this.ControlBox = false;
            this.Controls.Add(this.pictureBoxPasswordStatus);
            this.Controls.Add(this.labelCreateNoticePasswordRequirement);
            this.Controls.Add(this.textBoxPathWallet);
            this.Controls.Add(this.labelCreateSelectSavingPathWallet);
            this.Controls.Add(this.buttonSearchNewWalletFile);
            this.Controls.Add(this.buttonCreateYourWallet);
            this.Controls.Add(this.labelCreateSelectWalletPassword);
            this.Controls.Add(this.textBoxSelectWalletPassword);
            this.Controls.Add(this.labelCreateYourWallet);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CreateWallet";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "CreateWallet";
            this.Load += new System.EventHandler(this.CreateWallet_Load);
            this.Resize += new System.EventHandler(this.CreateWallet_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPasswordStatus)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

#endregion

#if WINDOWS
        private MetroFramework.Controls.MetroTextBox textBoxPathWallet;
        private MetroFramework.Controls.MetroTextBox textBoxSelectWalletPassword;
#else
        private System.Windows.Forms.TextBox textBoxPathWallet;
        private System.Windows.Forms.TextBox textBoxSelectWalletPassword;
#endif
        public Label labelCreateSelectSavingPathWallet;
#if WINDOWS
        public MetroFramework.Controls.MetroButton buttonSearchNewWalletFile;
        public MetroFramework.Controls.MetroButton buttonCreateYourWallet;
#else
        public System.Windows.Forms.Button buttonSearchNewWalletFile;
        public System.Windows.Forms.Button buttonCreateYourWallet;
#endif
        public Label labelCreateSelectWalletPassword;
        public Label labelCreateYourWallet;
        public Label labelCreateNoticePasswordRequirement;
        private PictureBox pictureBoxPasswordStatus;

    }
}