/*
 *  Copyright © 2016, Russell Libby 
 */

using Windows.UI.Xaml;
using System.Threading.Tasks;
using AzureStorage.Services.SettingsServices;
using Windows.ApplicationModel.Activation;
using Template10.Controls;
using Windows.UI.Xaml.Data;

namespace AzureStorage
{
    [Bindable]
    sealed partial class App : Template10.Common.BootStrapper
    {
        public App()
        {
            InitializeComponent();

            SplashFactory = (e) => new Views.Splash(e);

            #region App settings

            var _settings = SettingsService.Instance;

            RequestedTheme = _settings.AppTheme;
            CacheMaxDuration = _settings.CacheMaxDuration;
            ShowShellBackButton = _settings.UseShellBackButton;

            #endregion
        }

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

        public override async Task OnStartAsync(StartKind startKind, IActivatedEventArgs args)
        {
            // long-running startup tasks go here
            await Task.Delay(2000);

            NavigationService.Navigate(typeof(Views.MainPage));

            await Task.CompletedTask;
        }
    }
}

