/*
 *  Copyright © 2016, Russell Libby 
 */

using AzureStorage.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Template10.Mvvm;
using Template10.Services.NavigationService;
using Windows.UI.Xaml.Navigation;

namespace AzureStorage.ViewModels
{
    /// <summary>
    /// View model for table page/
    /// </summary>
    public class TablePageViewModel : ViewModelBase
    {
        #region Private fields

        private AccountResourceModel _resourceModel;
        private TableDataSource _dataSource;
        private bool _loading;

        #endregion

        #region Private methods

        private void OnLoadStart(object sender, EventArgs args)
        {
            Loading = true;
        }

        private void OnLoadComplete(object sender, EventArgs args)
        {
            Loading = false;
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public TablePageViewModel()
        {
        }

        #endregion

        #region Public methods

        /// <summary>
        /// View model is being navigated to.
        /// </summary>
        /// <param name="parameter">The parameter sent to the navigation.</param>
        /// <param name="mode">The navigation mode.</param>
        /// <param name="suspensionState">The colletion of item state.</param>
        /// <returns>The task to wait on.</returns>
        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            try
            {
                if (parameter != null)
                {
                    Resource = (AccountResourceModel)parameter;

                    _dataSource = new TableDataSource(Resource);

                    _dataSource.OnLoadStart += OnLoadStart;
                    _dataSource.OnLoadComplete += OnLoadComplete;

                    try
                    {
                        await _dataSource.LoadTable(100);
                    }
                    finally
                    {
                        RaisePropertyChanged("DatSource");
                    }
                }
            }
            finally
            {
                await Task.CompletedTask;
            }
        }

        /// <summary>
        /// View model is being navigated from due to the app being suspended.
        /// </summary>
        /// <param name="suspensionState">The colletion of item state.</param>
        /// <param name="suspending">True if being suspended.</param>
        /// <returns>The task to wait on.</returns>
        public override async Task OnNavigatedFromAsync(IDictionary<string, object> suspensionState, bool suspending)
        {
            try
            {

            }
            finally
            {
                await Task.CompletedTask;
            }
        }

        /// <summary>
        /// View model is being navigated away from.
        /// </summary>
        /// <param name="args">The navigation event arguments.</param>
        /// <returns>The async task to wait on.</returns>
        public override async Task OnNavigatingFromAsync(NavigatingEventArgs args)
        {
            try
            {
                DataSource.Clear();
                DataSource = null;

                args.Cancel = false;
            }
            finally
            {
                await Task.CompletedTask;
            }
        }

        #endregion

        #region Public properties

        /// <summary>
        /// The resource model for the page.
        /// </summary>
        public AccountResourceModel Resource
        {
            get { return _resourceModel; }
            set
            {
                _resourceModel = value;

                RaisePropertyChanged("Name");
                base.RaisePropertyChanged();
            }
        }

        /// <summary>
        /// The table data source.
        /// </summary>
        public TableDataSource DataSource
        {
            get { return _dataSource; }
            set
            {
                _dataSource = value;

                base.RaisePropertyChanged();
            }
        }

        /// <summary>
        /// The view model name.
        /// </summary>
        public string Name
        {
            get { return (_resourceModel == null) ? "Table" : string.Format("Table - {0}", _resourceModel.ResourceName); }
        }

        /// <summary>
        /// True if data is loading, otherwise false.
        /// </summary>
        public bool Loading
        {
            get { return _loading; }
            set
            {
                _loading = value;

                base.RaisePropertyChanged();
            }
        }

        #endregion
    }
}

