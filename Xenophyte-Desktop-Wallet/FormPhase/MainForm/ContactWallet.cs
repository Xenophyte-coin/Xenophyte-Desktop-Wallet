﻿using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using Xenophyte_Wallet.Features;
using Xenophyte_Wallet.FormPhase.ParallelForm;

namespace Xenophyte_Wallet.FormPhase.MainForm
{
    public partial class ContactWallet : Form
    {
        private bool contactLoaded;

        public ContactWallet()
        {
            InitializeComponent();
        }

        private void buttonAddContact_Click(object sender, EventArgs e)
        {
            var addContactForm = new AddContactWallet();
            addContactForm.ShowDialog(Program.WalletXenophyte);
            addContactForm.Dispose();
        }

        public void GetListControl()
        {
            if (Program.WalletXenophyte.ListControlSizeContactWallet.Count == 0)
                for (var i = 0; i < Controls.Count; i++)
                    if (i < Controls.Count)
                        Program.WalletXenophyte.ListControlSizeContactWallet.Add(
                            new Tuple<Size, Point>(Controls[i].Size, Controls[i].Location));
        }

        private void ContactWallet_Load(object sender, EventArgs e)
        {
            UpdateStyles();
            Program.WalletXenophyte.ResizeWalletInterface();
            if (!contactLoaded) // Load contact database file.
            {
                contactLoaded = true;
                if (ClassContact.ListContactWallet.Count > 0)
                    foreach (var contact in ClassContact.ListContactWallet)
                    {
                        string[] objectContact = {contact.Value.Item1, contact.Value.Item2, "X"};
                        var itemContact = new ListViewItem(objectContact);
                        listViewExContact.Items.Add(itemContact);
                    }
            }
        }

        private void ContactWallet_Resize(object sender, EventArgs e)
        {
            UpdateStyles();
        }

        private void listViewExContact_MouseClick(object sender, MouseEventArgs e)
        {
            var item = listViewExContact.GetItemAt(0, e.Y);

            var found = false;
            if (item == null) return;
            for (var ix = item.SubItems.Count - 1; ix >= 0; --ix)
                if (item.SubItems[ix].Bounds.Contains(e.Location))
                    if (!found)
                    {
                        found = true;
                        if (item.SubItems[ix].Text != "X")
                        {
                            Clipboard.SetText(item.SubItems[ix].Text);
#if WINDOWS
                            new Thread(() =>
                                    ClassFormPhase.MessageBoxInterface(
                                        item.SubItems[ix].Text + " " +
                                        ClassTranslation.GetLanguageTextFromOrder(
                                            "CONTACT_LIST_COPY_ACTION_CONTENT_TEXT"), string.Empty,
                                        MessageBoxButtons.OK, MessageBoxIcon.Information))
                                .Start();
#else
                        new Thread(delegate ()
                        {
                            MethodInvoker invoker =
 () => MessageBox.Show(Program.WalletXenophyte, item.SubItems[ix].Text + " " + ClassTranslation.GetLanguageTextFromOrder("CONTACT_LIST_COPY_ACTION_CONTENT_TEXT"));
                            BeginInvoke(invoker);
                        }).Start();
#endif
                        }
                        else
                        {
#if WINDOWS
                            if (ClassFormPhase.MessageBoxInterface(
                                    ClassTranslation.GetLanguageTextFromOrder(
                                        "CONTACT_LIST_REMOVE_ACTION_CONTENT_TEXT"), string.Empty,
                                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
#else
                            if (MessageBox.Show(ClassTranslation.GetLanguageTextFromOrder("CONTACT_LIST_REMOVE_ACTION_CONTENT_TEXT"), string.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
#endif
                            {
                                ClassContact.RemoveContact(item.SubItems[0].Text); // Remove contact by his name.
                                listViewExContact.Items.Remove(item);
                                listViewExContact.Refresh();
                            }
                        }

                        return;
                    }
        }
    }
}