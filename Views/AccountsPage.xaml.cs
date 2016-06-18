/*
 *  Copyright © 2016, Russell Libby 
 */

using AzureStorage.Controls;
using AzureStorage.Helpers;
using AzureStorage.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using System;
using System.Threading.Tasks;
using Template10.Common;
using Template10.Controls;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

#pragma warning disable CS4014

namespace AzureStorage.Views
{
    /// <summary>
    /// Accounts page.
    /// </summary>
    public sealed partial class AccountsPage : Page
    {
        #region Private methods

        /// <summary>
        /// Shows or hides the add account modal dialog.
        /// </summary>
        /// <param name="show">True to show the dialog, false to hide.</param>
        private void ShowAddAccount(bool show)
        {
            WindowWrapper.Current().Dispatcher.Dispatch(() =>
            {
                var modal = Window.Current.Content as ModalDialog;

                modal.ModalContent = addAccount;
                modal.IsModal = show;
            });
        }

        /// <summary>
        /// Event that is triggered when adding a new account.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void OnAddAccount(object sender, EventArgs e)
        {
            ShowAddAccount(false);

            Busy.SetBusy(true, "Verifying account...");

            Task.Run(async () =>
            {
                var account = new CloudStorageAccount(new StorageCredentials(addAccount.Account.AccountName, addAccount.Account.AccountKey), addAccount.Account.SuffixEndpoint, true);
                var tableClient = account.CreateCloudTableClient();
                var tables = await tableClient.ListTablesSegmentedAsync(null);

            }).ContinueWith((t) =>
            {
                Busy.SetBusy(false);

                if (t.IsFaulted)
                {
                    Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                    {
                        await Dialogs.Show(addAccount.Account.AccountName, "Failed to verify the specified storage account.");

                        ShowAddAccount(true);
                    });
                    return;
                }
                
                if (t.IsCompleted)
                {
                    Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        AccountsModel.Instance.Add(addAccount.Account.Copy());
                    });
                }
            });
        }

        /// <summary>
        /// Event that is triggered when the dialog is cancelled.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void OnCancel(object sender, EventArgs e)
        {
            ShowAddAccount(false);
        }

        /// <summary>
        /// Event that is triggered when the user wants to add a new account.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void AddAccount(object sender, RoutedEventArgs e)
        {
            addAccount.Account.Clear();

            ShowAddAccount(true);
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public AccountsPage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
        }

        #endregion
    }
}
