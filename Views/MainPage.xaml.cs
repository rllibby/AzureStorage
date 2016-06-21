/*
 *  Copyright � 2016, Russell Libby 
 */

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace AzureStorage.Views
{
    /// <summary>
    /// Main page.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        #region Constructor

        /// <summary>
        /// Consructor.
        /// </summary>
        public MainPage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
        }

        #endregion
    }
}
