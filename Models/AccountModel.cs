/*
 *  Copyright © 2016, Russell Libby 
 */

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

        private string _name;
        private string _key;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public AccountModel() { }

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
        /// Clears the account.
        /// </summary>
        public void Clear()
        {
            AccountName = null;
            AccountKey = null;
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
        /// Determines if the account can be added.
        /// </summary>
        [JsonIgnore]
        public bool CanAdd
        {
            get { return (!string.IsNullOrEmpty(_name) && !string.IsNullOrEmpty(_key)); }
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

                RaisePropertyChanged("CanAdd");
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

                RaisePropertyChanged("CanAdd");
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
