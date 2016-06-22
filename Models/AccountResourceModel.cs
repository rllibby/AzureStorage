/*
 *  Copyright © 2016, Russell Libby 
 */

using AzureStorage.Helpers;
using System;
using System.Threading.Tasks;
using Template10.Mvvm;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace AzureStorage.Models
{
    /// <summary>
    /// Wrapper for passing account and resource for detail page handling.
    /// </summary>
    public class AccountResourceModel : BindableBase
    {
        #region Private fields

        private readonly ContainerType _resourceType;
        private readonly AccountModel _account;
        private readonly string _resourceName;

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

        #region Public methods

        /// <summary>
        /// Determines if the resource still exists.
        /// </summary>
        /// <returns></returns>
        public async Task<bool?> Exists()
        {
            var storageAccount = _account.GetStorageAccount();

            try
            {
                switch (_resourceType)
                {
                    case ContainerType.BlobContainer:
                        var blobClient = storageAccount.CreateCloudBlobClient();
                        var container = blobClient.GetContainerReference(_resourceName);

                        return await container.ExistsAsync();

                    case ContainerType.Queue:
                        var queueClient = storageAccount.CreateCloudQueueClient();
                        var queue = queueClient.GetQueueReference(_resourceName);

                        return await queue.ExistsAsync();

                    case ContainerType.Table:
                        var tableClient = storageAccount.CreateCloudTableClient();
                        var table = tableClient.GetTableReference(_resourceName);

                        return await table.ExistsAsync();
                }
            }
            catch (Exception exception)
            {
                await Window.Current.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    await Dialogs.ShowException(_account.AccountName, "Failed to verify the resource existence.", exception, false);
                });

                return null;
            }

            return false;
        }

        /// <summary>
        /// Equality test.
        /// </summary>
        /// <param name="x">The left hand side.</param>
        /// <param name="y">The right hand side.</param>
        /// <returns>True if the account resource models are equal, otherwise false.</returns>
        public static bool Equals(AccountResourceModel x, AccountResourceModel y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null) || ReferenceEquals(y, null)) return false;

            return (ReferenceEquals(x.Account, y.Account) && x.ResourceName.Equals(y.ResourceName) && (x.ResourceType == y.ResourceType));
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

        /// <summary>
        /// Symbol source for container.
        /// </summary>
        public Symbol ResourceSymbol
        {
            get
            {
                switch (_resourceType)
                {
                    case ContainerType.BlobContainer:
                        return Symbol.Page2;

                    case ContainerType.Queue:
                        return Symbol.Message;

                    default:
                        return Symbol.ViewAll;
                }
            }
        }

        #endregion
    }
}
