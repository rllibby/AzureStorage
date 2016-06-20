/*
 *  Copyright © 2016, Russell Libby 
 */

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace AzureStorage.Views
{
    /// <summary>
    /// Accounts page.
    /// </summary>
    public sealed partial class AccountsPage : Page
    {
        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public AccountsPage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
        }

        #endregion
    }
}
