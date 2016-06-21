/*
 *  Copyright © 2016, Russell Libby 
 */

using System;
using Template10.Mvvm;

namespace AzureStorage.Models
{
    /// <summary>
    /// Wrapper for passing account and resource for detail page handling.
    /// </summary>
    public class AccountResourceModel : BindableBase
    {
        #region Private fields

        private readonly AccountModel _account;
        private readonly string _resourceName;
        private readonly ContainerType _resourceType;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="account">The azure storage account.</param>
        /// <param name="resourceName">The name of the resource.</param>
        /// <param name="resourceType">The resource type; blob container, queue or table.</param>
        public AccountResourceModel(AccountModel account, string resourceName, ContainerType resourceType)
        {
            if (account == null) throw new ArgumentNullException("account");
            if (string.IsNullOrEmpty(resourceName)) throw new ArgumentNullException("resourceName");

            _account = account;
            _resourceName = resourceName;
            _resourceType = resourceType;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// The azure storage account.
        /// </summary>
        public AccountModel Account
        {
            get { return _account; }
        }

        /// <summary>
        /// The resource name.
        /// </summary>
        public string ResourceName
        {
            get { return _resourceName; }
        }

        /// <summary>
        /// The resource type.
        /// </summary>
        public ContainerType ResourceType
        {
            get { return _resourceType; }
        }
        
        #endregion
    }
}
