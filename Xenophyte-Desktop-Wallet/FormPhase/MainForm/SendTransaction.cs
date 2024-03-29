﻿#if WINDOWS
using MetroFramework;
#endif
using System;
using System.Drawing;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Xenophyte_Connector_All.Wallet;
using Xenophyte_Wallet.Wallet;

namespace Xenophyte_Wallet.FormPhase.MainForm
{
    public sealed partial class SendTransaction : Form
    {
        private bool AutoUpdateTimeReceived;

        public SendTransaction()
        {
            InitializeComponent();
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            DoubleBuffered = true;
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams CP = base.CreateParams;
                CP.ExStyle = CP.ExStyle | 0x02000000; // WS_EX_COMPOSITED
                return CP;
            }
        }

        private void ButtonSendTransaction_Click(object sender, EventArgs e)
        {
            string amountstring = textBoxAmount.Text.Replace(",", ".");
            string feestring = textBoxFee.Text.Replace(",", ".");

            var checkAmount = CheckAmount(amountstring);
            var checkFee = CheckAmount(feestring);
            if (checkAmount.Item1)
            {
                var amountSend = checkAmount.Item2;
                if (checkFee.Item1)
                {
                    var feeSend = checkFee.Item2;
                    if (CheckAmountNetwork(amountSend + feeSend))
                    {
                        if ((textBoxWalletDestination.Text.Length >= 48 &&
                             textBoxWalletDestination.Text.Length <= 128) && Regex.IsMatch(
                                textBoxWalletDestination.Text,
                                "[a-z0-9]+", RegexOptions.IgnoreCase))
                        {
#if WINDOWS
                            if (ClassFormPhase.MessageBoxInterface(ClassTranslation.GetLanguageTextFromOrder("SEND_TRANSACTION_WALLET_MESSAGE_SUBMIT_CONTENT_TEXT").Replace(ClassTranslation.AmountSendOrder, "" + amountSend).Replace(ClassTranslation.TargetAddressOrder, textBoxWalletDestination.Text),
                                    ClassTranslation.GetLanguageTextFromOrder("SEND_TRANSACTION_WALLET_MESSAGE_SUBMIT_TITLE_TEXT"), MessageBoxButtons.YesNo, MessageBoxIcon.Warning) ==
                                DialogResult.Yes)
#else
                            if (MessageBox.Show(ClassFormPhase.WalletXenophyte, ClassTranslation.GetLanguageTextFromOrder("SEND_TRANSACTION_WALLET_MESSAGE_SUBMIT_CONTENT_TEXT").Replace(ClassTranslation.AmountSendOrder, "" + amountSend).Replace(ClassTranslation.TargetAddressOrder, textBoxWalletDestination.Text),
                                    ClassTranslation.GetLanguageTextFromOrder("SEND_TRANSACTION_WALLET_MESSAGE_SUBMIT_TITLE_TEXT"), MessageBoxButtons.YesNo, MessageBoxIcon.Warning) ==
                                DialogResult.Yes)
#endif
                            {
                                new Thread(async delegate ()
                                {
                                    ClassParallelForm.ShowWaitingForm();

                                    if (checkBoxHideWalletAddress.Checked)
                                    {
                                        await ClassWalletObject.WalletConnect.SendPacketWallet(
                                                    ClassWalletCommand.ClassWalletSendEnumeration.SendTransaction + "|" +
                                                    textBoxWalletDestination.Text + "|" + amountSend + "|" + feeSend + "|1", ClassWalletObject.Certificate, true);


                                    }
                                    else
                                    {
                                        await ClassWalletObject.WalletConnect.SendPacketWallet(
                                            ClassWalletCommand.ClassWalletSendEnumeration.SendTransaction + "|" +
                                            textBoxWalletDestination.Text + "|" + amountSend + "|" + feeSend + "|0", ClassWalletObject.Certificate, true);
                                    }

                                    MethodInvoker invoke = () =>
                                    {
                                        checkBoxHideWalletAddress.Checked = false;
                                        textBoxAmount.Text = "0.00000000";
                                        textBoxFee.Text = "0.00001000";
                                        textBoxWalletDestination.Text = string.Empty;
                                    };
                                    BeginInvoke(invoke);
                                }).Start();

                            }
                        }
                        else
                        {
#if WINDOWS
                            ClassFormPhase.MessageBoxInterface( ClassTranslation.GetLanguageTextFromOrder("SEND_TRANSACTION_WALLET_MESSAGE_ERROR_TARGET_CONTENT_TEXT"), string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
#else
                            MessageBox.Show(ClassFormPhase.WalletXenophyte, ClassTranslation.GetLanguageTextFromOrder("SEND_TRANSACTION_WALLET_MESSAGE_ERROR_TARGET_CONTENT_TEXT"));
#endif
                        }
                    }
                }
                else
                {
#if WINDOWS
                    ClassFormPhase.MessageBoxInterface( ClassTranslation.GetLanguageTextFromOrder("SEND_TRANSACTION_WALLET_MESSAGE_ERROR_FEE_CONTENT_TEXT"), string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
#else
                    MessageBox.Show(ClassFormPhase.WalletXenophyte, ClassTranslation.GetLanguageTextFromOrder("SEND_TRANSACTION_WALLET_MESSAGE_ERROR_FEE_CONTENT_TEXT"));
#endif
                }
            }
            else
            {
#if WINDOWS
                ClassFormPhase.MessageBoxInterface( ClassTranslation.GetLanguageTextFromOrder("SEND_TRANSACTION_WALLET_MESSAGE_ERROR_AMOUNT_CONTENT_TEXT"), string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
#else
                MessageBox.Show(ClassFormPhase.WalletXenophyte, ClassTranslation.GetLanguageTextFromOrder("SEND_TRANSACTION_WALLET_MESSAGE_ERROR_AMOUNT_CONTENT_TEXT"));
#endif
            }
        }

        /// <summary>
        /// Check amount before send.
        /// </summary>
        /// <param name="amount"></param>
        private Tuple<bool, Decimal> CheckAmount(string amount)
        {
            try
            {
                Decimal amountParse =
                    Decimal.Parse(amount, NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat);
                return new Tuple<bool, Decimal>(true, amountParse);
            }
            catch
            {
                // ignored
            }


            return new Tuple<bool, Decimal>(false, 0);
        }

        /// <summary>
        /// Check total amount with current balance.
        /// </summary>
        /// <param name="totalAmount"></param>
        /// <returns></returns>
        private bool CheckAmountNetwork(Decimal totalAmount)
        {
            try
            {
                string newTotalAmount = ClassWalletObject.WalletConnect.WalletAmount;
                Decimal amount = Decimal.Parse(newTotalAmount, NumberStyles.Any,
                    Program.GlobalCultureInfo);
                if (amount >= totalAmount)
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }

            return false;
        }


        public void GetListControl()
        {
            if (ClassFormPhase.WalletXenophyte.ListControlSizeSendTransaction.Count == 0)
            {
                for (int i = 0; i < Controls.Count; i++)
                {
                    if (i < Controls.Count)
                    {
                        ClassFormPhase.WalletXenophyte.ListControlSizeSendTransaction.Add(
                            new Tuple<Size, Point>(Controls[i].Size, Controls[i].Location));
                    }
                }
            }
        }


        private void SendTransaction_Load(object sender, EventArgs e)
        {
            UpdateStyles();
            ClassFormPhase.WalletXenophyte.ResizeWalletInterface();
            if (!AutoUpdateTimeReceived)
            {
                AutoUpdateTimeReceived = true;
                StartAutoUpdateTimeReceived();
            }
        }

        private async void StartAutoUpdateTimeReceived()
        {
            await Task.Run(async delegate()
            {
                while (true)
                {
                    await Task.Delay(100);
                    if (ClassWalletObject.SeedNodeConnectorWallet != null)
                    {
                        if (ClassWalletObject.SeedNodeConnectorWallet.GetStatusConnectToSeed(Program.IsLinux))
                        {
                            MethodInvoker invoke;
                            if (!string.IsNullOrEmpty(textBoxFee.Text))
                            {
                                if (Decimal.TryParse(textBoxFee.Text.Replace(".", ","), out var feeAmount))
                                {
                                    Decimal timePendingFromFee = ClassWalletObject.RemoteNodeTotalPendingTransactionInNetwork;



                                    if (timePendingFromFee <= 0)
                                    {
                                        timePendingFromFee = 1;
                                    }
                                    else
                                    {
                                        Decimal decreaseTime = ((feeAmount * 100000) / timePendingFromFee) * 100;
                                        if (decreaseTime > 0)
                                        {
                                            timePendingFromFee = timePendingFromFee - decreaseTime;
                                            if (timePendingFromFee <= 0)
                                            {
                                                timePendingFromFee = 1;
                                            }
                                            else
                                            {
                                                timePendingFromFee = (timePendingFromFee * 1) / 100;
                                            }
                                        }
                                    }

                                    if (timePendingFromFee < 60)
                                    {
                                        timePendingFromFee = Math.Round(timePendingFromFee, 0);
                                        invoke = () => textBoxTransactionTime.Text = timePendingFromFee + " " + ClassTranslation.GetLanguageTextFromOrder("SEND_TRANSACTION_WALLET_TIME_SECOND_TEXT");
                                        BeginInvoke(invoke);
                                    }
                                    else if (timePendingFromFee >= 60 && timePendingFromFee < 3600)
                                    {
                                        timePendingFromFee = timePendingFromFee / 60;
                                        timePendingFromFee = Math.Round(timePendingFromFee, 2);
                                        invoke = () => textBoxTransactionTime.Text = timePendingFromFee + " " + ClassTranslation.GetLanguageTextFromOrder("SEND_TRANSACTION_WALLET_TIME_MINUTE_TEXT");
                                        BeginInvoke(invoke);
                                    }
                                    else if (timePendingFromFee >= 3600 && timePendingFromFee < 84600)
                                    {
                                        timePendingFromFee = timePendingFromFee / 3600;
                                        timePendingFromFee = Math.Round(timePendingFromFee, 2);
                                        invoke = () => textBoxTransactionTime.Text = timePendingFromFee + " " + ClassTranslation.GetLanguageTextFromOrder("SEND_TRANSACTION_WALLET_TIME_HOUR_TEXT");
                                        BeginInvoke(invoke);
                                    }
                                    else if (timePendingFromFee >= 84600)
                                    {
                                        timePendingFromFee = timePendingFromFee / 84600;
                                        timePendingFromFee = Math.Round(timePendingFromFee, 2);
                                        invoke = () => textBoxTransactionTime.Text = timePendingFromFee + " " + ClassTranslation.GetLanguageTextFromOrder("SEND_TRANSACTION_WALLET_TIME_DAY_TEXT");
                                        BeginInvoke(invoke);
                                    }
                                }
                                else
                                {
                                    invoke = () => textBoxTransactionTime.Text = "N/A";
                                    BeginInvoke(invoke);
                                }
                            }
                        }
                    }
                }
            }).ConfigureAwait(false);
        }

        private void CheckBoxHideWalletAddress_Click(object sender, EventArgs e)
        {
            if (checkBoxHideWalletAddress.Checked)
            {
#if WINDOWS
                if (ClassFormPhase.MessageBoxInterface(
                        ClassTranslation.GetLanguageTextFromOrder("SEND_TRANSACTION_WALLET_MESSAGE_OPTION_ANONYMITY_CONTENT1_TEXT"),
                        ClassTranslation.GetLanguageTextFromOrder("SEND_TRANSACTION_WALLET_MESSAGE_OPTION_ANONYMITY_TITLE_TEXT"), MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.No)
#else
                if (MessageBox.Show(ClassFormPhase.WalletXenophyte,
                        ClassTranslation.GetLanguageTextFromOrder("SEND_TRANSACTION_WALLET_MESSAGE_OPTION_ANONYMITY_CONTENT1_TEXT"),
                        ClassTranslation.GetLanguageTextFromOrder("SEND_TRANSACTION_WALLET_MESSAGE_OPTION_ANONYMITY_TITLE_TEXT"), MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.No)
#endif
                {
                    checkBoxHideWalletAddress.Checked = false;
                }
            }
        }

        private void SendTransaction_Resize(object sender, EventArgs e)
        {
            UpdateStyles();
        }


        private void buttonFeeInformation_MouseHover(object sender, EventArgs e)
        {
            var toolTipFeeInformation = new ToolTip();
            toolTipFeeInformation.SetToolTip(buttonFeeInformation,
                ClassTranslation.GetLanguageTextFromOrder("SEND_TRANSACTION_WALLET_MESSAGE_FEE_INFORMATION_CONTENT_TEXT"));
        }

        private void buttonFeeInformation_Click(object sender, EventArgs e)
        {
#if WINDOWS
            ClassFormPhase.MessageBoxInterface(
                ClassTranslation.GetLanguageTextFromOrder("SEND_TRANSACTION_WALLET_MESSAGE_FEE_INFORMATION_CONTENT_TEXT"),
                ClassTranslation.GetLanguageTextFromOrder("SEND_TRANSACTION_WALLET_MESSAGE_FEE_INFORMATION_TITLE_TEXT"), MessageBoxButtons.OK, MessageBoxIcon.Information);
#else
            MessageBox.Show(ClassFormPhase.WalletXenophyte,
                ClassTranslation.GetLanguageTextFromOrder("SEND_TRANSACTION_WALLET_MESSAGE_FEE_INFORMATION_CONTENT_TEXT"),
                ClassTranslation.GetLanguageTextFromOrder("SEND_TRANSACTION_WALLET_MESSAGE_FEE_INFORMATION_TITLE_TEXT"), MessageBoxButtons.OK, MessageBoxIcon.Information);
#endif
        }

        private void buttonEstimatedTimeInformation_MouseHover(object sender, EventArgs e)
        {
            var toolTipFeeInformation = new ToolTip();
            toolTipFeeInformation.SetToolTip(buttonEstimatedTimeInformation,
                ClassTranslation.GetLanguageTextFromOrder("SEND_TRANSACTION_WALLET_MESSAGE_TIME_RECEIVE_INFORMATION_CONTENT_TEXT"));
        }

        private void buttonEstimatedTimeInformation_Click(object sender, EventArgs e)
        {
#if WINDOWS
            ClassFormPhase.MessageBoxInterface(
                ClassTranslation.GetLanguageTextFromOrder("SEND_TRANSACTION_WALLET_MESSAGE_TIME_RECEIVE_INFORMATION_CONTENT_TEXT"),
                ClassTranslation.GetLanguageTextFromOrder("SEND_TRANSACTION_WALLET_MESSAGE_TIME_RECEIVE_INFORMATION_TITLE_TEXT"), MessageBoxButtons.OK, MessageBoxIcon.Information);
#else
            MessageBox.Show(ClassFormPhase.WalletXenophyte,
                ClassTranslation.GetLanguageTextFromOrder("SEND_TRANSACTION_WALLET_MESSAGE_TIME_RECEIVE_INFORMATION_CONTENT_TEXT"),
                ClassTranslation.GetLanguageTextFromOrder("SEND_TRANSACTION_WALLET_MESSAGE_TIME_RECEIVE_INFORMATION_TITLE_TEXT"), MessageBoxButtons.OK, MessageBoxIcon.Information);
#endif
        }
    }
}