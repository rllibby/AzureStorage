/*
 *  Copyright © 2016, Russell Libby 
 */

using AzureStorage.ViewModels;
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
        #region Private fields

        private string _endpoint = "core.windows.net";
        private bool _validated = false;
        private bool _isSelected = false;
        private string _name;
        private string _key;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="accountName">The account name.</param>
        public AccountModel(string accountName)
        {
            if (string.IsNullOrEmpty(accountName)) throw new ArgumentNullException("accountName");

            AccountName = accountName;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// The storage account name.
        /// </summary>
        public string AccountName
        {
            get { return _name; }
            set
            {
                _name = value;
                Validated = false;

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
                Validated = false;

                base.RaisePropertyChanged();
            }
        }

        /// <summary>
        /// The endpoint for the storage account.
        /// </summary>
        public string Endpoint
        {
            get { return _endpoint; }
            set
            {
                _endpoint = value;
                Validated = false;

                base.RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Determines if the setting has been validated.
        /// </summary>
        public bool Validated
        {
            get { return _validated; }
            set
            {
                _validated = value;

                base.RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Determines if the item is selected.
        /// </summary>
        [JsonIgnore]
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;

                base.RaisePropertyChanged();
            }
        }

        /// <summary>
        /// The relay command for deleting the item.
        /// </summary>
        [JsonIgnore]
        public RelayCommand DeleteCommand { get; set; }

        /// <summary>
        /// The relay command for editing the item.
        /// </summary>
        [JsonIgnore]
        public RelayCommand EditCommand { get; set; }

        #endregion
    }
}
