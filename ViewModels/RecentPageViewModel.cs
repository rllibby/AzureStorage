/*
 *  Copyright © 2016, Russell Libby 
 */

using AzureStorage.Helpers;
using AzureStorage.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Template10.Mvvm;
using Template10.Services.NavigationService;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace AzureStorage.ViewModels
{
    /// <summary>
    /// View model for recent page.
    /// </summary>
    public class RecentPageViewModel : ViewModelBase
    {
        #region Private fields

        private RecentModel _recent = RecentModel.Instance;
        private DelegateCommand _clear;
        private bool _loading;

        #endregion

        #region Private methods

        /// <summary>
        /// Clears the recent list.
        /// </summary>
        private void ClearRecent()
        {
            _recent.Resources.Clear();
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public RecentPageViewModel()
        {
            _clear = new DelegateCommand(new Action(ClearRecent));
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
                if (suspending) { }
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
        /// <returns></returns>
        public override async Task OnNavigatingFromAsync(NavigatingEventArgs args)
        {
            try
            {
                args.Cancel = false;
            }
            finally
            {
                await Task.CompletedTask;
            }
        }

        /// <summary>
        /// Event that is triggered when the list item is clicked.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        public async void ListItemClicked(object sender, ItemClickEventArgs e)
        {
            var param = (e.ClickedItem as AccountResourceModel);

            if (param == null) return;

            Loading = true;

            try
            {
                var exists = await param.Exists();

                if (exists == null) return;
                if (!exists.Value)
                {
                    await Dialogs.Show(param.ResourceName, "The specified resource no longer exists.");

                    _recent.Remove(param);

                    return;
                }
            }
            finally
            {
                Loading = false;
            }

            RecentModel.Instance.Add(param);

            switch (param.ResourceType)
            {
                case ContainerType.BlobContainer:
                    break;

                case ContainerType.Queue:
                    break;

                case ContainerType.Table:
                    NavigationService.Navigate(typeof(Views.TablePage), param, new SuppressNavigationTransitionInfo());
                    break;
            }
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Command for clearing the list.
        /// </summary>
        public DelegateCommand Clear
        {
            get { return _clear; }
        }

        /// <summary>
        /// True if loading data, otherwise false.
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

        /// <summary>
        /// Returns the recent model.
        /// </summary>
        public RecentModel Recent
        {
            get { return _recent; }
        }

        #endregion
    }
}
