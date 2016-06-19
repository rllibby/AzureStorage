/*
 *  Copyright © 2016, Russell Libby 
 */

using AzureStorage.Helpers;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Linq;
using System.Threading.Tasks;
using Template10.Common;
using Template10.Mvvm;
using Windows.UI.Core;
using Windows.UI.Xaml;

#pragma warning disable CS4014

namespace AzureStorage.Models
{
    /// <summary>
    /// Class for maintaining the collection of tables, queues and blob containers.
    /// </summary>
    public class ResourceContainersModel : BindableBase
    {
        #region Private methods

        private readonly ObservableCollectionEx<ResourceContainerModel> _tables = new ObservableCollectionEx<ResourceContainerModel>();
        private readonly ObservableCollectionEx<ResourceContainerModel> _queues = new ObservableCollectionEx<ResourceContainerModel>();
        private readonly ObservableCollectionEx<ResourceContainerModel> _blobContainers = new ObservableCollectionEx<ResourceContainerModel>();
        private DelegateCommand _delete;
        private AccountModel _account;
        private bool _loading;
        private int _selected;
        private int _index;

        #endregion

        #region Private methods

        /// <summary>
        /// Deletes the selected blob containers.
        /// </summary>
        /// <param name="client">The blob client.</param>
        /// <returns>The async task to wait on.</returns>
        private async Task DeleteBlobContainers(CloudBlobClient client)
        {
            if (client == null) return;

            var selected = _blobContainers.Where(r => r.Selected);

            foreach (var blob in selected.ToList())
            {
                _blobContainers.Remove(blob); 

                try
                {
                    var container = client.GetContainerReference(blob.Name);

                    await container.DeleteIfExistsAsync();
                }
                finally
                {
                    blob.Selected = false;
                }
            }
        }

        /// <summary>
        /// Deletes the selected queues from the queue client.
        /// </summary>
        /// <param name="client">The queue client.</param>
        /// <returns>The async task to wait on.</returns>
        private async Task DeleteQueues(CloudQueueClient client)
        {
            if (client == null) return;

            var selected = _queues.Where(r => r.Selected);
             
            foreach (var queue in selected.ToList())
            {
                _queues.Remove(queue); 

                try
                {
                    var queueRef = client.GetQueueReference(queue.Name);

                    await queueRef.DeleteIfExistsAsync();
                }
                finally
                {
                    queue.Selected = false;
                }
            }
        }

        /// <summary>
        /// Deletes the selected tables from the queue client.
        /// </summary>
        /// <param name="client">The table client.</param>
        /// <returns>The async task to wait on.</returns>
        private async Task DeleteTables(CloudTableClient client)
        {
            if (client == null) return;

            var selected = _tables.Where(r => r.Selected);

            foreach (var table in selected.ToList())
            {
                _tables.Remove(table); 

                try
                {
                    var tableRef = client.GetTableReference(table.Name);

                    await tableRef.DeleteIfExistsAsync();
                }
                finally
                {
                    table.Selected = false;
                }
            }
        }

        /// <summary>
        /// Loads the containers from the blob client.
        /// </summary>
        /// <param name="client">The blob client.</param>
        /// <returns>The async task to wait on.</returns>
        private async Task LoadBlobContainers(CloudBlobClient client)
        {
            _blobContainers.Clear();

            if (client == null) return;

            BlobContinuationToken token = null;

            do
            {
                var segment = await client.ListContainersSegmentedAsync(token);

                foreach (var container in segment.Results)
                {
                    var item = new ResourceContainerModel(this, ContainerType.BlobContainer)
                    {
                        Name = container.Name,
                        Uri = container.Uri.AbsoluteUri
                    };

                    _blobContainers.Add(item);
                }

                token = segment.ContinuationToken;
            }
            while (token != null);
        }

        /// <summary>
        /// Loads the queues from the queue client.
        /// </summary>
        /// <param name="client">The queue client.</param>
        /// <returns>The async task to wait on.</returns>
        private async Task LoadQueues(CloudQueueClient client)
        {
            _queues.Clear();

            if (client == null) return;

            QueueContinuationToken token = null;

            do
            {
                var segment = await client.ListQueuesSegmentedAsync(token);

                foreach (var queue in segment.Results)
                {
                    var item = new ResourceContainerModel(this, ContainerType.Queue)
                    {
                        Name = queue.Name,
                        Uri = queue.Uri.AbsoluteUri
                    };

                    _queues.Add(item);
                }

                token = segment.ContinuationToken;
            }
            while (token != null);
        }

        /// <summary>
        /// Loads the tables from the table client.
        /// </summary>
        /// <param name="client">The table client.</param>
        /// <returns>The async task to wait on.</returns>
        private async Task LoadTables(CloudTableClient client)
        {
            _tables.Clear();

            if (client == null) return;

            TableContinuationToken token = null;

            do
            {
                var segment = await client.ListTablesSegmentedAsync(token);

                foreach (var table in segment.Results)
                {
                    var item = new ResourceContainerModel(this, ContainerType.Table)
                    {
                        Name = table.Name,
                        Uri = table.Uri.AbsoluteUri
                    };

                    _tables.Add(item);
                }

                token = segment.ContinuationToken;
            }
            while (token != null);
        }

        /// <summary>
        /// Delete the selected resources.
        /// </summary>
        private async Task DeleteSelectedResources()
        {
            Loading = true;

            try
            {
                if (_account == null) return;

                var storageAccount = new CloudStorageAccount(new StorageCredentials(_account.AccountName, _account.AccountKey), _account.SuffixEndpoint, true);
                var tableClient = storageAccount.CreateCloudTableClient();
                var blobClient = storageAccount.CreateCloudBlobClient();
                var queueClient = storageAccount.CreateCloudQueueClient();

                await DeleteBlobContainers(blobClient);
                await DeleteQueues(queueClient);
                await DeleteTables(tableClient);
            }
            catch
            {
                Window.Current.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    await Dialogs.Show(_account.AccountName, "Error removing resources from the storage account.");
                });
            }
            finally
            {
                ClearSelections();
                Loading = false;
            }
        }

        /// <summary>
        /// Deletes the selected items.
        /// </summary>
        private async void DeleteSelected()
        {
            var message = string.Empty;
            var count = _selected;

            if ((_account == null) || (_index < 0) || (_index > 2) || (count == 0)) return;
            
            switch (_index)
            {
                case 0:
                    message = string.Format("Delete the {0} selected blob container{1} from this account?", count, (count > 1) ? "s" : "");
                    break;

                case 1:
                    message = string.Format("Delete the {0} selected queue{1} from this account?", count, (count > 1) ? "s" : "");
                    break;

                case 2:
                    message = string.Format("Delete the {0} selected table{1} from this account?", count, (count > 1) ? "s" : "");
                    break;
            }

            var result = await Dialogs.ShowOkCancel(_account.AccountName, message);

            if (!result) return;

            await DeleteSelectedResources();
        }

        /// <summary>
        /// Determines if there are selected items that can be deleted.
        /// </summary>
        /// <returns></returns>
        private bool CanDeleteSelected()
        {
            return (_selected > 0);
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public ResourceContainersModel()
        {
            _delete = new DelegateCommand(new Action(DeleteSelected), CanDeleteSelected);
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Clears all resource collections.
        /// </summary>
        public void Clear()
        {
            try
            {
                _blobContainers.BeginUpdate();
                _tables.BeginUpdate();
                _queues.BeginUpdate();
            }
            finally
            {
                _selected = 0;
            }
        }

        /// <summary>
        /// Clears all selected items.
        /// </summary>
        public void ClearSelections()
        {
            foreach (var resource in _blobContainers) resource.Selected = false;
            foreach (var resource in _tables) resource.Selected = false;
            foreach (var resource in _queues) resource.Selected = false;
        }

        /// <summary>
        /// Loads the resources from the Azure storage account.
        /// </summary>
        /// <param name="dispatcher">The dispatcher to run the update on.</param>
        /// <returns>The async task to wait on.</returns>
        public async Task Load(IDispatcherWrapper dispatcher)
        {
            Loading = true;

            _account = AccountsModel.Instance.Current;

            try
            {
                Clear();

                if (_account == null) return;

                var storageAccount = new CloudStorageAccount(new StorageCredentials(_account.AccountName, _account.AccountKey), _account.SuffixEndpoint, true);
                var tableClient = storageAccount.CreateCloudTableClient();
                var blobClient = storageAccount.CreateCloudBlobClient();
                var queueClient = storageAccount.CreateCloudQueueClient();

                await LoadBlobContainers(blobClient);
                await LoadQueues(queueClient);
                await LoadTables(tableClient);
            }
            catch 
            {
                dispatcher.DispatchAsync(async () =>
                {
                    await Dialogs.Show(_account.AccountName, "Error loading resources from the storage account.");
                });
            }
            finally
            {
                _blobContainers.EndUpdate(dispatcher);
                _tables.EndUpdate(dispatcher);
                _queues.EndUpdate(dispatcher);

                Loading = false;
            }
        }

        #endregion

        #region Public properties

        /// <summary>
        /// The collection of tables.
        /// </summary>
        public ObservableCollectionEx<ResourceContainerModel> Tables
        {
            get { return _tables; }
        }

        /// <summary>
        /// The collection of queues.
        /// </summary>
        public ObservableCollectionEx<ResourceContainerModel> Queues
        {
            get { return _queues; }
        }

        /// <summary>
        /// The collection of blob containers.
        /// </summary>
        public ObservableCollectionEx<ResourceContainerModel> BlobContainers
        {
            get { return _blobContainers; }
        }

        /// <summary>
        /// The delegate command for deleting items.
        /// </summary>
        public DelegateCommand Delete
        {
            get { return _delete; }
        }

        /// <summary>
        /// True if loading, otherwise false.
        /// </summary>
        public bool Loading
        {
            get { return _loading; }
            set
            {
                if (_loading != value)
                {
                    _loading = value;

                    base.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// The number of selected items.
        /// </summary>
        public int Selected
        {
            get { return _selected; }
            set
            {
                _selected = value;
                _delete.RaiseCanExecuteChanged();

                base.RaisePropertyChanged();
            }
        }

        /// <summary>
        /// The currently active resource (0 = blobs, 1 = queues, 2 = tables)
        /// </summary>
        public int ResourceIndex
        {
            get { return _index; }
            set { _index = value; }
        }

        #endregion
    }
}
