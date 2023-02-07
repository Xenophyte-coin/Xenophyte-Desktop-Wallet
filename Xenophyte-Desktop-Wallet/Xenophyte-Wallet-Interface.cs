﻿#if WINDOWS
using MetroFramework.Forms;
#endif
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Xenophyte_Connector_All.Setting;
using Xenophyte_Connector_All.Utils;
using Xenophyte_Connector_All.Wallet;
using Xenophyte_Wallet.Features;
using Xenophyte_Wallet.FormPhase;
using Xenophyte_Wallet.FormPhase.MainForm;
using Xenophyte_Wallet.FormPhase.ParallelForm;
using Xenophyte_Wallet.Properties;
using Xenophyte_Wallet.Utility;
using Xenophyte_Wallet.Wallet.Setting;
using Xenophyte_Wallet.Wallet.Sync;
using Xenophyte_Wallet.Wallet.Sync.Object;
using Xenophyte_Wallet.Wallet.Tcp;
using ZXing;
using ZXing.QrCode;
#if WINDOWS
using MetroFramework;
using MetroFramework.Controls;

#endif

#if DEBUG
using Xenophyte_Wallet.Debug;
#endif

namespace Xenophyte_Wallet
{
#if WINDOWS
    public sealed partial class WalletXenophyte : MetroForm
#else
    public sealed partial class WalletXenophyte : Form
#endif
    {
        /// <summary>
        ///     Form objects
        /// </summary>
        public OpenWallet OpenWalletForm;

        public MainWallet MainWalletForm;
        public OverviewWallet OverviewWalletForm;
        public TransactionHistoryWallet TransactionHistoryWalletForm;
        public SendTransactionWallet SendTransactionWalletForm;
        public CreateWallet CreateWalletForm;
        public BlockExplorerWallet BlockWalletForm;
        public RestoreWallet RestoreWalletForm;
        public ContactWallet ContactWalletForm;
        public ClassWalletObject ClassWalletObject;
        public CancellationTokenSource WalletCancellationToken;
        public CancellationTokenSource WalletSyncCancellationToken;


        public ClassWalletSyncMode WalletSyncMode;
        public string WalletSyncHostname = string.Empty;
        public bool WalletEnableProxyMode;


        /// <summary>
        ///     Form resize objects
        /// </summary>
        public int CurrentInterfaceWidth;

        public int CurrentInterfaceHeight;
        public int BaseInterfaceWidth;
        public int BaseInterfaceHeight;
        public List<Tuple<Size, Point>> ListControlSizeBase = new List<Tuple<Size, Point>>();
        public List<Tuple<Size, Point>> ListControlSizeMain = new List<Tuple<Size, Point>>();
        public List<Tuple<Size, Point>> ListControlSizePanelWallet = new List<Tuple<Size, Point>>();
        public List<Tuple<Size, Point>> ListControlSizeBlock = new List<Tuple<Size, Point>>();
        public List<Tuple<Size, Point>> ListControlSizeCreateWallet = new List<Tuple<Size, Point>>();
        public List<Tuple<Size, Point>> ListControlSizeOpenWallet = new List<Tuple<Size, Point>>();
        public List<Tuple<Size, Point>> ListControlSizeOverview = new List<Tuple<Size, Point>>();
        public List<Tuple<Size, Point>> ListControlSizeSendTransaction = new List<Tuple<Size, Point>>();
        public List<Tuple<Size, Point>> ListControlSizeTransaction = new List<Tuple<Size, Point>>();
        public List<Tuple<Size, Point>> ListControlSizeTransactionTabPage = new List<Tuple<Size, Point>>();
        public List<Tuple<Size, Point>> ListControlSizeRestoreWallet = new List<Tuple<Size, Point>>();
        public List<Tuple<Size, Point>> ListControlSizeContactWallet = new List<Tuple<Size, Point>>();



        private const int ThreadUpdateNetworkStatsInterval = 1000;
        public int MaxTransactionPerPage = 100;
        public int MaxBlockPerPage = 100;
        private const int MinSizeTransactionHash = 100;

        /// <summary>
        ///     Boolean objects
        /// </summary>
        public bool EnableUpdateBlockWallet;

        private bool _isCopyWalletAddress;
        public bool EnableTokenNetworkMode;

        /// <summary>
        ///     Objects transaction history & block explorer
        /// </summary>
        public Dictionary<int, string> ListBlockHashShowed = new Dictionary<int, string>();

        public int TotalTransactionRead;
        public int TotalAnonymityTransactionRead;
        public int TotalBlockRead;
        public int CurrentTransactionHistoryPageAnonymousReceived;
        public int CurrentTransactionHistoryPageAnonymousSend;
        public int CurrentTransactionHistoryPageNormalSend;
        public int CurrentTransactionHistoryPageNormalReceive;
        public int CurrentTransactionHistoryPageBlockReward;
        public int CurrentBlockExplorerPage;
        public int TotalTransactionAnonymousReceived;
        public int TotalTransactionAnonymousSend;
        public int TotalTransactionNormalReceived;
        public int TotalTransactionNormalSend;
        public int TotalTransactionBlockReward;
        public bool NormalTransactionLoaded;
        public bool AnonymousTransactionLoaded;
        private bool _firstStart;


        /// <summary>
        ///     Constructor.
        /// </summary>
        public WalletXenophyte()
        {
            Program.WalletXenophyte = this;
            MainWalletForm = new MainWallet();
            OpenWalletForm = new OpenWallet();
            OverviewWalletForm = new OverviewWallet();
            TransactionHistoryWalletForm = new TransactionHistoryWallet();
            SendTransactionWalletForm = new SendTransactionWallet();
            CreateWalletForm = new CreateWallet();
            BlockWalletForm = new BlockExplorerWallet();
            RestoreWalletForm = new RestoreWallet();
            ContactWalletForm = new ContactWallet();
            InitializeComponent();
            SetStyle(
                ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw |
                ControlStyles.OptimizedDoubleBuffer, true);
            ClassWalletObject = new ClassWalletObject();
            _firstStart = ClassWalletSetting.LoadSetting(); // Load the setting file.
        }

        public async Task InitializationWalletObject()
        {
            if (ClassWalletObject != null)
            {
                await Program.WalletXenophyte.ClassWalletObject.FullDisconnection(true, true);
                ClassWalletObject.Dispose();
                ClassWalletObject = null;
            }
            ClassWalletObject = new ClassWalletObject();
        }

        /// <summary>
        ///     Update drawing of GDI+
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.ExStyle |= 0x02000000; // Turn on WS_EX_COMPOSITED
                return cp;
            }
        }

        #region About Form Phase

        /// <summary>
        ///     Change form from actual Form Phase.
        /// </summary>
        public void SwitchForm(string formPhase)
        {
            if (ClassFormPhase.FormPhase != formPhase)
            {
                HideAllFormAsync();
                ClassFormPhase.FormPhase = formPhase;
                MethodInvoker invoke;
                switch (formPhase)
                {
                    case ClassFormPhaseEnumeration.Main:
                        invoke = () =>
                        {
                            MainWalletForm.TopLevel = false;
                            MainWalletForm.AutoScroll = false;
                            MainWalletForm.Parent = panelMainForm;
                            MainWalletForm.Show();
                            MainWalletForm.Refresh();
                            HideWalletAddressQrCode();
                            UpdateStyles();
                        };

                        BeginInvoke(invoke);
                        break;
                    case ClassFormPhaseEnumeration.CreateWallet:
                        invoke = () =>
                        {
                            CreateWalletForm.TopLevel = false;
                            CreateWalletForm.AutoScroll = false;
                            CreateWalletForm.Parent = panelMainForm;
                            CreateWalletForm.Show();
                            CreateWalletForm.Refresh();
                            HideWalletAddressQrCode();
                            UpdateStyles();
                        };
                        BeginInvoke(invoke);
                        break;
                    case ClassFormPhaseEnumeration.OpenWallet:
                        invoke = () =>
                        {
                            OpenWalletForm.TopLevel = false;
                            OpenWalletForm.AutoScroll = false;
                            OpenWalletForm.Parent = panelMainForm;
                            OpenWalletForm.Show();
                            OpenWalletForm.Refresh();
                            HideWalletAddressQrCode();
                            UpdateStyles();
                        };
                        BeginInvoke(invoke);
                        break;
                    case ClassFormPhaseEnumeration.Overview:
                        invoke = () =>
                        {
                            OverviewWalletForm.TopLevel = false;
                            OverviewWalletForm.AutoScroll = false;
                            OverviewWalletForm.Parent = panelMainForm;
                            OverviewWalletForm.Show();
                            OverviewWalletForm.Refresh();
                            UpdateStyles();
                        };
                        BeginInvoke(invoke);
                        break;
                    case ClassFormPhaseEnumeration.SendTransaction:
                        invoke = () =>
                        {
                            SendTransactionWalletForm.TopLevel = false;
                            SendTransactionWalletForm.AutoScroll = false;
                            SendTransactionWalletForm.Parent = panelMainForm;
                            SendTransactionWalletForm.Show();
                            SendTransactionWalletForm.Refresh();
                            UpdateStyles();
                        };
                        BeginInvoke(invoke);
                        break;
                    case ClassFormPhaseEnumeration.TransactionHistory:
                        invoke = () =>
                        {
                            TransactionHistoryWalletForm.TopLevel = false;
                            TransactionHistoryWalletForm.AutoScroll = false;
                            TransactionHistoryWalletForm.Parent = panelMainForm;
                            TransactionHistoryWalletForm.Show();
                            TransactionHistoryWalletForm.Refresh();
                            TransactionHistoryWalletForm.listViewAnonymityReceivedTransactionHistory.Refresh();
                            TransactionHistoryWalletForm.listViewAnonymitySendTransactionHistory.Refresh();
                            TransactionHistoryWalletForm.listViewBlockRewardTransactionHistory.Refresh();
                            TransactionHistoryWalletForm.listViewNormalReceivedTransactionHistory.Refresh();
                            TransactionHistoryWalletForm.listViewNormalSendTransactionHistory.Refresh();
                            buttonPreviousPage.Show();
                            buttonNextPage.Show();
                            buttonFirstPage.Show();
                            buttonLastPage.Show();
                            labelNoticeCurrentPage.Show();
                            buttonResearch.Show();
                            textBoxResearch.Show();
                            UpdateStyles();
                        };
                        BeginInvoke(invoke);
                        break;
                    case ClassFormPhaseEnumeration.BlockExplorer:
                        invoke = () =>
                        {
                            BlockWalletForm.TopLevel = false;
                            BlockWalletForm.AutoScroll = false;
                            BlockWalletForm.Parent = panelMainForm;
                            BlockWalletForm.Show();
                            BlockWalletForm.Refresh();
                            BlockWalletForm.listViewBlockExplorer.Refresh();
                            buttonPreviousPage.Show();
                            buttonNextPage.Show();
                            buttonFirstPage.Show();
                            buttonLastPage.Show();
                            labelNoticeCurrentPage.Show();
                            labelNoticeCurrentPage.Text = "" + CurrentBlockExplorerPage;
                            buttonResearch.Show();
                            textBoxResearch.Show();
                            UpdateStyles();
                        };
                        BeginInvoke(invoke);
                        break;
                    case ClassFormPhaseEnumeration.RestoreWallet:
                        invoke = () =>
                        {
                            RestoreWalletForm.TopLevel = false;
                            RestoreWalletForm.AutoScroll = false;
                            RestoreWalletForm.Parent = panelMainForm;
                            RestoreWalletForm.Show();
                            RestoreWalletForm.Refresh();
                            UpdateStyles();
                        };
                        BeginInvoke(invoke);
                        break;
                    case ClassFormPhaseEnumeration.ContactWallet:
                        invoke = () =>
                        {
                            ContactWalletForm.TopLevel = false;
                            ContactWalletForm.AutoScroll = false;
                            ContactWalletForm.Parent = panelMainForm;
                            ContactWalletForm.Show();
                            ContactWalletForm.Refresh();
                            UpdateStyles();
                        };
                        BeginInvoke(invoke);
                        break;
                }
            }
        }

        /// <summary>
        ///     Hide all form.
        /// </summary>
        private void HideAllFormAsync()
        {
            BeginInvoke((MethodInvoker)delegate
           {
               MainWalletForm.Hide();

               panelMainForm.Controls.Clear();

               OpenWalletForm.Hide();

               OverviewWalletForm.Hide();

               TransactionHistoryWalletForm.Hide();

               SendTransactionWalletForm.Hide();

               BlockWalletForm.Hide();

               RestoreWalletForm.Hide();

               ContactWalletForm.Hide();

               buttonPreviousPage.Hide();

               buttonNextPage.Hide();

               buttonFirstPage.Hide();

               buttonLastPage.Hide();

               labelNoticeCurrentPage.Hide();

               buttonResearch.Hide();

               textBoxResearch.Hide();

               UpdateStyles();
           });
        }

        /// <summary>
        ///     Update current page number of transaction history
        /// </summary>
        public void UpdateCurrentPageNumberTransactionHistory()
        {
            if (ClassFormPhase.FormPhase == ClassFormPhaseEnumeration.TransactionHistory)
            {
                MethodInvoker invoke = null;
                if (TransactionHistoryWalletForm.tabPageNormalTransactionSend.Visible) // Normal transaction send list
                    invoke = () => labelNoticeCurrentPage.Text = "" + CurrentTransactionHistoryPageNormalSend;
                if (TransactionHistoryWalletForm.tabPageNormalTransactionReceived.Visible
                ) // Normal transaction received list
                    invoke = () => labelNoticeCurrentPage.Text = "" + CurrentTransactionHistoryPageNormalReceive;
                if (TransactionHistoryWalletForm.tabPageAnonymityTransactionSend.Visible
                ) // Anonymous transaction send list 
                    invoke = () => labelNoticeCurrentPage.Text = "" + CurrentTransactionHistoryPageAnonymousSend;
                if (TransactionHistoryWalletForm.tabPageAnonymityTransactionReceived.Visible
                ) // Anonymous transaction received list 
                    invoke = () => labelNoticeCurrentPage.Text = "" + CurrentTransactionHistoryPageAnonymousReceived;
                if (TransactionHistoryWalletForm.tabPageBlockRewardTransaction.Visible
                ) // block reward transaction list 
                    invoke = () => labelNoticeCurrentPage.Text = "" + CurrentTransactionHistoryPageBlockReward;

                BeginInvoke(invoke);
            }
        }

        /// <summary>
        ///     Update overview network stats automaticaly.
        /// </summary>
        public void UpdateNetworkStats()
        {
            try
            {
                if (WalletCancellationToken != null)
                {
                    Task.Factory.StartNew(async () =>
                    {
                        if (EnableTokenNetworkMode)
                            while (!ClassWalletObject.WalletClosed)
                            {
                                UpdateNetworkStatsLabel();
                                await Task.Delay(ThreadUpdateNetworkStatsInterval);
                            }
                        else
                            while (ClassWalletObject.SeedNodeConnectorWallet.ReturnStatus() &&
                                   !ClassWalletObject.WalletClosed)
                            {
                                if (ClassWalletObject.SeedNodeConnectorWallet != null)
                                    if (ClassWalletObject.SeedNodeConnectorWallet.ReturnStatus() &&
                                        !ClassWalletObject.WalletClosed)
                                        UpdateNetworkStatsLabel();

                                await Task.Delay(ThreadUpdateNetworkStatsInterval);
                            }
                    }, WalletCancellationToken.Token, TaskCreationOptions.DenyChildAttach, TaskScheduler.Current).ConfigureAwait(false);
                }
            }
            catch
            {

            }
        }

        /// <summary>
        ///     Update labels dedicated to network informations
        /// </summary>
        private void UpdateNetworkStatsLabel()
        {

          ;

            if (!string.IsNullOrEmpty(ClassWalletObject.TotalBlockMined) &&
                !string.IsNullOrEmpty(ClassWalletObject.CoinCirculating) &&
                !string.IsNullOrEmpty(ClassWalletObject.CoinMaxSupply) &&
                !string.IsNullOrEmpty(ClassWalletObject.NetworkDifficulty) &&
                !string.IsNullOrEmpty(ClassWalletObject.NetworkHashrate) &&
                !string.IsNullOrEmpty(ClassWalletObject.TotalFee) &&
                !string.IsNullOrEmpty(ClassWalletObject.LastBlockFound))
            {
                UpdateOverviewLabelBlockMined(ClassWalletObject.TotalBlockMined);
                UpdateOverviewLabelCoinCirculating(ClassWalletObject.CoinCirculating);
                UpdateOverviewLabelCoinMaxSupply(ClassWalletObject.CoinMaxSupply);
                UpdateOverviewLabelNetworkDifficulty(ClassWalletObject.NetworkDifficulty);
                UpdateOverviewLabelNetworkHashrate(ClassWalletObject.NetworkHashrate);
                UpdateOverviewLabelTransactionFee(ClassWalletObject.TotalFee);
                UpdateOverviewLabelLastBlockFound(ClassWalletObject.LastBlockFound);

                MethodInvoker invoke = () =>
                    labelNoticeTotalPendingTransactionOnReceive.Text =
                        ClassTranslation.GetLanguageTextFromOrder(
                            ClassTranslationEnumeration.panelwallettotalpendingtransactiononreceivetext) + " " +
                        ClassWalletObject.TotalTransactionPendingOnReceive;
                BeginInvoke(invoke);
            }
        }

        #endregion

        #region Event

        /// <summary>
        ///     Event activated on load.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WalletXenophyte_Load(object sender, EventArgs e)
        {
            Hide();
            Thread.Sleep(100);
            Show();
            Text += Assembly.GetExecutingAssembly().GetName().Version;

            pictureBoxLogo.BackgroundImage = Resources.logo_web_transparent;

            // Initialize form size.
            CurrentInterfaceWidth = Width;
            CurrentInterfaceHeight = Height;
            BaseInterfaceHeight = Height;
            BaseInterfaceWidth = Width;
            ClassFormPhase.InitializeMainInterface(this);
            labelCoinName.Text = "Coin Name: " + ClassConnectorSetting.CoinName;
            if (ListControlSizeBase.Count == 0)
                for (var i = 0; i < Controls.Count; i++)
                    if (i < Controls.Count)
                        ListControlSizeBase.Add(new Tuple<Size, Point>(Controls[i].Size, Controls[i].Location));
            if (ListControlSizePanelWallet.Count == 0)
                for (var i = 0; i < panelControlWallet.Controls.Count; i++)
                    if (i < panelControlWallet.Controls.Count)
                        ListControlSizePanelWallet.Add(new Tuple<Size, Point>(panelControlWallet.Controls[i].Size,
                            panelControlWallet.Controls[i].Location));
            CurrentTransactionHistoryPageAnonymousReceived = 1;
            CurrentTransactionHistoryPageAnonymousSend = 1;
            CurrentTransactionHistoryPageNormalSend = 1;
            CurrentTransactionHistoryPageNormalReceive = 1;
            CurrentTransactionHistoryPageBlockReward = 1;
            CurrentBlockExplorerPage = 1;
            Width += 10;
            Height += 10;

            // Initialize language.
            foreach (var key in ClassTranslation.LanguageDatabases.Keys)
                languageToolStripMenuItem.DropDownItems.Add(ClassTranslation.UppercaseFirst(key), null,
                    LanguageSubMenuItem_Click);
            UpdateGraphicLanguageText();
            UpdateColorStyle(Color.White, Color.Black, Color.White, Color.LightSkyBlue);
        }

        /// <summary>
        ///     Event click for change current language.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LanguageSubMenuItem_Click(object sender, EventArgs e)
        {
            var tooltipitem = (ToolStripItem)sender;
            if (ClassTranslation.ChangeCurrentLanguage(tooltipitem.Text))
            {
                ClassWalletSetting.SaveSetting();
                UpdateGraphicLanguageText();
            }
        }

        /// <summary>
        ///     Update graphic language text.
        /// </summary>
        public void UpdateGraphicLanguageText()
        {
            // Main Interface.
            fileToolStripMenuItem.Text =
                ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration.menufiletext);
            mainMenuToolStripMenuItem.Text =
                ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration.menufilemainmenutext);
            createWalletToolStripMenuItem.Text =
                ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration.menufilecreatewalletmenutext);
            openWalletToolStripMenuItem.Text =
                ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration.menufileopenwalletmenutext);
            restoreWalletToolStripMenuItem.Text =
                ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration.menufilerestorewalletmenutext);
            closeWalletToolStripMenuItem.Text =
                ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration.menufilefunctionclosewallettext);
            exitToolStripMenuItem.Text =
                ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration.menufilefunctionexittext);
            languageToolStripMenuItem.Text =
                ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration.menulanguagetext);
            settingToolStripMenuItem.Text =
                ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration.menusettingtext);
            securityToolStripMenuItem.Text = ClassTranslation.GetLanguageTextFromOrder("MENU_SETTING_SECURITY_TEXT");
            changePasswordToolStripMenuItem.Text =
                ClassTranslation.GetLanguageTextFromOrder("SUBMENU_SETTING_SECURITY_CONFIG_CHANGE_PASSWORD_TEXT");
            settingPinCodeToolStripMenuItem.Text =
                ClassTranslation.GetLanguageTextFromOrder("SUBMENU_SETTING_SECURITY_CONFIG_PIN_CODE_TEXT");
            syncToolStripMenuItem.Text = ClassTranslation.GetLanguageTextFromOrder("MENU_SETTING_SYNC_TEXT");
            remoteNodeSettingToolStripMenuItem.Text =
                ClassTranslation.GetLanguageTextFromOrder("SUBMENU_SETTING_SYNC_REMOTE_NODE_CONFIG_TEXT");
            resyncTransactionToolStripMenuItem.Text =
                ClassTranslation.GetLanguageTextFromOrder("SUBMENU_SETTING_RESYNC_TRANSACTION_TEXT");
            resyncBlockToolStripMenuItem.Text =
                ClassTranslation.GetLanguageTextFromOrder("SUBMENU_SETTING_RESYNC_BLOCK_TEXT");
            helpToolStripMenuItem.Text = ClassTranslation.GetLanguageTextFromOrder("SUBMENU_HELP_TEXT");
            aboutToolStripMenuItem.Text = ClassTranslation.GetLanguageTextFromOrder("SUBMENU_HELP_ABOUT_TEXT");
            if (ClassWalletObject.WalletConnect != null)
            {
                var showPendingAmount = false;
                if (ClassWalletObject.WalletAmountInPending != null)
                    if (!string.IsNullOrEmpty(ClassWalletObject.WalletAmountInPending))
                        showPendingAmount = true;
                if (!showPendingAmount)
                    labelNoticeWalletBalance.Text =
                        ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration.panelwalletbalancetext) +
                        " " +
                        ClassWalletObject.WalletConnect.WalletAmount + " " + ClassConnectorSetting.CoinNameMin;
                else
                    labelNoticeWalletBalance.Text =
                        ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration.panelwalletbalancetext) +
                        " " +
                        ClassWalletObject.WalletConnect.WalletAmount + " " + ClassConnectorSetting.CoinNameMin + " | " +
                        ClassTranslation.GetLanguageTextFromOrder("PANEL_WALLET_PENDING_BALANCE_TEXT") + " " +
                        ClassWalletObject.WalletAmountInPending + " " + ClassConnectorSetting.CoinNameMin;
                labelNoticeWalletAddress.Text =
                    ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration.panelwalletaddresstext) +
                    " " + ClassWalletObject.WalletConnect.WalletAddress;
                labelNoticeTotalPendingTransactionOnReceive.Text =
                    ClassTranslation.GetLanguageTextFromOrder(
                        ClassTranslationEnumeration.panelwallettotalpendingtransactiononreceivetext) + " " +
                    ClassWalletObject.TotalTransactionPendingOnReceive;
            }
            else
            {
                labelNoticeWalletBalance.Text =
                    ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration.panelwalletbalancetext);
                labelNoticeWalletAddress.Text =
                    ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration.panelwalletaddresstext);
                labelNoticeTotalPendingTransactionOnReceive.Text =
                    ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration
                        .panelwallettotalpendingtransactiononreceivetext);
            }

            buttonOverviewWallet.Text = ClassTranslation.GetLanguageTextFromOrder("BUTTON_WALLET_OVERVIEW_TEXT");
            metroButtonSendTransactionWallet.Text =
                ClassTranslation.GetLanguageTextFromOrder("BUTTON_WALLET_SEND_TRANSACTION_TEXT");
            metroButtonTransactionWallet.Text =
                ClassTranslation.GetLanguageTextFromOrder("BUTTON_WALLET_TRANSACTION_HISTORY_TEXT");
            metroButtonBlockExplorerWallet.Text =
                ClassTranslation.GetLanguageTextFromOrder("BUTTON_WALLET_BLOCK_EXPLORER_TEXT");
            buttonContactWallet.Text = ClassTranslation.GetLanguageTextFromOrder("BUTTON_WALLET_CONTACT_TEXT");
#if WINDOWS
            UpdateStyles();
#endif
            // Block explorer menu.
            BlockWalletForm.listViewBlockExplorer.Columns[0].Text =
                ClassTranslation.GetLanguageTextFromOrder("GRID_BLOCK_EXPLORER_COLUMN_ID_TEXT");
            BlockWalletForm.listViewBlockExplorer.Columns[1].Text =
                ClassTranslation.GetLanguageTextFromOrder("GRID_BLOCK_EXPLORER_COLUMN_HASH_TEXT");
            BlockWalletForm.listViewBlockExplorer.Columns[2].Text =
                ClassTranslation.GetLanguageTextFromOrder("GRID_BLOCK_EXPLORER_COLUMN_REWARD_TEXT");
            BlockWalletForm.listViewBlockExplorer.Columns[3].Text =
                ClassTranslation.GetLanguageTextFromOrder("GRID_BLOCK_EXPLORER_COLUMN_DIFFICULTY_TEXT");
            BlockWalletForm.listViewBlockExplorer.Columns[4].Text =
                ClassTranslation.GetLanguageTextFromOrder("GRID_BLOCK_EXPLORER_COLUMN_DATE_CREATE_TEXT");
            BlockWalletForm.listViewBlockExplorer.Columns[5].Text =
                ClassTranslation.GetLanguageTextFromOrder("GRID_BLOCK_EXPLORER_COLUMN_DATE_FOUND_TEXT");
            BlockWalletForm.listViewBlockExplorer.Columns[6].Text =
                ClassTranslation.GetLanguageTextFromOrder("GRID_BLOCK_EXPLORER_COLUMN_TRANSACTION_HASH_TEXT");
            if (BlockWalletForm._labelWaitingText != null)
                BlockWalletForm._labelWaitingText.Text =
                    ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration.waitingmenulabeltext);
#if WINDOWS
            BlockWalletForm.Refresh();
#endif
            // Create wallet menu.
            CreateWalletForm.labelCreateYourWallet.Text =
                ClassTranslation.GetLanguageTextFromOrder("CREATE_WALLET_LABEL_TITLE_TEXT");
            CreateWalletForm.labelCreateNoticePasswordRequirement.Text =
                ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration
                    .createwalletlabelpasswordrequirementtext);
            CreateWalletForm.labelCreateSelectSavingPathWallet.Text =
                ClassTranslation.GetLanguageTextFromOrder("CREATE_WALLET_LABEL_SELECT_WALLET_FILE_TEXT");
            CreateWalletForm.labelCreateSelectWalletPassword.Text =
                ClassTranslation.GetLanguageTextFromOrder("CREATE_WALLET_LABEL_INPUT_WALLET_PASSWORD_TEXT");
            CreateWalletForm.buttonCreateYourWallet.Text =
                ClassTranslation.GetLanguageTextFromOrder("CREATE_WALLET_BUTTON_SUBMIT_CREATE_TEXT");
#if WINDOWS
            CreateWalletForm.Refresh();
#endif
            // Main wallet menu.
            MainWalletForm.labelNoticeWelcomeWallet.Text =
                ClassTranslation.GetLanguageTextFromOrder("MAIN_WALLET_LABEL_WELCOME_INFORMATION_TEXT");
            MainWalletForm.labelNoticeLanguageAndIssue.Text =
                ClassTranslation.GetLanguageTextFromOrder("MAIN_WALLET_LABEL_HELP_INFORMATION_TEXT");
            MainWalletForm.buttonMainOpenMenuWallet.Text =
                ClassTranslation.GetLanguageTextFromOrder("MAIN_WALLET_BUTTON_OPEN_WALLET_MENU_TEXT");
            MainWalletForm.buttonMainCreateWallet.Text =
                ClassTranslation.GetLanguageTextFromOrder("MAIN_WALLET_BUTTON_CREATE_WALLET_MENU_TEXT");
#if WINDOWS
            MainWalletForm.Refresh();
#endif
            // Open wallet menu.
            OpenWalletForm.labelOpenYourWallet.Text =
                ClassTranslation.GetLanguageTextFromOrder("OPEN_WALLET_LABEL_TITLE_TEXT");
            OpenWalletForm.labelOpenFileSelected.Text =
                ClassTranslation.GetLanguageTextFromOrder("OPEN_WALLET_LABEL_FILE_SELECTED_TEXT") + " " +
                OpenWalletForm._fileSelectedPath;

            OpenWalletForm.labelWriteYourWalletPassword.Text =
                ClassTranslation.GetLanguageTextFromOrder("OPEN_WALLET_LABEL_YOUR_PASSWORD_TEXT");
            OpenWalletForm.buttonSearchWalletFile.Text =
                ClassTranslation.GetLanguageTextFromOrder("OPEN_WALLET_BUTTON_SEARCH_WALLET_FILE_TEXT");
            OpenWalletForm.buttonOpenYourWallet.Text =
                ClassTranslation.GetLanguageTextFromOrder("OPEN_WALLET_BUTTON_SUBMIT_WALLET_FILE_TEXT");
#if WINDOWS
            OpenWalletForm.Refresh();
#endif

            OverviewWalletForm.labelTextNetworkStats.Text =
                ClassTranslation.GetLanguageTextFromOrder("OVERVIEW_WALLET_LABEL_TITLE_TEXT");
            try
            {
                // Overview wallet menu.
                if (ClassWalletObject.WalletConnect != null)
                {
                    UpdateOverviewLabelBlockMined(ClassWalletObject.TotalBlockMined);
                    UpdateOverviewLabelCoinCirculating(ClassWalletObject.CoinCirculating);
                    UpdateOverviewLabelCoinMaxSupply(ClassWalletObject.CoinMaxSupply);
                    UpdateOverviewLabelNetworkDifficulty(ClassWalletObject.NetworkDifficulty);
                    UpdateOverviewLabelNetworkHashrate(ClassWalletObject.NetworkHashrate);
                    UpdateOverviewLabelTransactionFee(ClassWalletObject.TotalFee);
                    UpdateOverviewLabelLastBlockFound(ClassWalletObject.LastBlockFound);
                }
                else
                {
                    OverviewWalletForm.labelTextCoinMaxSupply.Text =
                        ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration
                            .overviewwalletlabelcoinmaxsupplytext);
                    OverviewWalletForm.labelTextCoinCirculating.Text =
                        ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration
                            .overviewwalletlabelcoincirculatingtext);
                    OverviewWalletForm.labelTextTransactionFee.Text =
                        ClassTranslation.GetLanguageTextFromOrder(
                            ClassTranslationEnumeration.overviewwalletlabeltransactionfeeaccumulatedtext);
                    OverviewWalletForm.labelTextCoinMined.Text =
                        ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration
                            .overviewwalletlabeltotalcoinminedtext);
                    OverviewWalletForm.labelTextBlockchainHeight.Text =
                        ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration
                            .overviewwalletlabelblockchainheighttext);
                    OverviewWalletForm.labelTextTotalBlockMined.Text =
                        ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration
                            .overviewwalletlabeltotalblockminedtext);
                    OverviewWalletForm.labelTextTotalBlockLeft.Text =
                        ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration
                            .overviewwalletlabeltotalblocklefttext);
                    OverviewWalletForm.labelTextNetworkDifficulty.Text =
                        ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration
                            .overviewwalletlabelnetworkdifficultytext);
                    OverviewWalletForm.labelTextNetworkHashrate.Text =
                        ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration
                            .overviewwalletlabelnetworkhashratetext);
                    OverviewWalletForm.labelTextLastBlockFound.Text =
                        ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration
                            .overviewwalletlabellastblockfoundtext);
                    OverviewWalletForm.labelTextTotalCoinInPending.Text =
                        ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration
                            .overviewwalletlabeltotalcoinpending);
                }
            }
            catch
            {
            }
#if WINDOWS
            OverviewWalletForm.Refresh();
#endif

            // Send transaction wallet menu.
            SendTransactionWalletForm.labelSendTransaction.Text =
                ClassTranslation.GetLanguageTextFromOrder("SEND_TRANSACTION_WALLET_LABEL_TITLE_TEXT");
            SendTransactionWalletForm.labelWalletDestination.Text =
                ClassTranslation.GetLanguageTextFromOrder("SEND_TRANSACTION_WALLET_LABEL_WALLET_ADDRESS_TARGET_TEXT");
            SendTransactionWalletForm.labelAmount.Text =
                ClassTranslation.GetLanguageTextFromOrder("SEND_TRANSACTION_WALLET_LABEL_AMOUNT_TEXT");
            SendTransactionWalletForm.labelFeeTransaction.Text =
                ClassTranslation.GetLanguageTextFromOrder("SEND_TRANSACTION_WALLET_LABEL_FEE_TEXT");
            SendTransactionWalletForm.metroLabelEstimatedTimeReceived.Text =
                ClassTranslation.GetLanguageTextFromOrder("SEND_TRANSACTION_WALLET_LABEL_ESTIMATED_RECEIVE_TIME_TEXT");
            SendTransactionWalletForm.checkBoxHideWalletAddress.Text =
                ClassTranslation.GetLanguageTextFromOrder("SEND_TRANSACTION_WALLET_CHECKBOX_OPTION_ANONYMITY_TEXT");
            SendTransactionWalletForm.buttonSendTransaction.Text =
                ClassTranslation.GetLanguageTextFromOrder("SEND_TRANSACTION_WALLET_BUTTON_SUBMIT_TRANSACTION_TEXT");
#if WINDOWS
            SendTransactionWalletForm.Refresh();
#endif

            // Transaction history wallet menu.
            TransactionHistoryWalletForm.listViewBlockRewardTransactionHistory.Columns[0].Text =
                ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COLUMN_ID");
            TransactionHistoryWalletForm.listViewBlockRewardTransactionHistory.Columns[1].Text =
                ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COLUMN_DATE");
            TransactionHistoryWalletForm.listViewBlockRewardTransactionHistory.Columns[2].Text =
                ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COLUMN_TYPE");
            TransactionHistoryWalletForm.listViewBlockRewardTransactionHistory.Columns[3].Text =
                ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COLUMN_HASH");
            TransactionHistoryWalletForm.listViewBlockRewardTransactionHistory.Columns[4].Text =
                ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COLUMN_AMOUNT");
            TransactionHistoryWalletForm.listViewBlockRewardTransactionHistory.Columns[5].Text =
                ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COLUMN_FEE");
            TransactionHistoryWalletForm.listViewBlockRewardTransactionHistory.Columns[6].Text =
                ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COLUMN_ADDRESS");
            TransactionHistoryWalletForm.listViewBlockRewardTransactionHistory.Columns[7].Text =
                ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COLUMN_DATE_RECEIVED");
            TransactionHistoryWalletForm.listViewBlockRewardTransactionHistory.Columns[8].Text =
                ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COLUMN_BLOCK_HEIGHT_SRC");
            TransactionHistoryWalletForm.listViewAnonymityReceivedTransactionHistory.Columns[0].Text =
                ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COLUMN_ID");
            TransactionHistoryWalletForm.listViewAnonymityReceivedTransactionHistory.Columns[1].Text =
                ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COLUMN_DATE");
            TransactionHistoryWalletForm.listViewAnonymityReceivedTransactionHistory.Columns[2].Text =
                ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COLUMN_TYPE");
            TransactionHistoryWalletForm.listViewAnonymityReceivedTransactionHistory.Columns[3].Text =
                ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COLUMN_HASH");
            TransactionHistoryWalletForm.listViewAnonymityReceivedTransactionHistory.Columns[4].Text =
                ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COLUMN_AMOUNT");
            TransactionHistoryWalletForm.listViewAnonymityReceivedTransactionHistory.Columns[5].Text =
                ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COLUMN_FEE");
            TransactionHistoryWalletForm.listViewAnonymityReceivedTransactionHistory.Columns[6].Text =
                ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COLUMN_ADDRESS");
            TransactionHistoryWalletForm.listViewAnonymityReceivedTransactionHistory.Columns[7].Text =
                ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COLUMN_DATE_RECEIVED");
            TransactionHistoryWalletForm.listViewAnonymityReceivedTransactionHistory.Columns[8].Text =
                ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COLUMN_BLOCK_HEIGHT_SRC");
            TransactionHistoryWalletForm.listViewAnonymitySendTransactionHistory.Columns[0].Text =
                ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COLUMN_ID");
            TransactionHistoryWalletForm.listViewAnonymitySendTransactionHistory.Columns[1].Text =
                ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COLUMN_DATE");
            TransactionHistoryWalletForm.listViewAnonymitySendTransactionHistory.Columns[2].Text =
                ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COLUMN_TYPE");
            TransactionHistoryWalletForm.listViewAnonymitySendTransactionHistory.Columns[3].Text =
                ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COLUMN_HASH");
            TransactionHistoryWalletForm.listViewAnonymitySendTransactionHistory.Columns[4].Text =
                ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COLUMN_AMOUNT");
            TransactionHistoryWalletForm.listViewAnonymitySendTransactionHistory.Columns[5].Text =
                ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COLUMN_FEE");
            TransactionHistoryWalletForm.listViewAnonymitySendTransactionHistory.Columns[6].Text =
                ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COLUMN_ADDRESS");
            TransactionHistoryWalletForm.listViewAnonymitySendTransactionHistory.Columns[7].Text =
                ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COLUMN_DATE_RECEIVED");
            TransactionHistoryWalletForm.listViewAnonymitySendTransactionHistory.Columns[8].Text =
                ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COLUMN_BLOCK_HEIGHT_SRC");
            TransactionHistoryWalletForm.listViewNormalReceivedTransactionHistory.Columns[0].Text =
                ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COLUMN_ID");
            TransactionHistoryWalletForm.listViewNormalReceivedTransactionHistory.Columns[1].Text =
                ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COLUMN_DATE");
            TransactionHistoryWalletForm.listViewNormalReceivedTransactionHistory.Columns[2].Text =
                ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COLUMN_TYPE");
            TransactionHistoryWalletForm.listViewNormalReceivedTransactionHistory.Columns[3].Text =
                ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COLUMN_HASH");
            TransactionHistoryWalletForm.listViewNormalReceivedTransactionHistory.Columns[4].Text =
                ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COLUMN_AMOUNT");
            TransactionHistoryWalletForm.listViewNormalReceivedTransactionHistory.Columns[5].Text =
                ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COLUMN_FEE");
            TransactionHistoryWalletForm.listViewNormalReceivedTransactionHistory.Columns[6].Text =
                ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COLUMN_ADDRESS");
            TransactionHistoryWalletForm.listViewNormalReceivedTransactionHistory.Columns[7].Text =
                ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COLUMN_DATE_RECEIVED");
            TransactionHistoryWalletForm.listViewNormalReceivedTransactionHistory.Columns[8].Text =
                ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COLUMN_BLOCK_HEIGHT_SRC");
            TransactionHistoryWalletForm.listViewNormalSendTransactionHistory.Columns[0].Text =
                ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COLUMN_ID");
            TransactionHistoryWalletForm.listViewNormalSendTransactionHistory.Columns[1].Text =
                ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COLUMN_DATE");
            TransactionHistoryWalletForm.listViewNormalSendTransactionHistory.Columns[2].Text =
                ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COLUMN_TYPE");
            TransactionHistoryWalletForm.listViewNormalSendTransactionHistory.Columns[3].Text =
                ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COLUMN_HASH");
            TransactionHistoryWalletForm.listViewNormalSendTransactionHistory.Columns[4].Text =
                ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COLUMN_AMOUNT");
            TransactionHistoryWalletForm.listViewNormalSendTransactionHistory.Columns[5].Text =
                ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COLUMN_FEE");
            TransactionHistoryWalletForm.listViewNormalSendTransactionHistory.Columns[6].Text =
                ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COLUMN_ADDRESS");
            TransactionHistoryWalletForm.listViewNormalSendTransactionHistory.Columns[7].Text =
                ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COLUMN_DATE_RECEIVED");
            TransactionHistoryWalletForm.listViewNormalSendTransactionHistory.Columns[8].Text =
                ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COLUMN_BLOCK_HEIGHT_SRC");
            TransactionHistoryWalletForm.tabPageNormalTransactionSend.Text =
                ClassTranslation.GetLanguageTextFromOrder(
                    "TRANSACTION_HISTORY_WALLET_TAB_NORMAL_SEND_TRANSACTION_LIST_TEXT");
            TransactionHistoryWalletForm.tabPageAnonymityTransactionSend.Text =
                ClassTranslation.GetLanguageTextFromOrder(
                    "TRANSACTION_HISTORY_WALLET_TAB_ANONYMOUS_SEND_TRANSACTION_LIST_TEXT");
            TransactionHistoryWalletForm.tabPageNormalTransactionReceived.Text =
                ClassTranslation.GetLanguageTextFromOrder(
                    "TRANSACTION_HISTORY_WALLET_TAB_NORMAL_RECEIVED_TRANSACTION_LIST_TEXT");
            TransactionHistoryWalletForm.tabPageAnonymityTransactionReceived.Text =
                ClassTranslation.GetLanguageTextFromOrder(
                    "TRANSACTION_HISTORY_WALLET_TAB_ANONYMOUS_RECEIVE_TRANSACTION_LIST_TEXT");
            TransactionHistoryWalletForm.tabPageBlockRewardTransaction.Text =
                ClassTranslation.GetLanguageTextFromOrder(
                    "TRANSACTION_HISTORY_WALLET_TAB_BLOCK_REWARD_RECEIVED_LIST_TEXT");
            if (TransactionHistoryWalletForm._labelWaitingText != null)
                TransactionHistoryWalletForm._labelWaitingText.Text =
                    ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_WAITING_MESSAGE_SYNC_TEXT");
#if WINDOWS
            TransactionHistoryWalletForm.Refresh();
#endif
            // Restore wallet menu.
            RestoreWalletForm.labelTextRestore.Text =
                ClassTranslation.GetLanguageTextFromOrder("RESTORE_WALLET_LABEL_TITLE_TEXT");
            RestoreWalletForm.labelCreateSelectSavingPathWallet.Text =
                ClassTranslation.GetLanguageTextFromOrder("RESTORE_WALLET_LABEL_SELECT_PATH_FILE_TEXT");
            RestoreWalletForm.labelPrivateKey.Text =
                ClassTranslation.GetLanguageTextFromOrder("RESTORE_WALLET_LABEL_PRIVATE_KEY_TEXT");
            RestoreWalletForm.labelPassword.Text =
                ClassTranslation.GetLanguageTextFromOrder("RESTORE_WALLET_LABEL_PASSWORD_TEXT");
            RestoreWalletForm.buttonRestoreYourWallet.Text =
                ClassTranslation.GetLanguageTextFromOrder("RESTORE_WALLET_BUTTON_SUBMIT_RESTORE_TEXT");
#if WINDOWS
            RestoreWalletForm.Refresh();
#endif

            // Contact wallet menu.
            ContactWalletForm.buttonAddContact.Text =
                ClassTranslation.GetLanguageTextFromOrder("CONTACT_BUTTON_ADD_CONTACT_TEXT");
            ContactWalletForm.listViewExContact.Columns[0].Text =
                ClassTranslation.GetLanguageTextFromOrder("CONTACT_LIST_COLUMN_NAME_TEXT");
            ContactWalletForm.listViewExContact.Columns[1].Text =
                ClassTranslation.GetLanguageTextFromOrder("CONTACT_LIST_COLUMN_ADDRESS_TEXT");
            ContactWalletForm.listViewExContact.Columns[2].Text =
                ClassTranslation.GetLanguageTextFromOrder("CONTACT_LIST_COLUMN_ACTION_TEXT");
            ContactWalletForm.Refresh();
        }

        /// <summary>
        ///     Event click from menu for switch to main phase.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void MainMenuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ClassWalletObject.WalletConnect != null)
            {
                if (EnableTokenNetworkMode)
                {
                    if (!ClassWalletObject.WalletClosed) await ClassWalletObject.FullDisconnection(true);
                }
                else
                {
                    if (ClassWalletObject.SeedNodeConnectorWallet != null)
                        if (ClassWalletObject.SeedNodeConnectorWallet.ReturnStatus() &&
                            ClassWalletObject.WalletConnect.WalletPhase != ClassWalletPhase.Create)
                            await ClassWalletObject.FullDisconnection(true);
                }
            }

            ClassFormPhase.SwitchFormPhase(ClassFormPhaseEnumeration.Main);
        }

        /// <summary>
        ///     Event click from menu for switch to create wallet phase.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void CreateWalletToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ClassWalletObject.WalletConnect != null)
            {
                if (EnableTokenNetworkMode)
                {
                    if (!ClassWalletObject.WalletClosed) await ClassWalletObject.FullDisconnection(true);
                }
                else
                {
                    if (ClassWalletObject.SeedNodeConnectorWallet != null)
                        if (ClassWalletObject.SeedNodeConnectorWallet.ReturnStatus() &&
                            ClassWalletObject.WalletConnect.WalletPhase != ClassWalletPhase.Create)
                            await ClassWalletObject.FullDisconnection(true);
                }
            }

            ClassFormPhase.SwitchFormPhase(ClassFormPhaseEnumeration.CreateWallet);
        }

        /// <summary>
        ///     Event click from menu for switch to open wallet phase.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OpenWalletToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ClassWalletObject.WalletConnect != null)
            {
                if (EnableTokenNetworkMode)
                {
                    if (!ClassWalletObject.WalletClosed) await ClassWalletObject.FullDisconnection(true);
                }
                else
                {
                    if (ClassWalletObject.SeedNodeConnectorWallet != null)
                        if (ClassWalletObject.SeedNodeConnectorWallet.ReturnStatus() &&
                            ClassWalletObject.WalletConnect.WalletPhase != ClassWalletPhase.Create)
                            await ClassWalletObject.FullDisconnection(true);
                }
            }

            ClassFormPhase.SwitchFormPhase(ClassFormPhaseEnumeration.OpenWallet);
        }

        /// <summary>
        ///     Event click from menu for exit the program.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClassPeerList.SavePeerList();
            Process.GetCurrentProcess().Kill();
        }

        /// <summary>
        ///     Event enable when the interface form of the wallet is closed, used in case for force to kill the whole process.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WalletXenophyte_FormClosed(object sender, FormClosedEventArgs e)
        {
            ClassPeerList.SavePeerList();
            Process.GetCurrentProcess().Kill();
        }

        /// <summary>
        ///     Event enable when the interface form of the wallet is closing, used in case for force to kill the whole process.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WalletXenophyte_FormClosing(object sender, FormClosingEventArgs e)
        {
            ClassPeerList.SavePeerList();
            Process.GetCurrentProcess().Kill();
        }

        /// <summary>
        ///     Switch to overview menu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonOverviewWallet_Click(object sender, EventArgs e)
        {
            if (EnableTokenNetworkMode)
            {
                if (!ClassWalletObject.WalletClosed) ClassFormPhase.SwitchFormPhase(ClassFormPhaseEnumeration.Overview);
            }
            else
            {
                if (ClassWalletObject.SeedNodeConnectorWallet != null)
                    if (ClassWalletObject.SeedNodeConnectorWallet.ReturnStatus() &&
                        ClassWalletObject.WalletConnect.WalletPhase != ClassWalletPhase.Create)
                        ClassFormPhase.SwitchFormPhase(ClassFormPhaseEnumeration.Overview);
            }
        }

        /// <summary>
        ///     Close wallet and switch to main menu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void CloseWalletToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (EnableTokenNetworkMode)
            {
                if (!ClassWalletObject.WalletClosed) await ClassWalletObject.FullDisconnection(true);
            }
            else
            {
                if (ClassWalletObject.SeedNodeConnectorWallet != null)
                    if (ClassWalletObject.SeedNodeConnectorWallet.ReturnStatus() &&
                        ClassWalletObject.WalletConnect.WalletPhase != ClassWalletPhase.Create)
                        await ClassWalletObject.FullDisconnection(true);
            }

            ClassFormPhase.SwitchFormPhase(ClassFormPhaseEnumeration.Main);
        }

        /// <summary>
        ///     Switch to send transaction menu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MetroButtonSendTransactionWallet_Click(object sender, EventArgs e)
        {
            if (EnableTokenNetworkMode)
            {
                if (!ClassWalletObject.WalletClosed)
                    ClassFormPhase.SwitchFormPhase(ClassFormPhaseEnumeration.SendTransaction);
            }
            else
            {
                if (ClassWalletObject.SeedNodeConnectorWallet != null)
                    if (ClassWalletObject.SeedNodeConnectorWallet.ReturnStatus() &&
                        ClassWalletObject.WalletConnect.WalletPhase != ClassWalletPhase.Create)
                        ClassFormPhase.SwitchFormPhase(ClassFormPhaseEnumeration.SendTransaction);
            }
        }

        /// <summary>
        ///     Switch to transaction history menu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MetroButtonTransactionWallet_Click(object sender, EventArgs e)
        {
            if (EnableTokenNetworkMode)
            {
                if (!ClassWalletObject.WalletClosed)
                    ClassFormPhase.SwitchFormPhase(ClassFormPhaseEnumeration.TransactionHistory);
            }
            else
            {
                if (ClassWalletObject.SeedNodeConnectorWallet != null)
                    if (ClassWalletObject.SeedNodeConnectorWallet.ReturnStatus() &&
                        ClassWalletObject.WalletConnect.WalletPhase != ClassWalletPhase.Create)
                        ClassFormPhase.SwitchFormPhase(ClassFormPhaseEnumeration.TransactionHistory);
            }
        }

        /// <summary>
        ///     Switch to block explorer menu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MetroButtonBlockExplorerWallet_Click(object sender, EventArgs e)
        {
            if (EnableTokenNetworkMode)
            {
                if (!ClassWalletObject.WalletClosed)
                    ClassFormPhase.SwitchFormPhase(ClassFormPhaseEnumeration.BlockExplorer);
            }
            else
            {
                if (ClassWalletObject.SeedNodeConnectorWallet != null)
                    if (ClassWalletObject.SeedNodeConnectorWallet.ReturnStatus() &&
                        ClassWalletObject.WalletConnect.WalletPhase != ClassWalletPhase.Create)
                        ClassFormPhase.SwitchFormPhase(ClassFormPhaseEnumeration.BlockExplorer);
            }
        }

        /// <summary>
        ///     Refresh automaticaly the interface.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimerRefresh_Tick(object sender, EventArgs e)
        {

            if (Width < BaseInterfaceWidth)
                Width = BaseInterfaceWidth;
            else if (Width == BaseInterfaceWidth) Width += 10;

            if (Height < BaseInterfaceHeight)
                Height = BaseInterfaceHeight;
            else if (Height == BaseInterfaceHeight) Height += 10;

            if (Program.WalletXenophyte != null) // Get list of all controls of each menu.
            {
                MainWalletForm.GetListControl();
                OverviewWalletForm.GetListControl();
                CreateWalletForm.GetListControl();
                BlockWalletForm.GetListControl();
                OpenWalletForm.GetListControl();
                SendTransactionWalletForm.GetListControl();
                TransactionHistoryWalletForm.GetListControl();
                RestoreWalletForm.GetListControl();
                ContactWalletForm.GetListControl();
                ResizeWalletInterface();
            }
            UpdateStyles();
        }

        /// <summary>
        ///     Show QR Code generated from wallet address.
        /// </summary>
        /// <param name="walletAddress"></param>
        public void ShowWalletAddressQRCode(string walletAddress)
        {
            var options = new QrCodeEncodingOptions
            {
                DisableECI = true,
                CharacterSet = "UTF-8",
                Width = 250,
                Height = 250
            };

            var qr = new BarcodeWriter
            {
                Options = options,
                Format = BarcodeFormat.QR_CODE
            };
            var result = new Bitmap(qr.Write(walletAddress.Trim()));
            MethodInvoker invoke = () => pictureBoxQRCodeWallet.BackgroundImage = result;
            pictureBoxQRCodeWallet.BeginInvoke(invoke);
        }

        /// <summary>
        ///     Hide QR Code generated from wallet address.
        /// </summary>
        public void HideWalletAddressQrCode()
        {
            try
            {
                pictureBoxQRCodeWallet.Image = null;
                pictureBoxQRCodeWallet.Invalidate();
                pictureBoxQRCodeWallet.BringToFront();
                pictureBoxQRCodeWallet.BackgroundImage = null;
            }
            catch
            {
            }
        }

        /// <summary>
        ///     Open change password menu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChangePasswordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!EnableTokenNetworkMode)
                if (ClassWalletObject.WalletConnect != null)
                    if (ClassWalletObject.WalletConnect.WalletPhase != ClassWalletPhase.Create)
                    {
                        var changeWalletPassword = new ChangeWalletPasswordWallet
                        { StartPosition = FormStartPosition.CenterParent };
                        changeWalletPassword.ShowDialog(this);
                    }
        }

        /// <summary>
        ///     Open pin code setting menu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SettingPinCodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!EnableTokenNetworkMode)
                if (ClassWalletObject.WalletConnect != null)
                    if (ClassWalletObject.WalletConnect.WalletPhase != ClassWalletPhase.Create)
                    {
                        var pinCodeSetting = new PinCodeSettingWallet { StartPosition = FormStartPosition.CenterParent };
                        pinCodeSetting.ShowDialog(this);
                    }
        }

        /// <summary>
        ///     Resync manually transaction history.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void resyncTransactionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ClassWalletObject.WalletConnect != null)
            {
                if (EnableTokenNetworkMode)
                {
                    if (!ClassWalletObject.WalletClosed)
                    {
#if WINDOWS
                        if (MetroMessageBox.Show(this,
                                ClassTranslation.GetLanguageTextFromOrder("RESYNC_TRANSACTION_HISTORY_CONTENT_TEXT"),
                                ClassTranslation.GetLanguageTextFromOrder("RESYNC_TRANSACTION_HISTORY_TITLE_TEXT"),
                                MessageBoxButtons.YesNo, MessageBoxIcon.Information) ==
                            DialogResult.Yes)
#else
                        if (MessageBox.Show(this, ClassTranslation.GetLanguageTextFromOrder("RESYNC_TRANSACTION_HISTORY_CONTENT_TEXT"),
                                ClassTranslation.GetLanguageTextFromOrder("RESYNC_TRANSACTION_HISTORY_TITLE_TEXT"), MessageBoxButtons.YesNo, MessageBoxIcon.Information) ==
                            DialogResult.Yes)
#endif
                            TransactionHistoryWalletForm.ResyncTransactionAsync();
                    }
                }
                else
                {
                    if (ClassWalletObject.SeedNodeConnectorWallet != null)
                        if (ClassWalletObject.SeedNodeConnectorWallet.ReturnStatus() &&
                            ClassWalletObject.WalletConnect.WalletPhase != ClassWalletPhase.Create)
                        {
#if WINDOWS
                            if (MetroMessageBox.Show(this,
                                    ClassTranslation.GetLanguageTextFromOrder(
                                        "RESYNC_TRANSACTION_HISTORY_CONTENT_TEXT"),
                                    ClassTranslation.GetLanguageTextFromOrder("RESYNC_TRANSACTION_HISTORY_TITLE_TEXT"),
                                    MessageBoxButtons.YesNo, MessageBoxIcon.Information) ==
                                DialogResult.Yes)
#else
                        if (MessageBox.Show(this, ClassTranslation.GetLanguageTextFromOrder("RESYNC_TRANSACTION_HISTORY_CONTENT_TEXT"),
                                ClassTranslation.GetLanguageTextFromOrder("RESYNC_TRANSACTION_HISTORY_TITLE_TEXT"), MessageBoxButtons.YesNo, MessageBoxIcon.Information) ==
                            DialogResult.Yes)
#endif
                                TransactionHistoryWalletForm.ResyncTransactionAsync();
                        }
                }
            }
        }

        /// <summary>
        ///     Open remote node setting menu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void remoteNodeSettingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var remoteNodeSetting = new RemoteNodeSettingWallet { StartPosition = FormStartPosition.CenterParent };
            remoteNodeSetting.ShowDialog(this);
        }

        /// <summary>
        ///     Detect when the size of the interface change.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WalletXenophyte_SizeChanged(object sender, EventArgs e)
        {
            if (Width < BaseInterfaceWidth) Width = BaseInterfaceWidth;

            if (Height < BaseInterfaceHeight) Height = BaseInterfaceHeight;
            //UpdateStyles();
        }

        /// <summary>
        ///     Event call when the user want to resize the interface.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WalletXenophyte_Resize(object sender, EventArgs e)
        {
#if WINDOWS
            ResizeWalletInterface();
#endif
        }

        /// <summary>
        ///     Resync block explorer manually.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void resyncBlockToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ClassWalletObject.WalletConnect != null)
            {
                if (EnableTokenNetworkMode)
                {
                    if (!ClassWalletObject.WalletClosed)
                    {
#if WINDOWS
                        if (MetroMessageBox.Show(this,
                                ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration
                                    .resyncblockexplorercontenttext),
                                ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration
                                    .resyncblockexplorertitletext),
                                MessageBoxButtons.YesNo, MessageBoxIcon.Information) ==
                            DialogResult.Yes)
#else
                        if (MessageBox.Show(this, ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration.resyncblockexplorercontenttext),
                                ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration.resyncblockexplorertitletext), MessageBoxButtons.YesNo, MessageBoxIcon.Information) ==
                            DialogResult.Yes)
#endif
                            BlockWalletForm.ResyncBlock();
                    }
                }
                else
                {
                    if (ClassWalletObject.SeedNodeConnectorWallet != null)
                        if (ClassWalletObject.SeedNodeConnectorWallet.ReturnStatus() &&
                            ClassWalletObject.WalletConnect.WalletPhase != ClassWalletPhase.Create)
                        {
#if WINDOWS
                            if (MetroMessageBox.Show(this,
                                    ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration
                                        .resyncblockexplorercontenttext),
                                    ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration
                                        .resyncblockexplorertitletext),
                                    MessageBoxButtons.YesNo, MessageBoxIcon.Information) ==
                                DialogResult.Yes)
#else
                        if (MessageBox.Show(this, ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration.resyncblockexplorercontenttext),
                                ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration.resyncblockexplorertitletext), MessageBoxButtons.YesNo, MessageBoxIcon.Information) ==
                            DialogResult.Yes)
#endif
                                BlockWalletForm.ResyncBlock();
                        }
                }
            }
        }

        /// <summary>
        ///     Go to the previous page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonPreviousPage_Click(object sender, EventArgs e)
        {
            if (ClassFormPhase.FormPhase == ClassFormPhaseEnumeration.TransactionHistory)
            {
                if (!ClassWalletObject.InSyncTransaction && !ClassWalletObject.InSyncTransactionAnonymity)
                {
                    if (TransactionHistoryWalletForm.tabPageNormalTransactionSend.Visible
                    ) // Normal transaction send list
                        if (CurrentTransactionHistoryPageNormalSend > 1)
                        {
                            CurrentTransactionHistoryPageNormalSend--;
                            labelNoticeCurrentPage.Text = "" + CurrentTransactionHistoryPageNormalSend;
                            CleanTransactionHistory();
                        }

                    if (TransactionHistoryWalletForm.tabPageNormalTransactionReceived.Visible
                    ) // Normal transaction received list
                        if (CurrentTransactionHistoryPageNormalReceive > 1)
                        {
                            CurrentTransactionHistoryPageNormalReceive--;
                            labelNoticeCurrentPage.Text = "" + CurrentTransactionHistoryPageNormalReceive;
                            CleanTransactionHistory();
                        }

                    if (TransactionHistoryWalletForm.tabPageAnonymityTransactionSend.Visible
                    ) // Anonymous transaction send list 
                        if (CurrentTransactionHistoryPageAnonymousSend > 1)
                        {
                            CurrentTransactionHistoryPageAnonymousSend--;
                            labelNoticeCurrentPage.Text = "" + CurrentTransactionHistoryPageAnonymousSend;
                            CleanTransactionHistory();
                        }

                    if (TransactionHistoryWalletForm.tabPageAnonymityTransactionReceived.Visible
                    ) // Anonymous transaction received list 
                        if (CurrentTransactionHistoryPageAnonymousReceived > 1)
                        {
                            CurrentTransactionHistoryPageAnonymousReceived--;
                            labelNoticeCurrentPage.Text = "" + CurrentTransactionHistoryPageAnonymousReceived;
                            CleanTransactionHistory();
                        }

                    if (TransactionHistoryWalletForm.tabPageBlockRewardTransaction.Visible
                    ) // block reward transaction list 
                        if (CurrentTransactionHistoryPageBlockReward > 1)
                        {
                            CurrentTransactionHistoryPageBlockReward--;
                            labelNoticeCurrentPage.Text = "" + CurrentTransactionHistoryPageBlockReward;
                            CleanTransactionHistory();
                        }
                }
            }
            else if (ClassFormPhase.FormPhase == ClassFormPhaseEnumeration.BlockExplorer)
            {
                if (CurrentBlockExplorerPage > 1)
                {
                    CurrentBlockExplorerPage--;
                    labelNoticeCurrentPage.Text = "" + CurrentBlockExplorerPage;
                    StopUpdateBlockHistory(false, true);
                }
            }
        }

        /// <summary>
        ///     Go to the next page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonNextPage_Click(object sender, EventArgs e)
        {
            if (ClassFormPhase.FormPhase == ClassFormPhaseEnumeration.TransactionHistory)
            {
                if (!ClassWalletObject.InSyncTransaction && !ClassWalletObject.InSyncTransactionAnonymity)
                {
                    if (TransactionHistoryWalletForm.tabPageNormalTransactionSend.Visible
                    ) // Normal transaction send list
                    {
                        var difference = TotalTransactionNormalSend + MaxTransactionPerPage -
                                         TotalTransactionNormalSend;
                        if ((CurrentTransactionHistoryPageNormalSend + 1) * MaxTransactionPerPage <=
                            TotalTransactionNormalSend + difference)
                        {
                            CurrentTransactionHistoryPageNormalSend++;
                            labelNoticeCurrentPage.Text = "" + CurrentTransactionHistoryPageNormalSend;
                            CleanTransactionHistory();
                        }
                    }

                    if (TransactionHistoryWalletForm.tabPageNormalTransactionReceived.Visible
                    ) // Normal transaction received list
                    {
                        var difference = TotalTransactionNormalReceived + MaxTransactionPerPage -
                                         TotalTransactionNormalReceived;

                        if ((CurrentTransactionHistoryPageNormalReceive + 1) * MaxTransactionPerPage <=
                            TotalTransactionNormalReceived + difference)
                        {
                            CurrentTransactionHistoryPageNormalReceive++;
                            labelNoticeCurrentPage.Text = "" + CurrentTransactionHistoryPageNormalReceive;
                            CleanTransactionHistory();
                        }
                    }

                    if (TransactionHistoryWalletForm.tabPageAnonymityTransactionSend.Visible
                    ) // Anonymous transaction send list 
                    {
                        var difference = TotalTransactionAnonymousSend + MaxTransactionPerPage -
                                         TotalTransactionAnonymousSend;

                        if ((CurrentTransactionHistoryPageAnonymousSend + 1) * MaxTransactionPerPage <=
                            TotalTransactionAnonymousSend + difference)
                        {
                            CurrentTransactionHistoryPageAnonymousSend++;
                            labelNoticeCurrentPage.Text = "" + CurrentTransactionHistoryPageAnonymousSend;
                            CleanTransactionHistory();
                        }
                    }

                    if (TransactionHistoryWalletForm.tabPageAnonymityTransactionReceived.Visible
                    ) // Anonymous transaction received list 
                    {
                        var difference = TotalTransactionAnonymousReceived + MaxTransactionPerPage -
                                         TotalTransactionAnonymousReceived;
                        if ((CurrentTransactionHistoryPageAnonymousReceived + 1) * MaxTransactionPerPage <=
                            TotalTransactionAnonymousReceived + difference)
                        {
                            CurrentTransactionHistoryPageAnonymousReceived++;
                            labelNoticeCurrentPage.Text = "" + CurrentTransactionHistoryPageAnonymousReceived;
                            CleanTransactionHistory();
                        }
                    }

                    if (TransactionHistoryWalletForm.tabPageBlockRewardTransaction.Visible
                    ) // block reward transaction list 
                    {
                        var difference = TotalTransactionBlockReward + MaxTransactionPerPage -
                                         TotalTransactionBlockReward;

                        if ((CurrentTransactionHistoryPageBlockReward + 1) * MaxTransactionPerPage <=
                            TotalTransactionBlockReward + difference)
                        {
                            CurrentTransactionHistoryPageBlockReward++;
                            labelNoticeCurrentPage.Text = "" + CurrentTransactionHistoryPageBlockReward;
                            CleanTransactionHistory();
                        }
                    }
                }
            }
            else if (ClassFormPhase.FormPhase == ClassFormPhaseEnumeration.BlockExplorer)
            {
                var numberMaxPage = (float)(ClassBlockCache.ListBlock.Count - 1) / MaxBlockPerPage;
                numberMaxPage += 0.5f;
                numberMaxPage = (float)Math.Round(numberMaxPage, 0);
                if (numberMaxPage <= 0) numberMaxPage = 1;
                if (CurrentBlockExplorerPage + 1 <= numberMaxPage)
                {
                    CurrentBlockExplorerPage++;
                    labelNoticeCurrentPage.Text = "" + CurrentBlockExplorerPage;
                    StopUpdateBlockHistory(false, true);
                }
            }
        }

        /// <summary>
        ///     Go to the first page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonFirstPage_Click(object sender, EventArgs e)
        {
            if (ClassFormPhase.FormPhase == ClassFormPhaseEnumeration.TransactionHistory)
            {
                if (!ClassWalletObject.InSyncTransaction && !ClassWalletObject.InSyncTransactionAnonymity)
                {
                    if (TransactionHistoryWalletForm.tabPageNormalTransactionSend.Visible
                    ) // Normal transaction send list
                    {
                        CurrentTransactionHistoryPageNormalSend = 1;
                        labelNoticeCurrentPage.Text = "" + CurrentTransactionHistoryPageNormalSend;
                        CleanTransactionHistory();
                    }

                    if (TransactionHistoryWalletForm.tabPageNormalTransactionReceived.Visible
                    ) // Normal transaction received list
                    {
                        CurrentTransactionHistoryPageNormalReceive = 1;
                        labelNoticeCurrentPage.Text = "" + CurrentTransactionHistoryPageNormalReceive;
                        CleanTransactionHistory();
                    }

                    if (TransactionHistoryWalletForm.tabPageAnonymityTransactionSend.Visible
                    ) // Anonymous transaction send list 
                    {
                        CurrentTransactionHistoryPageAnonymousSend = 1;
                        labelNoticeCurrentPage.Text = "" + CurrentTransactionHistoryPageAnonymousSend;
                        CleanTransactionHistory();
                    }

                    if (TransactionHistoryWalletForm.tabPageAnonymityTransactionReceived.Visible
                    ) // Anonymous transaction received list 
                    {
                        CurrentTransactionHistoryPageAnonymousReceived = 1;
                        labelNoticeCurrentPage.Text = "" + CurrentTransactionHistoryPageNormalReceive;
                        CleanTransactionHistory();
                    }

                    if (TransactionHistoryWalletForm.tabPageBlockRewardTransaction.Visible
                    ) // block reward transaction list 
                    {
                        CurrentTransactionHistoryPageBlockReward = 1;
                        labelNoticeCurrentPage.Text = "" + CurrentTransactionHistoryPageBlockReward;
                        CleanTransactionHistory();
                    }
                }
            }
            else if (ClassFormPhase.FormPhase == ClassFormPhaseEnumeration.BlockExplorer)
            {
                CurrentBlockExplorerPage = 1;
                labelNoticeCurrentPage.Text = "" + CurrentBlockExplorerPage;
                StopUpdateBlockHistory(false, true);
            }
        }

        /// <summary>
        ///     Got to the last page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonLastPage_Click(object sender, EventArgs e)
        {
            if (ClassFormPhase.FormPhase == ClassFormPhaseEnumeration.TransactionHistory)
            {
                if (!ClassWalletObject.InSyncTransaction && !ClassWalletObject.InSyncTransactionAnonymity)
                {
                    if (TransactionHistoryWalletForm.tabPageNormalTransactionSend.Visible
                    ) // Normal transaction send list
                    {
                        var numberParge = (float)TotalTransactionNormalSend / MaxTransactionPerPage;
                        numberParge += 0.5f;
                        numberParge = (float)Math.Round(numberParge, 0);
                        if (numberParge <= 0) numberParge = 1;
                        CurrentTransactionHistoryPageNormalSend = (int)numberParge;
                        labelNoticeCurrentPage.Text = "" + CurrentTransactionHistoryPageNormalSend;
                        CleanTransactionHistory();
                    }

                    if (TransactionHistoryWalletForm.tabPageNormalTransactionReceived.Visible
                    ) // Normal transaction received list
                    {
                        var numberParge = (float)TotalTransactionNormalReceived / MaxTransactionPerPage;
                        numberParge += 0.5f;
                        numberParge = (float)Math.Round(numberParge, 0);
                        if (numberParge <= 0) numberParge = 1;
                        CurrentTransactionHistoryPageNormalReceive = (int)numberParge;
                        labelNoticeCurrentPage.Text = "" + CurrentTransactionHistoryPageNormalReceive;
                        CleanTransactionHistory();
                    }

                    if (TransactionHistoryWalletForm.tabPageAnonymityTransactionSend.Visible
                    ) // Anonymous transaction send list 
                    {
                        var numberParge = (float)TotalTransactionAnonymousSend / MaxTransactionPerPage;
                        numberParge += 0.5f;
                        numberParge = (float)Math.Round(numberParge, 0);
                        if (numberParge <= 0) numberParge = 1;
                        CurrentTransactionHistoryPageAnonymousSend = (int)numberParge;
                        labelNoticeCurrentPage.Text = "" + CurrentTransactionHistoryPageAnonymousSend;
                        CleanTransactionHistory();
                    }

                    if (TransactionHistoryWalletForm.tabPageAnonymityTransactionReceived.Visible
                    ) // Anonymous transaction received list 
                    {
                        var numberParge = (float)TotalTransactionAnonymousReceived / MaxTransactionPerPage;
                        numberParge += 0.5f;
                        numberParge = (float)Math.Round(numberParge, 0);
                        if (numberParge <= 0) numberParge = 1;
                        CurrentTransactionHistoryPageAnonymousReceived = (int)numberParge;
                        labelNoticeCurrentPage.Text = "" + CurrentTransactionHistoryPageAnonymousReceived;
                        CleanTransactionHistory();
                    }

                    if (TransactionHistoryWalletForm.tabPageBlockRewardTransaction.Visible
                    ) // block reward transaction list 
                    {
                        var numberParge = (float)TotalTransactionBlockReward / MaxTransactionPerPage;
                        numberParge += 0.5f;
                        numberParge = (float)Math.Round(numberParge, 0);
                        if (numberParge <= 0) numberParge = 1;
                        CurrentTransactionHistoryPageBlockReward = (int)numberParge;
                        labelNoticeCurrentPage.Text = "" + CurrentTransactionHistoryPageBlockReward;
                        CleanTransactionHistory();
                    }
                }
            }
            else if (ClassFormPhase.FormPhase == ClassFormPhaseEnumeration.BlockExplorer)
            {
                var numberParge = (float)(ClassBlockCache.ListBlock.Count - 1) / MaxBlockPerPage;
                numberParge += 0.5f;
                numberParge = (float)Math.Round(numberParge, 0);
                if (numberParge <= 0) numberParge = 1;
                CurrentBlockExplorerPage = (int)numberParge;
                labelNoticeCurrentPage.Text = "" + CurrentBlockExplorerPage;
                StopUpdateBlockHistory(false, true);
            }
        }

        /// <summary>
        ///     Copy the wallet address by click.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void labelNoticeWalletAddress_Click(object sender, EventArgs e)
        {
            if (!_isCopyWalletAddress)
            {
                _isCopyWalletAddress = true;
                if (Environment.OSVersion.Platform == PlatformID.Unix)
                    LinuxClipboard.SetText(ClassWalletObject.WalletConnect.WalletAddress);
                else // Windows (normaly)
                    Clipboard.SetText(ClassWalletObject.WalletConnect.WalletAddress);
                try
                {
                    Task.Factory.StartNew(async () =>
                    {
                        var oldText = labelNoticeWalletAddress.Text;
                        var oldColor = labelNoticeWalletAddress.ForeColor;
                        MethodInvoker invoke = () => labelNoticeWalletAddress.Text = oldText + @" copied.";
                        BeginInvoke(invoke);
                        invoke = () => labelNoticeWalletAddress.ForeColor = Color.Lime;
                        BeginInvoke(invoke);
                        await Task.Delay(1000);
                        invoke = () => labelNoticeWalletAddress.ForeColor = oldColor;
                        BeginInvoke(invoke);
                        invoke = () =>
                            labelNoticeWalletAddress.Text =
                                ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration
                                    .panelwalletaddresstext) + " " +
                                ClassWalletObject.WalletConnect.WalletAddress;
                        BeginInvoke(invoke);
                        _isCopyWalletAddress = false;
                    }, WalletCancellationToken.Token, TaskCreationOptions.RunContinuationsAsynchronously, TaskScheduler.Current).ConfigureAwait(false);
                }
                catch
                {

                }
            }
        }

        /// <summary>
        ///     Switch to wallet restore menu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void restoreWalletToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (EnableTokenNetworkMode)
            {
                if (!ClassWalletObject.WalletClosed) await ClassWalletObject.FullDisconnection(true);
            }
            else
            {
                if (ClassWalletObject.SeedNodeConnectorWallet != null)
                    if (ClassWalletObject.SeedNodeConnectorWallet.ReturnStatus())
                        await ClassWalletObject.FullDisconnection(true);
            }

            ClassFormPhase.SwitchFormPhase(ClassFormPhaseEnumeration.RestoreWallet);
        }

        /// <summary>
        ///     Open about menu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var aboutMenu = new AboutWallet { StartPosition = FormStartPosition.CenterParent };
            aboutMenu.ShowDialog(this);
        }

        /// <summary>
        ///     Open about menu by clicking on copyright.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void labelCopyright_Click(object sender, EventArgs e)
        {
            var aboutMenu = new AboutWallet { StartPosition = FormStartPosition.CenterParent };
            aboutMenu.ShowDialog(this);
        }

        /// <summary>
        ///     Reach the official website.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void linkLabelWebsite_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://xenophyte.com/");
        }

        /// <summary>
        ///     Switch to contact menu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonContactWallet_Click(object sender, EventArgs e)
        {
            if (EnableTokenNetworkMode)
            {
                if (!ClassWalletObject.WalletClosed)
                    ClassFormPhase.SwitchFormPhase(ClassFormPhaseEnumeration.ContactWallet);
            }
            else
            {
                if (ClassWalletObject.SeedNodeConnectorWallet != null)
                    if (ClassWalletObject.SeedNodeConnectorWallet.ReturnStatus() &&
                        ClassWalletObject.WalletConnect.WalletPhase != ClassWalletPhase.Create)
                        ClassFormPhase.SwitchFormPhase(ClassFormPhaseEnumeration.ContactWallet);
            }
        }

        #endregion

        #region About Wallet Sync

        /// <summary>
        ///     Update overview label coin max supply
        /// </summary>
        /// <param name="info"></param>
        public void UpdateOverviewLabelCoinMaxSupply(string info)
        {
            void MethodInvoker()
            {
                try
                {
                    OverviewWalletForm.labelTextCoinMaxSupply.Text =
                        ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration
                            .overviewwalletlabelcoinmaxsupplytext) + " " +
                        info.ToString(Program.GlobalCultureInfo).Replace(",", ".") + " " + ClassConnectorSetting.CoinNameMin;
                }
                catch
                {
                    OverviewWalletForm.labelTextCoinMaxSupply.Text =
                        ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration
                            .overviewwalletlabelcoinmaxsupplytext);
                }
            }

            BeginInvoke((MethodInvoker)MethodInvoker);
        }

        /// <summary>
        ///     Update overview label coin circulating
        /// </summary>
        /// <param name="info"></param>
        public void UpdateOverviewLabelCoinCirculating(string info)
        {
            void MethodInvoker()
            {
                try
                {
                    OverviewWalletForm.labelTextCoinCirculating.Text =
                        ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration
                            .overviewwalletlabelcoincirculatingtext) + " " +
                        info.ToString(Program.GlobalCultureInfo).Replace(",", ".") + " " + ClassConnectorSetting.CoinNameMin;
                }
                catch
                {
                    ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration
                        .overviewwalletlabelcoincirculatingtext);
                }
            }


            BeginInvoke((MethodInvoker)MethodInvoker);
        }

        /// <summary>
        ///     Update overview label block mined and some others
        /// </summary>
        /// <param name="info"></param>
        public void UpdateOverviewLabelBlockMined(string info)
        {
            try
            {
                var totalBlockLeft =
                    Math.Round(
                        decimal.Parse(ClassWalletObject.CoinMaxSupply.Replace(".", ","), NumberStyles.Any,
                            Program.GlobalCultureInfo) / ClassConnectorSetting.ConstantBlockReward -
                        decimal.Parse(info), 0);
                var totalCoinMined = decimal.Parse(info) * ClassConnectorSetting.ConstantBlockReward;
                var totalInPending = totalCoinMined -
                                     (decimal.Parse(ClassWalletObject.CoinCirculating.Replace(".", ","),
                                          NumberStyles.Any, Program.GlobalCultureInfo) +
                                      decimal.Parse(ClassWalletObject.TotalFee.Replace(".", ","), NumberStyles.Any,
                                          Program.GlobalCultureInfo));
                var blockchainHeight = int.Parse(info) + 1;


                MethodInvoker invoke = () =>
                {
                    OverviewWalletForm.labelTextCoinMined.Text =
                        ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration
                            .overviewwalletlabeltotalcoinminedtext) + " " +
                        totalCoinMined.ToString(Program.GlobalCultureInfo).Replace(",", ".") + " " + ClassConnectorSetting.CoinNameMin;
                    OverviewWalletForm.labelTextBlockchainHeight.Text =
                        ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration
                            .overviewwalletlabelblockchainheighttext) +
                        " " + blockchainHeight;
                    OverviewWalletForm.labelTextTotalBlockMined.Text =
                        ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration
                            .overviewwalletlabeltotalblockminedtext) +
                        " " + info;
                    OverviewWalletForm.labelTextTotalBlockLeft.Text =
                        ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration
                            .overviewwalletlabeltotalblocklefttext) + " " +
                        totalBlockLeft;
                    OverviewWalletForm.labelTextTotalCoinInPending.Text =
                        ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration
                            .overviewwalletlabeltotalcoinpending) + " " +
                        totalInPending.ToString(Program.GlobalCultureInfo).Replace(",", ".") + " " + ClassConnectorSetting.CoinNameMin;
                };
                BeginInvoke(invoke);
            }
            catch
            {
                MethodInvoker invoke = () =>
                {
                    OverviewWalletForm.labelTextCoinMined.Text =
                        ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration
                            .overviewwalletlabeltotalcoinminedtext);
                    OverviewWalletForm.labelTextBlockchainHeight.Text =
                        ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration
                            .overviewwalletlabelblockchainheighttext);
                    OverviewWalletForm.labelTextTotalBlockMined.Text =
                        ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration
                            .overviewwalletlabeltotalblockminedtext);
                    OverviewWalletForm.labelTextTotalBlockLeft.Text =
                        ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration
                            .overviewwalletlabeltotalblocklefttext);
                    OverviewWalletForm.labelTextTotalCoinInPending.Text =
                        ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration
                            .overviewwalletlabeltotalcoinpending);
                };
                BeginInvoke(invoke);
            }
        }

        /// <summary>
        ///     Update overview label network difficulty
        /// </summary>
        /// <param name="info"></param>
        public void UpdateOverviewLabelNetworkDifficulty(string info)
        {
            void MethodInvoker()
            {
                try
                {
                    OverviewWalletForm.labelTextNetworkDifficulty.Text =
                        ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration
                            .overviewwalletlabelnetworkdifficultytext) +
                        " " + info.Replace(",", ".");
                }
                catch
                {
                    OverviewWalletForm.labelTextNetworkDifficulty.Text =
                        ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration
                            .overviewwalletlabelnetworkdifficultytext);
                }
            }

            BeginInvoke((MethodInvoker)MethodInvoker);
        }

        /// <summary>
        ///     Update overview label network hashrate;
        /// </summary>
        /// <param name="info"></param>
        public void UpdateOverviewLabelNetworkHashrate(string info)
        {
            try
            {
                info = ClassUtils.GetTranslateHashrate(info, 2);

                void MethodInvoker()
                {
                    try
                    {
                        OverviewWalletForm.labelTextNetworkHashrate.Text =
                            ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration
                                .overviewwalletlabelnetworkhashratetext) +
                            " " + info.Replace(",", ".");
                    }
                    catch
                    {
                        OverviewWalletForm.labelTextNetworkHashrate.Text =
                            ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration
                                .overviewwalletlabelnetworkhashratetext);
                    }
                }

                BeginInvoke((MethodInvoker)MethodInvoker);
            }
            catch
            {
            }
        }

        /// <summary>
        ///     Update overview label total fee.
        /// </summary>
        /// <param name="info"></param>
        public void UpdateOverviewLabelTransactionFee(string info)
        {
            try
            {
                void MethodInvoker()
                {
                    try
                    {
                        OverviewWalletForm.labelTextTransactionFee.Text =
                            ClassTranslation.GetLanguageTextFromOrder(
                                ClassTranslationEnumeration.overviewwalletlabeltransactionfeeaccumulatedtext) + " " +
                            info.ToString(Program.GlobalCultureInfo).Replace(",", ".") + " " + ClassConnectorSetting.CoinNameMin;
                    }
                    catch
                    {
                        OverviewWalletForm.labelTextTransactionFee.Text =
                            ClassTranslation.GetLanguageTextFromOrder(
                                ClassTranslationEnumeration.overviewwalletlabeltransactionfeeaccumulatedtext);
                    }
                }

                BeginInvoke((MethodInvoker)MethodInvoker);
            }
            catch
            {
            }
        }

        /// <summary>
        ///     Update label sync information.
        /// </summary>
        /// <param name="info"></param>
        public void UpdateLabelSyncInformation(string info)
        {
            try
            {
                void MethodInvoker()
                {
                    labelSyncInformation.Text = info;
                }

                BeginInvoke((MethodInvoker)MethodInvoker);
            }
            catch
            {
            }
        }

        /// <summary>
        ///     Update overview label last block found.
        /// </summary>
        /// <param name="info"></param>
        public void UpdateOverviewLabelLastBlockFound(string info)
        {
            try
            {
                void MethodInvoker()
                {
                    try
                    {
                        OverviewWalletForm.labelTextLastBlockFound.Text =
                            ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration
                                .overviewwalletlabellastblockfoundtext) +
                            " " + info;
                    }
                    catch
                    {
                        OverviewWalletForm.labelTextLastBlockFound.Text =
                            ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration
                                .overviewwalletlabellastblockfoundtext);
                    }
                }

                BeginInvoke((MethodInvoker)MethodInvoker);
            }
            catch
            {
            }
        }

        /// <summary>
        ///     Clean wallet sync interface
        /// </summary>
        public void CleanSyncInterfaceWallet()
        {
            try
            {
                MethodInvoker invoke = () =>
                {
                    labelNoticeWalletAddress.Text =
                        ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration.panelwalletaddresstext);
                    labelNoticeWalletBalance.Text =
                        ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration.panelwalletbalancetext);
                    OverviewWalletForm.labelTextCoinMaxSupply.Text =
                        ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration
                            .overviewwalletlabelcoinmaxsupplytext) +
                        " In Sync";
                    OverviewWalletForm.labelTextCoinCirculating.Text =
                        ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration
                            .overviewwalletlabelcoincirculatingtext) +
                        " In Sync";
                    OverviewWalletForm.labelTextTransactionFee.Text =
                        ClassTranslation.GetLanguageTextFromOrder(
                            ClassTranslationEnumeration.overviewwalletlabeltransactionfeeaccumulatedtext) + " In Sync";
                    OverviewWalletForm.labelTextCoinMined.Text =
                        ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration
                            .overviewwalletlabeltotalcoinminedtext) +
                        " In Sync";
                    OverviewWalletForm.labelTextBlockchainHeight.Text =
                        ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration
                            .overviewwalletlabelblockchainheighttext) +
                        " In Sync";
                    OverviewWalletForm.labelTextTotalBlockMined.Text =
                        ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration
                            .overviewwalletlabeltotalblockminedtext) +
                        " In Sync";
                    OverviewWalletForm.labelTextTotalBlockLeft.Text =
                        ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration
                            .overviewwalletlabeltotalblocklefttext) +
                        " In Sync";
                    OverviewWalletForm.labelTextNetworkDifficulty.Text =
                        ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration
                            .overviewwalletlabelnetworkdifficultytext) +
                        " In Sync";
                    OverviewWalletForm.labelTextNetworkHashrate.Text =
                        ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration
                            .overviewwalletlabelnetworkhashratetext) +
                        " In Sync";
                    OverviewWalletForm.labelTextLastBlockFound.Text =
                        ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration
                            .overviewwalletlabellastblockfoundtext) +
                        " In Sync";
                    labelSyncInformation.Text = "Sync & Wallet disconnected.";
                    labelNoticeTotalPendingTransactionOnReceive.Text =
                        ClassTranslation.GetLanguageTextFromOrder(
                            ClassTranslationEnumeration.panelwallettotalpendingtransactiononreceivetext);
                };
                BeginInvoke(invoke);
            }
            catch
            {
            }
        }

        /// <summary>
        /// Clean the transaction history.
        /// </summary>
        public void CleanTransactionHistory()
        {
            TransactionHistoryWalletForm.listViewNormalReceivedTransactionHistory.Items.Clear();
            TransactionHistoryWalletForm.listViewAnonymityReceivedTransactionHistory.Items.Clear();
            TransactionHistoryWalletForm.listViewAnonymitySendTransactionHistory.Items.Clear();
            TransactionHistoryWalletForm.listViewBlockRewardTransactionHistory.Items.Clear();
            TransactionHistoryWalletForm.listViewNormalSendTransactionHistory.Items.Clear();
            NormalTransactionLoaded = false;
            AnonymousTransactionLoaded = false;
            TotalTransactionAnonymousReceived = 0;
            TotalTransactionAnonymousSend = 0;
            TotalTransactionBlockReward = 0;
            TotalTransactionNormalReceived = 0;
            TotalTransactionNormalSend = 0;
            TotalTransactionRead = 0;
            TotalAnonymityTransactionRead = 0;

        }

        /// <summary>
        ///     Disable update block history.
        /// </summary>
        public void StopUpdateBlockHistory(bool restart, bool switchPage)
        {
            try
            {
                if (!switchPage)
                {
                    CurrentBlockExplorerPage = 1;
                    TotalBlockRead = 0;
                    ListBlockHashShowed.Clear();
                    EnableUpdateBlockWallet = false;

                    void MethodInvoker()
                    {
                        BlockWalletForm.listViewBlockExplorer.Items.Clear();
                    }

                    BeginInvoke((MethodInvoker)MethodInvoker);
                }
                else
                {
                    TotalBlockRead = 0;
                    ListBlockHashShowed.Clear();

                    void MethodInvoker()
                    {
                        BlockWalletForm.listViewBlockExplorer.Items.Clear();
                    }

                    BeginInvoke((MethodInvoker)MethodInvoker);
                }

                if (restart)
                {
                    EnableUpdateBlockWallet = false;
                    BlockWalletForm.StartUpdateBlockSync(this);
                }
            }
            catch
            {
            }
        }

        #endregion

        #region  Wallet Resize Interface Functions.

        /// <summary>
        ///     Resize interface and each controls inside automaticaly.
        /// </summary>
        public void ResizeWalletInterface()
        {
            try
            {
                void MethodInvoker()
                {
                    if (Height > BaseInterfaceHeight || Width > BaseInterfaceWidth)
                    {
                        if (Program.WalletXenophyte != null)
                        {
                            if (CurrentInterfaceWidth != Width && Width >= BaseInterfaceWidth ||
                                CurrentInterfaceHeight != Height && Height >= BaseInterfaceHeight)
                            {
                                #region Update Width

                                for (var i = 0; i < ListControlSizeBase.Count; i++)
                                    if (i < ListControlSizeBase.Count)
                                        if (i < Controls.Count)
                                        {
                                            var i1 = i;
                                            var currentWidth = BaseInterfaceWidth;

                                            var ratioWidth = (float)Width / currentWidth;
                                            var controlWitdh = ListControlSizeBase[i1].Item1.Width * ratioWidth;
                                            var controlLocationX = ListControlSizeBase[i1].Item2.X * ratioWidth;
                                            float controlLocationY = Controls[i1].Location.Y;
                                            var ignore =
                                                Controls[i1] is DataGridView || Controls[i1] is ListView ||
                                                Controls[i1] is TabPage;

                                            if (!ignore)
                                            {
                                                if (Controls[i1] is Label
#if WINDOWS
                                                    || Controls[i1] is MetroLabel
#endif
                                                )
                                                {
                                                    Controls[i1].Font = new Font(Controls[i1].Font.FontFamily,
                                                        Width * 1.000003f / 100, Controls[i1].Font.Style);
                                                    Controls[i1].Location = new Point((int)controlLocationX,
                                                        (int)controlLocationY);
                                                }
                                                else
                                                {
                                                    Controls[i1].Width = (int)controlWitdh;
                                                    Controls[i1].Location = new Point((int)controlLocationX,
                                                        (int)controlLocationY);
                                                }
                                            }
                                        }

                                if (ListControlSizeMain.Count > 0)
                                    for (var i = 0; i < ListControlSizeMain.Count; i++)
                                        if (i < ListControlSizeMain.Count)
                                            if (i < MainWalletForm.Controls.Count)
                                            {
                                                var i1 = i;
                                                var currentWidth = BaseInterfaceWidth;

                                                var ratioWidth = (float)Width / currentWidth;
                                                var controlWitdh = ListControlSizeMain[i1].Item1.Width * ratioWidth;
                                                var controlLocationX = ListControlSizeMain[i1].Item2.X * ratioWidth;
                                                float controlLocationY = MainWalletForm.Controls[i1].Location.Y;
                                                if (MainWalletForm.Controls[i1] is Label
#if WINDOWS
                                                    ||
                                                    MainWalletForm.Controls[i1] is MetroLabel
#endif
                                                )
                                                {
                                                    MainWalletForm.Controls[i1].Font =
                                                        new Font(MainWalletForm.Controls[i1].Font.FontFamily,
                                                            MainWalletForm.Width * 1.0014f / 100,
                                                            MainWalletForm.Controls[i1].Font.Style);
                                                    MainWalletForm.Controls[i1].Location =
                                                        new Point((int)controlLocationX, (int)controlLocationY);
                                                }
                                                else
                                                {
                                                    MainWalletForm.Controls[i1].Width = (int)controlWitdh;
                                                    MainWalletForm.Controls[i1].Location =
                                                        new Point((int)controlLocationX, (int)controlLocationY);
                                                }
                                            }

                                if (ListControlSizePanelWallet.Count > 0)
                                    for (var i = 0; i < ListControlSizePanelWallet.Count; i++)
                                        if (i < ListControlSizePanelWallet.Count)
                                            if (i < panelControlWallet.Controls.Count)
                                            {
                                                var i1 = i;
                                                var currentWidth = BaseInterfaceWidth;

                                                var ratioWidth = (float)Width / currentWidth;
                                                var controlWitdh =
                                                    ListControlSizePanelWallet[i1].Item1.Width * ratioWidth;
                                                var controlLocationX =
                                                    ListControlSizePanelWallet[i1].Item2.X * ratioWidth;
                                                float controlLocationY = panelControlWallet.Controls[i1].Location.Y;
                                                if (panelControlWallet.Controls[i1] is Label
#if WINDOWS
                                                    ||
                                                    panelControlWallet.Controls[i1] is MetroLabel
#endif
                                                )
                                                {
                                                    panelControlWallet.Controls[i1].Font =
                                                        new Font(panelControlWallet.Controls[i1].Font.FontFamily,
                                                            panelControlWallet.Width * 1.0014f / 100,
                                                            panelControlWallet.Controls[i1].Font.Style);
                                                    panelControlWallet.Controls[i1].Location =
                                                        new Point((int)controlLocationX, (int)controlLocationY);
                                                }
                                                else
                                                {
                                                    panelControlWallet.Controls[i1].Width = (int)controlWitdh;
                                                    panelControlWallet.Controls[i1].Location =
                                                        new Point((int)controlLocationX, (int)controlLocationY);
                                                }
                                            }


                                if (ListControlSizeOpenWallet.Count > 0)
                                    for (var i = 0; i < ListControlSizeOpenWallet.Count; i++)
                                        if (i < ListControlSizeOpenWallet.Count)
                                            if (i < OpenWalletForm.Controls.Count)
                                            {
                                                var i1 = i;
                                                var currentWidth = BaseInterfaceWidth;

                                                var ratioWidth = (float)Width / currentWidth;
                                                var controlWitdh =
                                                    ListControlSizeOpenWallet[i1].Item1.Width * ratioWidth;
                                                var controlLocationX =
                                                    ListControlSizeOpenWallet[i1].Item2.X * ratioWidth;
                                                float controlLocationY = OpenWalletForm.Controls[i1].Location.Y;
                                                if (OpenWalletForm.Controls[i1] is Label
#if WINDOWS
                                                    ||
                                                    OpenWalletForm.Controls[i1] is MetroLabel
#endif
                                                )
                                                {
                                                    OpenWalletForm.Controls[i1].Font =
                                                        new Font(OpenWalletForm.Controls[i1].Font.FontFamily,
                                                            OpenWalletForm.Width * 1.0014f / 100,
                                                            OpenWalletForm.Controls[i1].Font.Style);
                                                    OpenWalletForm.Controls[i1].Location =
                                                        new Point((int)controlLocationX, (int)controlLocationY);
                                                }
                                                else
                                                {
                                                    OpenWalletForm.Controls[i1].Width = (int)controlWitdh;
                                                    OpenWalletForm.Controls[i1].Location =
                                                        new Point((int)controlLocationX, (int)controlLocationY);
                                                }
                                            }

                                if (ListControlSizeOverview.Count > 0)
                                    for (var i = 0; i < ListControlSizeOverview.Count; i++)
                                        if (i < ListControlSizeOverview.Count)
                                            if (i < OverviewWalletForm.Controls.Count)
                                            {
                                                var i1 = i;
                                                var currentWidth = BaseInterfaceWidth;

                                                var ratioWidth = (float)Width / currentWidth;
                                                var controlWitdh =
                                                    ListControlSizeOverview[i1].Item1.Width * ratioWidth;
                                                var controlLocationX =
                                                    ListControlSizeOverview[i1].Item2.X * ratioWidth;
                                                float controlLocationY = OverviewWalletForm.Controls[i1].Location.Y;

                                                if (OverviewWalletForm.Controls[i1] is Label
#if WINDOWS
                                                    ||
                                                    OverviewWalletForm.Controls[i1] is MetroLabel
#endif
                                                )
                                                {
                                                    OverviewWalletForm.Controls[i1].Font =
                                                        new Font(OverviewWalletForm.Controls[i1].Font.FontFamily,
                                                            OverviewWalletForm.Width * 1.0014f / 100,
                                                            OverviewWalletForm.Controls[i1].Font.Style);
                                                    OverviewWalletForm.Controls[i1].Location =
                                                        new Point((int)controlLocationX, (int)controlLocationY);
                                                }
                                                else
                                                {
                                                    OverviewWalletForm.Controls[i1].Width = (int)controlWitdh;
                                                    OverviewWalletForm.Controls[i1].Location =
                                                        new Point((int)controlLocationX, (int)controlLocationY);
                                                }
                                            }

                                if (ListControlSizeTransaction.Count > 0)
                                    for (var i = 0; i < ListControlSizeTransaction.Count; i++)
                                        if (i < ListControlSizeTransaction.Count)
                                            if (i < TransactionHistoryWalletForm.Controls.Count)
                                            {
                                                var i1 = i;

                                                var ignore =
                                                    TransactionHistoryWalletForm.Controls[i1] is DataGridView ||
                                                    TransactionHistoryWalletForm.Controls[i1] is ListView ||
                                                    TransactionHistoryWalletForm.Controls[i1] is TabPage ||
                                                    TransactionHistoryWalletForm.Controls[i1] is Panel;

                                                if (!ignore)
                                                {
                                                    var currentWidth = BaseInterfaceWidth;

                                                    var ratioWidth = (float)Width / currentWidth;
                                                    var controlWitdh =
                                                        ListControlSizeTransaction[i1].Item1.Width * ratioWidth;
                                                    var controlLocationX =
                                                        ListControlSizeTransaction[i1].Item2.X * ratioWidth;
                                                    float controlLocationY = TransactionHistoryWalletForm.Controls[i1]
                                                        .Location.Y;

                                                    if (TransactionHistoryWalletForm.Controls[i1] is Label
#if WINDOWS
                                                        ||
                                                        TransactionHistoryWalletForm.Controls[i1] is MetroLabel
#endif
                                                    )
                                                    {
                                                        TransactionHistoryWalletForm.Controls[i1].Font = new Font(
                                                            TransactionHistoryWalletForm.Controls[i1].Font.FontFamily,
                                                            TransactionHistoryWalletForm.Width * 1.0014f /
                                                            100,
                                                            TransactionHistoryWalletForm.Controls[i1].Font.Style);
                                                        TransactionHistoryWalletForm.Controls[i1].Location =
                                                            new Point((int)controlLocationX, (int)controlLocationY);
                                                    }
                                                    else
                                                    {
                                                        TransactionHistoryWalletForm.Controls[i1].Width =
                                                            (int)controlWitdh;
                                                        TransactionHistoryWalletForm.Controls[i1].Location =
                                                            new Point((int)controlLocationX, (int)controlLocationY);
                                                    }
                                                }
                                            }


                                if (ListControlSizeSendTransaction.Count > 0)
                                    for (var i = 0; i < ListControlSizeSendTransaction.Count; i++)
                                        if (i < ListControlSizeSendTransaction.Count)
                                            if (i < SendTransactionWalletForm.Controls.Count)
                                            {
                                                var i1 = i;
                                                var currentWidth = BaseInterfaceWidth;

                                                var ratioWidth = (float)Width / currentWidth;
                                                var controlWitdh =
                                                    ListControlSizeSendTransaction[i1].Item1.Width * ratioWidth;
                                                var controlLocationX =
                                                    ListControlSizeSendTransaction[i1].Item2.X * ratioWidth;
                                                float controlLocationY =
                                                    SendTransactionWalletForm.Controls[i1].Location.Y;
                                                if (SendTransactionWalletForm.Controls[i1] is Label
#if WINDOWS
                                                    ||
                                                    SendTransactionWalletForm.Controls[i1] is MetroLabel
#endif
                                                )
                                                {
                                                    SendTransactionWalletForm.Controls[i1].Font = new Font(
                                                        SendTransactionWalletForm.Controls[i1].Font.FontFamily,
                                                        SendTransactionWalletForm.Width * 1.0014f / 100,
                                                        SendTransactionWalletForm.Controls[i1].Font.Style);
                                                    SendTransactionWalletForm.Controls[i1].Location =
                                                        new Point((int)controlLocationX, (int)controlLocationY);
                                                }
                                                else
                                                {
                                                    SendTransactionWalletForm.Controls[i1].Font = new Font(
                                                        SendTransactionWalletForm.Controls[i1].Font.FontFamily,
                                                        SendTransactionWalletForm.Width * 1.0014f / 100,
                                                        SendTransactionWalletForm.Controls[i1].Font.Style);
                                                    SendTransactionWalletForm.Controls[i1].Width = (int)controlWitdh;
                                                    SendTransactionWalletForm.Controls[i1].Location =
                                                        new Point((int)controlLocationX, (int)controlLocationY);
                                                }
                                            }

                                if (ListControlSizeCreateWallet.Count > 0)
                                    for (var i = 0; i < ListControlSizeCreateWallet.Count; i++)
                                        if (i < ListControlSizeCreateWallet.Count)
                                            if (i < CreateWalletForm.Controls.Count)
                                            {
                                                var i1 = i;
                                                var currentWidth = BaseInterfaceWidth;

                                                var ratioWidth = (float)Width / currentWidth;
                                                var controlWitdh =
                                                    ListControlSizeCreateWallet[i1].Item1.Width * ratioWidth;
                                                var controlLocationX =
                                                    ListControlSizeCreateWallet[i1].Item2.X * ratioWidth;
                                                float controlLocationY = CreateWalletForm.Controls[i1].Location.Y;
                                                if (CreateWalletForm.Controls[i1] is Label
#if WINDOWS
                                                    ||
                                                    CreateWalletForm.Controls[i1] is MetroLabel
#endif
                                                )
                                                {
                                                    CreateWalletForm.Controls[i1].Font =
                                                        new Font(CreateWalletForm.Controls[i1].Font.FontFamily,
                                                            CreateWalletForm.Width * 1.0014f / 100,
                                                            CreateWalletForm.Controls[i1].Font.Style);
                                                    CreateWalletForm.Controls[i1].Location =
                                                        new Point((int)controlLocationX, (int)controlLocationY);
                                                }
                                                else
                                                {
                                                    CreateWalletForm.Controls[i1].Width = (int)controlWitdh;
                                                    CreateWalletForm.Controls[i1].Location =
                                                        new Point((int)controlLocationX, (int)controlLocationY);
                                                }
                                            }

                                if (ListControlSizeRestoreWallet.Count > 0)
                                    for (var i = 0; i < ListControlSizeRestoreWallet.Count; i++)
                                        if (i < ListControlSizeRestoreWallet.Count)
                                            if (i < RestoreWalletForm.Controls.Count)
                                            {
                                                var i1 = i;
                                                var currentWidth = BaseInterfaceWidth;

                                                var ratioWidth = (float)Width / currentWidth;
                                                var controlWitdh =
                                                    ListControlSizeRestoreWallet[i1].Item1.Width * ratioWidth;
                                                var controlLocationX =
                                                    ListControlSizeRestoreWallet[i1].Item2.X * ratioWidth;
                                                float controlLocationY = RestoreWalletForm.Controls[i1].Location.Y;
                                                if (RestoreWalletForm.Controls[i1] is Label
#if WINDOWS
                                                    ||
                                                    RestoreWalletForm.Controls[i1] is MetroLabel
#endif
                                                )
                                                {
                                                    RestoreWalletForm.Controls[i1].Font =
                                                        new Font(RestoreWalletForm.Controls[i1].Font.FontFamily,
                                                            RestoreWalletForm.Width * 1.0014f / 100,
                                                            RestoreWalletForm.Controls[i1].Font.Style);
                                                    RestoreWalletForm.Controls[i1].Location =
                                                        new Point((int)controlLocationX, (int)controlLocationY);
                                                }
                                                else
                                                {
                                                    RestoreWalletForm.Controls[i1].Width = (int)controlWitdh;
                                                    RestoreWalletForm.Controls[i1].Location =
                                                        new Point((int)controlLocationX, (int)controlLocationY);
                                                }
                                            }

                                if (ListControlSizeContactWallet.Count > 0)
                                    for (var i = 0; i < ListControlSizeContactWallet.Count; i++)
                                        if (i < ListControlSizeContactWallet.Count)
                                            if (i < ContactWalletForm.Controls.Count)
                                            {
                                                var i1 = i;
                                                var currentWidth = BaseInterfaceWidth;

                                                var ratioWidth = (float)Width / currentWidth;
                                                var controlWitdh =
                                                    ListControlSizeContactWallet[i1].Item1.Width * ratioWidth;
                                                var controlLocationX =
                                                    ListControlSizeContactWallet[i1].Item2.X * ratioWidth;
                                                float controlLocationY = ContactWalletForm.Controls[i1].Location.Y;
                                                if (ContactWalletForm.Controls[i1] is Label
#if WINDOWS
                                                    ||
                                                    ContactWalletForm.Controls[i1] is MetroLabel
#endif
                                                )
                                                {
                                                    ContactWalletForm.Controls[i1].Font =
                                                        new Font(ContactWalletForm.Controls[i1].Font.FontFamily,
                                                            ContactWalletForm.Width * 1.0014f / 100,
                                                            ContactWalletForm.Controls[i1].Font.Style);
                                                    ContactWalletForm.Controls[i1].Location =
                                                        new Point((int)controlLocationX, (int)controlLocationY);
                                                }
                                                else
                                                {
                                                    ContactWalletForm.Controls[i1].Width = (int)controlWitdh;
                                                    ContactWalletForm.Controls[i1].Location =
                                                        new Point((int)controlLocationX, (int)controlLocationY);
                                                }
                                            }

                                if (ListControlSizeBlock.Count > 0)
                                    for (var i = 0; i < ListControlSizeBlock.Count; i++)
                                        if (i < ListControlSizeBlock.Count)
                                            if (i < BlockWalletForm.Controls.Count)
                                            {
                                                var i1 = i;

                                                var ignore =
                                                    BlockWalletForm.Controls[i1] is DataGridView ||
                                                    BlockWalletForm.Controls[i1] is ListView ||
                                                    BlockWalletForm.Controls[i1] is TabPage ||
                                                    BlockWalletForm.Controls[i1] is Panel;

                                                if (!ignore)
                                                {
                                                    var currentWidth = BaseInterfaceWidth;

                                                    var ratioWidth = (float)Width / currentWidth;
                                                    var controlWitdh =
                                                        ListControlSizeBlock[i1].Item1.Width * ratioWidth;
                                                    var controlLocationX =
                                                        ListControlSizeBlock[i1].Item2.X * ratioWidth;
                                                    float controlLocationY = BlockWalletForm.Controls[i1].Location.Y;
                                                    BlockWalletForm.Controls[i1].Width = (int)controlWitdh;
                                                    BlockWalletForm.Controls[i1].Location =
                                                        new Point((int)controlLocationX, (int)controlLocationY);
                                                }
                                            }

                                #endregion


                                #region Update Height

                                for (var i = 0; i < ListControlSizeBase.Count; i++)
                                    if (i < ListControlSizeBase.Count)
                                        if (i < Controls.Count)
                                        {
                                            var i1 = i;
                                            var currentHeight = BaseInterfaceHeight;

                                            var ratioHeight = (float)Height / currentHeight;
                                            var controlWitdh = ListControlSizeBase[i1].Item1.Height * ratioHeight;
                                            float controlLocationX = Controls[i1].Location.X;
                                            var controlLocationY = ListControlSizeBase[i1].Item2.Y * ratioHeight;
                                            Controls[i1].Height = (int)controlWitdh;
                                            Controls[i1].Location = new Point((int)controlLocationX,
                                                (int)controlLocationY);
                                        }

                                for (var i = 0; i < ListControlSizeMain.Count; i++)
                                    if (i < ListControlSizeMain.Count)
                                        if (i < MainWalletForm.Controls.Count)
                                        {
                                            var i1 = i;
                                            var currentHeight = BaseInterfaceHeight;

                                            var ratioHeight = (float)Height / currentHeight;
                                            var controlWitdh = ListControlSizeMain[i1].Item1.Height * ratioHeight;
                                            float controlLocationX = MainWalletForm.Controls[i1].Location.X;
                                            var controlLocationY = ListControlSizeMain[i1].Item2.Y * ratioHeight;
                                            MainWalletForm.Controls[i1].Height = (int)controlWitdh;
                                            MainWalletForm.Controls[i1].Location = new Point((int)controlLocationX,
                                                (int)controlLocationY);
                                        }

                                for (var i = 0; i < ListControlSizePanelWallet.Count; i++)
                                    if (i < ListControlSizePanelWallet.Count)
                                        if (i < panelControlWallet.Controls.Count)
                                        {
                                            var i1 = i;
                                            var currentHeight = BaseInterfaceHeight;

                                            var ratioHeight = (float)Height / currentHeight;
                                            var controlWitdh =
                                                ListControlSizePanelWallet[i1].Item1.Height * ratioHeight;
                                            float controlLocationX = panelControlWallet.Controls[i1].Location.X;
                                            var controlLocationY = ListControlSizePanelWallet[i1].Item2.Y * ratioHeight;
                                            panelControlWallet.Controls[i1].Height = (int)controlWitdh;
                                            panelControlWallet.Controls[i1].Location = new Point((int)controlLocationX,
                                                (int)controlLocationY);
                                        }


                                for (var i = 0; i < ListControlSizeOpenWallet.Count; i++)
                                    if (i < ListControlSizeOpenWallet.Count)
                                        if (i < OpenWalletForm.Controls.Count)
                                        {
                                            var i1 = i;
                                            var currentHeight = BaseInterfaceHeight;

                                            var ratioHeight = (float)Height / currentHeight;
                                            var controlWitdh =
                                                ListControlSizeOpenWallet[i1].Item1.Height * ratioHeight;
                                            float controlLocationX = OpenWalletForm.Controls[i1].Location.X;
                                            var controlLocationY =
                                                ListControlSizeOpenWallet[i1].Item2.Y * ratioHeight;
                                            OpenWalletForm.Controls[i1].Height = (int)controlWitdh;
                                            OpenWalletForm.Controls[i1].Location = new Point((int)controlLocationX,
                                                (int)controlLocationY);
                                        }

                                for (var i = 0; i < ListControlSizeOverview.Count; i++)
                                    if (i < ListControlSizeOverview.Count)
                                        if (i < OverviewWalletForm.Controls.Count)
                                        {
                                            var i1 = i;
                                            var currentHeight = BaseInterfaceHeight;

                                            var ratioHeight = (float)Height / currentHeight;
                                            var controlWitdh = ListControlSizeOverview[i1].Item1.Height * ratioHeight;
                                            float controlLocationX = OverviewWalletForm.Controls[i1].Location.X;
                                            var controlLocationY = ListControlSizeOverview[i1].Item2.Y * ratioHeight;
                                            OverviewWalletForm.Controls[i1].Height = (int)controlWitdh;
                                            OverviewWalletForm.Controls[i1].Location = new Point((int)controlLocationX,
                                                (int)controlLocationY);
                                        }

                                for (var i = 0; i < ListControlSizeTransaction.Count; i++)
                                    if (i < ListControlSizeTransaction.Count)
                                        if (i < TransactionHistoryWalletForm.Controls.Count)
                                        {
                                            var i1 = i;
                                            var ignore = TransactionHistoryWalletForm.Controls[i1] is DataGridView ||
                                                         TransactionHistoryWalletForm.Controls[i1] is ListView ||
                                                         TransactionHistoryWalletForm.Controls[i1] is TabPage ||
                                                         TransactionHistoryWalletForm.Controls[i1] is Panel;
                                            if (!ignore)
                                            {
                                                var currentHeight = BaseInterfaceHeight;

                                                var ratioHeight = (float)Height / currentHeight;
                                                var controlWitdh =
                                                    ListControlSizeTransaction[i1].Item1.Height * ratioHeight;
                                                float controlLocationX =
                                                    TransactionHistoryWalletForm.Controls[i1].Location.X;
                                                var controlLocationY =
                                                    ListControlSizeTransaction[i1].Item2.Y * ratioHeight;
                                                TransactionHistoryWalletForm.Controls[i1].Height = (int)controlWitdh;
                                                TransactionHistoryWalletForm.Controls[i1].Location =
                                                    new Point((int)controlLocationX, (int)controlLocationY);
                                            }
                                        }

                                for (var i = 0; i < ListControlSizeSendTransaction.Count; i++)
                                    if (i < ListControlSizeSendTransaction.Count)
                                        if (i < SendTransactionWalletForm.Controls.Count)
                                        {
                                            var i1 = i;

                                            var currentHeight = BaseInterfaceHeight;

                                            var ratioHeight = (float)Height / currentHeight;
                                            var controlWitdh =
                                                ListControlSizeSendTransaction[i1].Item1.Height * ratioHeight;
                                            float controlLocationX = SendTransactionWalletForm.Controls[i1].Location.X;
                                            var controlLocationY =
                                                ListControlSizeSendTransaction[i1].Item2.Y * ratioHeight;
                                            SendTransactionWalletForm.Controls[i1].Height = (int)controlWitdh;
                                            SendTransactionWalletForm.Controls[i1].Location =
                                                new Point((int)controlLocationX, (int)controlLocationY);
                                        }

                                for (var i = 0; i < ListControlSizeCreateWallet.Count; i++)
                                    if (i < ListControlSizeCreateWallet.Count)
                                        if (i < CreateWalletForm.Controls.Count)
                                        {
                                            var i1 = i;
                                            var currentHeight = BaseInterfaceHeight;

                                            var ratioHeight = (float)Height / currentHeight;
                                            var controlWitdh =
                                                ListControlSizeCreateWallet[i1].Item1.Height * ratioHeight;
                                            float controlLocationX = CreateWalletForm.Controls[i1].Location.X;
                                            var controlLocationY =
                                                ListControlSizeCreateWallet[i1].Item2.Y * ratioHeight;
                                            CreateWalletForm.Controls[i1].Height = (int)controlWitdh;
                                            CreateWalletForm.Controls[i1].Location = new Point((int)controlLocationX,
                                                (int)controlLocationY);
                                        }

                                for (var i = 0; i < ListControlSizeRestoreWallet.Count; i++)
                                    if (i < ListControlSizeRestoreWallet.Count)
                                        if (i < RestoreWalletForm.Controls.Count)
                                        {
                                            var i1 = i;
                                            var currentHeight = BaseInterfaceHeight;

                                            var ratioHeight = (float)Height / currentHeight;
                                            var controlWitdh =
                                                ListControlSizeRestoreWallet[i1].Item1.Height * ratioHeight;
                                            float controlLocationX = RestoreWalletForm.Controls[i1].Location.X;
                                            var controlLocationY =
                                                ListControlSizeRestoreWallet[i1].Item2.Y * ratioHeight;
                                            RestoreWalletForm.Controls[i1].Height = (int)controlWitdh;
                                            RestoreWalletForm.Controls[i1].Location = new Point((int)controlLocationX,
                                                (int)controlLocationY);
                                        }

                                for (var i = 0; i < ListControlSizeContactWallet.Count; i++)
                                    if (i < ListControlSizeContactWallet.Count)
                                        if (i < ContactWalletForm.Controls.Count)
                                        {
                                            var i1 = i;
                                            var currentHeight = BaseInterfaceHeight;

                                            var ratioHeight = (float)Height / currentHeight;
                                            var controlWitdh =
                                                ListControlSizeContactWallet[i1].Item1.Height * ratioHeight;
                                            float controlLocationX = ContactWalletForm.Controls[i1].Location.X;
                                            var controlLocationY =
                                                ListControlSizeContactWallet[i1].Item2.Y * ratioHeight;
                                            ContactWalletForm.Controls[i1].Height = (int)controlWitdh;
                                            ContactWalletForm.Controls[i1].Location = new Point((int)controlLocationX,
                                                (int)controlLocationY);
                                        }

                                for (var i = 0; i < ListControlSizeBlock.Count; i++)
                                    if (i < ListControlSizeBlock.Count)
                                        if (i < BlockWalletForm.Controls.Count)
                                        {
                                            var i1 = i;
                                            var ignore =
                                                BlockWalletForm.Controls[i1] is DataGridView ||
                                                BlockWalletForm.Controls[i1] is ListView ||
                                                BlockWalletForm.Controls[i1] is TabPage ||
                                                BlockWalletForm.Controls[i1] is Panel;

                                            if (!ignore)
                                            {
                                                var currentHeight = BaseInterfaceHeight;

                                                var ratioHeight = (float)Height / currentHeight;
                                                var controlWitdh =
                                                    ListControlSizeBlock[i1].Item1.Height * ratioHeight;
                                                float controlLocationX = BlockWalletForm.Controls[i1].Location.X;
                                                var controlLocationY = ListControlSizeBlock[i1].Item2.Y * ratioHeight;
                                                BlockWalletForm.Controls[i1].Height = (int)controlWitdh;
                                                BlockWalletForm.Controls[i1].Location =
                                                    new Point((int)controlLocationX, (int)controlLocationY);
                                            }
                                        }

                                #endregion

                                CurrentInterfaceHeight = Height;
                                OpenWalletForm.Size = panelMainForm.Size;
                                MainWalletForm.Size = panelMainForm.Size;
                                OverviewWalletForm.Size = panelMainForm.Size;
                                TransactionHistoryWalletForm.Size = panelMainForm.Size;
                                SendTransactionWalletForm.Size = panelMainForm.Size;
                                CreateWalletForm.Size = panelMainForm.Size;
                                BlockWalletForm.Size = panelMainForm.Size;
                                RestoreWalletForm.Size = panelMainForm.Size;
                                ContactWalletForm.Size = panelMainForm.Size;
                            }
                            else
                            {
                                if (Height < BaseInterfaceHeight) Height = BaseInterfaceHeight;

                                if (Width < BaseInterfaceWidth) Width = BaseInterfaceWidth;
                            }
                        }
                    }
                    else // Restore interface size.
                    {
                        if (Program.WalletXenophyte != null)
                        {
                            if (ListControlSizeBase.Count > 0)
                                for (var i = 0; i < ListControlSizeBase.Count; i++)
                                    if (i < ListControlSizeBase.Count)
                                        if (i < Controls.Count)
                                        {
                                            Controls[i].Size = ListControlSizeBase[i].Item1;
                                            Controls[i].Location = ListControlSizeBase[i].Item2;
                                            if (Controls[i] is Label
#if WINDOWS
                                                || Controls[i] is MetroLabel
#endif
                                            )
                                                Controls[i].Font = new Font(Controls[i].Font.FontFamily,
                                                    Width * 1.000003f / 100, Controls[i].Font.Style);
                                        }

                            if (ListControlSizeMain.Count > 0)
                                for (var i = 0; i < ListControlSizeMain.Count; i++)
                                    if (i < ListControlSizeMain.Count)
                                        if (i < MainWalletForm.Controls.Count)
                                        {
                                            MainWalletForm.Controls[i].Size = ListControlSizeMain[i].Item1;
                                            MainWalletForm.Controls[i].Location = ListControlSizeMain[i].Item2;
                                            if (MainWalletForm.Controls[i] is Label
#if WINDOWS
                                                || MainWalletForm.Controls[i] is MetroLabel
#endif
                                            )
                                                MainWalletForm.Controls[i].Font =
                                                    new Font(MainWalletForm.Controls[i].Font.FontFamily,
                                                        MainWalletForm.Width * 1.0014f / 100,
                                                        MainWalletForm.Controls[i].Font.Style);
                                        }

                            if (ListControlSizePanelWallet.Count > 0)
                                for (var i = 0; i < ListControlSizePanelWallet.Count; i++)
                                    if (i < ListControlSizePanelWallet.Count)
                                        if (i < panelControlWallet.Controls.Count)
                                        {
                                            panelControlWallet.Controls[i].Size = ListControlSizePanelWallet[i].Item1;
                                            panelControlWallet.Controls[i].Location =
                                                ListControlSizePanelWallet[i].Item2;
                                            if (panelControlWallet.Controls[i] is Label
#if WINDOWS
                                                || panelControlWallet.Controls[i] is MetroLabel
#endif
                                            )
                                                panelControlWallet.Controls[i].Font =
                                                    new Font(panelControlWallet.Controls[i].Font.FontFamily,
                                                        panelControlWallet.Width * 1.0014f / 100,
                                                        panelControlWallet.Controls[i].Font.Style);
                                        }

                            if (ListControlSizeOpenWallet.Count > 0)
                                for (var i = 0; i < ListControlSizeOpenWallet.Count; i++)
                                    if (i < ListControlSizeOpenWallet.Count)
                                        if (i < OpenWalletForm.Controls.Count)
                                        {
                                            OpenWalletForm.Controls[i].Size = ListControlSizeOpenWallet[i].Item1;
                                            OpenWalletForm.Controls[i].Location = ListControlSizeOpenWallet[i].Item2;
                                            if (OpenWalletForm.Controls[i] is Label
#if WINDOWS
                                                || OpenWalletForm.Controls[i] is MetroLabel
#endif
                                            )
                                                OpenWalletForm.Controls[i].Font =
                                                    new Font(OpenWalletForm.Controls[i].Font.FontFamily,
                                                        OpenWalletForm.Width * 1.0014f / 100,
                                                        OpenWalletForm.Controls[i].Font.Style);
                                        }

                            if (ListControlSizeOverview.Count > 0)
                                for (var i = 0; i < ListControlSizeOverview.Count; i++)
                                    if (i < ListControlSizeOverview.Count)
                                        if (i < OverviewWalletForm.Controls.Count)
                                        {
                                            OverviewWalletForm.Controls[i].Size = ListControlSizeOverview[i].Item1;
                                            OverviewWalletForm.Controls[i].Location = ListControlSizeOverview[i].Item2;
                                            if (OverviewWalletForm.Controls[i] is Label
#if WINDOWS
                                                || OverviewWalletForm.Controls[i] is MetroLabel
#endif
                                            )
                                                OverviewWalletForm.Controls[i].Font =
                                                    new Font(OverviewWalletForm.Controls[i].Font.FontFamily,
                                                        OverviewWalletForm.Width * 1.0014f / 100,
                                                        OverviewWalletForm.Controls[i].Font.Style);
                                        }

                            if (ListControlSizeTransaction.Count > 0)
                                for (var i = 0; i < ListControlSizeTransaction.Count; i++)
                                    if (i < ListControlSizeTransaction.Count)
                                        if (i < TransactionHistoryWalletForm.Controls.Count)
                                        {
                                            TransactionHistoryWalletForm.Controls[i].Size =
                                                ListControlSizeTransaction[i].Item1;
                                            TransactionHistoryWalletForm.Controls[i].Location =
                                                ListControlSizeTransaction[i].Item2;
                                            if (TransactionHistoryWalletForm.Controls[i] is Label
#if WINDOWS
                                                || TransactionHistoryWalletForm.Controls[i] is MetroLabel
#endif
                                            )
                                                TransactionHistoryWalletForm.Controls[i].Font = new Font(
                                                    TransactionHistoryWalletForm.Controls[i].Font.FontFamily,
                                                    TransactionHistoryWalletForm.Width * 1.0014f / 100,
                                                    TransactionHistoryWalletForm.Controls[i].Font.Style);
                                        }

                            if (ListControlSizeSendTransaction.Count > 0)
                                for (var i = 0; i < ListControlSizeSendTransaction.Count; i++)
                                    if (i < ListControlSizeSendTransaction.Count)
                                        if (i < SendTransactionWalletForm.Controls.Count)
                                        {
                                            SendTransactionWalletForm.Controls[i].Size =
                                                ListControlSizeSendTransaction[i].Item1;
                                            SendTransactionWalletForm.Controls[i].Location =
                                                ListControlSizeSendTransaction[i].Item2;
                                            if (SendTransactionWalletForm.Controls[i] is Label
#if WINDOWS
                                                || SendTransactionWalletForm.Controls[i] is MetroLabel
#endif
                                            )
                                                SendTransactionWalletForm.Controls[i].Font = new Font(
                                                    SendTransactionWalletForm.Controls[i].Font.FontFamily,
                                                    SendTransactionWalletForm.Width * 1.0014f / 100,
                                                    SendTransactionWalletForm.Controls[i].Font.Style);
                                        }

                            if (ListControlSizeCreateWallet.Count > 0)
                                for (var i = 0; i < ListControlSizeCreateWallet.Count; i++)
                                    if (i < ListControlSizeCreateWallet.Count)
                                        if (i < CreateWalletForm.Controls.Count)
                                        {
                                            CreateWalletForm.Controls[i].Size = ListControlSizeCreateWallet[i].Item1;
                                            CreateWalletForm.Controls[i].Location =
                                                ListControlSizeCreateWallet[i].Item2;
                                            if (CreateWalletForm.Controls[i] is Label
#if WINDOWS
                                                || CreateWalletForm.Controls[i] is MetroLabel
#endif
                                            )
                                                CreateWalletForm.Controls[i].Font =
                                                    new Font(CreateWalletForm.Controls[i].Font.FontFamily,
                                                        CreateWalletForm.Width * 1.0014f / 100,
                                                        CreateWalletForm.Controls[i].Font.Style);
                                        }

                            if (ListControlSizeBlock.Count > 0)
                                for (var i = 0; i < ListControlSizeBlock.Count; i++)
                                    if (i < ListControlSizeBlock.Count)
                                        if (i < BlockWalletForm.Controls.Count)
                                        {
                                            BlockWalletForm.Controls[i].Size = ListControlSizeBlock[i].Item1;
                                            BlockWalletForm.Controls[i].Location = ListControlSizeBlock[i].Item2;
                                            if (BlockWalletForm.Controls[i] is Label
#if WINDOWS
                                                || BlockWalletForm.Controls[i] is MetroLabel
#endif
                                            )
                                                BlockWalletForm.Controls[i].Font =
                                                    new Font(BlockWalletForm.Controls[i].Font.FontFamily,
                                                        BlockWalletForm.Width * 1.0014f / 100,
                                                        BlockWalletForm.Controls[i].Font.Style);
                                        }

                            if (ListControlSizeRestoreWallet.Count > 0)
                                for (var i = 0; i < ListControlSizeRestoreWallet.Count; i++)
                                    if (i < ListControlSizeRestoreWallet.Count)
                                        if (i < RestoreWalletForm.Controls.Count)
                                        {
                                            RestoreWalletForm.Controls[i].Size = ListControlSizeRestoreWallet[i].Item1;
                                            RestoreWalletForm.Controls[i].Location =
                                                ListControlSizeRestoreWallet[i].Item2;
                                            if (RestoreWalletForm.Controls[i] is Label
#if WINDOWS
                                                || RestoreWalletForm.Controls[i] is MetroLabel
#endif
                                            )
                                                RestoreWalletForm.Controls[i].Font =
                                                    new Font(RestoreWalletForm.Controls[i].Font.FontFamily,
                                                        RestoreWalletForm.Width * 1.0014f / 100,
                                                        RestoreWalletForm.Controls[i].Font.Style);
                                        }

                            if (ListControlSizeContactWallet.Count > 0)
                                for (var i = 0; i < ListControlSizeContactWallet.Count; i++)
                                    if (i < ListControlSizeContactWallet.Count)
                                        if (i < ContactWalletForm.Controls.Count)
                                        {
                                            ContactWalletForm.Controls[i].Size = ListControlSizeContactWallet[i].Item1;
                                            ContactWalletForm.Controls[i].Location =
                                                ListControlSizeContactWallet[i].Item2;
                                            if (ContactWalletForm.Controls[i] is Label
#if WINDOWS
                                                || ContactWalletForm.Controls[i] is MetroLabel
#endif
                                            )
                                                ContactWalletForm.Controls[i].Font =
                                                    new Font(ContactWalletForm.Controls[i].Font.FontFamily,
                                                        ContactWalletForm.Width * 1.0014f / 100,
                                                        ContactWalletForm.Controls[i].Font.Style);
                                        }
                        }

                        Height = BaseInterfaceHeight;
                        CurrentInterfaceHeight = Height;
                        Width = BaseInterfaceWidth;
                        CurrentInterfaceWidth = Width;
                    }
                }

                BeginInvoke((MethodInvoker)MethodInvoker);
            }
            catch
            {
            }
        }

        #endregion

        #region About Wallet Theme

        private void darkerToolStipMenuItem_Click(object sender, EventArgs e)
        {
            UpdateColorStyle(Color.FromArgb(17, 17, 17), Color.White, Color.PaleTurquoise, Color.CornflowerBlue);
        }

        private void lightToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UpdateColorStyle(Color.White, Color.Black, Color.White, Color.LightSkyBlue);
        }

        private void darkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UpdateColorStyle(Color.FromArgb(47, 49, 54), Color.White, Color.PaleTurquoise, Color.DeepSkyBlue);
        }

        /// <summary>
        ///     Change colors of all controls of the wallet interface.
        /// </summary>
        /// <param name="background"></param>
        /// <param name="text"></param>
        private void UpdateColorStyle(Color background, Color text, Color backgroundList, Color menuStripBackground)
        {
#if WINDOWS

            MetroColors.Custom = background;
            Theme = MetroThemeStyle.Custom;
            UseCustomForeColor = true;
            ForeColor = text;

#else
            BackColor = background;
#endif
            menuStripMenu.BackColor = menuStripBackground;


            for (var i = 0; i < Controls.Count; i++)
                if (i < Controls.Count)
                    Controls[i].ForeColor = text;
            textBoxResearch.ForeColor = Color.Black;
            OpenWalletForm.BackColor = background;
            for (var i = 0; i < OpenWalletForm.Controls.Count; i++)
                if (i < OpenWalletForm.Controls.Count)
                    if (!(OpenWalletForm.Controls[i] is TextBox))
                    {
                        OpenWalletForm.Controls[i].BackColor = background;
                        OpenWalletForm.Controls[i].ForeColor = text;
                    }

            MainWalletForm.BackColor = background;
            for (var i = 0; i < MainWalletForm.Controls.Count; i++)
                if (i < MainWalletForm.Controls.Count)
                    if (!(MainWalletForm.Controls[i] is TextBox))
                    {
                        MainWalletForm.Controls[i].BackColor = background;
                        MainWalletForm.Controls[i].ForeColor = text;
                    }

            OverviewWalletForm.BackColor = background;
            for (var i = 0; i < OverviewWalletForm.Controls.Count; i++)
                if (i < OverviewWalletForm.Controls.Count)
                    if (!(OverviewWalletForm.Controls[i] is TextBox))
                    {
                        OverviewWalletForm.Controls[i].BackColor = background;
                        OverviewWalletForm.Controls[i].ForeColor = text;
                    }

            SendTransactionWalletForm.BackColor = background;
            for (var i = 0; i < SendTransactionWalletForm.Controls.Count; i++)
                if (i < SendTransactionWalletForm.Controls.Count)
                    if (!(SendTransactionWalletForm.Controls[i] is TextBox))
                    {
                        SendTransactionWalletForm.Controls[i].BackColor = background;
                        SendTransactionWalletForm.Controls[i].ForeColor = text;
                    }

            CreateWalletForm.BackColor = background;
            for (var i = 0; i < CreateWalletForm.Controls.Count; i++)
                if (i < CreateWalletForm.Controls.Count)
                    if (!(CreateWalletForm.Controls[i] is TextBox))
                    {
                        CreateWalletForm.Controls[i].BackColor = background;
                        CreateWalletForm.Controls[i].ForeColor = text;
                    }

            RestoreWalletForm.BackColor = background;
            for (var i = 0; i < RestoreWalletForm.Controls.Count; i++)
                if (i < RestoreWalletForm.Controls.Count)
                    if (!(RestoreWalletForm.Controls[i] is TextBox))
                    {
                        RestoreWalletForm.Controls[i].BackColor = background;
                        RestoreWalletForm.Controls[i].ForeColor = text;
                    }

            for (var i = 0; i < panelControlWallet.Controls.Count; i++)
                if (i < panelControlWallet.Controls.Count)
                    panelControlWallet.Controls[i].ForeColor = text;
            buttonLastPage.ForeColor = Color.Black;
            buttonFirstPage.ForeColor = Color.Black;
            buttonNextPage.ForeColor = Color.Black;
            buttonPreviousPage.ForeColor = Color.Black;
            ContactWalletForm.buttonAddContact.ForeColor = Color.Black;
            pictureBoxLogo.BackColor = background;
        }

        #endregion

        /// <summary>
        ///     Research an element.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxResearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keys.Enter == e.KeyCode)
            {
                if (ClassFormPhase.FormPhase == ClassFormPhaseEnumeration.TransactionHistory)
                    DoTransactionResearch(textBoxResearch.Text);
                else if (ClassFormPhase.FormPhase == ClassFormPhaseEnumeration.BlockExplorer)
                    DoBlockResearch(textBoxResearch.Text);
            }
        }

        /// <summary>
        ///     Research an element.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonResearch_Click(object sender, EventArgs e)
        {
            if (ClassFormPhase.FormPhase == ClassFormPhaseEnumeration.TransactionHistory)
                DoTransactionResearch(textBoxResearch.Text);
            else if (ClassFormPhase.FormPhase == ClassFormPhaseEnumeration.BlockExplorer)
                DoBlockResearch(textBoxResearch.Text);
        }

        /// <summary>
        ///     Do rearch of block element.
        /// </summary>
        /// <param name="elementToSearch"></param>
        private void DoBlockResearch(string elementToSearch)
        {
            try
            {
                var elementIdFound = 0;
                var elementFound = false;
                foreach (var blockObject in ClassBlockCache.ListBlock.ToArray())
                    if (!elementFound)
                    {
                        if (blockObject.Value.BlockHash.Contains(elementToSearch))
                            elementFound = true;
                        else if (blockObject.Value.BlockTransactionHash.Contains(elementToSearch))
                            elementFound = true;
                        else if (blockObject.Value.BlockHeight.Contains(elementToSearch))
                            elementFound = true;
                        else
                            elementIdFound++;
                    }

                if (elementFound)
                {
                    var walletResearchElementForm = new SearchWalletExplorer
                    {
                        StartPosition = FormStartPosition.CenterParent
                    };
                    walletResearchElementForm.AppendText(ClassBlockCache.ListBlock.ElementAt(elementIdFound).Value
                        .ConcatBlockElement());
                    walletResearchElementForm.ShowDialog(this);
#if DEBUG
                Log.WriteLine("Element found: " + ClassBlockCache.ListBlock.ElementAt(elementIdFound).Value.ConcatBlockElement());
#endif
                }
                else
                {
#if WINDOWS

                    ClassFormPhase.MessageBoxInterface(elementToSearch + " not found.", "Not found",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
#endif
#if LINUX
                    MessageBox.Show(this, elementToSearch + " not found.", "Not found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
#endif
                }
            }
            catch
            {
            }
        }

        /// <summary>
        ///     Do rearch of block element.
        /// </summary>
        /// <param name="elementToSearch"></param>
        private void DoTransactionResearch(string elementToSearch)
        {
            var elementIdFound = 0;
            var elementFound = false;

            var useContactName = false;
            var useWalletAddress = false;

            if (elementToSearch.Length >= ClassConnectorSetting.MinWalletAddressSize &&
                elementToSearch.Length <= ClassConnectorSetting.MaxWalletAddressSize)
            {
#if WINDOWS
                if (ClassFormPhase.MessageBoxInterface("Do you want to research by wallet address?",
                        "Research mode", MessageBoxButtons.YesNo, MessageBoxIcon.Information) ==
                    DialogResult.Yes) useWalletAddress = true;
#endif
#if LINUX
                    if (MessageBox.Show(this, "Do you want to research by wallet address?", "Research mode", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                    {
                        useWalletAddress = true;
                    }
#endif
                if (!useWalletAddress)
                    if (elementToSearch.Length < MinSizeTransactionHash)
                    {
#if WINDOWS
                        if (ClassFormPhase.MessageBoxInterface("Do you want to research by contact name?",
                                "Research mode", MessageBoxButtons.YesNo, MessageBoxIcon.Information) ==
                            DialogResult.Yes) useContactName = true;
#endif
#if LINUX
                            if (MessageBox.Show(this, "Do you want to research by contact name?", "Research mode", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                            {
                                useContactName = true;
                            }
#endif
                    }
            }
            else
            {
                if (elementToSearch.Length < MinSizeTransactionHash)
                {
#if WINDOWS
                    if (ClassFormPhase.MessageBoxInterface("Do you want to research by contact name?",
                            "Research mode", MessageBoxButtons.YesNo, MessageBoxIcon.Information) ==
                        DialogResult.Yes) useContactName = true;
#endif
#if LINUX
                        if (MessageBox.Show(this, "Do you want to research by contact name?", "Research mode", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                        {
                            useContactName = true;
                        }
#endif
                }
            }

            MethodInvoker invokeResearch = () =>
            {
                try
                {
                    if (!useContactName)
                    {
                        if (!useWalletAddress) // Usually when the user target a transaction hash.
                        {
                            decimal transactionSendNormalCounter = 0;
                            decimal transactionRecvNormalCounter = 0;
                            decimal transactionSendAnonymousCounter = 0;
                            decimal transactionBlockRewardCounter = 0;
                            decimal transactionRecvAnonymousCounter = 0;


                            foreach (var transactionObject in ClassWalletTransactionCache.ListTransaction.ToArray())
                                if (!elementFound)
                                {
                                    switch (transactionObject.Value.TransactionType)
                                    {
                                        case ClassWalletTransactionType.SendTransaction:
                                            transactionSendNormalCounter++;
                                            break;
                                        case ClassWalletTransactionType.ReceiveTransaction:
                                            if (transactionObject.Value.TransactionWalletAddress.Contains(ClassWalletTransactionType.BlockchainTransaction))
                                                transactionBlockRewardCounter++;
                                            else if (transactionObject.Value.TransactionWalletAddress == ClassWalletTransactionType.AnonymousTransaction)
                                                transactionRecvAnonymousCounter++;
                                            else
                                                transactionRecvNormalCounter++;
                                            break;
                                    }

                                    if (transactionObject.Value.TransactionHash == elementToSearch)
                                        elementFound = true;
                                    else
                                        elementIdFound++;
                                }

                            if (elementFound)
                            {
                                var walletResearchElementForm = new SearchWalletExplorer
                                {
                                    StartPosition = FormStartPosition.CenterParent
                                };

                                var transactionObject =
                                    ClassWalletTransactionCache.ListTransaction.ElementAt(elementIdFound);
                                decimal pageNumber = 1;


                                switch (transactionObject.Value.TransactionType)
                                {
                                    case ClassWalletTransactionType.SendTransaction:
                                        var totalPage =
                                            Math.Ceiling((decimal)(TotalTransactionNormalSend / MaxTransactionPerPage)) +
                                            1;

                                        var target = TotalTransactionNormalSend - transactionSendNormalCounter;
                                        for (var i = 0; i < totalPage; i++)
                                            if (target >= i * MaxTransactionPerPage &&
                                                target <= (i + 1) * MaxTransactionPerPage)
                                            {
#if DEBUG
                                            Console.WriteLine(target + " between: "+ (i * MaxTransactionPerPage) + "/" + ((i + 1) * MaxTransactionPerPage));
#endif
                                                pageNumber = i + 1;
                                            }

                                        break;
                                    case ClassWalletTransactionType.ReceiveTransaction:
                                        if (transactionObject.Value.TransactionWalletAddress.Contains(ClassWalletTransactionType.BlockchainTransaction))
                                        {
                                            totalPage = Math.Ceiling(
                                                            (decimal)(TotalTransactionBlockReward /
                                                                       MaxTransactionPerPage)) + 1;

                                            target = TotalTransactionBlockReward - transactionBlockRewardCounter;
                                            for (var i = 0; i < totalPage; i++)
                                                if (target >= i * MaxTransactionPerPage &&
                                                    target <= (i + 1) * MaxTransactionPerPage)
                                                {
#if DEBUG
                                                Console.WriteLine(target + " between: "+ (i * MaxTransactionPerPage) + "/" + ((i + 1) * MaxTransactionPerPage));
#endif
                                                    pageNumber = i + 1;
                                                }
                                        }
                                        else if (transactionObject.Value.TransactionWalletAddress == ClassWalletTransactionType.AnonymousTransaction)
                                        {
                                            totalPage = Math.Ceiling(
                                                            (decimal)(TotalTransactionAnonymousReceived /
                                                                       MaxTransactionPerPage)) + 1;

                                            target = TotalTransactionAnonymousReceived - transactionRecvAnonymousCounter;
                                            for (var i = 0; i < totalPage; i++)
                                                if (target >= i * MaxTransactionPerPage &&
                                                    target <= (i + 1) * MaxTransactionPerPage)
                                                {
#if DEBUG
                                                Console.WriteLine(target + " between: "+ (i * MaxTransactionPerPage) + "/" + ((i + 1) * MaxTransactionPerPage));
#endif
                                                    pageNumber = i + 1;
                                                }
                                        }
                                        else
                                        {
                                            totalPage = Math.Ceiling(
                                                            (decimal)(TotalTransactionNormalReceived /
                                                                       MaxTransactionPerPage)) + 1;

                                            target = TotalTransactionNormalReceived - transactionRecvNormalCounter;
                                            for (var i = 0; i < totalPage; i++)
                                                if (target >= i * MaxTransactionPerPage &&
                                                    target <= (i + 1) * MaxTransactionPerPage)
                                                {
#if DEBUG
                                                Console.WriteLine(target + " between: "+ (i * MaxTransactionPerPage) + "/" + ((i + 1) * MaxTransactionPerPage));
#endif
                                                    pageNumber = i + 1;
                                                }
                                        }

                                        break;
                                }

                                if (pageNumber < 1) pageNumber = 1;


                                walletResearchElementForm.AppendText(transactionObject.Value.ConcatTransactionElement(pageNumber.ToString()));
                                walletResearchElementForm.ShowDialog(this);

#if DEBUG
                            Log.WriteLine("Element found: " + ClassBlockCache.ListBlock.ElementAt(elementIdFound).Value.ConcatBlockElement());
#endif
                            }
                            else
                            {

                                elementIdFound = 0;
                                foreach (var transactionObject in ClassWalletTransactionAnonymityCache.ListTransaction
                                    .ToArray())
                                    if (!elementFound)
                                    {
                                        transactionSendAnonymousCounter++;
                                        if (transactionObject.Value.TransactionHash.Contains(elementToSearch))
                                            elementFound = true;
                                        else
                                            elementIdFound++;
                                    }


                                if (elementFound)
                                {
                                    var transactionObject =
                                        ClassWalletTransactionAnonymityCache.ListTransaction.ElementAt(elementIdFound);
                                    decimal pageNumber = 0;

                                    var totalPage =
                                        Math.Ceiling((decimal)(TotalTransactionAnonymousSend / MaxTransactionPerPage)) + 1;

                                    var target = TotalTransactionAnonymousSend - transactionSendAnonymousCounter;
                                    for (var i = 0; i < totalPage; i++)
                                        if (target >= i * MaxTransactionPerPage &&
                                            target <= (i + 1) * MaxTransactionPerPage)
                                        {
#if DEBUG
                                        Console.WriteLine(target + " between: "+ (i * MaxTransactionPerPage) + "/" + ((i + 1) * MaxTransactionPerPage));
#endif
                                            pageNumber = i + 1;
                                        }

                                    if (pageNumber < 1) pageNumber = 1;
                                    var walletResearchElementForm = new SearchWalletExplorer
                                    {
                                        StartPosition = FormStartPosition.CenterParent
                                    };
                                    walletResearchElementForm.AppendText(transactionObject.Value.ConcatTransactionElement(pageNumber.ToString()));
                                    walletResearchElementForm.ShowDialog(this);

#if DEBUG
                            Log.WriteLine("Element found: " + ClassBlockCache.ListBlock.ElementAt(elementIdFound).Value.ConcatBlockElement());
#endif
                                }
                                else
                                {
#if WINDOWS
                                    ClassFormPhase.MessageBoxInterface(elementToSearch + " not found.", "Not found",
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
#endif
#if LINUX
                                MessageBox.Show(this, elementToSearch + " not found.", "Not found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
#endif
                                }
                            }
                        }
                        else // Search by wallet address
                        {
                            decimal transactionSendNormalCounter = 0;
                            decimal transactionRecvNormalCounter = 0;
                            decimal transactionSendAnonymousCounter = 0;
                            decimal transactionBlockRewardCounter = 0;
                            decimal transactionRecvAnonymousCounter = 0;

                            var walletAddressTransactionFound = false;
                            var walletResearchElementForm = new SearchWalletExplorer
                            {
                                StartPosition = FormStartPosition.CenterParent
                            };


                            foreach (var transactionObject in ClassWalletTransactionCache.ListTransaction.ToArray())
                            {
                                switch (transactionObject.Value.TransactionType)
                                {
                                    case ClassWalletTransactionType.SendTransaction:
                                        transactionSendNormalCounter++;
                                        break;
                                    case ClassWalletTransactionType.ReceiveTransaction:
                                        if (transactionObject.Value.TransactionWalletAddress.Contains(ClassWalletTransactionType.BlockchainTransaction))
                                            transactionBlockRewardCounter++;
                                        else if (transactionObject.Value.TransactionWalletAddress == ClassWalletTransactionType.AnonymousTransaction)
                                            transactionRecvAnonymousCounter++;
                                        else
                                            transactionRecvNormalCounter++;
                                        break;
                                }

                                if (transactionObject.Value.TransactionWalletAddress == elementToSearch)
                                {
                                    walletAddressTransactionFound = true;
                                    decimal pageNumber = 1;

                                    switch (transactionObject.Value.TransactionType)
                                    {
                                        case ClassWalletTransactionType.SendTransaction:
                                            var totalPage =
                                                Math.Ceiling(
                                                    (decimal)(TotalTransactionNormalSend / MaxTransactionPerPage)) + 1;

                                            var target = TotalTransactionNormalSend - transactionSendNormalCounter;
                                            for (var i = 0; i < totalPage; i++)
                                                if (target >= i * MaxTransactionPerPage &&
                                                    target <= (i + 1) * MaxTransactionPerPage)
                                                {
#if DEBUG
                                            Console.WriteLine(target + " between: "+ (i * MaxTransactionPerPage) + "/" + ((i + 1) * MaxTransactionPerPage));
#endif
                                                    pageNumber = i + 1;
                                                }

                                            break;
                                        case ClassWalletTransactionType.ReceiveTransaction:
                                            if (transactionObject.Value.TransactionWalletAddress.Contains(ClassWalletTransactionType.BlockchainTransaction))
                                            {
                                                totalPage = Math.Ceiling(
                                                                (decimal)(TotalTransactionBlockReward /
                                                                           MaxTransactionPerPage)) + 1;

                                                target = TotalTransactionBlockReward - transactionBlockRewardCounter;
                                                for (var i = 0; i < totalPage; i++)
                                                    if (target >= i * MaxTransactionPerPage &&
                                                        target <= (i + 1) * MaxTransactionPerPage)
                                                    {
#if DEBUG
                                                Console.WriteLine(target + " between: "+ (i * MaxTransactionPerPage) + "/" + ((i + 1) * MaxTransactionPerPage));
#endif
                                                        pageNumber = i + 1;
                                                    }
                                            }
                                            else if (transactionObject.Value.TransactionWalletAddress == ClassWalletTransactionType.AnonymousTransaction)
                                            {
                                                totalPage = Math.Ceiling(
                                                                (decimal)(TotalTransactionAnonymousReceived /
                                                                           MaxTransactionPerPage)) + 1;

                                                target = TotalTransactionAnonymousReceived -
                                                         transactionRecvAnonymousCounter;
                                                for (var i = 0; i < totalPage; i++)
                                                    if (target >= i * MaxTransactionPerPage &&
                                                        target <= (i + 1) * MaxTransactionPerPage)
                                                    {
#if DEBUG
                                                Console.WriteLine(target + " between: "+ (i * MaxTransactionPerPage) + "/" + ((i + 1) * MaxTransactionPerPage));
#endif
                                                        pageNumber = i + 1;
                                                    }
                                            }
                                            else
                                            {
                                                totalPage = Math.Ceiling(
                                                                (decimal)(TotalTransactionNormalReceived /
                                                                           MaxTransactionPerPage)) + 1;

                                                target = TotalTransactionNormalReceived - transactionRecvNormalCounter;
                                                for (var i = 0; i < totalPage; i++)
                                                    if (target >= i * MaxTransactionPerPage &&
                                                        target <= (i + 1) * MaxTransactionPerPage)
                                                    {
#if DEBUG
                                                Console.WriteLine(target + " between: "+ (i * MaxTransactionPerPage) + "/" + ((i + 1) * MaxTransactionPerPage));
#endif
                                                        pageNumber = i + 1;
                                                    }
                                            }

                                            break;
                                    }

                                    if (pageNumber < 1) pageNumber = 1;
                                    walletResearchElementForm.AppendText(
                                        transactionObject.Value.ConcatTransactionElement(pageNumber.ToString()));
                                }
                            }


                            foreach (var transactionObject in ClassWalletTransactionAnonymityCache.ListTransaction.ToArray()
                            )
                            {
                                TotalTransactionAnonymousSend++;

                                if (transactionObject.Value.TransactionWalletAddress == elementToSearch)
                                {
                                    walletAddressTransactionFound = true;
                                    decimal pageNumber = 0;

                                    var totalPage =
                                        Math.Ceiling((decimal)(TotalTransactionAnonymousSend / MaxTransactionPerPage)) + 1;

                                    var target = TotalTransactionAnonymousSend - transactionSendAnonymousCounter;
                                    for (var i = 0; i < totalPage; i++)
                                        if (target >= i * MaxTransactionPerPage &&
                                            target <= (i + 1) * MaxTransactionPerPage)
                                        {
#if DEBUG
                                        Console.WriteLine(target + " between: "+ (i * MaxTransactionPerPage) + "/" + ((i + 1) * MaxTransactionPerPage));
#endif
                                            pageNumber = i + 1;
                                        }

                                    if (pageNumber < 1) pageNumber = 1;
                                    walletResearchElementForm.AppendText(
                                        transactionObject.Value.ConcatTransactionElement(pageNumber.ToString()));
                                }
                            }



                            if (!walletAddressTransactionFound)
                            {
#if WINDOWS
                                ClassFormPhase.MessageBoxInterface(elementToSearch + " not found.", "Not found",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
#endif
#if LINUX
                            MessageBox.Show(this, elementToSearch + " not found.", "Not found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
#endif
                                walletResearchElementForm.Dispose();
                            }
                            else
                            {
                                walletResearchElementForm.ShowDialog(this);
                            }
                        }
                    }
                    else // Search by contact name
                    {
                        if (ClassContact.ListContactWallet.ContainsKey(elementToSearch.ToLower()))
                        {
                            decimal transactionSendNormalCounter = 0;
                            decimal transactionRecvNormalCounter = 0;
                            decimal transactionSendAnonymousCounter = 0;
                            decimal transactionBlockRewardCounter = 0;
                            decimal transactionRecvAnonymousCounter = 0;

                            var contactTransactionFound = false;
                            var walletAddressFromContactName =
                                ClassContact.GetWalletAddressFromContactName(elementToSearch);
                            var walletResearchElementForm = new SearchWalletExplorer
                            {
                                StartPosition = FormStartPosition.CenterParent
                            };


                            foreach (var transactionObject in ClassWalletTransactionCache.ListTransaction.ToArray())
                            {
                                switch (transactionObject.Value.TransactionType)
                                {
                                    case ClassWalletTransactionType.SendTransaction:
                                        transactionSendNormalCounter++;
                                        break;
                                    case ClassWalletTransactionType.ReceiveTransaction:
                                        if (transactionObject.Value.TransactionWalletAddress.Contains(
                                            ClassWalletTransactionType.BlockchainTransaction))
                                            transactionBlockRewardCounter++;
                                        else if (transactionObject.Value.TransactionWalletAddress ==
                                                 ClassWalletTransactionType.AnonymousTransaction)
                                            transactionRecvAnonymousCounter++;
                                        else
                                            transactionRecvNormalCounter++;
                                        break;
                                }

                                if (transactionObject.Value.TransactionWalletAddress == walletAddressFromContactName)
                                {
                                    contactTransactionFound = true;
                                    decimal pageNumber = 1;

                                    switch (transactionObject.Value.TransactionType)
                                    {
                                        case ClassWalletTransactionType.SendTransaction:
                                            var totalPage =
                                                Math.Ceiling(
                                                    (decimal)(TotalTransactionNormalSend / MaxTransactionPerPage)) + 1;

                                            var target = TotalTransactionNormalSend - transactionSendNormalCounter;
                                            for (var i = 0; i < totalPage; i++)
                                                if (target >= i * MaxTransactionPerPage &&
                                                    target <= (i + 1) * MaxTransactionPerPage)
                                                {
#if DEBUG
                                            Console.WriteLine(target + " between: "+ (i * MaxTransactionPerPage) + "/" + ((i + 1) * MaxTransactionPerPage));
#endif
                                                    pageNumber = i + 1;
                                                }

                                            break;
                                        case ClassWalletTransactionType.ReceiveTransaction:
                                            if (transactionObject.Value.TransactionWalletAddress.Contains(
                                                ClassWalletTransactionType.BlockchainTransaction))
                                            {
                                                totalPage = Math.Ceiling(
                                                                (decimal)(TotalTransactionBlockReward /
                                                                           MaxTransactionPerPage)) + 1;

                                                target = TotalTransactionBlockReward - transactionBlockRewardCounter;
                                                for (var i = 0; i < totalPage; i++)
                                                    if (target >= i * MaxTransactionPerPage &&
                                                        target <= (i + 1) * MaxTransactionPerPage)
                                                    {
#if DEBUG
                                                Console.WriteLine(target + " between: "+ (i * MaxTransactionPerPage) + "/" + ((i + 1) * MaxTransactionPerPage));
#endif
                                                        pageNumber = i + 1;
                                                    }
                                            }
                                            else if (transactionObject.Value.TransactionWalletAddress ==
                                                     ClassWalletTransactionType.AnonymousTransaction)
                                            {
                                                totalPage = Math.Ceiling(
                                                                (decimal)(TotalTransactionAnonymousReceived /
                                                                           MaxTransactionPerPage)) + 1;

                                                target = TotalTransactionAnonymousReceived -
                                                         transactionRecvAnonymousCounter;
                                                for (var i = 0; i < totalPage; i++)
                                                    if (target >= i * MaxTransactionPerPage &&
                                                        target <= (i + 1) * MaxTransactionPerPage)
                                                    {
#if DEBUG
                                                Console.WriteLine(target + " between: "+ (i * MaxTransactionPerPage) + "/" + ((i + 1) * MaxTransactionPerPage));
#endif
                                                        pageNumber = i + 1;
                                                    }
                                            }
                                            else
                                            {
                                                totalPage = Math.Ceiling(
                                                                (decimal)(TotalTransactionNormalReceived /
                                                                           MaxTransactionPerPage)) + 1;

                                                target = TotalTransactionNormalReceived - transactionRecvNormalCounter;
                                                for (var i = 0; i < totalPage; i++)
                                                    if (target >= i * MaxTransactionPerPage &&
                                                        target <= (i + 1) * MaxTransactionPerPage)
                                                    {
#if DEBUG
                                                Console.WriteLine(target + " between: "+ (i * MaxTransactionPerPage) + "/" + ((i + 1) * MaxTransactionPerPage));
#endif
                                                        pageNumber = i + 1;
                                                    }
                                            }

                                            break;
                                    }

                                    if (pageNumber < 1) pageNumber = 1;
                                    walletResearchElementForm.AppendText(
                                        transactionObject.Value.ConcatTransactionElement(pageNumber.ToString()));
                                }
                            }

                            foreach (var transactionObject in ClassWalletTransactionAnonymityCache.ListTransaction.ToArray()
                            )
                            {
                                TotalTransactionAnonymousSend++;
                                if (transactionObject.Value.TransactionWalletAddress == walletAddressFromContactName)
                                {
                                    contactTransactionFound = true;
                                    decimal pageNumber = 0;

                                    var totalPage =
                                        Math.Ceiling((decimal)(TotalTransactionAnonymousSend / MaxTransactionPerPage)) + 1;

                                    var target = TotalTransactionAnonymousSend - transactionSendAnonymousCounter;
                                    for (var i = 0; i < totalPage; i++)
                                        if (target >= i * MaxTransactionPerPage &&
                                            target <= (i + 1) * MaxTransactionPerPage)
                                        {
#if DEBUG
                                        Console.WriteLine(target + " between: "+ (i * MaxTransactionPerPage) + "/" + ((i + 1) * MaxTransactionPerPage));
#endif
                                            pageNumber = i + 1;
                                        }

                                    if (pageNumber < 1) pageNumber = 1;
                                    walletResearchElementForm.AppendText(
                                        transactionObject.Value.ConcatTransactionElement(pageNumber.ToString()));
                                }
                            }
                            if (!contactTransactionFound)
                            {
#if WINDOWS
                                ClassFormPhase.MessageBoxInterface(elementToSearch + " not found.", "Not found",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
#endif
#if LINUX
                            MessageBox.Show(this, elementToSearch + " not found.", "Not found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
#endif
                                walletResearchElementForm.Dispose();
                            }
                            else
                            {
                                walletResearchElementForm.ShowDialog(this);
                            }
                        }
                        else
                        {

#if WINDOWS
                            ClassFormPhase.MessageBoxInterface(elementToSearch + " not found.", "Not found",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
#endif
#if LINUX
                        MessageBox.Show(this, elementToSearch + " not found.", "Not found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
#endif
                        }
                    }

                    UpdateStyles();

                }
                catch
                {
                    // Ignored.
                }
            };
            BeginInvoke(invokeResearch);

        }

        /// <summary>
        ///     Export every tx synced into a CSV file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void transactionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ClassWalletObject.WalletConnect != null)
            {
                if (!EnableTokenNetworkMode)
                {
                    if (!ClassWalletObject.WalletClosed) SaveTransactionIntoCsv();
                }
                else
                {
                    if (ClassWalletObject.SeedNodeConnectorWallet != null)
                    {
                        if (ClassWalletObject.SeedNodeConnectorWallet.ReturnStatus() &&
                            ClassWalletObject.WalletConnect.WalletPhase != ClassWalletPhase.Create &&
                            ClassWalletObject.WalletConnect.WalletPhase != ClassWalletPhase.Restore)
                        {
                            SaveTransactionIntoCsv();
                        }
                        else
                        {
#if WINDOWS
                            ClassFormPhase.MessageBoxInterface(
                                "You need to be connected to your wallet for export transaction.", "Export System",
                                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
#endif
#if LINUX
                        MessageBox.Show(this, "You need to be connected to your wallet for export transaction.", "Export System", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
#endif
                        }
                    }
                    else
                    {
#if WINDOWS
                        ClassFormPhase.MessageBoxInterface(
                            "You need to be connected to your wallet for export transaction.", "Export System",
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
#endif
#if LINUX
                    MessageBox.Show(this, "You need to be connected to your wallet for export transaction.", "Export System", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
#endif
                    }
                }
            }
            else
            {
#if WINDOWS
                ClassFormPhase.MessageBoxInterface("You need to be connected to your wallet for export transaction.",
                    "Export System", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
#endif
#if LINUX
                MessageBox.Show(this, "You need to be connected to your wallet for export transaction.", "Export System", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
#endif
            }
        }

        /// <summary>
        ///     Save every transactions synced into a CSV file.
        /// </summary>
        private void SaveTransactionIntoCsv()
        {
            var csvPath = string.Empty;
            using (var saveFileDialogWallet = new SaveFileDialog
            {
                InitialDirectory = AppDomain.CurrentDomain.BaseDirectory,
                Filter = @"CSV File (*.csv) | *.csv",
                FilterIndex = 2,
                RestoreDirectory = true,
                FileName = "transaction_" + DateTimeOffset.Now.ToUnixTimeSeconds() + ".csv"
            })
            {
                if (saveFileDialogWallet.ShowDialog() == DialogResult.OK)
                    if (saveFileDialogWallet.FileName != "")
                    {
                        csvPath = saveFileDialogWallet.FileName;



                        if (ClassWalletTransactionCache.ListTransaction != null &&
                            ClassWalletTransactionAnonymityCache.ListTransaction != null)
                        {
                            var totalNormalTransaction = ClassWalletTransactionCache.ListTransaction.Count;

                            var totalAnonymousTransaction = ClassWalletTransactionAnonymityCache.ListTransaction.Count;

                            var totalTransaction = totalNormalTransaction + totalAnonymousTransaction;

                            if (totalTransaction > 0)
                            {
                                var startExport = false;

#if WINDOWS
                                if (ClassFormPhase.MessageBoxInterface(
                                        "Do you want to export " + totalTransaction + " transaction(s)?",
                                        "Export Transaction", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) ==
                                    DialogResult.Yes) startExport = true;
#endif
#if LINUX
                                            if (MessageBox.Show(this, "Do you want to export " + totalTransaction + " transaction(s)?", "Export Transaction", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                                            {
                                                startExport = true;
                                            }
#endif
                                if (startExport)
                                {
                                    File.Create(csvPath).Close();


                                    using (var writer = new StreamWriter(csvPath))
                                    {
                                        writer.WriteLine(
                                            "\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\"",
                                            ClassTranslation.GetLanguageTextFromOrder(
                                                "TRANSACTION_HISTORY_WALLET_COLUMN_TYPE"),
                                            ClassTranslation.GetLanguageTextFromOrder(
                                                "TRANSACTION_HISTORY_WALLET_COLUMN_HASH"),
                                            ClassTranslation.GetLanguageTextFromOrder(
                                                "TRANSACTION_HISTORY_WALLET_COLUMN_ADDRESS"),
                                            ClassTranslation.GetLanguageTextFromOrder(
                                                "TRANSACTION_HISTORY_WALLET_COLUMN_AMOUNT"),
                                            ClassTranslation.GetLanguageTextFromOrder(
                                                "TRANSACTION_HISTORY_WALLET_COLUMN_FEE"),
                                            ClassTranslation.GetLanguageTextFromOrder(
                                                "TRANSACTION_HISTORY_WALLET_COLUMN_BLOCK_HEIGHT_SRC"),
                                            ClassTranslation.GetLanguageTextFromOrder(
                                                "TRANSACTION_HISTORY_WALLET_COLUMN_DATE"),
                                            ClassTranslation.GetLanguageTextFromOrder(
                                                "TRANSACTION_HISTORY_WALLET_COLUMN_DATE_RECEIVED"));

                                        if (ClassWalletTransactionCache.ListTransaction.Count > 0)
                                        {

                                            for (int i = 0; i < totalNormalTransaction; i++)
                                            {
                                                if (i < ClassWalletTransactionCache.ListTransactionIndex.Count)
                                                {
                                                    if (ClassWalletTransactionCache.ListTransactionIndex.ContainsKey(i))
                                                    {
                                                        if (ClassWalletTransactionCache.ListTransaction.ContainsKey(ClassWalletTransactionCache.ListTransactionIndex[i]))
                                                        {
                                                            ClassWalletTransactionObject transactionObject = ClassWalletTransactionCache.ListTransaction[ClassWalletTransactionCache.ListTransactionIndex[i]];

                                                            if (transactionObject != null)
                                                            {
                                                                var dateTimeSend = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                                                                dateTimeSend = dateTimeSend.AddSeconds(transactionObject.TransactionTimestampSend);
                                                                dateTimeSend = dateTimeSend.ToLocalTime();
                                                                var dateTimeRecv = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                                                                dateTimeRecv = dateTimeRecv.AddSeconds(transactionObject.TransactionTimestampRecv);
                                                                dateTimeRecv = dateTimeRecv.ToLocalTime();

                                                                var transactionResult = string.Format(
                                                                    "\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\"",
                                                                    transactionObject.TransactionType,
                                                                    transactionObject.TransactionHash,
                                                                    transactionObject.TransactionWalletAddress,
                                                                    transactionObject.TransactionAmount,
                                                                    transactionObject.TransactionFee,
                                                                    transactionObject.TransactionBlockchainHeight,
                                                                    dateTimeSend.ToString(CultureInfo.InvariantCulture),
                                                                    dateTimeRecv.ToString(CultureInfo.InvariantCulture)
                                                                );

                                                                writer.WriteLine(transactionResult);
                                                            }
                                                        }
                                                    }
                                                }
                                            }

                                        }

                                        if (ClassWalletTransactionAnonymityCache.ListTransaction.Count > 0)
                                        {
                                            for (int i = 0; i < totalAnonymousTransaction; i++)
                                            {
                                                if (i < ClassWalletTransactionAnonymityCache.ListTransactionIndex.Count)
                                                {
                                                    if (ClassWalletTransactionAnonymityCache.ListTransactionIndex.ContainsKey(i))
                                                    {
                                                        if (ClassWalletTransactionAnonymityCache.ListTransaction.ContainsKey(ClassWalletTransactionAnonymityCache.ListTransactionIndex[i]))
                                                        {
                                                            ClassWalletTransactionObject transactionObject = ClassWalletTransactionAnonymityCache.ListTransaction[ClassWalletTransactionAnonymityCache.ListTransactionIndex[i]];

                                                            if (transactionObject != null)
                                                            {
                                                                var dateTimeSend = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                                                                dateTimeSend = dateTimeSend.AddSeconds(transactionObject.TransactionTimestampSend);
                                                                dateTimeSend = dateTimeSend.ToLocalTime();
                                                                var dateTimeRecv = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                                                                dateTimeRecv = dateTimeRecv.AddSeconds(transactionObject.TransactionTimestampRecv);
                                                                dateTimeRecv = dateTimeRecv.ToLocalTime();

                                                                var transactionResult = string.Format(
                                                                    "\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\"",
                                                                    transactionObject.TransactionType,
                                                                    transactionObject.TransactionHash,
                                                                    transactionObject.TransactionWalletAddress,
                                                                    transactionObject.TransactionAmount,
                                                                    transactionObject.TransactionFee,
                                                                    transactionObject.TransactionBlockchainHeight,
                                                                    dateTimeSend.ToString(CultureInfo.InvariantCulture),
                                                                    dateTimeRecv.ToString(CultureInfo.InvariantCulture)
                                                                );

                                                                writer.WriteLine(transactionResult);
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }

#if WINDOWS
                                    ClassFormPhase.MessageBoxInterface("Export Complete.", "Export System",
                                        MessageBoxButtons.OK, MessageBoxIcon.Information);
#endif
#if LINUX
                                                MessageBox.Show(this, "Export Complete.", "Export System", MessageBoxButtons.OK, MessageBoxIcon.Information);
#endif
                                }
                            }
                            else
                            {
#if WINDOWS
                                ClassFormPhase.MessageBoxInterface("Their is no transaction to export",
                                    "Export Transaction", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
#endif
#if LINUX
                                            MessageBox.Show(this, "Their is no transaction to export", "Export Transaction", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
#endif
                            }
                        }
                        else
                        {
#if WINDOWS
                            ClassFormPhase.MessageBoxInterface("Their is no transaction to export",
                                "Export Transaction", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
#endif
#if LINUX
                                        MessageBox.Show(this, "Their is no transaction to export", "Export Transaction", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
#endif
                        }
                    }
            }
        }
    }
}