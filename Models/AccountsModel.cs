
using AzureStorage.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Template10.Mvvm;
using Windows.Storage;

namespace AzureStorage.Models
{
    /// <summary>
    /// The collection of accounts.
    /// </summary>
    public class AccountsModel : BindableBase
    {
        #region Private constants

        private const string SavedAccounts = "SavedAccounts";
        private const string SavedIndex = "SavedIndex";

        #endregion

        #region Private fields

        private static readonly ObservableCollectionEx<AccountModel> _accounts = new ObservableCollectionEx<AccountModel>();
        private static AccountsModel _instance = new AccountsModel();
        private DelegateCommand _delete;
        private int _index;

        #endregion

        #region Private methods

        /// <summary>
        /// Loads the accounts from storage.
        /// </summary>
        private void LoadState()
        {
            var data = (string)ApplicationData.Current.LocalSettings.Values[SavedAccounts];

            if (string.IsNullOrEmpty(data)) return;

            _accounts.Set(JsonConvert.DeserializeObject<List<AccountModel>>(data));

            var index = ApplicationData.Current.LocalSettings.Values[SavedIndex];

            if (index != null) SelectedIndex = (int)index;
        }

        /// <summary>
        /// Saves the accounts data.
        /// </summary>
        private void SaveState()
        {
            ApplicationData.Current.LocalSettings.Values[SavedAccounts] = JsonConvert.SerializeObject(_accounts);
            ApplicationData.Current.LocalSettings.Values[SavedIndex] = _index;
        }

        /// <summary>
        /// Determines if the account can be deleted.
        /// </summary>
        /// <returns></returns>
        private bool CanDeleteAccount()
        {
            return (_index >= 0);
        }

        /// <summary>
        /// Deletes the current account.
        /// </summary>
        private async void DeleteAccount()
        {
            if ((_index < 0) || (_index >= _accounts.Count))
            {
                _delete.RaiseCanExecuteChanged();
                return;
            }

            var index = _index;
            var account = _accounts[_index];
            var result = await Dialogs.ShowOkCancel(account.AccountName, string.Format("Delete the storage account '{0}'?", account.AccountName));

            if (!result) return;

            try
            {
                _accounts.RemoveAt(index);

                RecentModel.Instance.Remove(account);

                while (index >= _accounts.Count) index--;

                SelectedIndex = index;
            }
            finally
            {
                SaveState();
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        private AccountsModel()
        {
            _delete = new DelegateCommand(new Action(DeleteAccount), CanDeleteAccount);
            _index = (-1);

            LoadState();
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Adds a new account to the collection.
        /// </summary>
        /// <param name="account">The account object to add.</param>
        public void Add(AccountModel account)
        {
            if (account == null) throw new ArgumentNullException("account");

            var index = _accounts.Count;

            try
            {
                _accounts.Add(account);
                SelectedIndex = index;
            }
            finally
            {
                SaveState();
            }
        }

        #endregion

        #region Public properties

        /// <summary>
        /// The static instance.
        /// </summary>
        public static AccountsModel Instance
        {
            get { return _instance; }
        }

        /// <summary>
        /// Returns the selected account instance.
        /// </summary>
        public AccountModel Current
        {
            get {  return ((_index < 0) ? null : _accounts[_index]); }
        }

        /// <summary>
        /// The collection of accounts.
        /// </summary>
        public ObservableCollectionEx<AccountModel> Accounts
        {
            get { return _accounts; }
        }

        /// <summary>
        /// The index of the selected account.
        /// </summary>
        public int SelectedIndex
        {
            get { return _index; }
            set
            {
                if (_index != value)
                {
                    if (value < _accounts.Count) _index = value;
                    ApplicationData.Current.LocalSettings.Values[SavedIndex] = _index;

                    base.RaisePropertyChanged();
                }

                _delete.RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// The delegate command for delete.
        /// </summary>
        public DelegateCommand Delete
        {
            get { return _delete; }
        }

        #endregion
    }
}
