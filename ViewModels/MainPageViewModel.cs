/*
 *  Copyright © 2016, Russell Libby 
 */

using AzureStorage.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Template10.Mvvm;
using Template10.Services.NavigationService;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace AzureStorage.ViewModels
{
    /// <summary>
    /// View model for main page.
    /// </summary>
    public class MainPageViewModel : ViewModelBase
    {
        #region Private fields

        private ResourceContainersModel _resources;
        private DelegateCommand _refresh;
        private int _index;

        #endregion

        #region Private methods

        /// <summary>
        /// Loads the resources from the azure storage account.
        /// </summary>
        private void Load()
        {
            RaisePropertyChanged("Name");

            Dispatcher.DispatchAsync(async () =>
            {
                await _resources.Load(Dispatcher);

            });
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public MainPageViewModel()
        {
            _resources = new ResourceContainersModel();
            _refresh = new DelegateCommand(new Action(Load));
            _index = 0;
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
                Load();
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
            if (suspending)
            {
             
            }

            await Task.CompletedTask;
        }

        /// <summary>
        /// View model is being navigated away from.
        /// </summary>
        /// <param name="args">The navigation event arguments.</param>
        /// <returns></returns>
        public override async Task OnNavigatingFromAsync(NavigatingEventArgs args)
        {
            args.Cancel = false;

            await Task.CompletedTask;
        }

        /// <summary>
        /// Maintains the currently selected pivot item.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        public void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var pivot = sender as Pivot;

            _resources.ClearSelections();

            if (pivot != null) _index = _resources.ResourceIndex = pivot.SelectedIndex;  
        }

        #endregion

        #region Public properties

        /// <summary>
        /// The collection of resources.
        /// </summary>
        public ResourceContainersModel Resources
        {
            get { return _resources; }
        }

        /// <summary>
        /// The delegate command for refreshing the resource collections.
        /// </summary>
        public DelegateCommand Refresh
        {
            get { return _refresh; }
        }

        /// <summary>
        /// Gets the account name.
        /// </summary>
        public string Name
        {
            get
            {
                var account = AccountsModel.Instance.Current;

                return (account == null) ? "Resources" : account.AccountName;
            }
        }

        #endregion
    }
}

