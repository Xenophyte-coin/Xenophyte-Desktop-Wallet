using System;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using Xenophyte_Connector_All.Setting;
using Xenophyte_Wallet.Debug;
using Xenophyte_Wallet.Features;
using Xenophyte_Wallet.FormCustom;
using Xenophyte_Wallet.Utility;
using Xenophyte_Wallet.Wallet.Sync;

namespace Xenophyte_Wallet.FormPhase.MainForm
{
    public sealed partial class BlockExplorerWallet : Form
    {
        public Label _labelWaitingText = new Label();
        private ClassPanel _panelWaitingSync;
        public bool IsShowed;
        public bool IsShowedWaitingBlock;

        public BlockExplorerWallet()
        {
            InitializeComponent();
            SetStyle(
                ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw |
                ControlStyles.OptimizedDoubleBuffer, true);
            DoubleBuffered = true;
            AutoScroll = true;
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
            if (Program.WalletXenophyte.ListControlSizeBlock.Count == 0)
                for (var i = 0; i < Controls.Count; i++)
                    if (i < Controls.Count)
                        Program.WalletXenophyte.ListControlSizeBlock.Add(
                            new Tuple<Size, Point>(Controls[i].Size, Controls[i].Location));
        }

        /// <summary>
        ///     Force to resync blocks.
        /// </summary>
        public async void ResyncBlock()
        {
            if (ClassBlockCache.RemoveWalletBlockCache())
            {
                MethodInvoker invoke = () =>
                {
                    try
                    {
                        listViewBlockExplorer.Items.Clear();
                    }
                    catch
                    {
                    }
                };
                Program.WalletXenophyte.BeginInvoke(invoke);
                try
                {
                    Program.WalletXenophyte.ListBlockHashShowed.Clear();
                }
                catch
                {
                }


                await Program.WalletXenophyte.ClassWalletObject.DisconnectRemoteNodeTokenSync();
                Program.WalletXenophyte.ClassWalletObject.WalletOnUseSync = false;


                Program.WalletXenophyte.StopUpdateBlockHistory(true, false);
            }
        }

        private void Block_Load(object sender, EventArgs e)
        {
            IsShowed = true;
            UpdateStyles();
            listViewBlockExplorer.ListViewItemSorter = new ListViewComparer(0, SortOrder.Descending);


            _panelWaitingSync = new ClassPanel
            {
                Width = (int)(Width / 1.5f),
                Height = (int)(Height / 5.5f),
                BackColor = Color.LightBlue
            };
            _panelWaitingSync.Location = new Point
            {
                X = Program.WalletXenophyte.BlockWalletForm.Width / 2 - _panelWaitingSync.Width / 2,
                Y = Program.WalletXenophyte.BlockWalletForm.Height / 2 - _panelWaitingSync.Height / 2
            };

            _labelWaitingText.AutoSize = true;
            _labelWaitingText.Font = new Font(_labelWaitingText.Font.FontFamily, 9f, FontStyle.Bold);
            _labelWaitingText.Text = ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration.waitingmenulabeltext);
            _panelWaitingSync.Controls.Add(_labelWaitingText);
            _labelWaitingText.Location = new Point
            {
                X = _panelWaitingSync.Width / 2 - _labelWaitingText.Width / 2,
                Y = _panelWaitingSync.Height / 2 - _labelWaitingText.Height / 2
            };
            _labelWaitingText.Show();
            Controls.Add(_panelWaitingSync);
            _panelWaitingSync.Show();
            IsShowed = true;
            Program.WalletXenophyte.ResizeWalletInterface();
        }

        private void Block_Resize(object sender, EventArgs e)
        {
            UpdateStyles();
        }

        protected override void OnResize(EventArgs e)
        {
            if (_panelWaitingSync != null)
            {
                _panelWaitingSync.Width = (int)(Width / 1.5f);
                _panelWaitingSync.Height = (int)(Height / 5.5f);
                _panelWaitingSync.Location = new Point
                {
                    X = Program.WalletXenophyte.BlockWalletForm.Width / 2 -
                        _panelWaitingSync.Width / 2,
                    Y = Program.WalletXenophyte.BlockWalletForm.Height / 2 -
                        _panelWaitingSync.Height / 2
                };
                _labelWaitingText.Location = new Point
                {
                    X = _panelWaitingSync.Width / 2 - _labelWaitingText.Width / 2,
                    Y = _panelWaitingSync.Height / 2 - _labelWaitingText.Height / 2
                };
                UpdateStyles();
            }

            base.OnResize(e);
        }

        public void ShowWaitingSyncBlockPanel()
        {
            if (!IsShowedWaitingBlock)
                Program.WalletXenophyte.Invoke((MethodInvoker)delegate
               {
                   _panelWaitingSync.Visible = true;
                   _panelWaitingSync.Show();
                   _panelWaitingSync.BringToFront();
                   _panelWaitingSync.Width = (int)(Width / 1.5f);
                   _panelWaitingSync.Height = (int)(Height / 5.5f);
                   _panelWaitingSync.Location = new Point
                   {
                       X = Program.WalletXenophyte.BlockWalletForm.Width / 2 -
                           _panelWaitingSync.Width / 2,
                       Y = Program.WalletXenophyte.BlockWalletForm.Height / 2 -
                           _panelWaitingSync.Height / 2
                   };
                   _labelWaitingText.Location = new Point
                   {
                       X = _panelWaitingSync.Width / 2 - _labelWaitingText.Width / 2,
                       Y = _panelWaitingSync.Height / 2 - _labelWaitingText.Height / 2
                   };
                   IsShowedWaitingBlock = true;
                   UpdateStyles();
               });
        }

        public void HideWaitingSyncBlockPanel()
        {
            if (IsShowedWaitingBlock)
                Program.WalletXenophyte.Invoke((MethodInvoker)delegate
               {
                   _panelWaitingSync.Visible = false;
                   _panelWaitingSync.Hide();
                   IsShowedWaitingBlock = false;
                   UpdateStyles();
               });
        }


        private void listViewBlockExplorer_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                var item = listViewBlockExplorer.GetItemAt(0, e.Y);

                var found = false;
                if (item == null) return;
                for (var ix = item.SubItems.Count - 1; ix >= 0; --ix)
                    if (item.SubItems[ix].Bounds.Contains(e.Location))
                        if (!found)
                        {
                            found = true;
                            Clipboard.SetText(item.SubItems[ix].Text);
#if WINDOWS
                            Task.Factory.StartNew(() =>
                                    ClassFormPhase.MessageBoxInterface(
                                        item.SubItems[ix].Text + " " +
                                        ClassTranslation.GetLanguageTextFromOrder(
                                            ClassTranslationEnumeration.transactionhistorywalletcopytext),
                                        string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Information))
                                .ConfigureAwait(false);
#else
                            LinuxClipboard.SetText(item.SubItems[ix].Text);
                            Task.Factory.StartNew(() =>
                            {
                                MethodInvoker invoker =
 () => MessageBox.Show(Program.WalletXenophyte, item.SubItems[ix].Text + " " + ClassTranslation.GetLanguageTextFromOrder(ClassTranslationEnumeration.transactionhistorywalletcopytext));
                                BeginInvoke(invoker);
                            }).ConfigureAwait(false);
#endif
                            return;
                        }
            }
            catch
            {
            }
        }


        /// <summary>
        ///     Sort block explorer by block id
        /// </summary>
        public void SortingBlockExplorer()
        {
            MethodInvoker invoke = () => listViewBlockExplorer.Sort();
            listViewBlockExplorer.BeginInvoke(invoke);
        }

        private void timerShowBlockExplorer_Tick(object sender, EventArgs e)
        {
            try
            {
                StartUpdateBlockSync(Program.WalletXenophyte);
            }
            catch
            {

            }
        }

        /// <summary>
        ///     Start update block sync.
        /// </summary>
        public void StartUpdateBlockSync(WalletXenophyte walletXenophyte)
        {

            if (!ClassBlockCache.OnLoad)
            {
                try
                {
                    if (!walletXenophyte.ClassWalletObject.InSyncBlock || ClassBlockCache.ListBlock.Count >= walletXenophyte.ClassWalletObject.TotalBlockInSync)
                    {
                        if (this.IsShowed)
                        {
                            this.HideWaitingSyncBlockPanel();

                            var minShow = (walletXenophyte.CurrentBlockExplorerPage - 1) * walletXenophyte.MaxBlockPerPage;
                            var maxShow = walletXenophyte.CurrentBlockExplorerPage * walletXenophyte.MaxBlockPerPage;

                            for (var i = minShow; i < maxShow; i++)
                            {
                                if (i < ClassBlockCache.ListBlockIndex.Count)
                                {
                                    var blockTarget = ClassBlockCache.ListBlock.Count - 1 - i;

                                    if (ClassBlockCache.ListBlockIndex.ContainsKey(blockTarget))
                                    {
                                        if (ClassBlockCache.ListBlock.ContainsKey(ClassBlockCache.ListBlockIndex[blockTarget]))
                                        {
                                            var blockObject = ClassBlockCache.ListBlock[ClassBlockCache.ListBlockIndex[blockTarget]];
                                            var blockId = int.Parse(blockObject.BlockHeight);
                                            if (!walletXenophyte.ListBlockHashShowed.ContainsKey(blockId))
                                            {
                                                walletXenophyte.ListBlockHashShowed.Add(blockId, blockObject.BlockHash);

                                                var dateTimeCreate = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                                                dateTimeCreate =
                                                    dateTimeCreate.AddSeconds(
                                                        int.Parse(blockObject.BlockTimestampCreate));
                                                dateTimeCreate = dateTimeCreate.ToLocalTime();
                                                var dateTimeFound = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                                                dateTimeFound =
                                                    dateTimeFound.AddSeconds(
                                                        int.Parse(blockObject.BlockTimestampFound));
                                                dateTimeFound = dateTimeFound.ToLocalTime();

                                                string[] row =
                                                {
                                                                blockObject.BlockHeight, blockObject.BlockHash,
                                                                blockObject.BlockReward, blockObject.BlockDifficulty,
                                                                dateTimeCreate.ToString(CultureInfo.InvariantCulture),
                                                                dateTimeFound.ToString(CultureInfo.InvariantCulture),
                                                                blockObject.BlockTransactionHash
                                                            };
                                                var listViewItem = new ListViewItem(row);


                                                if (walletXenophyte.TotalBlockRead < maxShow)
                                                {
                                                    void Invoker()
                                                    {
                                                        this.listViewBlockExplorer.Items.Add(
                                                            listViewItem);
                                                    }

                                                    BeginInvoke((MethodInvoker)Invoker);
                                                    walletXenophyte.TotalBlockRead++;
                                                }
                                                else
                                                {
                                                    void Invoker()
                                                    {
                                                        this.listViewBlockExplorer.Items.Insert(
                                                            0,
                                                            listViewItem);
                                                    }

                                                    BeginInvoke((MethodInvoker)Invoker);
                                                    walletXenophyte.TotalBlockRead++;
                                                }
                                            }
                                        }
                                    }
                                }

                                if (ClassFormPhase.FormPhase ==
                                    ClassFormPhaseEnumeration.BlockExplorer)
                                {
                                    if (walletXenophyte.ClassWalletObject.InSyncBlock)
                                        walletXenophyte.UpdateLabelSyncInformation(
                                            "Total blocks synced: " +
                                            ClassBlockCache.ListBlock.Count + "/" + walletXenophyte.ClassWalletObject.TotalBlockInSync + ".");
                                    else
                                        walletXenophyte.UpdateLabelSyncInformation(
                                            "Total blocks loaded: " +
                                            ClassBlockCache.ListBlock.Count +
                                            "/" + walletXenophyte.ClassWalletObject.TotalBlockInSync + ".");
                                }
                            }

                            if (this.IsShowed)
                                if (this.listViewBlockExplorer.Items.Count > 0)
                                    this.SortingBlockExplorer();
                        }
                    }
                    else
                    {
                        if (this.IsShowed) this.ShowWaitingSyncBlockPanel();
                    }
                }
                catch (Exception error)
                {
#if DEBUG
                    Log.WriteLine("Error loading blocks: " + error.Message);
#endif
                    Console.WriteLine("Error loading blocks: " + error.Message);
                }

                try
                {
                    if (ClassFormPhase.FormPhase == ClassFormPhaseEnumeration.BlockExplorer)
                    {
                        if (walletXenophyte.ClassWalletObject.InSyncBlock)
                            walletXenophyte.UpdateLabelSyncInformation(
                                "Total blocks synced: " + ClassBlockCache.ListBlock.Count + "/" + walletXenophyte.ClassWalletObject.TotalBlockInSync + ".");
                        else
                            walletXenophyte.UpdateLabelSyncInformation(
                                "Total blocks loaded: " + ClassBlockCache.ListBlock.Count + "/" + walletXenophyte.ClassWalletObject.TotalBlockInSync + ".");
                    }

                    if (ClassFormPhase.FormPhase == ClassFormPhaseEnumeration.Overview)
                        if (walletXenophyte.ClassWalletObject.InSyncBlock &&
                            !walletXenophyte.ClassWalletObject.InSyncTransactionAnonymity &&
                            !walletXenophyte.ClassWalletObject.InSyncTransaction)
                            try
                            {
                                if (ClassConnectorSetting.SeedNodeIp.ContainsKey(walletXenophyte.ClassWalletObject
                                    .ListWalletConnectToRemoteNode[9].RemoteNodeHost))
                                    walletXenophyte.UpdateLabelSyncInformation(
                                        "Total blocks synced: " + ClassBlockCache.ListBlock.Count +
                                        "/" + walletXenophyte.ClassWalletObject.TotalBlockInSync + " from Seed Node: " + walletXenophyte.ClassWalletObject.ListWalletConnectToRemoteNode[9]
                                            .RemoteNodeHost +
                                        " | " + ClassConnectorSetting
                                            .SeedNodeIp[walletXenophyte.ClassWalletObject.ListWalletConnectToRemoteNode[0]
                                                .RemoteNodeHost].Item1 + ".");
                                else
                                    walletXenophyte.UpdateLabelSyncInformation(
                                        "Total blocks synced: " + ClassBlockCache.ListBlock.Count +
                                        "/" + walletXenophyte.ClassWalletObject.TotalBlockInSync + " from node: " + walletXenophyte.ClassWalletObject.ListWalletConnectToRemoteNode[0]
                                            .RemoteNodeHost +
                                        ".");
                            }
                            catch
                            {
                            }
                }
                catch
                {
                }
            }
        }
    }
}