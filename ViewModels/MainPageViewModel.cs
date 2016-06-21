/*
 *  Copyright © 2016, Russell Libby 
 */

using AzureStorage.Controls;
using AzureStorage.Models;
using AzureStorage.Views;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Template10.Common;
using Template10.Controls;
using Template10.Mvvm;
using Template10.Services.NavigationService;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace AzureStorage.ViewModels
{
    /// <summary>
    /// View model for main page.
    /// </summary>
    public class MainPageViewModel : ViewModelBase
    {
        #region Private fields

        private AddResourceControl _addResource = new AddResourceControl();
        private ResourceContainersModel _resources;
        private DelegateCommand _refresh;
        private DelegateCommand _add;
        private int _index;

        #endregion

        #region Private methods

        /// <summary>
        /// Shows or hides the add resource modal dialog.
        /// </summary>
        /// <param name="show">True to show the dialog, false to hide.</param>
        private void ShowAddResource(bool show)
        {
            WindowWrapper.Current().Dispatcher.Dispatch(() =>
            {
                var modal = Window.Current.Content as ModalDialog;

                modal.ModalContent = _addResource;
                modal.IsModal = show;
            });
        }

        /// <summary>
        /// Event that is triggered when the user wants to add a new resource.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void AddResource()
        {
            try
            {
                _addResource.Resource.Name = string.Empty;
                _addResource.Resource.ResourceType = _resources.CurrentType;
            }
            finally
            {
                ShowAddResource(true);
            }
        }

        /// <summary>
        /// Event that is triggered when the add button is clicked.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void OnAddResource(object sender, System.EventArgs e)
        {
            ShowAddResource(false);

            var storageAccount = _resources.Account;

            if (storageAccount == null) return;

            Busy.SetBusy(true, "Verifying name...");

            Task.Run(async () =>
            {
                var account = new CloudStorageAccount(new StorageCredentials(storageAccount.AccountName, storageAccount.AccountKey), storageAccount.SuffixEndpoint, true);

                switch (_addResource.Resource.ResourceType)
                {
                    case ContainerType.BlobContainer:
                        var blobClient = account.CreateCloudBlobClient();
                        var container = blobClient.GetContainerReference(_addResource.Resource.Name);

                        await container.CreateAsync();

                        _addResource.Resource.Uri = container.Uri.AbsoluteUri;

                        return;

                    case ContainerType.Queue:
                        var queueClient = account.CreateCloudQueueClient();
                        var queue = queueClient.GetQueueReference(_addResource.Resource.Name);

                        await queue.CreateAsync();

                        _addResource.Resource.Uri = queue.Uri.AbsoluteUri;

                        return;

                    default:
                        var tableClient = account.CreateCloudTableClient();
                        var table = tableClient.GetTableReference(_addResource.Resource.Name);

                        await table.CreateAsync();

                        _addResource.Resource.Uri = table.Uri.AbsoluteUri;

                        return;
                }
            }).ContinueWith((t) =>
            {
                Busy.SetBusy(false);

                if (t.IsFaulted)
                {
                    WindowWrapper.Current().Dispatcher.Dispatch(async () =>
                    {
                        var message = string.Empty;

                        switch (_addResource.Resource.ResourceType)
                        {
                            case ContainerType.BlobContainer:
                                message = string.Format("Failed to create the blob container '{0}'.", _addResource.Resource.Name);
                                break;

                            case ContainerType.Queue:
                                message = string.Format("Failed to create the queue '{0}'.", _addResource.Resource.Name);
                                break;

                            default:
                                message = string.Format("Failed to create the table '{0}'.", _addResource.Resource.Name);
                                break;
                        }

                        var result = await Helpers.Dialogs.ShowException(storageAccount.AccountName, message, t.Exception);

                        if (!result) return;

                        ShowAddResource(true);
                    });

                    return;
                }

                if (t.IsCompleted)
                {
                    WindowWrapper.Current().Dispatcher.Dispatch(() =>
                    {
                        var resource = new ResourceContainerModel(_resources, _resources.CurrentType);

                        resource.Name = _addResource.Resource.Name;
                        resource.Uri = _addResource.Resource.Uri;

                        switch (resource.ResourceType)
                        {
                            case ContainerType.BlobContainer:
                                _resources.BlobContainers.Add(resource);
                                break;

                            case ContainerType.Queue:
                                _resources.Queues.Add(resource);
                                break;

                            default:
                                _resources.Tables.Add(resource);
                                break;
                        }
                    });
                }
            });
        }

        /// <summary>
        /// Event that is triggered when the dialog is cancelled.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void OnCancelResource(object sender, System.EventArgs e)
        {
            ShowAddResource(false);
        }

        /// <summary>
        /// Determines if a new resource can be added.
        /// </summary>
        /// <returns></returns>
        private bool CanAddResource()
        {
            return (_resources.Account != null);
        }

        /// <summary>
        /// Loads the resources from the azure storage account.
        /// </summary>
        private async void Load()
        {
            RaisePropertyChanged("Name");

            await _resources.Load(Dispatcher);
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
            _add = new DelegateCommand(new Action(AddResource), CanAddResource);
            _addResource.VerticalAlignment = VerticalAlignment.Center;
            _addResource.Margin = new Thickness(20, 0, 20, 0);
            _addResource.OnAdd += OnAddResource;
            _addResource.OnCancel += OnCancelResource;
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
                if ((_resources.Account == null) || (_resources.Account != AccountsModel.Instance.Current)) Load();
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

        /// <summary>
        /// Event that is triggered when the list item is clicked.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        public void ListItemClicked(object sender, ItemClickEventArgs e)
        {
            if (_resources.SelectionMode.HasValue && _resources.SelectionMode.Value) return;
            if (_resources.Account == null) return;

            var resource = (e.ClickedItem as ResourceContainerModel);

            if (resource == null) return;

            var param = new AccountResourceModel(_resources.Account, resource.Name, resource.ResourceType);
                
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
        /// The command handler for the add action.
        /// </summary>
        public DelegateCommand Add
        {
            get { return _add; }
        }

        /// <summary>
        /// Gets the account name.
        /// </summary>
        public string Name
        {
            get
            {
                var account = AccountsModel.Instance.Current;

                return (account == null) ? "Resources" : string.Format("Account - {0}", account.AccountName);
            }
        }

        #endregion
    }
}

