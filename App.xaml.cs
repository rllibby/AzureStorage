/*
 *  Copyright © 2016, Russell Libby 
 */

using AzureStorage.Models;
using AzureStorage.Services.SettingsServices;
using System.Threading.Tasks;
using Template10.Controls;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Animation;

namespace AzureStorage
{
    /// <summary>
    /// The application.
    /// </summary>
    [Bindable]
    sealed partial class App : Template10.Common.BootStrapper
    {
        #region Private fields

        private AccountsModel _accounts;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public App()
        {
            InitializeComponent();

            SplashFactory = (e) => new Views.Splash(e);

            var _settings = SettingsService.Instance;

            RequestedTheme = _settings.AppTheme;
            CacheMaxDuration = _settings.CacheMaxDuration;
            ShowShellBackButton = _settings.UseShellBackButton;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Application initialization.
        /// </summary>
        /// <param name="args">The activated event arguments.</param>
        /// <returns>The task to wait on.</returns>
        public override async Task OnInitializeAsync(IActivatedEventArgs args)
        {
            if (Window.Current.Content as ModalDialog == null)
            {
                var nav = NavigationServiceFactory(BackButton.Attach, ExistingContent.Include);

                Window.Current.Content = new ModalDialog
                {
                    DisableBackButtonWhenModal = true,
                    Content = new Views.Shell(nav),
                    ModalContent = new Views.Busy(),
                };
            }

            await Task.CompletedTask;
        }

        /// <summary>
        /// Appliaction starting.
        /// </summary>
        /// <param name="startKind">The start kind.</param>
        /// <param name="args">The activated event arguments.</param>
        /// <returns>The task to wait on.</returns>
        public override async Task OnStartAsync(StartKind startKind, IActivatedEventArgs args)
        {
            try
            {
                _accounts = AccountsModel.Instance;

                NavigationService.Navigate(typeof(Views.MainPage), null, new SuppressNavigationTransitionInfo());
            }
            finally
            {
                await Task.CompletedTask;
            }
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Expose the static accounts model to the whole application.
        /// </summary>
        public AccountsModel Accounts
        {
            get { return _accounts; }
        }

        #endregion
    }
}

