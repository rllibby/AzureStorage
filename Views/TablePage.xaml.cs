/*
 *  Copyright © 2016, Russell Libby 
 */

using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Controls;

namespace AzureStorage.Views
{
    /// <summary>
    /// Table page.
    /// </summary>
    public sealed partial class TablePage : Page
    {
        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public TablePage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Disabled;
        }

        #endregion
    }
}

