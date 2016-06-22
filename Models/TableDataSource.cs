/*
 *  Copyright © 2016, Russell Libby 
 */

using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using Windows.Foundation;
using System.Dynamic;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Windows.UI.Xaml;
using Windows.UI.Core;

namespace AzureStorage.Models
{
    /// <summary>
    /// Table data source which handles loading of dynamic data rows from Azure.
    /// </summary>
    public class TableDataSource : ObservableCollection<dynamic>, ISupportIncrementalLoading
    {
        #region Private constants

        private const string NullValue = "null";

        #endregion

        #region Private fields

        private TableContinuationToken _continue = null;
        private AccountResourceModel _resource;
        private CloudTableClient _client;

        #endregion

        #region Private methods

        /// <summary>
        /// Gets the next set of records from the cloud table.
        /// </summary>
        /// <param name="count">The count or rows to download.</param>
        /// <returns></returns>
        private async Task<IList<dynamic>> GetNextSet(int count)
        {
            var result = new List<dynamic>();

            if (_continue == null) return result;

            var table = _client.GetTableReference(_resource.ResourceName);
            var query = new TableQuery<ElasticEntityModel>().Take(Math.Min(count, 1000));

            while (_continue != null)
            {
                var segment = await table.ExecuteQuerySegmentedAsync(query, _continue);

                foreach (var row in segment.Results)
                {
                    result.Add(WrapEntity(row));
                    count--;
                }

                _continue = segment.ContinuationToken;

                if (count <= 0) break;
            }

            return result;
        }

        /// <summary>
        /// Creates a dynamic object based on the entity.
        /// </summary>
        /// <param name="entity">The entity to wrap.</param>
        /// <returns>The dyanmic object.</returns>
        private dynamic WrapEntity(ElasticEntityModel entity)
        {
            var result = new ExpandoObject() as IDictionary<string, object>;

            result.Add("PartitionKey", entity.PartitionKey);
            result.Add("RowKey", entity.RowKey);
            result.Add("Timestamp", entity.Timestamp);
            result.Add("ETag", entity.ETag);

            foreach (var property in entity.Properties)
            {
                result.Add(property.Key, ((property.Value == null) || (property.Value.PropertyAsObject == null)) ? NullValue : property.Value.PropertyAsObject.ToString());
            }

            return result;
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="resource">The account resource model.</param>
        public TableDataSource(AccountResourceModel resource)
        {
            if (resource == null) throw new ArgumentNullException("resource");

            _client = resource.Account.GetStorageAccount().CreateCloudTableClient();
            _resource = resource;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Loads the first set of rows from the table.
        /// </summary>
        /// <param name="initialCount">The initial row count to load.</param>
        /// <returns>The async task to wait on.</returns>
        public async Task LoadTable(int initialCount = 100)
        {
            var dispatcher = Window.Current.Dispatcher;

            await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { OnLoadStart?.Invoke(this, new EventArgs()); });

            _continue = null;

            var table = _client.GetTableReference(_resource.ResourceName);
            var query = new TableQuery<ElasticEntityModel>().Take(initialCount + 1);
            var segment = await table.ExecuteQuerySegmentedAsync(query, _continue);

            Clear();

            try
            {
                foreach (var row in segment.Results) Add(WrapEntity(row));

                await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { OnLoadComplete?.Invoke(this, new EventArgs()); });
            }
            finally
            {
                _continue = segment.ContinuationToken;
            }
        }

        /// <summary>
        /// Loads more rows from the table.
        /// </summary>
        /// <param name="count">The suggested number of items to load.</param>
        /// <returns>The load more items result.</returns>
        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            var dispatcher = Window.Current.Dispatcher;

            return Task.Run(async () =>
            {
                await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { OnLoadStart?.Invoke(this, new EventArgs()); });

                var result = await GetNextSet(1000);
                var resultCount = (uint)result.Count;

                await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    foreach (var row in result) Add(row);

                    OnLoadComplete?.Invoke(this, new EventArgs());
                });

                return new LoadMoreItemsResult() { Count = resultCount };

            }).AsAsyncOperation();
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Event for start of data load.
        /// </summary>
        public EventHandler OnLoadStart;

        /// <summary>
        /// Event for end of data load.
        /// </summary>
        public EventHandler OnLoadComplete;

        /// <summary>
        /// Determines if more rows are available.
        /// </summary>
        public bool HasMoreItems
        {
            get { return (_continue != null); }
        }

        #endregion
    }
}
