/*
 *  Copyright © 2016, Russell Libby 
 */

using AzureStorage.Helpers;
using AzureStorage.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using System;
using System.Threading.Tasks;
using Windows.UI.Core;
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
        /// Event that is triggered when adding a new account.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void OnAddAccount(object sender, EventArgs e)
        {
            AddModal.IsModal = false;
            accounts.IsEnabled = true;
            add.IsEnabled = true;

            Busy.SetBusy(true, "Validating account with Azure...");

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
                        await Dialogs.Show("Validation Failed", "Failed to validate the specified Azure storage account.");

                        accounts.IsEnabled = false;
                        add.IsEnabled = false;
                        AddModal.IsModal = true;
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
            accounts.IsEnabled = true;
            add.IsEnabled = true;
            AddModal.IsModal = false;
        }

        /// <summary>
        /// Event that is triggered when the user wants to add a new account.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void AddAccount(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            addAccount.Account.Clear();
            accounts.IsEnabled = false;
            add.IsEnabled = false;
            AddModal.IsModal = true;
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
