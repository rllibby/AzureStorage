/*
 *  Copyright © 2016, Russell Libby 
 */

using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Newtonsoft.Json;
using System;
using Template10.Mvvm;

namespace AzureStorage.Models
{
    /// <summary>
    /// Model object for maintaining account information.
    /// </summary>
    public class AccountModel : BindableBase
    {
        #region Private constants

        private const string Endpoint = "core.windows.net";

        #endregion

        #region Private fields

        private DelegateCommand _add;
        private string _name;
        private string _key;

        #endregion

        #region Private methods

        /// <summary>
        /// Determines if the account defintion can be added.
        /// </summary>
        /// <returns></returns>
        private bool CanAddAccount()
        {
            return (!string.IsNullOrEmpty(_name) && !string.IsNullOrEmpty(_key));
        }

        /// <summary>
        /// Action called when adding an account.
        /// </summary>
        private void AddAccount()
        {
            OnAdd?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public AccountModel()
        {
            _add = new DelegateCommand(new Action(AddAccount), CanAddAccount);
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="accountName">The account name.</param>
        public AccountModel(string accountName)
        {
            if (string.IsNullOrEmpty(accountName)) throw new ArgumentNullException("accountName");

            _name = accountName;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Returns an instance of CloudStorageAccount based on the current state.
        /// </summary>
        /// <returns>The cloud storage account on success.</returns>
        public CloudStorageAccount GetStorageAccount()
        {
            return new CloudStorageAccount(new StorageCredentials(_name, _key), Endpoint, true);
        }

        /// <summary>
        /// Clears the account.
        /// </summary>
        public void Clear()
        {
            _name = _key = null;

            _add.RaiseCanExecuteChanged();

            RaisePropertyChanged("AccountName");
            RaisePropertyChanged("AccountKey");
        }
        
        /// <summary>
        /// Returns the string version of the object.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return _name ?? base.ToString();
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Event handler to notify when added.
        /// </summary>
        public event EventHandler OnAdd;

        /// <summary>
        /// Delegate command for adding an account.
        /// </summary>
        public DelegateCommand Add
        {
            get { return _add; }
        }

        /// <summary>
        /// The storage account name.
        /// </summary>
        public string AccountName
        {
            get { return _name; }
            set
            {
                _name = value;

                _add.RaiseCanExecuteChanged();
                base.RaisePropertyChanged();
            }
        }

        /// <summary>
        /// The storage account key.
        /// </summary>
        public string AccountKey
        {
            get { return _key; }
            set
            {
                _key = value;

                _add.RaiseCanExecuteChanged();
                base.RaisePropertyChanged();
            }
        }

        /// <summary>
        /// The suffix for the azure endpoint.
        /// </summary>
        [JsonIgnore]
        public string SuffixEndpoint
        {
            get { return Endpoint; }
        }

        #endregion
    }
}
