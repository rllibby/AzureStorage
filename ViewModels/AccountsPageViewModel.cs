/*
 *  Copyright © 2016, Russell Libby 
 */

using AzureStorage.Controls;
using AzureStorage.Helpers;
using AzureStorage.Models;
using AzureStorage.Views;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using System;
using System.Threading.Tasks;
using Template10.Common;
using Template10.Controls;
using Template10.Mvvm;
using Windows.UI.Xaml;

namespace AzureStorage.ViewModels
{
    /// <summary>
    /// View model for Accounts page.
    /// </summary>
    class AccountsPageViewModel : ViewModelBase
    {
        #region Private fields

        private AddAccountControl _addAccount = new AddAccountControl();
        private DelegateCommand _add;

        #endregion

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

                modal.ModalContent = _addAccount;
                modal.IsModal = show;
            });
        }

        /// <summary>
        /// Event that is triggered when the user wants to add a new account.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void AddAccount()
        {
            try
            {
                _addAccount.Account.Clear();
            }
            finally
            {
                ShowAddAccount(true);
            }
        }

        /// <summary>
        /// Event that is triggered when the add button is clicked.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void OnAddAccount(object sender, System.EventArgs e)
        {
            ShowAddAccount(false);

            Busy.SetBusy(true, "Verifying account...");

            Task.Run(async () =>
            {
                var account = new CloudStorageAccount(new StorageCredentials(_addAccount.Account.AccountName, _addAccount.Account.AccountKey), _addAccount.Account.SuffixEndpoint, true);
                var tableClient = account.CreateCloudTableClient();
                var tables = await tableClient.ListTablesSegmentedAsync(null);

            }).ContinueWith((t) =>
            {
                Busy.SetBusy(false);

                if (t.IsFaulted)
                {
                    WindowWrapper.Current().Dispatcher.Dispatch(async () =>
                    {
                        var result = await Helpers.Dialogs.ShowException(_addAccount.Account.AccountName, string.Format("Failed to add the storage account '{0}'.", _addAccount.Account.AccountName), t.Exception);

                        if (!result) return;

                        ShowAddAccount(true);
                    });

                    return;
                }

                if (t.IsCompleted)
                {
                    WindowWrapper.Current().Dispatcher.Dispatch(() =>
                    {
                        AccountsModel.Instance.Add(_addAccount.Account.Copy());
                    });
                }
            });
        }

        /// <summary>
        /// Event that is triggered when the dialog is cancelled.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void OnCancelAccount(object sender, System.EventArgs e)
        {
            ShowAddAccount(false);
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public AccountsPageViewModel()
        {
            _add = new DelegateCommand(new Action(AddAccount));

            _addAccount.VerticalAlignment = VerticalAlignment.Center;
            _addAccount.Margin = new Thickness(20, 0, 20, 0);
            _addAccount.OnAdd += OnAddAccount;
            _addAccount.OnCancel += OnCancelAccount;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// The collection of accounts.
        /// </summary>
        public AccountsModel Accounts
        {
            get { return AccountsModel.Instance; }
        }

        /// <summary>
        /// The command handler for the add action.
        /// </summary>
        public DelegateCommand Add
        {
            get { return _add; }
        }

        #endregion
    }
}
