﻿using System;
using Xenophyte_Wallet.FormCustom;

namespace Xenophyte_Wallet
{
    sealed partial class WalletXenophyte
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur Windows Form

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WalletXenophyte));
            this.menuStripMenu = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mainMenuToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createWalletToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openWalletToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeWalletToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.restoreWalletToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.languageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.securityToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.changePasswordToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingPinCodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.syncToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.remoteNodeSettingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resyncTransactionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resyncBlockToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.themeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lightToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.darkToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.darkerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.transactionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.metroButtonBlockExplorerWallet = new System.Windows.Forms.Button();
            this.metroButtonTransactionWallet = new System.Windows.Forms.Button();
            this.metroButtonSendTransactionWallet = new System.Windows.Forms.Button();
            this.buttonOverviewWallet = new System.Windows.Forms.Button();
            this.labelSyncInformation = new System.Windows.Forms.Label();
            this.timerRefresh = new System.Windows.Forms.Timer(this.components);
            this.labelCoinName = new System.Windows.Forms.Label();
            this.buttonPreviousPage = new System.Windows.Forms.Button();
            this.labelNoticeCurrentPage = new System.Windows.Forms.Label();
            this.buttonNextPage = new System.Windows.Forms.Button();
            this.buttonLastPage = new System.Windows.Forms.Button();
            this.buttonFirstPage = new System.Windows.Forms.Button();
            this.labelCopyright = new System.Windows.Forms.Label();
            this.linkLabelWebsite = new System.Windows.Forms.LinkLabel();
            this.buttonContactWallet = new System.Windows.Forms.Button();
            this.buttonResearch = new System.Windows.Forms.Button();
            this.textBoxResearch = new System.Windows.Forms.TextBox();
            this.panelMainForm = new Xenophyte_Wallet.FormCustom.ClassPanel();
            this.panelControlWallet = new Xenophyte_Wallet.FormCustom.ClassPanel();
            this.labelNoticeTotalPendingTransactionOnReceive = new System.Windows.Forms.Label();
            this.labelNoticeWalletBalance = new System.Windows.Forms.Label();
            this.labelNoticeWalletAddress = new System.Windows.Forms.Label();
            this.pictureBoxQRCodeWallet = new System.Windows.Forms.PictureBox();
            this.pictureBoxLogo = new System.Windows.Forms.PictureBox();
            this.menuStripMenu.SuspendLayout();
            this.panelControlWallet.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxQRCodeWallet)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStripMenu
            // 
            this.menuStripMenu.BackColor = System.Drawing.Color.LightSkyBlue;
            this.menuStripMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.languageToolStripMenuItem,
            this.settingToolStripMenuItem,
            this.themeToolStripMenuItem,
            this.exportToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStripMenu.Location = new System.Drawing.Point(20, 60);
            this.menuStripMenu.Name = "menuStripMenu";
            this.menuStripMenu.Size = new System.Drawing.Size(992, 24);
            this.menuStripMenu.TabIndex = 0;
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mainMenuToolStripMenuItem,
            this.createWalletToolStripMenuItem,
            this.openWalletToolStripMenuItem,
            this.closeWalletToolStripMenuItem,
            this.restoreWalletToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // mainMenuToolStripMenuItem
            // 
            this.mainMenuToolStripMenuItem.Name = "mainMenuToolStripMenuItem";
            this.mainMenuToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.mainMenuToolStripMenuItem.Text = "Main Menu";
            this.mainMenuToolStripMenuItem.Click += new System.EventHandler(this.MainMenuToolStripMenuItem_Click);
            // 
            // createWalletToolStripMenuItem
            // 
            this.createWalletToolStripMenuItem.Name = "createWalletToolStripMenuItem";
            this.createWalletToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.createWalletToolStripMenuItem.Text = "Create Wallet";
            this.createWalletToolStripMenuItem.Click += new System.EventHandler(this.CreateWalletToolStripMenuItem_Click);
            // 
            // openWalletToolStripMenuItem
            // 
            this.openWalletToolStripMenuItem.Name = "openWalletToolStripMenuItem";
            this.openWalletToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.openWalletToolStripMenuItem.Text = "Open Wallet";
            this.openWalletToolStripMenuItem.Click += new System.EventHandler(this.OpenWalletToolStripMenuItem_Click);
            // 
            // closeWalletToolStripMenuItem
            // 
            this.closeWalletToolStripMenuItem.Name = "closeWalletToolStripMenuItem";
            this.closeWalletToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.closeWalletToolStripMenuItem.Text = "Close Wallet";
            this.closeWalletToolStripMenuItem.Click += new System.EventHandler(this.CloseWalletToolStripMenuItem_Click);
            // 
            // restoreWalletToolStripMenuItem
            // 
            this.restoreWalletToolStripMenuItem.Name = "restoreWalletToolStripMenuItem";
            this.restoreWalletToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.restoreWalletToolStripMenuItem.Text = "Restore Wallet";
            this.restoreWalletToolStripMenuItem.Click += new System.EventHandler(this.restoreWalletToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.ExitToolStripMenuItem_Click);
            // 
            // languageToolStripMenuItem
            // 
            this.languageToolStripMenuItem.Name = "languageToolStripMenuItem";
            this.languageToolStripMenuItem.Size = new System.Drawing.Size(71, 20);
            this.languageToolStripMenuItem.Text = "Language";
            // 
            // settingToolStripMenuItem
            // 
            this.settingToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.securityToolStripMenuItem,
            this.syncToolStripMenuItem});
            this.settingToolStripMenuItem.Name = "settingToolStripMenuItem";
            this.settingToolStripMenuItem.Size = new System.Drawing.Size(56, 20);
            this.settingToolStripMenuItem.Text = "Setting";
            // 
            // securityToolStripMenuItem
            // 
            this.securityToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.changePasswordToolStripMenuItem,
            this.settingPinCodeToolStripMenuItem});
            this.securityToolStripMenuItem.Name = "securityToolStripMenuItem";
            this.securityToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.securityToolStripMenuItem.Text = "Security";
            // 
            // changePasswordToolStripMenuItem
            // 
            this.changePasswordToolStripMenuItem.Name = "changePasswordToolStripMenuItem";
            this.changePasswordToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.changePasswordToolStripMenuItem.Text = "Change Password";
            this.changePasswordToolStripMenuItem.Click += new System.EventHandler(this.ChangePasswordToolStripMenuItem_Click);
            // 
            // settingPinCodeToolStripMenuItem
            // 
            this.settingPinCodeToolStripMenuItem.Name = "settingPinCodeToolStripMenuItem";
            this.settingPinCodeToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.settingPinCodeToolStripMenuItem.Text = "Setting Pin Code";
            this.settingPinCodeToolStripMenuItem.Click += new System.EventHandler(this.SettingPinCodeToolStripMenuItem_Click);
            // 
            // syncToolStripMenuItem
            // 
            this.syncToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.remoteNodeSettingToolStripMenuItem,
            this.resyncTransactionToolStripMenuItem,
            this.resyncBlockToolStripMenuItem});
            this.syncToolStripMenuItem.Name = "syncToolStripMenuItem";
            this.syncToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.syncToolStripMenuItem.Text = "Sync";
            // 
            // remoteNodeSettingToolStripMenuItem
            // 
            this.remoteNodeSettingToolStripMenuItem.Name = "remoteNodeSettingToolStripMenuItem";
            this.remoteNodeSettingToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.remoteNodeSettingToolStripMenuItem.Text = "Remote Node Setting";
            this.remoteNodeSettingToolStripMenuItem.Click += new System.EventHandler(this.remoteNodeSettingToolStripMenuItem_Click);
            // 
            // resyncTransactionToolStripMenuItem
            // 
            this.resyncTransactionToolStripMenuItem.Name = "resyncTransactionToolStripMenuItem";
            this.resyncTransactionToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.resyncTransactionToolStripMenuItem.Text = "Resync Transaction";
            this.resyncTransactionToolStripMenuItem.Click += new System.EventHandler(this.resyncTransactionToolStripMenuItem_Click);
            // 
            // resyncBlockToolStripMenuItem
            // 
            this.resyncBlockToolStripMenuItem.Name = "resyncBlockToolStripMenuItem";
            this.resyncBlockToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.resyncBlockToolStripMenuItem.Text = "Resync Block";
            this.resyncBlockToolStripMenuItem.Click += new System.EventHandler(this.resyncBlockToolStripMenuItem_Click);
            // 
            // themeToolStripMenuItem
            // 
            this.themeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lightToolStripMenuItem,
            this.darkToolStripMenuItem,
            this.darkerToolStripMenuItem});
            this.themeToolStripMenuItem.Name = "themeToolStripMenuItem";
            this.themeToolStripMenuItem.Size = new System.Drawing.Size(55, 20);
            this.themeToolStripMenuItem.Text = "Theme";
            // 
            // lightToolStripMenuItem
            // 
            this.lightToolStripMenuItem.Name = "lightToolStripMenuItem";
            this.lightToolStripMenuItem.Size = new System.Drawing.Size(108, 22);
            this.lightToolStripMenuItem.Text = "Light";
            this.lightToolStripMenuItem.Click += new System.EventHandler(this.lightToolStripMenuItem_Click);
            // 
            // darkToolStripMenuItem
            // 
            this.darkToolStripMenuItem.Name = "darkToolStripMenuItem";
            this.darkToolStripMenuItem.Size = new System.Drawing.Size(108, 22);
            this.darkToolStripMenuItem.Text = "Dark";
            this.darkToolStripMenuItem.Click += new System.EventHandler(this.darkToolStripMenuItem_Click);
            // 
            // darkerToolStripMenuItem
            // 
            this.darkerToolStripMenuItem.Name = "darkerToolStripMenuItem";
            this.darkerToolStripMenuItem.Size = new System.Drawing.Size(108, 22);
            this.darkerToolStripMenuItem.Text = "Darker";
            this.darkerToolStripMenuItem.Click += new System.EventHandler(this.darkerToolStipMenuItem_Click);
            // 
            // exportToolStripMenuItem
            // 
            this.exportToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.transactionToolStripMenuItem});
            this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            this.exportToolStripMenuItem.Size = new System.Drawing.Size(53, 20);
            this.exportToolStripMenuItem.Text = "Export";
            // 
            // transactionToolStripMenuItem
            // 
            this.transactionToolStripMenuItem.Name = "transactionToolStripMenuItem";
            this.transactionToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
            this.transactionToolStripMenuItem.Text = "Transaction";
            this.transactionToolStripMenuItem.Click += new System.EventHandler(this.transactionToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // metroButtonBlockExplorerWallet
            // 
            this.metroButtonBlockExplorerWallet.Location = new System.Drawing.Point(20, 357);
            this.metroButtonBlockExplorerWallet.Name = "metroButtonBlockExplorerWallet";
            this.metroButtonBlockExplorerWallet.Size = new System.Drawing.Size(194, 54);
            this.metroButtonBlockExplorerWallet.TabIndex = 3;
            this.metroButtonBlockExplorerWallet.Text = "Block Explorer";
            this.metroButtonBlockExplorerWallet.Click += new System.EventHandler(this.MetroButtonBlockExplorerWallet_Click);
            // 
            // metroButtonTransactionWallet
            // 
            this.metroButtonTransactionWallet.Location = new System.Drawing.Point(20, 297);
            this.metroButtonTransactionWallet.Name = "metroButtonTransactionWallet";
            this.metroButtonTransactionWallet.Size = new System.Drawing.Size(194, 54);
            this.metroButtonTransactionWallet.TabIndex = 2;
            this.metroButtonTransactionWallet.Text = "Transactions History";
            this.metroButtonTransactionWallet.Click += new System.EventHandler(this.MetroButtonTransactionWallet_Click);
            // 
            // metroButtonSendTransactionWallet
            // 
            this.metroButtonSendTransactionWallet.Location = new System.Drawing.Point(20, 237);
            this.metroButtonSendTransactionWallet.Name = "metroButtonSendTransactionWallet";
            this.metroButtonSendTransactionWallet.Size = new System.Drawing.Size(194, 54);
            this.metroButtonSendTransactionWallet.TabIndex = 1;
            this.metroButtonSendTransactionWallet.Text = "Send Transaction";
            this.metroButtonSendTransactionWallet.Click += new System.EventHandler(this.MetroButtonSendTransactionWallet_Click);
            // 
            // buttonOverviewWallet
            // 
            this.buttonOverviewWallet.Location = new System.Drawing.Point(20, 177);
            this.buttonOverviewWallet.Name = "buttonOverviewWallet";
            this.buttonOverviewWallet.Size = new System.Drawing.Size(194, 54);
            this.buttonOverviewWallet.TabIndex = 0;
            this.buttonOverviewWallet.Text = "Overview";
            this.buttonOverviewWallet.Click += new System.EventHandler(this.ButtonOverviewWallet_Click);
            // 
            // labelSyncInformation
            // 
            this.labelSyncInformation.AutoSize = true;
            this.labelSyncInformation.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelSyncInformation.Location = new System.Drawing.Point(17, 710);
            this.labelSyncInformation.Name = "labelSyncInformation";
            this.labelSyncInformation.Size = new System.Drawing.Size(103, 15);
            this.labelSyncInformation.TabIndex = 3;
            this.labelSyncInformation.Text = "Not connected.";
            // 
            // timerRefresh
            // 
            this.timerRefresh.Enabled = true;
            this.timerRefresh.Tick += new System.EventHandler(this.TimerRefresh_Tick);
            // 
            // labelCoinName
            // 
            this.labelCoinName.AutoSize = true;
            this.labelCoinName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelCoinName.Location = new System.Drawing.Point(17, 674);
            this.labelCoinName.Name = "labelCoinName";
            this.labelCoinName.Size = new System.Drawing.Size(165, 16);
            this.labelCoinName.TabIndex = 4;
            this.labelCoinName.Text = "Coin Name: Xenophyte";
            // 
            // buttonPreviousPage
            // 
            this.buttonPreviousPage.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonPreviousPage.Location = new System.Drawing.Point(735, 679);
            this.buttonPreviousPage.Name = "buttonPreviousPage";
            this.buttonPreviousPage.Size = new System.Drawing.Size(75, 23);
            this.buttonPreviousPage.TabIndex = 8;
            this.buttonPreviousPage.Text = "<";
            this.buttonPreviousPage.UseVisualStyleBackColor = true;
            this.buttonPreviousPage.Click += new System.EventHandler(this.buttonPreviousPage_Click);
            // 
            // labelNoticeCurrentPage
            // 
            this.labelNoticeCurrentPage.AutoSize = true;
            this.labelNoticeCurrentPage.Location = new System.Drawing.Point(826, 683);
            this.labelNoticeCurrentPage.Name = "labelNoticeCurrentPage";
            this.labelNoticeCurrentPage.Size = new System.Drawing.Size(13, 13);
            this.labelNoticeCurrentPage.TabIndex = 9;
            this.labelNoticeCurrentPage.Text = "1";
            this.labelNoticeCurrentPage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // buttonNextPage
            // 
            this.buttonNextPage.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonNextPage.Location = new System.Drawing.Point(856, 679);
            this.buttonNextPage.Name = "buttonNextPage";
            this.buttonNextPage.Size = new System.Drawing.Size(75, 23);
            this.buttonNextPage.TabIndex = 7;
            this.buttonNextPage.Text = ">";
            this.buttonNextPage.UseVisualStyleBackColor = true;
            this.buttonNextPage.Click += new System.EventHandler(this.buttonNextPage_Click);
            // 
            // buttonLastPage
            // 
            this.buttonLastPage.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonLastPage.Location = new System.Drawing.Point(937, 679);
            this.buttonLastPage.Name = "buttonLastPage";
            this.buttonLastPage.Size = new System.Drawing.Size(75, 23);
            this.buttonLastPage.TabIndex = 10;
            this.buttonLastPage.Text = ">>";
            this.buttonLastPage.UseVisualStyleBackColor = true;
            this.buttonLastPage.Click += new System.EventHandler(this.buttonLastPage_Click);
            // 
            // buttonFirstPage
            // 
            this.buttonFirstPage.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonFirstPage.Location = new System.Drawing.Point(654, 679);
            this.buttonFirstPage.Name = "buttonFirstPage";
            this.buttonFirstPage.Size = new System.Drawing.Size(75, 23);
            this.buttonFirstPage.TabIndex = 11;
            this.buttonFirstPage.Text = "<<";
            this.buttonFirstPage.UseVisualStyleBackColor = true;
            this.buttonFirstPage.Click += new System.EventHandler(this.buttonFirstPage_Click);
            // 
            // labelCopyright
            // 
            this.labelCopyright.AutoSize = true;
            this.labelCopyright.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelCopyright.Location = new System.Drawing.Point(17, 736);
            this.labelCopyright.Name = "labelCopyright";
            this.labelCopyright.Size = new System.Drawing.Size(259, 15);
            this.labelCopyright.TabIndex = 12;
            this.labelCopyright.Text = "Copyright © "+DateTime.Now.Year+" Xenophyte developer.";
            this.labelCopyright.Click += new System.EventHandler(this.labelCopyright_Click);
            // 
            // linkLabelWebsite
            // 
            this.linkLabelWebsite.AutoSize = true;
            this.linkLabelWebsite.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.linkLabelWebsite.Location = new System.Drawing.Point(850, 736);
            this.linkLabelWebsite.Name = "linkLabelWebsite";
            this.linkLabelWebsite.Size = new System.Drawing.Size(146, 15);
            this.linkLabelWebsite.TabIndex = 13;
            this.linkLabelWebsite.TabStop = true;
            this.linkLabelWebsite.Text = "https://xenophyte.com";
            this.linkLabelWebsite.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelWebsite_LinkClicked);
            // 
            // buttonContactWallet
            // 
            this.buttonContactWallet.Location = new System.Drawing.Point(20, 417);
            this.buttonContactWallet.Name = "buttonContactWallet";
            this.buttonContactWallet.Size = new System.Drawing.Size(194, 54);
            this.buttonContactWallet.TabIndex = 14;
            this.buttonContactWallet.Text = "Contact";
            this.buttonContactWallet.Click += new System.EventHandler(this.buttonContactWallet_Click);
            // 
            // buttonResearch
            // 
            this.buttonResearch.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonResearch.Location = new System.Drawing.Point(220, 678);
            this.buttonResearch.Name = "buttonResearch";
            this.buttonResearch.Size = new System.Drawing.Size(75, 23);
            this.buttonResearch.TabIndex = 11;
            this.buttonResearch.Text = "Search";
            this.buttonResearch.UseVisualStyleBackColor = true;
            this.buttonResearch.Visible = false;
            this.buttonResearch.Click += new System.EventHandler(this.buttonResearch_Click);
            // 
            // textBoxResearch
            // 
            this.textBoxResearch.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxResearch.Location = new System.Drawing.Point(301, 680);
            this.textBoxResearch.Name = "textBoxResearch";
            this.textBoxResearch.Size = new System.Drawing.Size(347, 20);
            this.textBoxResearch.TabIndex = 11;
            this.textBoxResearch.Visible = false;
            this.textBoxResearch.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxResearch_KeyDown);
            // 
            // panelMainForm
            // 
            this.panelMainForm.Location = new System.Drawing.Point(220, 177);
            this.panelMainForm.Name = "panelMainForm";
            this.panelMainForm.Size = new System.Drawing.Size(792, 496);
            this.panelMainForm.TabIndex = 6;
            // 
            // panelControlWallet
            // 
            this.panelControlWallet.Controls.Add(this.labelNoticeTotalPendingTransactionOnReceive);
            this.panelControlWallet.Controls.Add(this.labelNoticeWalletBalance);
            this.panelControlWallet.Controls.Add(this.labelNoticeWalletAddress);
            this.panelControlWallet.Location = new System.Drawing.Point(20, 87);
            this.panelControlWallet.Name = "panelControlWallet";
            this.panelControlWallet.Size = new System.Drawing.Size(902, 84);
            this.panelControlWallet.TabIndex = 1;
            this.panelControlWallet.Visible = false;
            // 
            // labelNoticeTotalPendingTransactionOnReceive
            // 
            this.labelNoticeTotalPendingTransactionOnReceive.AutoSize = true;
            this.labelNoticeTotalPendingTransactionOnReceive.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelNoticeTotalPendingTransactionOnReceive.Location = new System.Drawing.Point(4, 58);
            this.labelNoticeTotalPendingTransactionOnReceive.Name = "labelNoticeTotalPendingTransactionOnReceive";
            this.labelNoticeTotalPendingTransactionOnReceive.Size = new System.Drawing.Size(256, 15);
            this.labelNoticeTotalPendingTransactionOnReceive.TabIndex = 9;
            this.labelNoticeTotalPendingTransactionOnReceive.Text = "Total Pending Transaction On Receive:";
            // 
            // labelNoticeWalletBalance
            // 
            this.labelNoticeWalletBalance.AutoSize = true;
            this.labelNoticeWalletBalance.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelNoticeWalletBalance.Location = new System.Drawing.Point(4, 6);
            this.labelNoticeWalletBalance.Name = "labelNoticeWalletBalance";
            this.labelNoticeWalletBalance.Size = new System.Drawing.Size(69, 16);
            this.labelNoticeWalletBalance.TabIndex = 7;
            this.labelNoticeWalletBalance.Text = "Balance:";
            // 
            // labelNoticeWalletAddress
            // 
            this.labelNoticeWalletAddress.AutoSize = true;
            this.labelNoticeWalletAddress.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelNoticeWalletAddress.Location = new System.Drawing.Point(4, 31);
            this.labelNoticeWalletAddress.Name = "labelNoticeWalletAddress";
            this.labelNoticeWalletAddress.Size = new System.Drawing.Size(106, 15);
            this.labelNoticeWalletAddress.TabIndex = 5;
            this.labelNoticeWalletAddress.Text = "Wallet Address:";
            this.labelNoticeWalletAddress.Click += new System.EventHandler(this.labelNoticeWalletAddress_Click);
            // 
            // pictureBoxQRCodeWallet
            // 
            this.pictureBoxQRCodeWallet.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBoxQRCodeWallet.Location = new System.Drawing.Point(928, 87);
            this.pictureBoxQRCodeWallet.Name = "pictureBoxQRCodeWallet";
            this.pictureBoxQRCodeWallet.Size = new System.Drawing.Size(84, 84);
            this.pictureBoxQRCodeWallet.TabIndex = 16;
            this.pictureBoxQRCodeWallet.TabStop = false;
            // 
            // pictureBoxLogo
            // 
            this.pictureBoxLogo.BackgroundImage = global::Xenophyte_Wallet.Properties.Resources.logo_web_transparent;
            this.pictureBoxLogo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBoxLogo.Location = new System.Drawing.Point(20, 477);
            this.pictureBoxLogo.Name = "pictureBoxLogo";
            this.pictureBoxLogo.Size = new System.Drawing.Size(194, 194);
            this.pictureBoxLogo.TabIndex = 15;
            this.pictureBoxLogo.TabStop = false;
            // 
            // WalletXenophyte
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1032, 754);
            this.Controls.Add(this.pictureBoxQRCodeWallet);
            this.Controls.Add(this.pictureBoxLogo);
            this.Controls.Add(this.buttonContactWallet);
            this.Controls.Add(this.linkLabelWebsite);
            this.Controls.Add(this.labelCopyright);
            this.Controls.Add(this.buttonFirstPage);
            this.Controls.Add(this.buttonLastPage);
            this.Controls.Add(this.labelNoticeCurrentPage);
            this.Controls.Add(this.buttonPreviousPage);
            this.Controls.Add(this.buttonNextPage);
            this.Controls.Add(this.labelCoinName);
            this.Controls.Add(this.labelSyncInformation);
            this.Controls.Add(this.panelMainForm);
            this.Controls.Add(this.panelControlWallet);
            this.Controls.Add(this.menuStripMenu);
            this.Controls.Add(this.buttonOverviewWallet);
            this.Controls.Add(this.metroButtonSendTransactionWallet);
            this.Controls.Add(this.metroButtonTransactionWallet);
            this.Controls.Add(this.metroButtonBlockExplorerWallet);
            this.Controls.Add(this.buttonResearch);
            this.Controls.Add(this.textBoxResearch);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStripMenu;
            this.Name = "WalletXenophyte";
            this.Text = "Xenophyte Desktop Wallet - v";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.WalletXenophyte_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.WalletXenophyte_FormClosed);
            this.Load += new System.EventHandler(this.WalletXenophyte_Load);
            this.SizeChanged += new System.EventHandler(this.WalletXenophyte_SizeChanged);
            this.Resize += new System.EventHandler(this.WalletXenophyte_Resize);
            this.menuStripMenu.ResumeLayout(false);
            this.menuStripMenu.PerformLayout();
            this.panelControlWallet.ResumeLayout(false);
            this.panelControlWallet.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxQRCodeWallet)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStripMenu;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem createWalletToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openWalletToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeWalletToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem languageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem settingToolStripMenuItem;
        private System.Windows.Forms.Button buttonOverviewWallet;
        private System.Windows.Forms.Button metroButtonBlockExplorerWallet;
        private System.Windows.Forms.Button metroButtonTransactionWallet;
        private System.Windows.Forms.Button metroButtonSendTransactionWallet;
        private ClassPanel panelMainForm;
        private System.Windows.Forms.ToolStripMenuItem mainMenuToolStripMenuItem;
        public ClassPanel panelControlWallet;
        private System.Windows.Forms.Label labelSyncInformation;
        private System.Windows.Forms.Timer timerRefresh;
        private System.Windows.Forms.ToolStripMenuItem securityToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem changePasswordToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem settingPinCodeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem syncToolStripMenuItem;
        private System.Windows.Forms.Label labelCoinName;
        private System.Windows.Forms.ToolStripMenuItem remoteNodeSettingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resyncTransactionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resyncBlockToolStripMenuItem;
        private System.Windows.Forms.Button buttonPreviousPage;
        private System.Windows.Forms.Label labelNoticeCurrentPage;
        private System.Windows.Forms.Button buttonNextPage;
        private System.Windows.Forms.Button buttonLastPage;
        private System.Windows.Forms.Button buttonFirstPage;
        public System.Windows.Forms.Label labelNoticeWalletAddress;
        public System.Windows.Forms.Label labelNoticeWalletBalance;
        public System.Windows.Forms.Label labelNoticeTotalPendingTransactionOnReceive;
        private System.Windows.Forms.ToolStripMenuItem restoreWalletToolStripMenuItem;
        private System.Windows.Forms.Label labelCopyright;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.LinkLabel linkLabelWebsite;
        private System.Windows.Forms.Button buttonContactWallet;
        private System.Windows.Forms.ToolStripMenuItem themeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem darkerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem lightToolStripMenuItem;
        private System.Windows.Forms.PictureBox pictureBoxLogo;
        private System.Windows.Forms.ToolStripMenuItem darkToolStripMenuItem;
        private System.Windows.Forms.PictureBox pictureBoxQRCodeWallet;

        private System.Windows.Forms.Button buttonResearch;
        private System.Windows.Forms.TextBox textBoxResearch;
        private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem transactionToolStripMenuItem;
    }
}