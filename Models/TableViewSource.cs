/*
 *  Copyright © 2016, Russell Libby 
 */

using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using Windows.Foundation;
using System.Dynamic;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Core;

namespace AzureStorage.Models
{
    /// <summary>
    /// Table view source which handles loading of dynamic data rows from Azure.
    /// </summary>
    public class TableViewSource : ObservableCollection<dynamic>, ISupportIncrementalLoading
    {
        #region Private constants

        private const string NullValue = "null";

        #endregion

        #region Private fields

        private TableContinuationToken _continue = null;
        private AccountResourceModel _resource;
        private CloudTableClient _client;
        private object _lock = new object();

        #endregion

        #region Private methods

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
        public TableViewSource(AccountResourceModel resource)
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
        /// <returns>The asunc task to wait on.</returns>
        public async Task LoadTable()
        {
            _continue = null;

            var table = _client.GetTableReference(_resource.ResourceName);
            var query = new TableQuery<ElasticEntityModel>().Take(100);
            var segment = await table.ExecuteQuerySegmentedAsync(query, _continue);

            try
            {
                Clear();

                foreach (var row in segment.Results)
                {
                    Add(WrapEntity(row));
                }
            }
            finally
            {
                _continue = segment.ContinuationToken;
            }
        }

        /// <summary>
        /// Loads more rows from the table.
        /// </summary>
        /// <param name="count">The number of items to load.</param>
        /// <returns></returns>
        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            var table = _client.GetTableReference(_resource.ResourceName);
            var query = new TableQuery<ElasticEntityModel>().Take((int)count);
            var segment = table.ExecuteQuerySegmentedAsync(query, _continue);
            var actual = (uint)0;

            try
            {
                segment.Wait();

                foreach (var row in segment.Result.Results)
                {
                    actual++;

                    Add(WrapEntity(row));
                }
            }
            finally
            {
                _continue = segment.Result.ContinuationToken;
            }

            return Task.Run(() => new LoadMoreItemsResult() { Count = actual }).AsAsyncOperation();
        }

        #endregion

        #region Public properties

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
