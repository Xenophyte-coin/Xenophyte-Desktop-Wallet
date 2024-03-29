﻿using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using MetroFramework;
using Xenophyte_Wallet.FormPhase.ParallelForm;

namespace Xenophyte_Wallet.FormPhase
{
    public class ClassParallelForm
    {
        public static bool PinFormShowed;
        public static bool WaitingFormShowed;
        public static bool WaitingCreateWalletFormShowed;
        public static bool WaitingForm2Showed;
        public static bool WaitingFormReconnectShowed;


        public static PinFormWallet PinForm = new PinFormWallet();
        public static WaitingForm WaitingForm = new WaitingForm();
        public static WaitingCreateWalletForm WaitingCreateWalletForm = new WaitingCreateWalletForm();
        public static WaitingForm WaitingForm2 = new WaitingForm();
        public static WaitingFormReconnect WaitingFormReconnect = new WaitingFormReconnect();


        /// <summary>
        ///     Show pin form only if he is not showed.
        /// </summary>
        public static async void ShowPinFormAsync()
        {
            await Task.Factory.StartNew(() =>
                {
                    try
                    {
                        if (PinForm != null)
                            if (!PinFormShowed)
                            {
                                PinFormShowed = true;
#if WINDOWS
                                Program.WalletXenophyte.Invoke((MethodInvoker) delegate
                                {
                                    PinForm.StartPosition = FormStartPosition.CenterParent;
                                    PinForm.TopMost = true;
                                    PinForm.BringToFront();
                                    PinForm.ShowDialog(Program.WalletXenophyte);

                                });
#else
                            Program.WalletXenophyte.BeginInvoke((MethodInvoker)delegate ()
                           {
                               PinForm.StartPosition = FormStartPosition.CenterParent;
                               PinForm.TopMost = true;
                               PinForm.Show(Program.WalletXenophyte);
                           });
#endif
                            }
                    }
                    catch
                    {
                        PinFormShowed = false;
                    }
                }, CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Current)
                .ConfigureAwait(false);
        }

        public static async void HidePinFormAsync()
        {
            await Task.Factory.StartNew(() =>
                {
                    if (PinForm != null)
                        try
                        {
                            if (PinFormShowed)
                            {
                                Program.WalletXenophyte.BeginInvoke((MethodInvoker) delegate { PinForm.Hide(); });
                                PinFormShowed = false;
                            }
                        }
                        catch
                        {
                        }
                }, CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Current)
                .ConfigureAwait(false);
        }

        /// <summary>
        ///     Show waiting form network.
        /// </summary>
        public static async void ShowWaitingFormAsync()
        {
            await Task.Factory.StartNew(() =>
                {
                    if (WaitingForm != null)
                        if (!WaitingFormShowed)
                        {
                            WaitingFormShowed = true;
#if WINDOWS
                            try
                            {
                                if (WaitingForm.Visible) HideWaitingFormAsync();
                                Program.WalletXenophyte.Invoke((MethodInvoker) delegate
                                {
                                    WaitingForm.StartPosition = FormStartPosition.CenterParent;
                                    WaitingForm.TopMost = false;
                                    WaitingForm.ShowDialog(Program.WalletXenophyte);
                                });
                            }
                            catch
                            {
                            }
#else
                        MethodInvoker invoke = () =>
                        {
                            WaitingForm.StartPosition = FormStartPosition.CenterParent;
                            WaitingForm.TopMost = true;
                            WaitingForm.Show(Program.WalletXenophyte);
                        };
                        Program.WalletXenophyte.BeginInvoke(invoke);
#endif
                        }
                }, CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Current)
                .ConfigureAwait(false);
        }

        /// <summary>
        ///     Hide waiting form.
        /// </summary>
        public static async void HideWaitingFormAsync()
        {
            await Task.Factory.StartNew(() =>
                {
                    if (WaitingForm != null)
                        if (WaitingFormShowed)
                        {
                            WaitingFormShowed = false;
                            try
                            {
                                MethodInvoker invoke = () => WaitingForm.Hide();
                                Program.WalletXenophyte.BeginInvoke(invoke);
                            }
                            catch
                            {
                            }
                        }
                }, CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Current)
                .ConfigureAwait(false);
        }


        /// <summary>
        ///     Show waiting reconnect form network.
        /// </summary>
        public static async void ShowWaitingReconnectFormAsync()
        {
            await Task.Factory.StartNew(() =>
                {
                    if (WaitingFormReconnect != null)
                        if (!WaitingFormReconnectShowed)
                        {
                            WaitingFormReconnectShowed = true;
                            try
                            {
#if WINDOWS
                                Program.WalletXenophyte.Invoke((MethodInvoker) delegate
                                {
                                    WaitingFormReconnect.StartPosition = FormStartPosition.CenterParent;
                                    WaitingFormReconnect.TopMost = false;
                                    WaitingFormReconnect.ShowDialog(Program.WalletXenophyte);
                                });
#else
                            Program.WalletXenophyte.Invoke((MethodInvoker)delegate ()
                            {
                                WaitingFormReconnect.StartPosition = FormStartPosition.CenterParent;
                                WaitingFormReconnect.TopMost = true;
                                WaitingFormReconnect.Show(Program.WalletXenophyte);
                            });
#endif
                            }
                            catch
                            {
                                WaitingFormReconnectShowed = false;
                            }
                        }
                }, CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Current)
                .ConfigureAwait(false);
        }

        /// <summary>
        ///     Hide waiting reconnect form.
        /// </summary>
        public static async void HideWaitingReconnectFormAsync()
        {
            await Task.Factory.StartNew(() =>
                {
                    try
                    {
                        if (WaitingFormReconnectShowed)
                        {
                            WaitingFormReconnectShowed = false;
                            try
                            {
                                WaitingFormReconnect.Invoke((MethodInvoker)delegate
                               {
                                   WaitingFormReconnect.Hide();
                                   WaitingFormReconnect.Refresh();
                               });
                            }
                            catch
                            {
                            }
                        }
                    }
                    catch
                    {

                    }
                }, CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Current)
                .ConfigureAwait(false);
        }


        /// <summary>
        ///     Show waiting form network.
        /// </summary>
        public static async void ShowWaitingForm2Async()
        {
            await Task.Factory.StartNew(() =>
                {
                    if (WaitingForm2 != null)
                        if (!WaitingForm2Showed)
                            try
                            {
                                WaitingForm2Showed = true;
#if WINDOWS
                                Program.WalletXenophyte.Invoke((MethodInvoker) delegate
                                {
                                    WaitingForm2.StartPosition = FormStartPosition.CenterParent;
                                    WaitingForm2.TopMost = false;
                                    WaitingForm2.ShowDialog(Program.WalletXenophyte);
                                });
#else
                            Program.WalletXenophyte.Invoke((MethodInvoker)delegate ()
                           {
                               WaitingForm2.StartPosition = FormStartPosition.CenterParent;
                               WaitingForm2.TopMost = true;
                               WaitingForm2.Show(Program.WalletXenophyte);
                           });
#endif
                            }
                            catch
                            {
                            }
                }, CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Current)
                .ConfigureAwait(false);
        }

        /// <summary>
        ///     Hide waiting form.
        /// </summary>
        public static async Task HideWaitingForm2Async()
        {
            await Task.Factory.StartNew(() =>
                {
                    try
                    {
                        if (WaitingForm2 != null)
                            if (WaitingForm2Showed)
                            {
                                WaitingForm2Showed = false;
                                WaitingForm2.Invoke((MethodInvoker) delegate
                                {
                                    WaitingForm2.Hide();
                                    WaitingForm2.Refresh();
                                });
                            }
                    }
                    catch
                    {
                    }
                }, CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Current)
                .ConfigureAwait(false);
        }


        /// <summary>
        ///     Show waiting dialog of create wallet.
        /// </summary>
        public static async void ShowWaitingCreateWalletFormAsync()
        {
            await Task.Factory.StartNew(() =>
                {
                    try
                    {
                        if (WaitingCreateWalletForm != null)
                            if (!WaitingCreateWalletFormShowed)
                            {
                                WaitingCreateWalletFormShowed = true;
#if WINDOWS
                                Program.WalletXenophyte.Invoke((MethodInvoker) delegate
                                {
                                    WaitingCreateWalletForm.StartPosition = FormStartPosition.CenterParent;
                                    WaitingCreateWalletForm.TopMost = false;
                                    WaitingCreateWalletForm.ShowDialog(Program.WalletXenophyte);
                                });
#else
                            Program.WalletXenophyte.Invoke((MethodInvoker)delegate ()
                           {
                               WaitingCreateWalletForm.StartPosition = FormStartPosition.CenterParent;
                               WaitingCreateWalletForm.TopMost = true;
                               WaitingCreateWalletForm.Show(Program.WalletXenophyte);
                           });
#endif
                            }
                    }
                    catch
                    {
                    }
                }, CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Current)
                .ConfigureAwait(false);
        }

        /// <summary>
        ///     Hide waiting dialog of create wallet.
        /// </summary>
        public static async void HideWaitingCreateWalletFormAsync()
        {
            await Task.Factory.StartNew(() =>
                {
                    try
                    {
                        if (WaitingCreateWalletForm != null)
                            if (WaitingCreateWalletFormShowed)
                            {
                                WaitingCreateWalletFormShowed = false;

                                WaitingCreateWalletForm.Invoke((MethodInvoker) delegate
                                {
                                    WaitingCreateWalletForm.Hide();
                                });
                            }
                    }
                    catch
                    {
                    }
                }, CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Current)
                .ConfigureAwait(false);
        }

    }
}