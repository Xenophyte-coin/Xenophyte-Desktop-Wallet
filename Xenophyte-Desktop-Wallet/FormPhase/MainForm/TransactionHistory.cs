﻿#if WINDOWS
using MetroFramework;
#endif
using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using Xenophyte_Wallet.FormCustom;
using Xenophyte_Wallet.Wallet;

namespace Xenophyte_Wallet.FormPhase.MainForm
{
    public sealed partial class TransactionHistory : Form
    {
        private ClassPanel _panelWaitingSync;
        public bool IsShowed;
        private Label _labelWaitingText = new Label();

        public TransactionHistory()
        {
            InitializeComponent();
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.DoubleBuffer, true);
            DoubleBuffered = true;
            AutoScroll = true;
            IsShowed = false;
        }


        public void ResyncTransaction()
        {
            ClassWalletObject.BlockTransactionSync = true;
            ClassFormPhase.WalletXenophyte.StopUpdateTransactionHistory(true, false);
            ClassWalletTransactionCache.RemoveWalletCache(ClassWalletObject.WalletConnect.WalletAddress);
            ClassWalletTransactionAnonymityCache.RemoveWalletCache(ClassWalletObject.WalletConnect.WalletAddress);
            ClassWalletTransactionCache.ListTransaction.Clear();
            ClassWalletTransactionAnonymityCache.ListTransaction.Clear();
            ClassWalletObject.InSyncTransaction = false;
            ClassWalletObject.InSyncTransactionAnonymity = false;
            ClassWalletObject.BlockTransactionSync = false;
            ClassFormPhase.WalletXenophyte.StartUpdateTransactionHistory();
        }

        public void AutoResizeColumns(ListView lv)
        {
            lv.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            ListView.ColumnHeaderCollection cc = lv.Columns;
            for (int i = 0; i < cc.Count; i++)
            {
                int colWidth = TextRenderer.MeasureText(cc[i].Text, lv.Font).Width + 30;
                if (colWidth > cc[i].Width || cc[i].Text == ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COLUMN_FEE") || cc[i].Text == ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COLUMN_AMOUNT"))
                {
                    cc[i].Width = colWidth + 30;
                }
            }
        }

        private void Transaction_Load(object sender, EventArgs e)
        {

            listViewAnonymityReceivedTransactionHistory.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            listViewAnonymityReceivedTransactionHistory.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);

            listViewAnonymitySendTransactionHistory.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            listViewAnonymitySendTransactionHistory.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);

            listViewBlockRewardTransactionHistory.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            listViewBlockRewardTransactionHistory.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);

            listViewNormalReceivedTransactionHistory.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            listViewNormalReceivedTransactionHistory.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);


            listViewNormalSendTransactionHistory.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            listViewNormalSendTransactionHistory.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);

            listViewAnonymityReceivedTransactionHistory.MultiSelect = true;
            listViewAnonymitySendTransactionHistory.MultiSelect = true;
            listViewBlockRewardTransactionHistory.MultiSelect = true;
            listViewNormalReceivedTransactionHistory.MultiSelect = true;
            listViewNormalSendTransactionHistory.MultiSelect = true;


            _panelWaitingSync = new ClassPanel
            {
                Width = (int) (Width / 1.5f), Height = (int) (Height / 5.5f), BackColor = Color.LightBlue
            };
            _panelWaitingSync.Location = new Point()
            {
                X = ClassFormPhase.WalletXenophyte.TransactionHistoryWalletForm.Width / 2 - _panelWaitingSync.Width / 2,
                Y = ClassFormPhase.WalletXenophyte.TransactionHistoryWalletForm.Height / 2 - _panelWaitingSync.Height / 2
            };

            _labelWaitingText.AutoSize = true;
            _labelWaitingText.Font = new Font(_labelWaitingText.Font.FontFamily, 9f, FontStyle.Bold);
            _labelWaitingText.Text = ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_WAITING_MESSAGE_SYNC_TEXT");
            _panelWaitingSync.Controls.Add(_labelWaitingText);
            _labelWaitingText.Location = new Point()
            {
                X = _panelWaitingSync.Width / 2 - _labelWaitingText.Width / 2,
                Y = _panelWaitingSync.Height / 2 - _labelWaitingText.Height / 2
            };
            _labelWaitingText.Show();
            Controls.Add(_panelWaitingSync);
            _panelWaitingSync.Show();
            IsShowed = true;

            listViewNormalSendTransactionHistory.Show();
            listViewBlockRewardTransactionHistory.Hide();
            listViewAnonymityReceivedTransactionHistory.Hide();
            listViewAnonymitySendTransactionHistory.Hide();
            listViewNormalReceivedTransactionHistory.Hide();
            UpdateStyles();
        }

        public void ShowWaitingSyncTransactionPanel()
        {
            void MethodInvoker()
            {
                _panelWaitingSync.Show();
                _panelWaitingSync.BringToFront();
                _panelWaitingSync.Width = (int) (Width / 1.5f);
                _panelWaitingSync.Height = (int) (Height / 5.5f);
                _panelWaitingSync.Location = new Point()
                {
                    X = ClassFormPhase.WalletXenophyte.TransactionHistoryWalletForm.Width / 2 -
                        _panelWaitingSync.Width / 2,
                    Y = ClassFormPhase.WalletXenophyte.TransactionHistoryWalletForm.Height / 2 -
                        _panelWaitingSync.Height / 2
                };
                _labelWaitingText.Location = new Point()
                {
                    X = _panelWaitingSync.Width / 2 - _labelWaitingText.Width / 2,
                    Y = _panelWaitingSync.Height / 2 - _labelWaitingText.Height / 2
                };
                Refresh();
            }

            BeginInvoke((MethodInvoker) MethodInvoker);
        }

        public void HideWaitingSyncTransactionPanel()
        {
            void MethodInvoker() => _panelWaitingSync.Hide();
            BeginInvoke((MethodInvoker) MethodInvoker);
        }

        protected override void OnResize(EventArgs e)
        {
            if (_panelWaitingSync != null)
            {
                _panelWaitingSync.Width = (int) (Width / 1.5f);
                _panelWaitingSync.Height = (int) (Height / 5.5f);
                _panelWaitingSync.Location = new Point()
                {
                    X = ClassFormPhase.WalletXenophyte.TransactionHistoryWalletForm.Width / 2 -
                        _panelWaitingSync.Width / 2,
                    Y = ClassFormPhase.WalletXenophyte.TransactionHistoryWalletForm.Height / 2 -
                        _panelWaitingSync.Height / 2
                };
                _labelWaitingText.Location = new Point()
                {
                    X = _panelWaitingSync.Width / 2 - _labelWaitingText.Width / 2,
                    Y = _panelWaitingSync.Height / 2 - _labelWaitingText.Height / 2
                };
                Refresh();
            }

            base.OnResize(e);
        }

        public void GetListControl()
        {
            if (ClassFormPhase.WalletXenophyte.ListControlSizeTransaction.Count == 0)
            {
                for (int i = 0; i < Controls.Count; i++)
                {
                    if (i < Controls.Count)
                    {
                        ClassFormPhase.WalletXenophyte.ListControlSizeTransaction.Add(
                            new Tuple<Size, Point>(Controls[i].Size, Controls[i].Location));
                    }
                }
            }

            if (ClassFormPhase.WalletXenophyte.ListControlSizeTransactionTabPage.Count == 0)
            {
                for (int i = 0; i < tabPageTransactionHistory.Controls.Count; i++)
                {
                    if (i < tabPageTransactionHistory.Controls.Count)
                    {
                        ClassFormPhase.WalletXenophyte.ListControlSizeTransactionTabPage.Add(
                            new Tuple<Size, Point>(tabPageTransactionHistory.Controls[i].Size,
                                tabPageTransactionHistory.Controls[i].Location));
                    }
                }
            }

            ClassFormPhase.WalletXenophyte.ResizeWalletInterface();
        }


            private void listViewNormalSendTransactionHistory_MouseClick(object sender, MouseEventArgs e)
        {
            ListViewItem item = listViewNormalSendTransactionHistory.GetItemAt(0, e.Y);

            bool found = false;
            if (item == null) return;
            for (int ix = item.SubItems.Count - 1; ix >= 0; --ix)
                if (item.SubItems[ix].Bounds.Contains(e.Location))
                {
                    if (!found)
                    {
                        found = true;
                        Clipboard.SetText(item.SubItems[ix].Text);
#if WINDOWS
                        new Thread(() =>
                                ClassFormPhase.MessageBoxInterface( item.SubItems[ix].Text + " " + ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COPY_TEXT"), string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Information))
                            .Start();
#else
                        new Thread(delegate ()
                        {
                            MethodInvoker invoker = () => MessageBox.Show(ClassFormPhase.WalletXenophyte, item.SubItems[ix].Text + " " + ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COPY_TEXT"));
                            BeginInvoke(invoker);
                        }).Start();
#endif
                        return;
                    }
                }
        }

        private void listViewNormalReceivedTransactionHistory_MouseClick(object sender, MouseEventArgs e)
        {
            ListViewItem item = listViewNormalReceivedTransactionHistory.GetItemAt(5, e.Y);
            if (item == null) return;
            for (int ix = item.SubItems.Count - 1; ix >= 0; --ix)
                if (item.SubItems[ix].Bounds.Contains(e.Location))
                {
                    Clipboard.SetText(item.SubItems[ix].Text);
#if WINDOWS
                        new Thread(() =>
                                ClassFormPhase.MessageBoxInterface( item.SubItems[ix].Text + " " + ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COPY_TEXT"), string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Information))
                            .Start();
#else
                    new Thread(delegate ()
                    {
                        MethodInvoker invoker = () => MessageBox.Show(ClassFormPhase.WalletXenophyte, item.SubItems[ix].Text + " " + ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COPY_TEXT"));
                        BeginInvoke(invoker);
                    }).Start();
#endif
                    return;
                }
        }

        private void listViewAnonymitySendTransactionHistory_MouseClick(object sender, MouseEventArgs e)
        {
            ListViewItem item = listViewAnonymitySendTransactionHistory.GetItemAt(5, e.Y);
            if (item == null) return;
            for (int ix = item.SubItems.Count - 1; ix >= 0; --ix)
                if (item.SubItems[ix].Bounds.Contains(e.Location))
                {
                    Clipboard.SetText(item.SubItems[ix].Text);
#if WINDOWS
                        new Thread(() =>
                                ClassFormPhase.MessageBoxInterface( item.SubItems[ix].Text + " " + ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COPY_TEXT"), string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Information))
                            .Start();
#else
                    new Thread(delegate()
                    {
                        MethodInvoker invoker = () => MessageBox.Show(ClassFormPhase.WalletXenophyte, item.SubItems[ix].Text + " " + ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COPY_TEXT"));
                        BeginInvoke(invoker);
                    }).Start();
#endif
                    return;
                }
        }

        private void listViewAnonymityReceivedTransactionHistory_MouseClick(object sender, MouseEventArgs e)
        {
            ListViewItem item = listViewAnonymityReceivedTransactionHistory.GetItemAt(5, e.Y);
            if (item == null) return;
            for (int ix = item.SubItems.Count - 1; ix >= 0; --ix)
                if (item.SubItems[ix].Bounds.Contains(e.Location))
                {
                    Clipboard.SetText(item.SubItems[ix].Text);
#if WINDOWS
                        new Thread(() =>
                                ClassFormPhase.MessageBoxInterface( item.SubItems[ix].Text + " " + ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COPY_TEXT"), string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Information))
                            .Start();
#else
                    new Thread(delegate ()
                    {
                        MethodInvoker invoker = () => MessageBox.Show(ClassFormPhase.WalletXenophyte, item.SubItems[ix].Text + " " + ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COPY_TEXT"));
                        BeginInvoke(invoker);
                    }).Start();
#endif
                    return;
                }
        }

        private void listViewBlockRewardTransactionHistory_MouseClick(object sender, MouseEventArgs e)
        {
            ListViewItem item = listViewBlockRewardTransactionHistory.GetItemAt(5, e.Y);
            if (item == null) return;
            for (int ix = item.SubItems.Count - 1; ix >= 0; --ix)
                if (item.SubItems[ix].Bounds.Contains(e.Location))
                {
                    Clipboard.SetText(item.SubItems[ix].Text);
#if WINDOWS
                        new Thread(() =>
                                ClassFormPhase.MessageBoxInterface( item.SubItems[ix].Text + " " + ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COPY_TEXT"), string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error))
                            .Start();
#else
                    new Thread(delegate ()
                    {
                        MethodInvoker invoker = () => MessageBox.Show(ClassFormPhase.WalletXenophyte, item.SubItems[ix].Text + " " + ClassTranslation.GetLanguageTextFromOrder("TRANSACTION_HISTORY_WALLET_COPY_TEXT"));
                        BeginInvoke(invoker);
                    }).Start();
#endif
                    return;
                }
        }

#region Ordering events.

        private ColumnHeader SortingColumn = null;

        private void listViewNormalSendTransactionHistory_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            ColumnHeader new_sorting_column = listViewNormalSendTransactionHistory.Columns[e.Column];

            SortOrder sort_order;
            if (SortingColumn == null)
            {
                sort_order = SortOrder.Ascending;
            }
            else
            {
                if (new_sorting_column == SortingColumn)
                {
                    sort_order = SortingColumn.Text.StartsWith("> ") ? SortOrder.Descending : SortOrder.Ascending;
                }
                else
                {
                    sort_order = SortOrder.Ascending;
                }

                SortingColumn.Text = SortingColumn.Text.Substring(2);
            }

            SortingColumn = new_sorting_column;
            if (sort_order == SortOrder.Ascending)
            {
                SortingColumn.Text = "> " + SortingColumn.Text;
            }
            else
            {
                SortingColumn.Text = "< " + SortingColumn.Text;
            }

            listViewNormalSendTransactionHistory.ListViewItemSorter =
                new ListViewComparer(e.Column, sort_order);

            listViewNormalSendTransactionHistory.Sort();
        }

        private ColumnHeader SortingColumn2 = null;

        private void listViewNormalReceivedTransactionHistory_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            ColumnHeader new_sorting_column = listViewNormalReceivedTransactionHistory.Columns[e.Column];

            SortOrder sort_order;
            if (SortingColumn2 == null)
            {
                sort_order = SortOrder.Ascending;
            }
            else
            {
                if (new_sorting_column == SortingColumn2)
                {
                    sort_order = SortingColumn2.Text.StartsWith("> ") ? SortOrder.Descending : SortOrder.Ascending;
                }
                else
                {
                    sort_order = SortOrder.Ascending;
                }

                SortingColumn2.Text = SortingColumn2.Text.Substring(2);
            }

            SortingColumn2 = new_sorting_column;
            if (sort_order == SortOrder.Ascending)
            {
                SortingColumn2.Text = "> " + SortingColumn2.Text;
            }
            else
            {
                SortingColumn2.Text = "< " + SortingColumn2.Text;
            }

            listViewNormalReceivedTransactionHistory.ListViewItemSorter =
                new ListViewComparer(e.Column, sort_order);

            listViewNormalReceivedTransactionHistory.Sort();
        }

        private ColumnHeader SortingColumn3 = null;

        private void listViewAnonymitySendTransactionHistory_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            ColumnHeader new_sorting_column = listViewAnonymitySendTransactionHistory.Columns[e.Column];

            SortOrder sort_order;
            if (SortingColumn3 == null)
            {
                sort_order = SortOrder.Ascending;
            }
            else
            {
                if (new_sorting_column == SortingColumn3)
                {
                    sort_order = SortingColumn3.Text.StartsWith("> ") ? SortOrder.Descending : SortOrder.Ascending;
                }
                else
                {
                    sort_order = SortOrder.Ascending;
                }

                SortingColumn3.Text = SortingColumn3.Text.Substring(2);
            }

            SortingColumn3 = new_sorting_column;
            if (sort_order == SortOrder.Ascending)
            {
                SortingColumn3.Text = "> " + SortingColumn3.Text;
            }
            else
            {
                SortingColumn3.Text = "< " + SortingColumn3.Text;
            }

            listViewAnonymitySendTransactionHistory.ListViewItemSorter =
                new ListViewComparer(e.Column, sort_order);

            listViewAnonymitySendTransactionHistory.Sort();
        }

        private ColumnHeader SortingColumn4 = null;

        private void listViewAnonymityReceivedTransactionHistory_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            ColumnHeader new_sorting_column = listViewAnonymityReceivedTransactionHistory.Columns[e.Column];

            SortOrder sort_order;
            if (SortingColumn4 == null)
            {
                sort_order = SortOrder.Ascending;
            }
            else
            {
                if (new_sorting_column == SortingColumn4)
                {
                    sort_order = SortingColumn4.Text.StartsWith("> ") ? SortOrder.Descending : SortOrder.Ascending;
                }
                else
                {
                    sort_order = SortOrder.Ascending;
                }

                SortingColumn4.Text = SortingColumn4.Text.Substring(2);
            }

            SortingColumn4 = new_sorting_column;
            if (sort_order == SortOrder.Ascending)
            {
                SortingColumn4.Text = "> " + SortingColumn4.Text;
            }
            else
            {
                SortingColumn4.Text = "< " + SortingColumn4.Text;
            }

            listViewAnonymityReceivedTransactionHistory.ListViewItemSorter =
                new ListViewComparer(e.Column, sort_order);

            listViewAnonymityReceivedTransactionHistory.Sort();
        }

        private ColumnHeader SortingColumn5 = null;

        private void listViewBlockRewardTransactionHistory_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            ColumnHeader new_sorting_column = listViewBlockRewardTransactionHistory.Columns[e.Column];

            SortOrder sort_order;
            if (SortingColumn5 == null)
            {
                sort_order = SortOrder.Ascending;
            }
            else
            {
                if (new_sorting_column == SortingColumn5)
                {
                    sort_order = SortingColumn5.Text.StartsWith("> ") ? SortOrder.Descending : SortOrder.Ascending;
                }
                else
                {
                    sort_order = SortOrder.Ascending;
                }

                SortingColumn5.Text = SortingColumn5.Text.Substring(2);
            }

            SortingColumn5 = new_sorting_column;
            if (sort_order == SortOrder.Ascending)
            {
                SortingColumn5.Text = "> " + SortingColumn5.Text;
            }
            else
            {
                SortingColumn5.Text = "< " + SortingColumn5.Text;
            }

            listViewBlockRewardTransactionHistory.ListViewItemSorter =
                new ListViewComparer(e.Column, sort_order);

            listViewBlockRewardTransactionHistory.Sort();
        }

#endregion

        private void Transaction_Resize(object sender, EventArgs e)
        {
            UpdateStyles();
        }


        private void tabPageTransactionHistory_Selected(object sender, TabControlEventArgs e)
        {
            ClassFormPhase.WalletXenophyte.UpdateCurrentPageNumberTransactionHistory();
            if (tabPageNormalTransactionSend.Visible) //  Normal transaction send list
            {
                listViewNormalSendTransactionHistory.Show();
                listViewBlockRewardTransactionHistory.Hide();
                listViewAnonymityReceivedTransactionHistory.Hide();
                listViewAnonymitySendTransactionHistory.Hide();
                listViewNormalReceivedTransactionHistory.Hide();
            }
            if (tabPageNormalTransactionReceived.Visible) // Normal transaction received list
            {
                listViewNormalSendTransactionHistory.Hide();
                listViewBlockRewardTransactionHistory.Hide();
                listViewAnonymityReceivedTransactionHistory.Hide();
                listViewAnonymitySendTransactionHistory.Hide();
                listViewNormalReceivedTransactionHistory.Show();
            }
            if (tabPageAnonymityTransactionSend.Visible) // Anonymous transaction send list 
            {
                listViewNormalSendTransactionHistory.Hide();
                listViewBlockRewardTransactionHistory.Hide();
                listViewAnonymityReceivedTransactionHistory.Hide();
                listViewAnonymitySendTransactionHistory.Show();
                listViewNormalReceivedTransactionHistory.Hide();
            }
            if (tabPageAnonymityTransactionReceived.Visible) // Anonymous transaction received list 
            {
                listViewNormalSendTransactionHistory.Hide();
                listViewBlockRewardTransactionHistory.Hide();
                listViewAnonymityReceivedTransactionHistory.Show();
                listViewAnonymitySendTransactionHistory.Hide();
                listViewNormalReceivedTransactionHistory.Hide();
            }
            if (tabPageBlockRewardTransaction.Visible) // block reward transaction list 
            {
                listViewNormalSendTransactionHistory.Hide();
                listViewBlockRewardTransactionHistory.Show();
                listViewAnonymityReceivedTransactionHistory.Hide();
                listViewAnonymitySendTransactionHistory.Hide();
                listViewNormalReceivedTransactionHistory.Hide();
            }
        }

        private void tabPageTransactionHistory_SelectedIndexChanged(object sender, EventArgs e)
        {
            ClassFormPhase.WalletXenophyte.UpdateCurrentPageNumberTransactionHistory();
            if (tabPageNormalTransactionSend.Visible) //  Normal transaction send list
            {
                listViewNormalSendTransactionHistory.Show();
                listViewBlockRewardTransactionHistory.Hide();
                listViewAnonymityReceivedTransactionHistory.Hide();
                listViewAnonymitySendTransactionHistory.Hide();
                listViewNormalReceivedTransactionHistory.Hide();
            }
            if (tabPageNormalTransactionReceived.Visible) // Normal transaction received list
            {
                listViewNormalSendTransactionHistory.Hide();
                listViewBlockRewardTransactionHistory.Hide();
                listViewAnonymityReceivedTransactionHistory.Hide();
                listViewAnonymitySendTransactionHistory.Hide();
                listViewNormalReceivedTransactionHistory.Show();
            }
            if (tabPageAnonymityTransactionSend.Visible) // Anonymous transaction send list 
            {
                listViewNormalSendTransactionHistory.Hide();
                listViewBlockRewardTransactionHistory.Hide();
                listViewAnonymityReceivedTransactionHistory.Hide();
                listViewAnonymitySendTransactionHistory.Show();
                listViewNormalReceivedTransactionHistory.Hide();
            }
            if (tabPageAnonymityTransactionReceived.Visible) // Anonymous transaction received list 
            {
                listViewNormalSendTransactionHistory.Hide();
                listViewBlockRewardTransactionHistory.Hide();
                listViewAnonymityReceivedTransactionHistory.Show();
                listViewAnonymitySendTransactionHistory.Hide();
                listViewNormalReceivedTransactionHistory.Hide();
            }
            if (tabPageBlockRewardTransaction.Visible) // block reward transaction list 
            {
                listViewNormalSendTransactionHistory.Hide();
                listViewBlockRewardTransactionHistory.Show();
                listViewAnonymityReceivedTransactionHistory.Hide();
                listViewAnonymitySendTransactionHistory.Hide();
                listViewNormalReceivedTransactionHistory.Hide();
            }
        }
    }

    public class ListViewComparer : System.Collections.IComparer
    {
        private int _columnNumber;
        private SortOrder _sortOrder;

        public ListViewComparer(int columnNumber,
            SortOrder sortOrder)
        {
            _columnNumber = columnNumber;
            _sortOrder = sortOrder;
        }

        public int Compare(object objectX, object objectY)
        {
            ListViewItem itemX = objectX as ListViewItem;
            ListViewItem itemY = objectY as ListViewItem;

            string stringX = null;
            if (itemX != null && itemX.SubItems.Count <= _columnNumber)
            {
                stringX = "";
            }
            else
            {
                if (itemX != null) stringX = itemX.SubItems[_columnNumber].Text;
            }

            string stringY = null;
            if (itemY != null && itemY.SubItems.Count <= _columnNumber)
            {
                stringY = "";
            }
            else
            {
                if (itemY != null) stringY = itemY.SubItems[_columnNumber].Text;
            }

            int result = 0;
            if (double.TryParse(stringX, out var double_x) &&
                double.TryParse(stringY, out var double_y))
            {
                result = double_x.CompareTo(double_y);
            }
            else
            {
                if (DateTime.TryParse(stringX, out var date_x) &&
                    DateTime.TryParse(stringY, out var date_y))
                {
                    result = date_x.CompareTo(date_y);
                }
                else
                {
                    if (stringX != null) result = stringX.CompareTo(stringY);
                }
            }

            if (_sortOrder == SortOrder.Ascending)
            {
                return result;
            }
            else
            {
                return -result;
            }
        }
    }

    public sealed class ListViewEx : ListView
    {
        private const int WM_LBUTTONDOWN = 0x0201;

        public ListViewEx() : base()
        {
            DoubleBuffered = true;
        }



        protected override void WndProc(ref Message m)

        {
            if (m.Msg == WM_LBUTTONDOWN)
            {
                Int16 x = (Int16) m.LParam;

                Int16 y = (Int16) ((int) m.LParam >> 16);


                MouseEventArgs e = new MouseEventArgs(MouseButtons.Left, 2, x, y, 0);


                // this.OnMouseClick(e);
#if WINDOWS
                UpdateStyles();
#endif
            }

            base.WndProc(ref m);
        }
    }
}