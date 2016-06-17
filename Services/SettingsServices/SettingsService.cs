/*
 *  Copyright © 2016, Russell Libby 
 */

using AzureStorage.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Template10.Common;
using Template10.Utils;
using Windows.UI.Xaml;

namespace AzureStorage.Services.SettingsServices
{
    public class SettingsService
    {
        #region Private fields

        Template10.Services.SettingsService.ISettingsHelper _helper;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        private SettingsService()
        {
            _helper = new Template10.Services.SettingsService.SettingsHelper();
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Returns the singleton of this type.
        /// </summary>
        public static SettingsService Instance { get; } = new SettingsService();
       
        /// <summary>
        /// Returns the stored accounts collection.
        /// </summary>
        public IList<AccountModel> Accounts
        {
            get
            {
                var accounts = _helper.Read(nameof(Accounts), string.Empty);

                if (string.IsNullOrEmpty(accounts)) return new List<AccountModel>();

                return JsonConvert.DeserializeObject<IList<AccountModel>>(accounts);
            }
            set
            {
                _helper.Write(nameof(Accounts), JsonConvert.SerializeObject(value));
            }
        }

        /// <summary>
        /// Determines if shell back button will be used.
        /// </summary>
        public bool UseShellBackButton
        {
            get { return _helper.Read<bool>(nameof(UseShellBackButton), true); }
            set
            {
                _helper.Write(nameof(UseShellBackButton), value);
                BootStrapper.Current.NavigationService.Dispatcher.Dispatch(() =>
                {
                    BootStrapper.Current.ShowShellBackButton = value;
                    BootStrapper.Current.UpdateShellBackButton();
                    BootStrapper.Current.NavigationService.Refresh();
                });
            }
        }

        /// <summary>
        /// The application theme.
        /// </summary>
        public ApplicationTheme AppTheme
        {
            get
            {
                var theme = ApplicationTheme.Light;
                var value = _helper.Read<string>(nameof(AppTheme), theme.ToString());
                return Enum.TryParse<ApplicationTheme>(value, out theme) ? theme : ApplicationTheme.Dark;
            }
            set
            {
                _helper.Write(nameof(AppTheme), value.ToString());
                (Window.Current.Content as FrameworkElement).RequestedTheme = value.ToElementTheme();
                Views.Shell.HamburgerMenu.RefreshStyles(value);
            }
        }

        /// <summary>
        /// The max duration for navigation cache.
        /// </summary>
        public TimeSpan CacheMaxDuration
        {
            get { return _helper.Read<TimeSpan>(nameof(CacheMaxDuration), TimeSpan.FromDays(2)); }
            set
            {
                _helper.Write(nameof(CacheMaxDuration), value);
                BootStrapper.Current.CacheMaxDuration = value;
            }
        }

        #endregion
    }
}

